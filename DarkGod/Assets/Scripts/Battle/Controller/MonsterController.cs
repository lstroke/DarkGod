using System;
using Common;
using UnityEngine;

namespace Battle.Controller
{
    /// <summary>
    /// 怪物表现实体角色控制器
    /// </summary>
    public class MonsterController : Controller
    {
        private void Update()
        {
            if (isMove)
            {
                SetDir();
                SetMove();
            }
        }

        private void SetDir()
        {
            float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
            Vector3 eulerAngle = new Vector3(0, angle, 0);
            transform.localEulerAngles = eulerAngle;
        }

        private void SetMove()
        {
            ctrl.Move(Constants.MonsterMoveSpeed * Time.deltaTime * transform.forward);
            ctrl.Move(Constants.PlayerMoveSpeed * Time.deltaTime * Vector3.down);
        }
    }
}