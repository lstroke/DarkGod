using System;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 飞龙循环动画
    /// </summary>
    public class LoopDragonAni : MonoBehaviour
    {
        private Animation ani;

        private void Awake()
        {
            ani = transform.GetComponent<Animation>();
        }

        private void Start()
        {
            if (ani)
            {
                InvokeRepeating("PlayDragonAni",0,20);
            }
        }

        private void PlayDragonAni()
        {
            if (ani)
            {
                ani.Play();
            }
        }
    }
}