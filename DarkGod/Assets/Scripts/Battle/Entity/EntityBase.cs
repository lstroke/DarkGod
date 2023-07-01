using System;
using System.Collections.Generic;
using Battle.FSM;
using Battle.Manager;
using Common;
using PEProtocol;
using Service;
using UnityEngine;

namespace Battle.Entity
{
    /// <summary>
    /// 逻辑实体基类
    /// </summary>
    public abstract class EntityBase
    {
        public AniState currentAniState = AniState.None;
        public BattleMgr battleMgr;
        public StateMgr stateMgr;
        private Controller.Controller controller;
        public SkillMgr skillMgr;
        public bool canControl = true;
        public bool canRlsSkill = true;
        public Queue<int> comboQue = new();
        public int nextSkillID;
        public SkillCfg curSkill;
        public EntityType entityType = EntityType.None;
        public EntityState entityState = EntityState.Normal;
        public BattleProps Props { get; protected set; }
        public string Name { get; set; }
        private int hp;
        private AudioSource audioSource;

        public List<int> skMoveCBLst = new();
        public List<int> skActionCBLst = new();
        public int skEndCB = -1;

        public int HP
        {
            get => hp;
            set { hp = value; }
        }

        public void Born()
        {
            stateMgr.ChangeState(this, AniState.Born);
        }

        public void Idle()
        {
            stateMgr.ChangeState(this, AniState.Idle);
        }

        public void Move()
        {
            stateMgr.ChangeState(this, AniState.Move);
        }

        public void Attack(int skillID)
        {
            stateMgr.ChangeState(this, AniState.Attack, skillID);
        }

        public void Hit()
        {
            stateMgr.ChangeState(this, AniState.Hit);
        }

        public void Die()
        {
            stateMgr.ChangeState(this, AniState.Die);
        }

        public void SetController(Controller.Controller ctrl)
        {
            controller = ctrl;
        }

        public void SetActive(bool active = true)
        {
            if (controller != null)
            {
                controller.gameObject.SetActive(active);
            }
        }

        public virtual void SetBlend(float blend)
        {
            if (controller != null)
            {
                controller.SetBlend(blend);
            }
        }

        public virtual void SetDir(Vector2 dir)
        {
            if (controller != null)
            {
                controller.Dir = dir;
            }
        }

        public virtual void SetAction(int act)
        {
            if (controller != null)
            {
                controller.SetAction(act);
            }
        }

        public virtual void SetFX(string name, float destroy)
        {
            if (controller != null)
            {
                controller.SetFX(name, destroy);
            }
        }

        public void SetCharacterCotoller(bool enable)
        {
            controller.GetComponent<CharacterController>().enabled = enable;
        }

        public void SetSkillMoveState(bool move, float speed = 0)
        {
            if (controller != null)
            {
                controller.SetSkillMoveState(move, speed);
            }
        }

        public virtual void SetBattleProps(BattleProps props)
        {
            HP = props.hp;
            Props = props;
        }

        #region 战斗信息显示

        public virtual void SetDodge()
        {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }

        public virtual void SetCritical(int critical)
        {
            GameRoot.Instance.dynamicWnd.SetCritical(Name, critical);
        }

        public virtual void Sethurt(int hurt)
        {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }

        public virtual void SetHPVal(int curHP)
        {
            GameRoot.Instance.dynamicWnd.SetHPVal(Name, curHP);
        }

        #endregion

        public virtual void SetAtkRotation(Vector2 dir, bool isOffset = true)
        {
            if (controller != null)
            {
                controller.SetAtkRotationCam(dir, isOffset);
            }
        }

        public virtual void SkillAttack(int skillID)
        {
            skillMgr.SkillAttack(this, skillID);
        }

        public virtual Vector2 GetDirInput()
        {
            return Vector2.zero;
        }

        public Vector3 GetPos()
        {
            return controller.transform.position;
        }

        public Transform GetTrans()
        {
            return controller.transform;
        }

        public AudioSource GetAudio()
        {
            return audioSource ??= controller.GetComponent<AudioSource>();
        }


        public AnimationClip[] GetAniClips()
        {
            if (controller != null)
            {
                return controller.ani.runtimeAnimatorController.animationClips;
            }

            return null;
        }

        public virtual bool GetBreakState()
        {
            return true;
        }

        /// <summary>
        /// 计算最近的目标方向
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 CalcTargetDir()
        {
            return Vector2.zero;
        }

        public void ExitCurSkill()
        {
            SetAction(Constants.ActionIdle);
            canControl = true;
            entityState = EntityState.Normal;
            if (curSkill != null && curSkill.isCombo)
            {
                if (comboQue.Count > 0)
                {
                    nextSkillID = comboQue.Dequeue();
                }
                else
                {
                    nextSkillID = 0;
                }
            }

            curSkill = null;
        }

        public void RmvSkillCB()
        {
            SetSkillMoveState(false);
            SetDir(Vector2.zero);
            while (skMoveCBLst.Count > 0)
            {
                TimerSvc.Instance.DelTask(skMoveCBLst[0]);
                skMoveCBLst.RemoveAt(0);
            }

            while (skActionCBLst.Count > 0)
            {
                TimerSvc.Instance.DelTask(skActionCBLst[0]);
                skActionCBLst.RemoveAt(0);
            }

            if (skEndCB != -1)
            {
                TimerSvc.Instance.DelTask(skEndCB);
                skEndCB = -1;
            }

            comboQue.Clear();
            nextSkillID = 0;
            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }

        public virtual void TickAILogic()
        {
        }

        public void RmvMoveCB(int tid)
        {
            for (int i = 0; i < skMoveCBLst.Count; i++)
            {
                if (skActionCBLst[i] == tid)
                {
                    skMoveCBLst.RemoveAt(i);
                    break;
                }
            }
        }

        public void RmvActionCB(int tid)
        {
            for (int i = 0; i < skActionCBLst.Count; i++)
            {
                if (skActionCBLst[i] == tid)
                {
                    skActionCBLst.RemoveAt(i);
                    break;
                }
            }
        }
    }
}