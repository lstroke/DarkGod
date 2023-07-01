using System;
using PEProtocol;
using UnityEngine;

namespace Service
{
    public class TimerSvc : MonoBehaviour
    {
        public static TimerSvc Instance;

        private PETimer pt;

        public void InitSvc()
        {
            Instance = this;
            pt = new PETimer();
            pt.SetLog((info) => { PECommon.Log(info); });
            PECommon.Log("TimerSvc initialized");
        }

        private void Update()
        {
            pt.Update();
        }

        public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond,
            int count = 1)
        {
            return pt.AddTimeTask(callback, delay, timeUnit, count);
        }

        public void DelTask(int tid)
        {
            pt.DeleteTimeTask(tid);
        }

        public double GteNowTime()
        {
            return pt.GetMillisecondsTime();
        }
    }
}