using System;
using Battle.FSM;
using Common;
using UnityEngine;

namespace Battle.Entity
{
    /// <summary>
    /// 怪物逻辑实体
    /// </summary>
    public class EntityMonster : EntityBase
    {
        public MonsterData md;

        public EntityMonster()
        {
            entityType = EntityType.Monster;
        }

        public override void SetBattleProps(BattleProps props)
        {
            int level = md.mLevel;
            Props = new BattleProps
            {
                hp = props.hp * level,
                ad = props.ad * level,
                ap = props.ap * level,
                addef = props.addef * level,
                apdef = props.apdef * level,
                dodge = props.dodge * level,
                pierce = props.pierce * level,
                critical = props.critical * level
            };
            HP = Props.hp;
        }

        private float checkTime = 2;
        private float checkCountTime;
        private float atkTime = 2;
        private float atkCountTime;
        private bool runAI = true;

        public override void TickAILogic()
        {
            if (currentAniState is not (AniState.Idle or AniState.Move)) return;
            if (battleMgr.isPauseGame)
            {
                Idle();
                return;
            }

            if (!runAI) return;


            float delta = Time.deltaTime;
            checkCountTime += delta;
            if (checkCountTime < checkTime) return;

            Vector2 dir = CalcTargetDir();
            if (!runAI || dir == Vector2.zero) return;
            if (!InAtkRange())
            {
                SetDir(dir);
                Move();
            }
            else
            {
                SetDir(Vector2.zero);
                atkCountTime += checkCountTime;
                if (atkCountTime > atkTime)
                {
                    SetAtkRotation(dir, false);
                    Attack(md.mCfg.skillID);
                    atkCountTime = 0;
                }
                else
                {
                    Idle();
                }

                checkCountTime = 0;
                checkTime = PETools.RDFloat(1, 3);
            }
        }

        public override Vector2 CalcTargetDir()
        {
            EntityPlayer entityPlayer = battleMgr.GetEntityPlayer();
            if (entityPlayer == null || entityPlayer.currentAniState == AniState.Die)
            {
                runAI = false;
                return Vector2.zero;
            }

            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = self.y = 0;
            if (Vector3.Distance(target, self) > Constants.searchDis) return Vector2.zero;

            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }

        public override void SetHPVal(int curHP)
        {
            if (md.mCfg.mType == MonsterType.Normal)
            {
                base.SetHPVal(curHP);
            }
            else if (md.mCfg.mType == MonsterType.Boss)
            {
                BattleSys.Instance.playerCtrlWnd.SetBossHPBarVal(curHP);
            }
        }

        private bool InAtkRange()
        {
            EntityPlayer entityPlayer = battleMgr.GetEntityPlayer();
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = self.y = 0;
            return Vector3.Distance(target, self) <= md.mCfg.atkDis;
        }

        public override bool GetBreakState()
        {
            if (md.mCfg.isStop)
            {
                if (curSkill != null)
                {
                    return curSkill.isBreak;
                }

                return true;
            }

            return false;
        }
    }
}