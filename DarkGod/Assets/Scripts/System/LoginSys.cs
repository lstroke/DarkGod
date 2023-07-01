using Common;
using PEProtocol;
using UIWindow;
using UnityEngine;

namespace System
{
    /// <summary>
    /// 登录注册业务系统
    /// </summary>
    public class LoginSys : SystemRoot
    {
        public static LoginSys Instance;
        public LoginWnd loginWnd;
        public CreateWnd createWnd;

        public override void InitSys()
        {
            base.InitSys();
            Instance = this;
            print("Init LoginSys...");
        }

        /// <summary>
        /// 进入登录场景
        /// </summary>
        public void EnterLogin()
        {
            //异步加载登录场景
            resSvc.AsyncLoadScene(Constants.SceneLogin, () =>
            {
                //加载完成以后再打开登录注册页面
                loginWnd.SetWndState();
                audioSvc.PlayBGMusic(Constants.BGLogin);
            });
        }

        public void RspLogin(GameMsg msg)
        {
            //更新本地存储的账号密码
            PlayerPrefs.SetString("Acct", loginWnd.iptAcct.text);
            PlayerPrefs.SetString("Pwd", loginWnd.iptPwd.text);

            GameRoot.AddTips("登录成功");
            GameRoot.Instance.SetPlayerData(msg.rspLogin);
            if (msg.rspLogin.playerData.name == "")
            {
                //打开角色创建界面
                createWnd.SetWndState();
            }
            else
            {
                //进入主城
                MainCitySys.Instance.EnterMainCity();
            }

            //关闭登录界面
            loginWnd.SetWndState(false);
        }

        public void RspRename(GameMsg msg)
        {
            GameRoot.Instance.SetPlayerName(msg.rspRename.name);
            //跳转场景进入主城
            //打开主城的界面
            MainCitySys.Instance.EnterMainCity();
            //关闭创建界面
            createWnd.SetWndState(false);
        }
    }
}