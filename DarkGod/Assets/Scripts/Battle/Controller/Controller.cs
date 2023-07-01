using System.Collections.Generic;
using Service;
using UnityEngine;

namespace Battle.Controller
{
    /// <summary>
    /// 表现实体控制器基类
    /// </summary>
    public abstract class Controller : MonoBehaviour
    {
        protected TimerSvc timerSvc;

        public CharacterController ctrl;
        public Transform hpRoot;
        public Animator ani;
        protected Dictionary<string, GameObject> fxDic = new();

        private static readonly int Blend = Animator.StringToHash("Blend");
        private static readonly int AniAction = Animator.StringToHash("Action");
        protected bool isMove;
        protected bool skillMove;
        protected float skillMoveSpeed;
        protected Transform camTrans;

        private Vector2 dir;

        public Vector2 Dir
        {
            get => dir;
            set
            {
                isMove = value != Vector2.zero;

                dir = value;
            }
        }

        public virtual void Init()
        {
            timerSvc = TimerSvc.Instance;
        }

        public virtual void SetBlend(float blend)
        {
            ani.SetFloat(Blend, blend);
        }

        public virtual void SetAction(int act)
        {
            ani.SetInteger(AniAction, act);
        }

        public virtual void SetFX(string fxName, float destroy)
        {
            if (string.IsNullOrEmpty(fxName)) return;

            if (fxDic.TryGetValue(fxName, out GameObject fx))
            {
                fx.SetActive(true);
                timerSvc.AddTimeTask((tid) => { fx.SetActive(false); }, destroy);
            }
        }

        public void SetSkillMoveState(bool move, float speed = 0)
        {
            skillMove = move;
            skillMoveSpeed = speed;
        }

        public virtual void SetAtkRotationCam(Vector2 atkDir, bool isOffset = true)
        {
            float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
            if (isOffset)
            {
                angle += camTrans.localEulerAngles.y;
            }

            Vector3 eulerAngle = new Vector3(0, angle, 0);
            transform.localEulerAngles = eulerAngle;
        }
    }
}