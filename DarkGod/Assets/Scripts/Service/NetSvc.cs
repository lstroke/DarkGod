using System;
using System.Collections;
using System.Collections.Generic;
using Net;
using PENet;
using PEProtocol;
using UnityEngine;
using LogType = PEProtocol.LogType;

namespace Service
{
    public class NetSvc : MonoBehaviour
    {
        public static NetSvc Instance;
        private PESocket<ClientSession, GameMsg> client;
        private static readonly string obj = "lock";
        private Queue<GameMsg> msgQue = new();

        public void InitSvc()
        {
            Instance = this;
            client = new();
            client.StartAsClient(SvcCfg.srvIP, SvcCfg.srvPort);

            client.SetLog(true, (string msg, int lv) =>
            {
                switch (lv)
                {
                    case 0:
                        Debug.Log(msg);
                        break;
                    case 1:
                        Debug.LogWarning(msg);
                        break;
                    case 2:
                        Debug.LogWarning(msg);
                        break;
                    case 3:
                        Debug.Log(msg);
                        break;
                }
            });

            print("NetSvc initialized");
        }

        public void SendMsg(GameMsg msg)
        {
            if (client.session != null)
            {
                client.session.SendMsg(msg);
            }
            else
            {
                GameRoot.AddTips("服务器未连接");
                InitSvc();
            }
        }

        public void AddNetMsg(GameMsg msg)
        {
            lock (obj)
            {
                msgQue.Enqueue(msg);
            }
        }

        private void Update()
        {
            lock (obj)
            {
                if (msgQue.Count > 0)
                {
                    var msg = msgQue.Dequeue();
                    ProcessMsg(msg);
                }
            }
        }

        private void ProcessMsg(GameMsg msg)
        {
            if (msg.err == (int)ErrorCode.None)
            {
                switch ((CMD)msg.cmd)
                {
                    case CMD.RspLogin:
                        LoginSys.Instance.RspLogin(msg);
                        break;
                    case CMD.RspRename:
                        LoginSys.Instance.RspRename(msg);
                        break;
                    case CMD.RspGuide:
                        MainCitySys.Instance.RspGuide(msg);
                        break;
                    case CMD.RspStrong:
                        MainCitySys.Instance.RspStrong(msg);
                        break;
                    case CMD.PshChat:
                        MainCitySys.Instance.PshChat(msg);
                        break;
                    case CMD.RspBuy:
                        MainCitySys.Instance.RspBuy(msg);
                        break;
                    case CMD.PshPower:
                        MainCitySys.Instance.PshPower(msg);
                        break;
                    case CMD.RspTaskReward:
                        MainCitySys.Instance.RspTaskReward(msg);
                        break;
                    case CMD.RspFBFight:
                        FubenSys.Instance.RspFBFight(msg);
                        break;
                    case CMD.RspFBFightEnd:
                        BattleSys.Instance.RspFightEnd(msg);
                        break;
                }

                if (msg.pshTaskPrg != null)
                {
                    MainCitySys.Instance.PshTaskPrg(msg);
                }
            }
            else
            {
                switch ((ErrorCode)msg.err)
                {
                    case ErrorCode.AcctIsOnline:
                        GameRoot.AddTips("当前账号已上线");
                        break;
                    case ErrorCode.WrongPwd:
                        GameRoot.AddTips("密码错误");
                        break;
                    case ErrorCode.NameIsExist:
                        GameRoot.AddTips("改名字已存在，请换一个名字");
                        break;
                    case ErrorCode.UpdateDBError:
                        PECommon.Log("数据库更新异常", LogType.Error);
                        GameRoot.AddTips("网络不稳定");
                        break;
                    case ErrorCode.ClientDataError:
                        GameRoot.AddTips("当前客户端数据异常，请重新登录");
                        break;
                    case ErrorCode.ExceedMaxLevel:
                        GameRoot.AddTips("当前装备已经满级");
                        break;
                    case ErrorCode.LackCoin:
                        GameRoot.AddTips("金币数量不足");
                        break;
                    case ErrorCode.LackCrystal:
                        GameRoot.AddTips("水晶数量不足");
                        break;
                    case ErrorCode.LackLevel:
                        GameRoot.AddTips("角色等级不足");
                        break;
                    case ErrorCode.LackDiamond:
                        GameRoot.AddTips("钻石数量不够");
                        break;
                    case ErrorCode.LackPower:
                        GameRoot.AddTips("体力不足");
                        break;
                }
            }
        }
    }
}