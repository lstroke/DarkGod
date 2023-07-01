using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    public class BattleEndWnd : WindowRoot
    {
        public Transform rewardTrans;
        public Button btnClose;
        public Button btnExit;
        public Button btnSure;
        public Text txtTime;
        public Text txtRestHP;
        public Text txtReward;
        public Animation ani;

        private FBEndType endType = FBEndType.None;

        protected override void InitWnd()
        {
            base.InitWnd();

            RefreshUI();
        }

        public void SetEndType(FBEndType endType)
        {
            this.endType = endType;
        }


        private int fbid;
        private int costTime;
        private int restHP;

        public void SetBattleEndData(int fbid, int costTime, int restHP)
        {
            this.fbid = fbid;
            this.costTime = costTime;
            this.restHP = restHP;
        }

        private void RefreshUI()
        {
            SetActive(btnExit.gameObject, false);
            SetActive(btnClose.gameObject, false);
            SetActive(rewardTrans, false);
            switch (endType)
            {
                case FBEndType.None:
                    break;
                case FBEndType.Pause:
                    SetActive(btnExit.gameObject);
                    SetActive(btnClose.gameObject);
                    Time.timeScale = 0;
                    break;
                case FBEndType.Win:
                    MapCfg mc = resSvc.GetMapCfgData(fbid);
                    int min = costTime / 60;
                    int sec = costTime % 60;
                    int coin = mc.coin;
                    int exp = mc.exp;
                    int crystal = mc.crystal;
                    SetText(txtTime, "通关事件：" + min + ":" + sec);
                    SetText(txtRestHP, "剩余血量：" + restHP);
                    SetText(txtReward,
                        "关卡奖励：" + Constants.Color(coin + "金币 ", TxtColor.Green) +
                        Constants.Color(exp + "经验 ", TxtColor.Yellow) +
                        Constants.Color(crystal + "水晶 ", TxtColor.Blue));
                    timerSvc.AddTimeTask((tid) =>
                    {
                        SetActive(rewardTrans);
                        ani.Play();
                    }, 1000);
                    break;
                case FBEndType.Lose:
                    SetActive(btnExit.gameObject);
                    audioSvc.PlayUIMusic(Constants.FBLose);
                    break;
            }
        }

        public void ClickCloseBtn()
        {
            Time.timeScale = 1;
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }

        public void ClickExitBtn()
        {
            Time.timeScale = 1;
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            //进入主城销毁当前战斗
            MainCitySys.Instance.EnterMainCity();
            BattleSys.Instance.DestroyBattle();
        }

        public void ClickSureBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            //进入主城销毁当前战斗
            MainCitySys.Instance.EnterMainCity();
            BattleSys.Instance.DestroyBattle();
            FubenSys.Instance.EnterFuben();
        }
    }

    public enum FBEndType
    {
        None,
        Pause,
        Win,
        Lose
    }
}