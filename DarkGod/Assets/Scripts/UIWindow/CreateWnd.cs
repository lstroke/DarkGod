using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace UIWindow
{
    /// <summary>
    /// 角色创建界面
    /// </summary>
    public class CreateWnd : WindowRoot
    {
        public InputField iptName;

        protected override void InitWnd()
        {
            base.InitWnd();
            iptName.text = resSvc.GetRDNameData(false);
        }

        public void ClickRandBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            iptName.text = resSvc.GetRDNameData(Convert.ToBoolean(PETools.RDInt(0, 1)));
        }

        public void ClickEnterBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            if (iptName.text != "" && iptName.text.Length < 8)
            {
                //发送名字到服务器
                GameMsg msg = new GameMsg()
                {
                    cmd = (int)CMD.ReqRename,
                    reqRename = new ReqRename()
                    {
                        name = iptName.text
                    }
                };
                netSvc.SendMsg(msg);
            }
            else
            {
                GameRoot.AddTips("当前名字不符合规范，请输入1到7个字符");
            }
        }
    }
}