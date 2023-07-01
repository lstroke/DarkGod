using Service;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 业务系统基类
    /// </summary>
    public class SystemRoot : MonoBehaviour
    {
        protected ResSvc resSvc;
        protected AudioSvc audioSvc;
        protected NetSvc netSvc;
        protected TimerSvc timerSvc;

        public virtual void InitSys()
        {
            resSvc = ResSvc.Instance;
            audioSvc = AudioSvc.Instance;
            netSvc = NetSvc.Instance;
            timerSvc = TimerSvc.Instance;
        }
    }
}