using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;

namespace Server.System.GuideSys
{
    /// <summary>
    /// 引导业务系统
    /// </summary>
    public class GuideSys
    {
        private static GuideSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        public static GuideSys Instance
        {
            get { return instance ?? (instance = new GuideSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("GuideSys initialized");
        }

        public void ReqGuide(MsgPack pack)
        {
            ReqGuide data = pack.msg.reqGuide;

            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspGuide
            };
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            GuideCfg gc = cfgSvc.GetGuideCfg(pd.guideid);
            //更新任务ID
            if (pd.guideid == data.guideid)
            {
                PshTaskPrg pshTaskPrg = null;
                //智者点播任务检测
                if (pd.guideid == 1001)
                {
                    pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd,1);
                }
                pd.guideid++;

                //更新玩家数据
                pd.coin += gc.coin;
                PECommon.CalcExp(pd, gc.exp);

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.rspGuide = new RspGuide()
                    {
                        guideid = pd.guideid,
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp
                    };
                    msg.pshTaskPrg = pshTaskPrg;
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