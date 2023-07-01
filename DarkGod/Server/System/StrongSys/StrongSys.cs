using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;

namespace Server.System.StrongSys
{
    public class StrongSys
    {
        private static StrongSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        public static StrongSys Instance
        {
            get { return instance ?? (instance = new StrongSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("StrongSys initialized");
        }

        public void RspStrong(MsgPack pack)
        {
            ReqStrong data = pack.msg.reqStrong;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspStrong
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            int curStarLv = pd.strongArr[data.pos];
            if (curStarLv == 10)
            {
                msg.err = (int)ErrorCode.ExceedMaxLevel;
            }
            else
            {
                StrongCfg nextSc = cfgSvc.GetStrongCfg(data.pos, curStarLv + 1);

                if (pd.lv < nextSc.minlv)
                {
                    msg.err = (int)ErrorCode.LackLevel;
                }
                else if (pd.coin < nextSc.coin)
                {
                    msg.err = (int)ErrorCode.LackCoin;
                }
                else if (pd.crystal < nextSc.crystal)
                {
                    msg.err = (int)ErrorCode.LackCrystal;
                }
                else
                {
                    pd.coin -= nextSc.coin;
                    pd.crystal -= nextSc.crystal;
                    pd.strongArr[data.pos]++;
                    pd.hp += nextSc.addhp;
                    pd.ad += nextSc.addhurt;
                    pd.ap += nextSc.addhurt;
                    pd.addef += nextSc.adddef;
                    pd.apdef += nextSc.adddef;

                    //更新数据库
                    if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                    {
                        msg.err = (int)ErrorCode.UpdateDBError;
                    }
                    else
                    {
                        msg.pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd,3);
                        msg.rspStrong = new RspStrong()
                        {
                            coin = pd.coin,
                            crystal = pd.crystal,
                            hp = pd.hp,
                            ad = pd.ad,
                            ap = pd.ap,
                            adddef = pd.addef,
                            apdef = pd.apdef,
                            strongArr = pd.strongArr
                        };
                    }
                }
            }

            pack.session.SendMsg(msg);
        }
    }
}