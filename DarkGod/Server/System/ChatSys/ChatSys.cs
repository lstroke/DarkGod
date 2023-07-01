using System.Collections.Generic;
using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;

namespace Server.System.ChatSys
{
    /// <summary>
    /// 聊天业务系统
    /// </summary>
    public class ChatSys
    {
        private static ChatSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        public static ChatSys Instance
        {
            get { return instance ?? (instance = new ChatSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("ChatSys initialized");
        }

        public void SndChat(MsgPack pack)
        {
            SndChat data = pack.msg.sndChat;
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.PshChat,
                pshChat = new PshChat()
                {
                    name = pd.name,
                    chat = data.chat
                }
            };
            //广播所有在线客户端
            List<ServerSession> lst = cacheSvc.GetOnlineServerSessions();
            //优化，将msg二进制化，节省时间
            byte[] bytes = PENet.PETool.PackNetMsg(msg);
            foreach (var session in lst)
            {
                session.SendMsg(bytes);
            }

            PshTaskPrg pshTaskPrg = TaskSys.TaskSys.Instance.CalcTaskPrg(pd, 6);
            if (pshTaskPrg != null)
            {
                if (cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    pack.session.SendMsg(new GameMsg()
                    {
                        cmd = (int)CMD.PshTaskPrg,
                        pshTaskPrg = pshTaskPrg
                    });
                }
                else
                {
                    pack.session.SendMsg(new GameMsg()
                    {
                        err = (int)ErrorCode.UpdateDBError
                    });
                }
            }
        }
    }
}