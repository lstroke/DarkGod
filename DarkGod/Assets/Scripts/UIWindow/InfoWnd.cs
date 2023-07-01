using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 角色信息展示界面
    /// </summary>
    public class InfoWnd : WindowRoot
    {
        #region UI Define

        [Header("角色面板属性")] public Text txtInfo;
        public Text txtExp;
        public Image imgExpPrg;
        public Text txtPower;
        public Image imgPowerPrg;

        public Text txtJob;
        public Text txtFight;
        public Text txtHp;
        public Text txtHurt;
        public Text txtDef;

        public RawImage imgChar;

        [Header("角色详细面板属性")] public Transform detailTrans;
        public Text dtxhp;
        public Text dtxad;
        public Text dtxap;
        public Text dtxaddef;
        public Text dtxapdef;
        public Text dtxdodge;
        public Text dtxpierce;
        public Text dtxcritical;

        #endregion

        protected override void InitWnd()
        {
            base.InitWnd();
            RegTouchEvts();
            RefreshUI();
        }

        private void RefreshUI()
        {
            SetActive(detailTrans, false);
            PlayerData pd = GameRoot.Instance.PlayerData;

            SetText(txtInfo, pd.name + " LV." + pd.lv);
            SetText(txtExp, pd.exp + "/" + PECommon.GetExpUpValueByLv(pd.lv));
            imgExpPrg.fillAmount = 1f * pd.exp / PECommon.GetExpUpValueByLv(pd.lv);
            SetText(txtPower, pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
            imgPowerPrg.fillAmount = 1f * pd.power / PECommon.GetPowerLimit(pd.lv);

            SetText(txtJob, "职业      " + "暗夜刺客");
            SetText(txtFight, "战力      " + PECommon.GetFightProps(pd));
            SetText(txtHp, "血量      " + pd.hp);
            SetText(txtHurt, "伤害      " + (pd.ad + pd.ap));
            SetText(txtDef, "防御      " + (pd.addef + pd.apdef));

            SetText(dtxhp, pd.hp);
            SetText(dtxad, pd.ad);
            SetText(dtxap, pd.ap);
            SetText(dtxaddef, pd.addef);
            SetText(dtxapdef, pd.apdef);
            SetText(dtxdodge, pd.dodge + "%");
            SetText(dtxpierce, pd.pierce + "%");
            SetText(dtxcritical, pd.critical + "%");
        }

        private Vector2 startPos;

        private void RegTouchEvts()
        {
            OnClickDown(imgChar.gameObject, (evt) =>
            {
                MainCitySys.Instance.SetStartRotate();
                startPos = evt.position;
            });

            OnDrag(imgChar.gameObject, (evt) =>
            {
                float rotate = (startPos.x - evt.position.x) * 0.4f;
                MainCitySys.Instance.SetPlayerRotate(rotate);
            });
        }

        public void ClickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            MainCitySys.Instance.CloseInfoWnd();
        }

        public void ClickDetailBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIOpenPage);
            SetActive(detailTrans);
        }

        public void ClickCloseDetailBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetActive(detailTrans, false);
        }
    }
}