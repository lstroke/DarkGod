using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;

namespace Server.System.BuySys
{
    /// <summary>
    /// 交易购买系统
    /// </summary>
    public class BuySys
    {
        private static BuySys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        public static BuySys Instance
        {
            get { return instance ?? (instance = new BuySys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("BuySys initialized");
        }

        public void ReqBuy(MsgPack pack)
        {
            ReqBuy data = pack.msg.reqBuy;
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspBuy
            };
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            switch (data.type)
            {
                case 0:
                    if (pd.diamond < 10)
                    {
                        msg.err = (int)ErrorCode.LackDiamond;
                    }
                    else
                    {
                        msg.pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd, 4);
                        pd.diamond -= 10;
                        pd.power += 100;
                    }

                    break;
                case 1:
                    if (pd.diamond < 10)
                    {
                        msg.err = (int)ErrorCode.LackDiamond;
                    }
                    else
                    {
                        msg.pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd, 5);
                        pd.diamond -= 10;
                        pd.coin += 1000;
                    }

                    break;
            }

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspBuy = new RspBuy()
                {
                    type = data.type,
                    diamond = pd.diamond,
                    coin = pd.coin,
                    power = pd.power
                };
            }

            pack.session.SendMsg(msg);
        }
    }
}