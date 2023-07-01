using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;
using Server.Service.TimerSvc;

namespace Server.System.FubenSys
{
    /// <summary>
    /// 副本系统
    /// </summary>
    public class FubenSys
    {
        private static FubenSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;
        private TimerSvc timerSvc;

        public static FubenSys Instance
        {
            get { return instance ?? (instance = new FubenSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            timerSvc = TimerSvc.Instance;
            PECommon.Log("FubenSys initialized");
        }

        public void ReqFBFight(MsgPack pack)
        {
            ReqFBFight data = pack.msg.reqFBFight;
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspFBFight
            };
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            int power = cfgSvc.GetMapCfgData(data.fbid).power;
            if (pd.fuben < data.fbid)
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }
            else if (pd.power < power)
            {
                msg.err = (int)ErrorCode.LackPower;
            }
            else
            {
                if (power > 0 && pd.power == PECommon.GetPowerLimit(pd.lv))
                {
                    pd.usepower = timerSvc.GetNowTime();
                }

                pd.power -= power;
                if (cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.rspFBFight = new RspFBFight()
                    {
                        fbid = data.fbid,
                        power = pd.power,
                        usepower = pd.usepower
                    };
                }
                else
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
            }

            pack.session.SendMsg(msg);
        }

        public void ReqFBFightEnd(MsgPack pack)
        {
            ReqFBFightEnd data = pack.msg.reqFbFightEnd;
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspFBFightEnd
            };

            if (data.win)
            {
                if (data.costTime > 10 && data.restHP > 0)
                {
                    MapCfg mc = cfgSvc.GetMapCfgData(data.fbid);
                    PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
                    pd.coin += mc.coin;
                    pd.crystal += mc.crystal;
                    PECommon.CalcExp(pd, mc.exp);
                    if (pd.fuben == data.fbid)
                    {
                        pd.fuben++;
                    }

                    if (cacheSvc.UpdatePlayerData(pd.id, pd))
                    {
                        RspFBFightEnd rspFbFightEnd = new RspFBFightEnd
                        {
                            win = data.win,
                            fbid = data.fbid,
                            restHP = data.restHP,
                            costTime = data.costTime,

                            coin = pd.coin,
                            lv = pd.lv,
                            exp = pd.exp,
                            crystal = pd.crystal,
                            fuben = pd.fuben
                        };
                        msg.rspFbFightEnd = rspFbFightEnd;
                        msg.pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd, 2);
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
            }
            else
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }

            pack.session.SendMsg(msg);
        }
    }
}