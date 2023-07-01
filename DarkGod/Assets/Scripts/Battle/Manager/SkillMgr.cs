using Battle.Entity;
using Common;
using PEProtocol;
using Service;
using UnityEngine;
using Random = System.Random;

namespace Battle.Manager
{
    /// <summary>
    /// 技能管理器
    /// </summary>
    public class SkillMgr : MonoBehaviour
    {
        private ResSvc resSvc;
        private TimerSvc timerSvc;


        public void Init()
        {
            resSvc = ResSvc.Instance;
            timerSvc = TimerSvc.Instance;
            PECommon.Log("SkillMgr Initialized");
        }

        public void SkillAttack(EntityBase entity, int skillID)
        {
            if (entity == null)
            {
                return;
            }

            if (entity.entityType == EntityType.Player)
            {
                if (entity.GetDirInput() == Vector2.zero)
                {
                    //自动搜索最近的怪物
                    Vector2 dir = entity.CalcTargetDir();
                    if (dir != Vector2.zero)
                    {
                        entity.SetAtkRotation(dir, false);
                    }
                }
                else
                {
                    entity.SetAtkRotation(entity.GetDirInput());
                }
            }

            AttackDamage(entity, skillID);
            AttackEffect(entity, skillID);
        }

        /// <summary>
        /// 技能效果表现
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="skillID"></param>
        private void AttackEffect(EntityBase entity, int skillID)
        {
            //取消角色控制
            entity.canControl = false;
            entity.SetDir(Vector2.zero);

            SkillCfg skillData = resSvc.GetSkillCfgData(skillID);
            if (!skillData.isCollide)
            {
                //忽略刚体碰撞
                int layerPlayer = LayerMask.NameToLayer("Player");
                int layerMonster = LayerMask.NameToLayer("Monster");
                Physics.IgnoreLayerCollision(layerPlayer, layerMonster);
                timerSvc.AddTimeTask((tid) => { Physics.IgnoreLayerCollision(layerPlayer, layerMonster, false); },
                    skillData.skillTime);
            }

            if (!skillData.isBreak)
            {
                entity.entityState = EntityState.BatiState;
            }

            entity.SetAction(skillData.aniAction);
            entity.SetFX(skillData.fx, skillData.skillTime);
            int sumDelay = 1;
            foreach (var skillMoveID in skillData.skillMoveLst)
            {
                SkillMoveCfg skillMoveCfg = resSvc.GetSkillMoveCfgData(skillMoveID);
                float speed = skillMoveCfg.moveDis / (skillMoveCfg.moveTime * 1f / 1000);
                sumDelay += skillMoveCfg.delayTime;
                int moveid = timerSvc.AddTimeTask((tid) =>
                {
                    entity.SetSkillMoveState(true, speed);
                    entity.RmvMoveCB(tid);
                }, sumDelay);
                entity.skMoveCBLst.Add(moveid);
                sumDelay += skillMoveCfg.moveTime;
                int stopid = timerSvc.AddTimeTask((tid) =>
                {
                    entity.SetSkillMoveState(false);
                    entity.RmvMoveCB(tid);
                }, sumDelay);
                entity.skMoveCBLst.Add(stopid);
            }

            entity.skEndCB = timerSvc.AddTimeTask((tid) => { entity.Idle(); }, skillData.skillTime);
        }

        private void AttackDamage(EntityBase entity, int skillID)
        {
            SkillCfg skillData = resSvc.GetSkillCfgData(skillID);
            int sumDelay = 1;
            for (int i = 0; i < skillData.skillActionLst.Count; i++)
            {
                SkillActionCfg sac = resSvc.GetSkillActionCfgData(skillData.skillActionLst[i]);
                sumDelay += sac.delayTime;
                int index = i;
                if (sumDelay > 0)
                {
                    int actId = timerSvc.AddTimeTask((tid) =>
                    {
                        SkillAction(entity, skillData, index);
                        entity.RmvActionCB(tid);
                    }, sumDelay);
                    entity.skActionCBLst.Add(actId);
                }
                else
                {
                    SkillAction(entity, skillData, index);
                }
            }
        }

        private void SkillAction(EntityBase entity, SkillCfg skillCfg, int index)
        {
            SkillActionCfg sac = resSvc.GetSkillActionCfgData(skillCfg.skillActionLst[index]);
            if (entity.entityType == EntityType.Player)
            {
                //获取场景里的所有怪物实体
                var monsterLst = entity.battleMgr.GetEntityMonsters();
                foreach (var em in monsterLst)
                {
                    if (InRange(entity.GetPos(), em.GetPos(), sac.radius) &&
                        InAngle(entity.GetTrans(), em.GetPos(), sac.angle))
                    {
                        CalcDamage(entity, em, skillCfg, skillCfg.skillDamageLst[index]);
                    }
                }
            }
            else if (entity.entityType == EntityType.Monster)
            {
                EntityPlayer player = entity.battleMgr.GetEntityPlayer();
                if (InRange(entity.GetPos(), player.GetPos(), sac.radius) &&
                    InAngle(entity.GetTrans(), player.GetPos(), sac.angle))
                {
                    CalcDamage(entity, player, skillCfg, skillCfg.skillDamageLst[index]);
                }
            }
        }

        private Random rd = new Random();

        private void CalcDamage(EntityBase caster, EntityBase target, SkillCfg skillCfg, int damage)
        {
            int dmgSum = damage;
            bool isCritical = false;
            if (skillCfg.dmgType == DamageType.AD)
            {
                //计算闪避
                int dodgeNum = PETools.RDInt(1, 100, rd);
                if (dodgeNum <= target.Props.dodge)
                {
                    target.SetDodge();
                    return;
                }

                //计算属性加成
                dmgSum += caster.Props.ad;
                //计算暴击
                int criticalNum = PETools.RDInt(1, 100, rd);
                if (criticalNum <= caster.Props.critical)
                {
                    isCritical = true;
                    float criticalRate = 1 + (PETools.RDInt(1, 100, rd) / 100f);
                    dmgSum = (int)criticalRate * dmgSum;
                    target.SetCritical(dmgSum);
                }

                //计算穿甲
                int addef = (int)((1 - caster.Props.pierce / 100f) * target.Props.addef);
                dmgSum -= addef;
            }
            else if (skillCfg.dmgType == DamageType.AP)
            {
                //计算属性加成
                dmgSum += caster.Props.ap;
                //计算法抗
                dmgSum -= target.Props.apdef;
            }

            dmgSum = Mathf.Max(dmgSum, 1);
            if (!isCritical)
            {
                target.Sethurt(dmgSum);
            }

            if (target.HP < dmgSum)
            {
                target.HP = 0;
                target.Die();
                if (target.entityType == EntityType.Monster)
                {
                    target.battleMgr.RmvMonster(target.Name);
                }
                else if (target.entityType == EntityType.Player)
                {
                    target.battleMgr.EndBattle(false, 0);
                }
            }
            else
            {
                target.HP -= dmgSum;
                if (target.entityState == EntityState.Normal && target.GetBreakState())
                {
                    target.Hit();
                }
            }

            target.SetHPVal(target.HP);
        }

        private bool InRange(Vector3 from, Vector3 to, float range)
        {
            from.y = to.y = 0;
            float dis = Vector3.Distance(from, to);
            return dis <= range;
        }

        private bool InAngle(Transform trans, Vector3 to, float angle)
        {
            if (angle == 360)
            {
                return true;
            }

            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;

            float ang = Vector3.Angle(start, dir);

            return ang <= angle / 2;
        }
    }
}