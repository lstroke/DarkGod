using System;
using System.Collections.Generic;
using PEProtocol;

namespace Server.Service.TimerSvc
{
    /// <summary>
    /// 定时服务
    /// </summary>
    public class TimerSvc
    {
        public class TaskPack
        {
            public int tid;
            public Action<int> cb;

            public TaskPack(int tid, Action<int> cb)
            {
                this.tid = tid;
                this.cb = cb;
            }
        }

        private static TimerSvc instance;
        private PETimer pt;
        private Queue<TaskPack> tpQue = new Queue<TaskPack>();
        private static readonly string tpQueLock = "tpQueLock";

        public static TimerSvc Instance
        {
            get { return instance ?? (instance = new TimerSvc()); }
        }

        public void Init()
        {
            pt = new PETimer(100);
            pt.SetLog((info) => { PECommon.Log(info); });
            pt.SetHandle((cb, tid) =>
            {
                if (cb != null)
                {
                    lock (tpQueLock)
                    {
                        tpQue.Enqueue(new TaskPack(tid, cb));
                    }
                }
            });
            PECommon.Log("TimerSvc initialized");
        }

        public void Update()
        {
            while (tpQue.Count > 0)
            {
                TaskPack tp;
                lock (tpQueLock)
                {
                    tp = tpQue.Dequeue();
                }

                tp?.cb(tp.tid);
            }
        }

        public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond,
            int count = 1)
        {
            return pt.AddTimeTask(callback, delay, timeUnit, count);
        }

        public long GetNowTime()
        {
            return (long)pt.GetMillisecondsTime();
        }
    }
}