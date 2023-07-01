using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    //主城UI界面
    public class MainCityWnd : WindowRoot
    {
        #region UIDefine

        public Text txtFight;
        public Text txtPower;
        public Image imgPowerPrg;
        public Text txtLevel;
        public Text txtName;
        public Text txtExpPrg;
        public Transform expPrgTrans;

        public Animation menuAni;

        public Image imgTouch;
        public Image imgDirBg;
        public Image imgDirPoint;

        public Button btnGuide;

        #endregion

        private Image[] expPrgImgs;
        private bool menuState = true;
        private GridLayoutGroup grid;
        private AutoGuideCfg curTaskData;
        private Image imgGuideBtn;

        #region MainFunctions

        protected override void InitWnd()
        {
            base.InitWnd();

            expPrgImgs = expPrgTrans.GetComponentsInChildren<Image>();
            imgGuideBtn = btnGuide.GetComponent<Image>();
            grid = expPrgTrans.GetComponent<GridLayoutGroup>();
            SetActive(imgDirPoint, false);
            RegisterTouchEvts();
            RefreshUI();
        }

        public void RefreshUI()
        {
            PlayerData pd = GameRoot.Instance.PlayerData;
            SetText(txtFight, PECommon.GetFightProps(pd));
            SetText(txtPower, "体力：" + pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
            imgPowerPrg.fillAmount = pd.power * 1f / PECommon.GetPowerLimit(pd.lv);
            SetText(txtLevel, pd.lv);
            SetText(txtName, pd.name);
            int expPrgVal = (int)(pd.exp * 1f / PECommon.GetExpUpValueByLv(pd.lv) * 100);
            SetText(txtExpPrg, expPrgVal + "%");
            int index = expPrgVal / 10;

            #region 经验条显示

            float globalRate = 1f * Constants.ScreenStandardHeight / Screen.height;
            float screenWidth = Screen.width * globalRate;
            float width = (screenWidth - 180) / 10;

            grid.cellSize = new Vector2(width, 7);

            for (int i = 0; i < expPrgImgs.Length; i++)
            {
                if (i < index)
                {
                    expPrgImgs[i].fillAmount = 1;
                }
                else if (i == index)
                {
                    expPrgImgs[i].fillAmount = expPrgVal % 10 / 10f;
                }
                else
                {
                    expPrgImgs[i].fillAmount = 0;
                }
            }

            #endregion

            #region 设置自动任务图标

            curTaskData = resSvc.GetAutoGuideData(pd.guideid);
            if (curTaskData != null)
            {
                SetGuideBtnIcon(curTaskData.npcID);
            }
            else
            {
                SetGuideBtnIcon(-1);
            }

            #endregion
        }

        private void SetGuideBtnIcon(int npcID)
        {
            string spPath = npcID switch
            {
                Constants.NPCWiseMan => PathDefine.WiseManHead,
                Constants.NPCGeneral => PathDefine.GeneralHead,
                Constants.NPCArtisan => PathDefine.ArtisanHead,
                Constants.NPCTrader => PathDefine.TraderHead,
                _ => PathDefine.TaskHead
            };

            SetSprite(imgGuideBtn, spPath);
        }

        #region ClickEvts

        public void ClickMenuBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIExtenBtn);
            menuState = !menuState;
            AnimationClip clip;
            if (menuState)
            {
                clip = menuAni.GetClip("OpenMCMenu");
            }
            else
            {
                clip = menuAni.GetClip("CloseMCMenu");
            }

            menuAni.Play(clip.name);
        }

        public void ClickHeadBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenInfoWnd();
        }

        public void ClickGuideBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            if (curTaskData != null)
            {
                MainCitySys.Instance.RunTask(curTaskData);
            }
            else
            {
                GameRoot.AddTips("当前版本的全部任务已完成");
            }
        }

        public void ClickStrongBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenStrongWnd();
        }

        public void ClickChatBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenChatWnd();
        }

        public void ClickBuyPowerBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenBuyWnd(0);
        }

        public void ClickMKCoinBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenBuyWnd(1);
        }

        public void ClickTaskBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.OpenTaskRewardWnd();
        }

        public void ClickFubenBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            MainCitySys.Instance.EnterFuben();
        }


        #region 轮盘事件

        private Vector2 startPos;
        private Vector2 defaultPos;

        public void RegisterTouchEvts()
        {
            defaultPos = imgDirBg.transform.position;
            OnClickDown(imgTouch.gameObject, (evt) =>
            {
                startPos = evt.position;
                SetActive(imgDirPoint);
                imgDirBg.transform.position = evt.position;
            });
            OnClickUp(imgTouch.gameObject, (evt) =>
            {
                imgDirBg.transform.position = defaultPos;
                SetActive(imgDirPoint, false);
                imgDirPoint.transform.localPosition = Vector2.zero;
                //方向信息传递
                MainCitySys.Instance.SetMoveDir(Vector2.zero);
            });
            //根据屏幕大小计算摇杆的距离（自适应）
            float pointDis = 1f * Screen.height / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
            OnDrag(imgTouch.gameObject, (evt) =>
            {
                Vector2 dir = evt.position - startPos;
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;

                //方向信息传递
                MainCitySys.Instance.SetMoveDir(dir.normalized);
            });
        }

        #endregion

        #endregion

        #endregion
    }
}