using System;
using PENet;
using PEProtocol;
using Service;
using UnityEngine;

namespace Net
{
    public class ClientSession : PESession<GameMsg>
    {
        protected override void OnConnected()
        {
            PECommon.Log("Server Connected");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PECommon.Log("RcvPack CMD:" + ((CMD)msg.cmd).ToString());
            NetSvc.Instance.AddNetMsg(msg);
        }

        protected override void OnDisConnected()
        {
            GameRoot.AddTips("服务连接断开");
            PECommon.Log("Client DisConnected");
        }
    }
}