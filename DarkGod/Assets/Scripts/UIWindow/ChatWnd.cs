using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    public class ChatWnd : WindowRoot
    {
        public Image imgWorld;
        public Image imgGuild;
        public Image imgFriend;
        public Text txtChat;
        public InputField iptChat;

        private int chatType;
        private List<string> chatList = new();
        private bool canSend = true;

        protected override void InitWnd()
        {
            base.InitWnd();

            chatType = 0;

            RefreshUI();
        }

        public void RefreshUI()
        {
            SetSprite(imgWorld, PathDefine.btnType2);
            SetSprite(imgGuild, PathDefine.btnType2);
            SetSprite(imgFriend, PathDefine.btnType2);
            switch (chatType)
            {
                case 0:
                    string chatMsg = string.Join('\n', chatList);
                    SetText(txtChat, chatMsg);
                    SetSprite(imgWorld, PathDefine.btnType1);
                    break;
                case 1:
                    SetText(txtChat, "尚未加入工会");
                    SetSprite(imgGuild, PathDefine.btnType1);
                    break;
                case 2:
                    SetText(txtChat, "暂无好友信息");
                    SetSprite(imgFriend, PathDefine.btnType1);
                    break;
            }
        }

        public void AddChatMsh(string chatName, string chat)
        {
            chatList.Add(Constants.Color(chatName + "：", TxtColor.Blue) + chat);
            if (chatList.Count > 12)
            {
                chatList.RemoveAt(0);
            }

            //显示是才刷新消息窗口，防止未调用init是出现引用报错
            if (GetWndState())
            {
                RefreshUI();
            }
        }

        public void ClickWorldBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            chatType = 0;
            RefreshUI();
        }

        public void ClickGuildBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            chatType = 1;
            RefreshUI();
        }

        public void ClickFriendBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            chatType = 2;
            RefreshUI();
        }

        public void ClickSendBtn()
        {
            if (!canSend)
            {
                GameRoot.AddTips("聊天消息每5秒只能发送一次");
                return;
            }

            string chat = iptChat.text.Trim();
            if (chat == "")
            {
                GameRoot.AddTips("尚未输入聊天信息");
            }
            else if (chat.Length > 12)
            {
                GameRoot.AddTips("输入的信息不能超过12个字");
            }
            else
            {
                GameMsg msg = new GameMsg()
                {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat()
                    {
                        chat = chat
                    }
                };
                iptChat.text = "";
                netSvc.SendMsg(msg);
                canSend = false;
                timerSvc.AddTimeTask((tid) => { canSend = true; }, 5, PETimeUnit.Second);
            }
        }

        public void ClickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }
    }
}