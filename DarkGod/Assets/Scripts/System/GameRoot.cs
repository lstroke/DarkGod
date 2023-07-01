using PEProtocol;
using Service;
using UIWindow;
using UnityEngine;

namespace System
{
    /// <summary>
    /// 游戏启动入口
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;
        public LoadingWnd loadingWnd;
        public DynamicWnd dynamicWnd;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            print("Game Start...");
            ClearUIRoot();
            Init();
        }

        private void ClearUIRoot()
        {
            Transform canvas = transform.Find("Canvas");
            for (int i = 0; i < canvas.childCount; i++)
            {
                canvas.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void Init()
        {
            //服务模块初始化
            NetSvc netSvc = GetComponent<NetSvc>();
            netSvc.InitSvc();

            ResSvc resSvc = GetComponent<ResSvc>();
            resSvc.InitSvc();

            AudioSvc audioSvc = GetComponent<AudioSvc>();
            audioSvc.InitSvc();

            TimerSvc timerSvc = GetComponent<TimerSvc>();
            timerSvc.InitSvc();

            //业务系统初始化
            LoginSys loginSys = GetComponent<LoginSys>();
            loginSys.InitSys();
            MainCitySys mainCitySys = GetComponent<MainCitySys>();
            mainCitySys.InitSys();
            FubenSys fubenSys = GetComponent<FubenSys>();
            fubenSys.InitSys();
            BattleSys battleSys = GetComponent<BattleSys>();
            battleSys.InitSys();

            dynamicWnd.SetWndState();
            //进入登录场景
            loginSys.EnterLogin();
        }

        public static void AddTips(string tips)
        {
            Instance.dynamicWnd.AddTips(tips);
        }

        private PlayerData playerData;

        public PlayerData PlayerData
        {
            get => playerData;
        }

        public void SetPlayerData(RspLogin data)
        {
            playerData = data.playerData;
        }

        public void SetPlayerName(string name)
        {
            PlayerData.name = name;
        }

        public void SetPlayerDataByGuide(RspGuide data)
        {
            playerData.coin = data.coin;
            playerData.lv = data.lv;
            playerData.exp = data.exp;
            playerData.guideid = data.guideid;
        }

        public void SetPlayerDataByStrong(RspStrong data)
        {
            playerData.coin = data.coin;
            playerData.crystal = data.crystal;
            playerData.hp = data.hp;
            playerData.ad = data.ad;
            playerData.ap = data.ap;
            playerData.addef = data.adddef;
            playerData.apdef = data.apdef;
            playerData.strongArr = data.strongArr;
        }

        public void SetPlayerDataByBuy(RspBuy data)
        {
            playerData.diamond = data.diamond;
            playerData.coin = data.coin;
            playerData.power = data.power;
        }

        public void SetPlayerDataByPower(PshPower data)
        {
            if (playerData != null)
            {
                playerData.power = data.power;
            }
        }

        public void SetPlayerDataByTaskReward(RspTaskReward data)
        {
            playerData.coin = data.coin;
            playerData.lv = data.lv;
            playerData.exp = data.exp;
            playerData.taskArr = data.taskArr;
        }

        public void SetPlayerDataByPshTask(PshTaskPrg data)
        {
            playerData.taskArr = data.taskArr;
        }

        public void SetPlayerDataByFBStart(RspFBFight data)
        {
            playerData.power = data.power;
            playerData.usepower = data.usepower;
        }
        
        public void SetPlayerDataByFBEnd(RspFBFightEnd data)
        {
            playerData.coin = data.coin;
            playerData.lv = data.lv;
            playerData.exp = data.exp;
            playerData.critical = data.crystal;
            playerData.fuben = data.fuben;
        }
    }
}