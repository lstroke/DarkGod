using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 加载进度界面
    /// </summary>
    public class LoadingWnd : WindowRoot
    {
        public Text txtTips;
        public Image imgFg;
        public Image imgPoint;
        public Text txtPrg;

        private float fgWidth;

        protected override void InitWnd()
        {
            base.InitWnd();
            fgWidth = imgFg.GetComponent<RectTransform>().sizeDelta.x;

            SetText(txtTips,"提示");
            SetText(txtPrg,"0%");
            imgFg.fillAmount = 0;
            imgPoint.transform.localPosition = new Vector3(-fgWidth / 2, 0, 0);
        }

        public void SetProgress(float prg)
        {
            SetText(txtPrg,(int)(prg * 100) + "%");
            imgFg.fillAmount = prg;

            float posX = -fgWidth / 2 + fgWidth * prg;
            imgPoint.transform.localPosition = new Vector3(posX, 0, 0);
        }
    }
}