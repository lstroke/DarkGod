using System;
using System.Collections.Generic;
using System.Linq;
using PEProtocol;
using Server.DB;
using Server.Service.NetSvc;

namespace Server.Cache
{
    /// <summary>
    /// 缓存层
    /// </summary>
    public class CacheSvc
    {
        private static CacheSvc instance;

        public static CacheSvc Instance
        {
            get { return instance ?? (instance = new CacheSvc()); }
        }

        private DBMgr dbMgr;

        public void Init()
        {
            dbMgr = DBMgr.Instance;
            PECommon.Log("CacheSvc initialized");
        }

        private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

        public bool IsAcctOnline(string acct)
        {
            return onLineAcctDic.ContainsKey(acct);
        }

        /// <summary>
        /// 根据账号密码返回对应玩家数据，密码错误返回null，账号不存在默认创建新账号
        /// </summary>
        /// <param name="acct">账号</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public PlayerData GetPlayerData(string acct, string pwd)
        {
            //从数据库查找账号数据
            return dbMgr.QueryPlayerData(acct, pwd);
        }


        /// <summary>
        /// 账号上线缓存数据
        /// </summary>
        /// <param name="acct">账号</param>
        /// <param name="session">会话</param>
        /// <param name="playerData">玩家数据</param>
        public void AcctOnline(string acct, ServerSession session, PlayerData playerData)
        {
            onLineAcctDic.Add(acct, session);
            onLineSessionDic.Add(session, playerData);
        }

        public bool IsNameExist(string name)
        {
            return dbMgr.QueryNameData(name);
        }

        public PlayerData GetPlayerDataBySession(ServerSession session)
        {
            if (onLineSessionDic.TryGetValue(session, out PlayerData playerData))
            {
                return playerData;
            }

            return null;
        }

        public ServerSession GetServerSessionByID(int id)
        {
            ServerSession session = null;
            foreach (var item in onLineSessionDic)
            {
                if (item.Value.id == id)
                {
                    session = item.Key;
                    break;
                }
            }

            return session;
        }

        public List<ServerSession> GetOnlineServerSessions()
        {
            return onLineAcctDic.Values.ToList();
        }

        public bool UpdatePlayerData(int id, PlayerData playerData)
        {
            return dbMgr.UpdatePlayerData(id, playerData);
        }

        public void AcctOffLine(ServerSession session)
        {
            foreach (var item in onLineAcctDic)
            {
                if (item.Value == session)
                {
                    onLineAcctDic.Remove(item.Key);
                    break;
                }
            }

            UpdatePlayerData(onLineSessionDic[session].id, onLineSessionDic[session]);
            bool succ = onLineSessionDic.Remove(session);
            PECommon.Log("OffLine Result:" + succ + " SessionID:" + session.sessionID);
        }

        public Dictionary<ServerSession, PlayerData> GetOnlinePlayers()
        {
            return onLineSessionDic;
        }
    }
}