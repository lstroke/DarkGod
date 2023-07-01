using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 登录注册界面
    /// </summary>
    public class LoginWnd : WindowRoot
    {
        public InputField iptAcct;
        public InputField iptPwd;
        public Button btnEnter;
        public Button btnNotice;

        protected override void InitWnd()
        {
            base.InitWnd();
            //获取本地存储的账号密码
            if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pwd"))
            {
                iptAcct.text = PlayerPrefs.GetString("Acct");
                iptPwd.text = PlayerPrefs.GetString("Pwd");
            }
            else
            {
                iptAcct.text = "";
                iptPwd.text = "";
            }
        }

        /// <summary>
        /// 点击进入游戏
        /// </summary>
        public void ClickEnterBtn()
        {
            audioSvc.PlayUIMusic(Constants.UILoginBtn);
            string _acct = iptAcct.text;
            string _pwd = iptPwd.text;
            if (_acct != "" && _pwd != "")
            {
                //发送网络消息，请求登录
                GameMsg gameMsg = new GameMsg
                {
                    cmd = (int)CMD.ReqLogin,
                    reqLogin = new ReqLogin()
                    {
                        acct =  _acct,
                        pwd = _pwd
                    }
                };
                netSvc.SendMsg(gameMsg);
            }
            else
            {
                GameRoot.AddTips("账号和密码不能为空");
            }
        }

        public void ClickNoticeBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            GameRoot.AddTips("功能正在开发中...");
        }
    }
}