using Common;
using UnityEngine;

namespace Battle.Controller
{
    /// <summary>
    /// 主角表现实体角色控制器
    /// </summary>
    public class PlayerController : Controller
    {
        public GameObject daggerskill1fx;
        public GameObject daggerskill2fx;
        public GameObject daggerskill3fx;
        public GameObject daggeratk1fx;
        public GameObject daggeratk2fx;
        public GameObject daggeratk3fx;
        public GameObject daggeratk4fx;
        public GameObject daggeratk5fx;
  
        private Vector3 camOffset;
        private static readonly int Blend = Animator.StringToHash("Blend");
        private float targetBlend;
        private float currentBlend;

        public override void Init()
        {
            base.Init();
            camTrans = Camera.main.transform;
            camOffset = transform.position - camTrans.position;
            if (daggerskill1fx)
            {
                fxDic.Add(daggerskill1fx.name, daggerskill1fx);
                fxDic.Add(daggerskill2fx.name, daggerskill2fx);
                fxDic.Add(daggerskill3fx.name, daggerskill3fx);
                fxDic.Add(daggeratk1fx.name, daggeratk1fx);
                fxDic.Add(daggeratk2fx.name, daggeratk2fx);
                fxDic.Add(daggeratk3fx.name, daggeratk3fx);
                fxDic.Add(daggeratk4fx.name, daggeratk4fx);
                fxDic.Add(daggeratk5fx.name, daggeratk5fx);
            }
        }

        void Update()
        {
            /*float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Dir = new Vector2(h, v).normalized;*/

            if (currentBlend != targetBlend)
            {
                UpdateMixBlend();
            }

            if (skillMove)
            {
                SetSkillMove();
                //相机跟随
                SetCam();
                return;
            }

            if (isMove)
            {
                //设置方向
                SetDir();
                //产生移动
                SetMove();
                //相机跟随
                SetCam();
            }
        }

        private void SetDir()
        {
            float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + camTrans.localEulerAngles.y;
            Vector3 eulerAngle = new Vector3(0, angle, 0);
            transform.localEulerAngles = eulerAngle;
        }

        private void SetMove()
        {
            ctrl.Move(Constants.PlayerMoveSpeed * Time.deltaTime * transform.forward);
            ctrl.Move(Constants.PlayerMoveSpeed * Time.deltaTime * Vector3.down);
        }

        private void SetSkillMove()
        {
            ctrl.Move(skillMoveSpeed * Time.deltaTime * transform.forward);
        }

        public void SetCam()
        {
            if (camTrans)
            {
                camTrans.position = transform.position - camOffset;
            }
        }

        public override void SetBlend(float blend)
        {
            targetBlend = blend;
        }

        private void UpdateMixBlend()
        {
            if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime)
            {
                currentBlend = targetBlend;
            }
            else if (currentBlend > targetBlend)
            {
                currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
            }
            else
            {
                currentBlend += Constants.AccelerSpeed * Time.deltaTime;
            }

            ani.SetFloat(Blend, currentBlend);
        }
    }
}