using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;
using Server.Service.TimerSvc;

namespace Server.System.TaskSys
{
    public class TaskSys
    {
        private static TaskSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;
        private TimerSvc timerSvc;

        public static TaskSys Instance
        {
            get { return instance ?? (instance = new TaskSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            timerSvc = TimerSvc.Instance;
            PECommon.Log("TaskSys initialized");
        }

        public void ReqTakeReward(MsgPack pack)
        {
            ReqTaskReward data = pack.msg.reqTaskReward;
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspTaskReward
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
            TaskRewardData trd =CalcTaskRewardData(pd, data.rid);

            if (trd.prgs == trc.count && !trd.taked)
            {
                pd.coin += trc.coin;
                PECommon.CalcExp(pd, trc.exp);
                trd.taked = true;
                //更新任务进度数据
                CalcTaskArr(pd, trd);

                if (cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.rspTaskReward = new RspTaskReward()
                    {
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        taskArr = pd.taskArr
                    };
                }
                else
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
            }
            else
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }

            pack.session.SendMsg(msg);
        }

        private TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
        {
            TaskRewardData trd = null;
            for (int i = 0; i < pd.taskArr.Length; i++)
            {
                string[] taskinfo = pd.taskArr[i].Split('|');
                if (int.Parse(taskinfo[0]) == rid)
                {
                    trd = new TaskRewardData()
                    {
                        ID = rid,
                        prgs = int.Parse(taskinfo[1]),
                        taked = "1".Equals(taskinfo[2])
                    };
                    break;
                }
            }

            return trd;
        }

        public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
        {
            string result = trd.ID + "|" + trd.prgs + "|" + (trd.taked ? "1" : "0");
            for (int i = 0; i < pd.taskArr.Length; i++)
            {
                if (int.Parse(pd.taskArr[i].Split('|')[0]) == trd.ID)
                {
                    pd.taskArr[i] = result;
                    return;
                }
            }
        }

        public PshTaskPrg CalcTaskPrg(PlayerData pd, int tid)
        {
            TaskRewardData trd = CalcTaskRewardData(pd, tid);
            TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);
            if (trd.prgs < trc.count)
            {
                trd.prgs++;

                CalcTaskArr(pd, trd);

                return new PshTaskPrg()
                {
                    taskArr = pd.taskArr
                };
            }

            return null;
        }
    }
}