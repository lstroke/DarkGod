using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    public class ItemEntityHp : MonoBehaviour
    {
        public Image imgHpGray;
        public Image imgHpRed;

        public Animation criticalAni;
        public Text txtCritical;
        public Animation dodegAni;
        public Animation hpAni;
        public Text txtHp;

        private RectTransform rect;
        private int hpMax;
        private int hpVal;
        private int hpCur;
        private Transform rootTrans;
        private readonly float scaleRate = 1f * Constants.ScreenStandardHeight / Screen.height;
        private readonly int hpSpeed = 700;

        private void Update()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
            rect.anchoredPosition = screenPos * scaleRate;

            SetYellowHPVal();
        }

        public void InitItemInfo(Transform root, int hp)
        {
            rect = GetComponent<RectTransform>();
            rootTrans = root;
            hpVal = hpCur = hpMax = hp;
            imgHpGray.fillAmount = 1;
            imgHpRed.fillAmount = 1;
        }

        public void SetCritical(int critical)
        {
            criticalAni.Stop();
            txtCritical.text = "暴击 " + critical;
            criticalAni.Play();
        }

        public void SetDodge()
        {
            dodegAni.Stop();
            dodegAni.Play();
        }

        public void SetHurt(int hurt)
        {
            hpAni.Stop();
            txtHp.text = "-" + hurt;
            hpAni.Play();
        }

        public void setHPVal(int curVal)
        {
            hpCur = curVal;
            imgHpRed.fillAmount = curVal * 1f / hpMax;
        }

        private void SetYellowHPVal()
        {
            if (hpVal > hpCur)
            {
                hpVal = Mathf.Max(hpVal - (int)(hpSpeed * Time.deltaTime), hpCur);
                imgHpGray.fillAmount = hpVal * 1f / hpMax;
            }
            else
            {
                hpVal = hpCur;
            }
        }
    }
}