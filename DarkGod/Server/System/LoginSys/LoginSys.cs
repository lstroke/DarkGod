using System;
using PEProtocol;
using Server.Cache;
using Server.Service.NetSvc;
using Server.Service.TimerSvc;

namespace Server.System.LoginSys
{
    /// <summary>
    /// 登录业务系统
    /// </summary>
    public class LoginSys
    {
        private static LoginSys instance;
        private CacheSvc cacheSvc;
        private TimerSvc timerSvc;

        public static LoginSys Instance
        {
            get { return instance ?? (instance = new LoginSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            timerSvc = TimerSvc.Instance;
            PECommon.Log("LoginSys initialized");
        }

        public void ReqLogin(MsgPack pack)
        {
            ReqLogin data = pack.msg.reqLogin;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspLogin,
            };
            //当前账号是否已经上线
            if (cacheSvc.IsAcctOnline(data.acct))
            {
                //已上线：返回错误信息
                msg.err = (int)ErrorCode.AcctIsOnline;
            }
            else
            {
                //未上线：
                //账号是否存在
                PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pwd);
                if (pd == null)
                {
                    //存在检测密码，密码错误
                    msg.err = (int)ErrorCode.WrongPwd;
                }
                else if (pd.id == -1)
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    //计算体力恢复
                    long now = timerSvc.GetNowTime();
                    int addPower = (int)(now - pd.usepower) / PECommon.PowerAddSpace;
                    addPower = Math.Min(addPower, PECommon.GetPowerLimit(pd.lv) - pd.power);
                    if (addPower > 0)
                    {
                        pd.power += addPower;
                        pd.usepower += addPower * PECommon.PowerAddSpace;
                        if (!cacheSvc.UpdatePlayerData(pd.id,pd))
                        {
                            msg.err = (int)ErrorCode.UpdateDBError;
                            pack.session.SendMsg(msg);
                            return;
                        }
                    }
                    
                    msg.rspLogin = new RspLogin()
                    {
                        playerData = pd
                    };
                    //缓存账号数据
                    cacheSvc.AcctOnline(data.acct, pack.session, pd);
                }
            }

            //回应客户端
            pack.session.SendMsg(msg);
        }

        public void ReqRename(MsgPack pack)
        {
            ReqRename data = pack.msg.reqRename;
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.RspRename
            };
            //名字是否存在
            if (cacheSvc.IsNameExist(data.name))
            {
                //存在：返回错误码
                msg.err = (int)ErrorCode.NameIsExist;
            }
            else
            {
                //不存在：更新缓和和数据库，再返回客户端
                PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
                playerData.name = data.name;
                //数据库更新成功
                if (cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.rspRename = new RspRename()
                    {
                        name = data.name,
                    };
                }
                else
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
            }

            pack.session.SendMsg(msg);
        }

        public void ClearOfflineData(ServerSession session)
        {
            cacheSvc.AcctOffLine(session);
        }
    }
}