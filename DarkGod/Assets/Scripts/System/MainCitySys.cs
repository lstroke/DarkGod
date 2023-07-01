using Battle.Controller;
using Common;
using PEProtocol;
using UIWindow;
using UnityEngine;
using UnityEngine.AI;
using LogType = PEProtocol.LogType;

namespace System
{
    //主城业务系统
    public class MainCitySys : SystemRoot
    {
        public static MainCitySys Instance;
        public MainCityWnd mainCityWnd;
        public InfoWnd infoWnd;
        public GuideWnd guideWnd;
        public StrongWnd strongWnd;
        public ChatWnd chatWnd;
        public BuyWnd buyWnd;
        public TaskWnd taskWnd;

        private PlayerController playerCtrl;
        private Transform charCamTrans;
        private Transform[] npcPosTrans;
        private NavMeshAgent nav;
        private bool isNavGuide;

        public override void InitSys()
        {
            base.InitSys();
            Instance = this;
            PECommon.Log("MainCitySys initialized");
        }

        private void Update()
        {
            if (isNavGuide)
            {
                playerCtrl.SetCam();
                IsArriveNavPos();
            }
        }

        public void EnterMainCity()
        {
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            MapCfg mapData = resSvc.GetMapCfgData(Constants.MainCityMapID);
            if (mapData == null)
            {
                PECommon.Log("读取地图数据异常", LogType.Error);
                GameRoot.AddTips("读取地图数据异常");
                return;
            }

            resSvc.AsyncLoadScene(mapData.sceneName, () =>
            {
                PECommon.Log("Enter MainCity...");
                //加载游戏主角
                LoadPlayer(mapData);
                //打开主城场景UI
                mainCityWnd.SetWndState();
                //播放主城背景音乐
                audioSvc.PlayBGMusic(Constants.BGMainCity);
                //加载场景信息
                var map = GameObject.FindGameObjectWithTag("MapRoot");
                MainCityMap mcm = map.GetComponent<MainCityMap>();
                npcPosTrans = mcm.NPCPosTrans;
                //设置人物展示相机
                if (charCamTrans)
                {
                    charCamTrans.gameObject.SetActive(false);
                }
            });
        }

        public void CloseMainCityWnd()
        {
            mainCityWnd.SetWndState(false);
        }

        /// <summary>
        /// 加载主角
        /// </summary>
        /// <param name="mapData">地图数据</param>
        private void LoadPlayer(MapCfg mapData)
        {
            GameObject player = resSvc.LoadPrefab(PathDefine.AssassinCityPlayer, true);
            player.transform.position = mapData.playerBornPos;
            player.transform.localEulerAngles = mapData.playerBornRote;
            player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //相机初始化
            var mainCameraTrans = Camera.main.transform;
            mainCameraTrans.position = mapData.mainCamPos;
            mainCameraTrans.localEulerAngles = mapData.mainCamRote;

            playerCtrl = player.GetComponent<PlayerController>();
            playerCtrl.Init();

            nav = playerCtrl.GetComponent<NavMeshAgent>();
        }

        public void SetMoveDir(Vector2 dir)
        {
            StopNavTask();
            playerCtrl.Dir = dir;
            playerCtrl.SetBlend(dir.magnitude);
        }

        #region 个人信息窗口

        public void OpenInfoWnd()
        {
            StopNavTask();
            if (!charCamTrans)
            {
                charCamTrans = GameObject.FindGameObjectWithTag("charShowCam").transform;
            }

            //设置人物展示相机相对位置
            charCamTrans.position = playerCtrl.transform.position + playerCtrl.transform.forward * 3 +
                                    new Vector3(0, 1.2f, 0);
            charCamTrans.eulerAngles = new Vector3(0, 180 + playerCtrl.transform.eulerAngles.y, 0);
            charCamTrans.localScale = Vector3.one;
            charCamTrans.gameObject.SetActive(true);

            GetInitRotate();
            infoWnd.SetWndState();
        }

        public void CloseInfoWnd()
        {
            RollbackRotate();
            charCamTrans.gameObject.SetActive(false);
            infoWnd.SetWndState(false);
        }

        #endregion

        #region 人物展示

        private float startRotate;
        private float initRotate;

        private void GetInitRotate()
        {
            initRotate = playerCtrl.transform.eulerAngles.y;
        }

        public void SetStartRotate()
        {
            startRotate = playerCtrl.transform.eulerAngles.y;
        }

        public void SetPlayerRotate(float rotate)
        {
            playerCtrl.transform.eulerAngles = new Vector3(0, rotate + startRotate, 0);
        }

        private void RollbackRotate()
        {
            playerCtrl.transform.eulerAngles = new Vector3(0, initRotate, 0);
        }

        #endregion

        #region 任务引导

        private AutoGuideCfg curTaskData;

        public void RunTask(AutoGuideCfg agc)
        {
            curTaskData = agc;
            //解析任务数据
            if (curTaskData.npcID != -1)
            {
                nav.enabled = true;
                isNavGuide = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcPosTrans[curTaskData.npcID].position);
                playerCtrl.SetBlend(Constants.BlendMove);
            }
            else
            {
                OpenGuideWnd();
            }
        }


        private void IsArriveNavPos()
        {
            float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[curTaskData.npcID].position);
            if (dis < 0.5f)
            {
                isNavGuide = false;
                nav.isStopped = true;
                playerCtrl.SetBlend(Constants.BlendIdle);
                nav.enabled = false;
                OpenGuideWnd();
            }
        }

        private void StopNavTask()
        {
            if (isNavGuide)
            {
                isNavGuide = false;
                nav.isStopped = true;
                nav.enabled = false;
                playerCtrl.SetBlend(Constants.BlendIdle);
            }
        }

        private void OpenGuideWnd()
        {
            guideWnd.SetWndState();
        }

        public AutoGuideCfg GetCurTaskData()
        {
            return curTaskData;
        }

        public void RspGuide(GameMsg msg)
        {
            RspGuide data = msg.rspGuide;
            GameRoot.AddTips(Constants.Color("任务奖励 金币 " + curTaskData.coin + " 经验 " + curTaskData.exp, TxtColor.Blue));
            switch (curTaskData.actID)
            {
                case 0:
                    //无操作
                    break;
                case 1:
                    //进入副本
                    EnterFuben();
                    break;
                case 2:
                    //进入强化界面
                    OpenStrongWnd();
                    break;
                case 3:
                    //进入体力购买
                    OpenBuyWnd(0);
                    break;
                case 4:
                    //进入金币铸造
                    OpenBuyWnd(1);
                    break;
                case 5:
                    //进入世界聊天
                    OpenChatWnd();
                    break;
            }

            GameRoot.Instance.SetPlayerDataByGuide(data);
            mainCityWnd.RefreshUI();
        }

        #endregion

        #region 强化界面

        public void OpenStrongWnd()
        {
            StopNavTask();
            strongWnd.SetWndState();
        }

        public void RspStrong(GameMsg msg)
        {
            int fightPre = PECommon.GetFightProps(GameRoot.Instance.PlayerData);
            GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
            int fightNow = PECommon.GetFightProps(GameRoot.Instance.PlayerData);
            GameRoot.AddTips(Constants.Color("战力提升 " + (fightNow - fightPre), TxtColor.Blue));

            strongWnd.RefreshItem();
            mainCityWnd.RefreshUI();
        }

        #endregion

        #region 聊天界面

        public void OpenChatWnd()
        {
            StopNavTask();
            chatWnd.SetWndState();
        }

        public void PshChat(GameMsg msg)
        {
            chatWnd.AddChatMsh(msg.pshChat.name, msg.pshChat.chat);
        }

        #endregion

        #region 购买界面

        public void OpenBuyWnd(int type)
        {
            StopNavTask();
            buyWnd.SetBuyType(type);
            buyWnd.SetWndState();
        }

        public void RspBuy(GameMsg msg)
        {
            RspBuy data = msg.rspBuy;
            GameRoot.Instance.SetPlayerDataByBuy(data);
            GameRoot.AddTips("购买成功");
            mainCityWnd.RefreshUI();
            buyWnd.SetWndState(false);
        }

        #endregion

        #region 体力系统

        public void PshPower(GameMsg msg)
        {
            GameRoot.Instance.SetPlayerDataByPower(msg.pshPower);
            if (mainCityWnd.GetWndState())
            {
                mainCityWnd.RefreshUI();
            }
        }

        #endregion

        #region 任务奖励界面

        public void OpenTaskRewardWnd()
        {
            StopNavTask();
            taskWnd.SetWndState();
        }

        public void RspTaskReward(GameMsg msg)
        {
            RspTaskReward data = msg.rspTaskReward;
            GameRoot.Instance.SetPlayerDataByTaskReward(data);
            GameRoot.AddTips(Constants.Color("恭喜获得奖励：", TxtColor.Blue) +
                             Constants.Color("金币+" + data.coin + "，经验+" + data.exp, TxtColor.Green));

            if (taskWnd.GetWndState())
            {
                taskWnd.RefreshUI();
            }

            mainCityWnd.RefreshUI();
        }

        public void PshTaskPrg(GameMsg msg)
        {
            PshTaskPrg data = msg.pshTaskPrg;
            GameRoot.Instance.SetPlayerDataByPshTask(data);
            if (taskWnd.GetWndState())
            {
                taskWnd.RefreshUI();
            }
        }

        #endregion

        #region 副本

        public void EnterFuben()
        {
            StopNavTask();
            FubenSys.Instance.EnterFuben();
        }

        #endregion
    }
}