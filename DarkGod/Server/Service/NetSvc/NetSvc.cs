using System.Collections.Generic;
using PENet;
using PEProtocol;
using Server.System.BuySys;
using Server.System.ChatSys;
using Server.System.FubenSys;
using Server.System.GuideSys;
using Server.System.LoginSys;
using Server.System.StrongSys;
using Server.System.TaskSys;

namespace Server.Service.NetSvc
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetSvc
    {
        private static NetSvc instance;

        public static NetSvc Instance
        {
            get { return instance ?? (instance = new NetSvc()); }
        }

        public void Init()
        {
            PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
            server.StartAsServer(SvcCfg.srvIP, SvcCfg.srvPort);

            PECommon.Log("NetSvc initialized");
        }

        public static readonly string obj = "lock";
        private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

        public void AddMsgQue(MsgPack pack)
        {
            lock (obj)
            {
                msgPackQue.Enqueue(pack);
            }
        }

        public void Update()
        {
            lock (obj)
            {
                if (msgPackQue.Count > 0)
                {
                    PECommon.Log("PackCount:" + msgPackQue.Count);
                    var pack = msgPackQue.Dequeue();
                    HandOutMsg(pack);
                }
            }
        }

        private void HandOutMsg(MsgPack pack)
        {
            switch ((CMD)pack.msg.cmd)
            {
                case CMD.ReqLogin:
                    LoginSys.Instance.ReqLogin(pack);
                    break;
                case CMD.ReqRename:
                    LoginSys.Instance.ReqRename(pack);
                    break;
                case CMD.ReqGuide:
                    GuideSys.Instance.ReqGuide(pack);
                    break;
                case CMD.ReqStrong:
                    StrongSys.Instance.RspStrong(pack);
                    break;
                case CMD.SndChat:
                    ChatSys.Instance.SndChat(pack);
                    break;
                case CMD.ReqBuy:
                    BuySys.Instance.ReqBuy(pack);
                    break;
                case CMD.ReqTaskReward:
                    TaskSys.Instance.ReqTakeReward(pack);
                    break;
                case CMD.ReqFBFight:
                    FubenSys.Instance.ReqFBFight(pack);
                    break;
                case CMD.ReqFBFightEnd:
                    FubenSys.Instance.ReqFBFightEnd(pack);
                    break;
            }
        }
    }

    public class MsgPack
    {
        public ServerSession session;
        public GameMsg msg;

        public MsgPack(ServerSession session, GameMsg msg)
        {
            this.session = session;
            this.msg = msg;
        }
    }
}