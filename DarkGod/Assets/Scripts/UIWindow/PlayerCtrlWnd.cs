using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 玩家控制界面
    /// </summary>
    public class PlayerCtrlWnd : WindowRoot
    {
        public Image imgTouch;
        public Image imgDirBg;
        public Image imgDirPoint;

        public Text txtLevel;
        public Text txtName;
        public Text txtExpPrg;
        public Transform expPrgTrans;

        private Image[] expPrgImgs;
        private GridLayoutGroup grid;
        private int HPSum;
        public Vector2 currentDir;

        protected override void InitWnd()
        {
            base.InitWnd();
            expPrgImgs = expPrgTrans.GetComponentsInChildren<Image>();
            grid = expPrgTrans.GetComponent<GridLayoutGroup>();
            SetActive(imgDirPoint, false);
            RegisterTouchEvts();
            SetBossHPBarState(false);

            HPSum = GameRoot.Instance.PlayerData.hp;
            SetText(txtSelfHP, HPSum + "/" + HPSum);
            imgSelfHP.fillAmount = 1;

            sk1CDTime = resSvc.GetSkillCfgData(Constants.PlayerSkill1).cdTime / 1000f;
            sk2CDTime = resSvc.GetSkillCfgData(Constants.PlayerSkill2).cdTime / 1000f;
            sk3CDTime = resSvc.GetSkillCfgData(Constants.PlayerSkill3).cdTime / 1000f;

            RefreshUI();
        }

        public void RefreshUI()
        {
            PlayerData pd = GameRoot.Instance.PlayerData;
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
        }

        public void ClickHeadBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            BattleSys.Instance.SetBattleEndWndState(FBEndType.Pause);
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
                currentDir = Vector2.zero;
                BattleSys.Instance.SetMoveDir(currentDir);
            });
            //根据屏幕大小计算摇杆的距离（自适应）
            float pointDis = 1f * Screen.height / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
            OnDrag(imgTouch.gameObject, (evt) =>
            {
                Vector2 dir = evt.position - startPos;
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;

                //方向信息传递
                currentDir = dir.normalized;
                BattleSys.Instance.SetMoveDir(currentDir);
            });
        }

        #endregion

        #region 技能CD

        public Image imgSk1CD;
        public Text txtSk1CD;
        public Image imgSk2CD;
        public Text txtSk2CD;
        public Image imgSk3CD;
        public Text txtSk3CD;
        private bool isSk1CD;
        private float sk1CDTime;
        private float sk1Timer;
        private bool isSk2CD;
        private float sk2CDTime;
        private float sk2Timer;
        private bool isSk3CD;
        private float sk3CDTime;
        private float sk3Timer;


        private void Update()
        {
            float delta = Time.deltaTime;
            if (isSk1CD)
            {
                sk1Timer -= delta;
                imgSk1CD.fillAmount = sk1Timer / sk1CDTime;
                SetText(txtSk1CD, (int)sk1Timer);
                if (sk1Timer <= 0)
                {
                    SetActive(imgSk1CD, false);
                    SetActive(txtSk1CD, false);
                    isSk1CD = false;
                }
            }

            if (isSk2CD)
            {
                sk2Timer -= delta;
                imgSk2CD.fillAmount = sk2Timer / sk2CDTime;
                SetText(txtSk2CD, (int)sk2Timer);
                if (sk2Timer <= 0)
                {
                    SetActive(imgSk2CD, false);
                    SetActive(txtSk2CD, false);
                    isSk2CD = false;
                }
            }

            if (isSk3CD)
            {
                sk3Timer -= delta;
                imgSk3CD.fillAmount = sk3Timer / sk3CDTime;
                SetText(txtSk3CD, (int)sk3Timer);
                if (sk3Timer <= 0)
                {
                    SetActive(imgSk3CD, false);
                    SetActive(txtSk3CD, false);
                    isSk3CD = false;
                }
            }

            if (transBossHPBar.gameObject.activeSelf)
            {
                SetYellowHPVal();
            }
        }

        #endregion

        #region 释放技能

        public bool GetCanRlsSkill()
        {
            return BattleSys.Instance.battleMgr.CanRlsSkill();
        }

        public void ClickNormalAtk()
        {
            BattleSys.Instance.ReqReleaseSkill(0);
        }

        public void ClickSkill1Atk()
        {
            if (!isSk1CD && GetCanRlsSkill())
            {
                BattleSys.Instance.ReqReleaseSkill(1);
                isSk1CD = true;
                SetActive(imgSk1CD);
                SetActive(txtSk1CD);
                imgSk1CD.fillAmount = 1;
                sk1Timer = sk1CDTime;
                SetText(txtSk1CD, (int)sk1Timer);
            }
        }

        public void ClickSkill2Atk()
        {
            if (!isSk2CD && GetCanRlsSkill())
            {
                BattleSys.Instance.ReqReleaseSkill(2);
                isSk2CD = true;
                SetActive(imgSk2CD);
                SetActive(txtSk2CD);
                imgSk2CD.fillAmount = 1;
                sk2Timer = sk2CDTime;
                SetText(txtSk2CD, (int)sk2Timer);
            }
        }

        public void ClickSkill3Atk()
        {
            if (!isSk3CD && GetCanRlsSkill())
            {
                BattleSys.Instance.ReqReleaseSkill(3);
                isSk3CD = true;
                SetActive(imgSk3CD);
                SetActive(txtSk3CD);
                imgSk3CD.fillAmount = 1;
                sk3Timer = sk3CDTime;
                SetText(txtSk3CD, (int)sk3Timer);
            }
        }

        #endregion

        #region 血条

        public Text txtSelfHP;
        public Image imgSelfHP;

        public void SetSelfHPBarVal(int val)
        {
            SetText(txtSelfHP, val + "/" + HPSum);
            imgSelfHP.fillAmount = val * 1f / HPSum;
        }

        #endregion

        #region Boss

        public Transform transBossHPBar;
        public Image imgRed;
        public Image imgYellow;
        private int redHPVal;
        private int yellowHPVal;
        private readonly int hpSpeed = 700;
        private int BossSumHP;

        public void SetBossHPBarState(bool state, int sumHP = 0)
        {
            SetActive(transBossHPBar, state);
            BossSumHP = redHPVal = yellowHPVal = sumHP;
            imgRed.fillAmount = 1;
            imgYellow.fillAmount = 1;
        }

        public void SetBossHPBarVal(int curVal)
        {
            redHPVal = curVal;
            imgRed.fillAmount = redHPVal * 1f / BossSumHP;
        }

        private void SetYellowHPVal()
        {
            if (yellowHPVal > redHPVal)
            {
                yellowHPVal = Mathf.Max(yellowHPVal - (int)(hpSpeed * Time.deltaTime), redHPVal);
                imgYellow.fillAmount = yellowHPVal * 1f / BossSumHP;
            }
            else
            {
                yellowHPVal = redHPVal;
            }
        }

        #endregion
    }
}