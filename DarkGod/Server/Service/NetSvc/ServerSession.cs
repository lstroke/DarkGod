using PENet;
using PEProtocol;
using Server.Common;
using Server.System.LoginSys;

namespace Server.Service.NetSvc
{
    public class ServerSession : PESession<GameMsg>
    {
        public int sessionID;

        protected override void OnConnected()
        {
            sessionID = ServerRoot.Instance.GetSessionID();
            PECommon.Log("SessionID:" + sessionID + " Client Connected");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PECommon.Log("SessionID:" + sessionID + " RcvPack CMD:" + (CMD)msg.cmd);
            NetSvc.Instance.AddMsgQue(new MsgPack(this, msg));
        }

        protected override void OnDisConnected()
        {
            LoginSys.Instance.ClearOfflineData(this);
            PECommon.Log("SessionID:" + sessionID + " Client DisConnected");
        }
    }
}