﻿using System.Collections.Generic;
using Battle.Entity;
using Battle.FSM;
using PEProtocol;
using UnityEngine;

namespace Battle.Manager
{
    /// <summary>
    /// 状态管理器
    /// </summary>
    public class StateMgr : MonoBehaviour
    {
        private Dictionary<AniState, IState> fsm = new();

        public void Init()
        {
            fsm.Add(AniState.Born, new StateBorn());
            fsm.Add(AniState.Idle, new StateIdle());
            fsm.Add(AniState.Move, new StateMove());
            fsm.Add(AniState.Attack, new StateAttack());
            fsm.Add(AniState.Hit, new StateHit());
            fsm.Add(AniState.Die,new StateDie());
            PECommon.Log("StateMgr Initialized");
        }

        public void ChangeState(EntityBase entity, AniState targetState, params object[] args)
        {
            if (entity.currentAniState == targetState)
            {
                return;
            }

            if (fsm.ContainsKey(targetState))
            {
                if (entity.currentAniState != AniState.None)
                {
                    fsm[entity.currentAniState].Exit(entity, args);
                }

                fsm[targetState].Enter(entity, args);
                fsm[targetState].Process(entity, args);
            }
        }
    }
}