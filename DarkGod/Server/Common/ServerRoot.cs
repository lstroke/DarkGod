using System;
using Server.Cache;
using Server.DB;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;
using Server.Service.TimerSvc;
using Server.System.BuySys;
using Server.System.ChatSys;
using Server.System.FubenSys;
using Server.System.GuideSys;
using Server.System.LoginSys;
using Server.System.PowerSys;
using Server.System.StrongSys;
using Server.System.TaskSys;

namespace Server.Common
{
    /// <summary>
    /// 服务器初始化
    /// </summary>
    public class ServerRoot
    {
        private static ServerRoot instance;

        public static ServerRoot Instance
        {
            get { return instance ?? (instance = new ServerRoot()); }
        }

        public void Init()
        {
            //数据层
            DBMgr.Instance.Init();
            //服务层
            CfgSvc.Instance.Init();
            CacheSvc.Instance.Init();
            NetSvc.Instance.Init();
            TimerSvc.Instance.Init();
            //业务系统层
            LoginSys.Instance.Init();
            GuideSys.Instance.Init();
            StrongSys.Instance.Init();
            ChatSys.Instance.Init();
            BuySys.Instance.Init();
            PowerSys.Instance.Init();
            TaskSys.Instance.Init();
            FubenSys.Instance.Init();
        }

        public void Update()
        {
            NetSvc.Instance.Update();
            TimerSvc.Instance.Update();
        }

        private int sessionID = 0;

        public int GetSessionID()
        {
            if (sessionID == Int32.MaxValue)
            {
                sessionID = 0;
            }

            return sessionID++;
        }
    }
}