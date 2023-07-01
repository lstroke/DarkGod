using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Battle.Entity
{
    /// <summary>
    /// 玩家逻辑实体
    /// </summary>
    public class EntityPlayer : EntityBase
    {
        public EntityPlayer()
        {
            entityType = EntityType.Player;
        }

        public override Vector2 GetDirInput()
        {
            return battleMgr.GetDirInput();
        }

        public override Vector2 CalcTargetDir()
        {
            EntityMonster monster = FindClosedTarget();
            if (monster == null)
            {
                return Vector3.zero;
            }

            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }

        /// <summary>
        /// 找到最近的怪物目标
        /// </summary>
        /// <returns></returns>
        private EntityMonster FindClosedTarget()
        {
            List<EntityMonster> lst = battleMgr.GetEntityMonsters();
            Vector3 self = GetPos();
            EntityMonster targetMonster = null;
            float dis = Constants.searchDis;
            foreach (var monster in lst)
            {
                float curDis = Vector3.Distance(monster.GetPos(), self);
                if (curDis < dis)
                {
                    targetMonster = monster;
                    dis = curDis;
                }
            }

            return targetMonster;
        }

        public override void SetDodge()
        {
            GameRoot.Instance.dynamicWnd.SetSelfDodge();
        }

        public override void Sethurt(int hurt)
        {
            BattleSys.Instance.playerCtrlWnd.SetSelfHPBarVal(Mathf.Max(HP - hurt, 0));
        }
    }
}