using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 副本选择界面
    /// </summary>
    public class FubenWnd : WindowRoot
    {
        public Transform pointerTrans;
        public Button[] fbBtnArr;
        public List<Points> wayPoint;
        private PlayerData pd;

        protected override void InitWnd()
        {
            base.InitWnd();
            pd = GameRoot.Instance.PlayerData;
            RefreshUI();
        }

        protected override void ClearWnd()
        {
            base.ClearWnd();
            StopCoroutine(PlayFubenAni());
        }

        public void RefreshUI()
        {
            StartCoroutine(PlayFubenAni());
        }

        private IEnumerator PlayFubenAni()
        {
            foreach (var fbBtn in fbBtnArr)
            {
                SetActive(fbBtn.gameObject, false);
            }

            foreach (var pointers in wayPoint)
            {
                foreach (var pointer in pointers.pointLst)
                {
                    SetActive(pointer, false);
                }
            }

            SetActive(pointerTrans, false);

            int fbid = pd.fuben % 10000;
            for (int i = 0; i < fbBtnArr.Length; i++)
            {
                foreach (var point in wayPoint[i].pointLst)
                {
                    SetActive(point);
                    yield return new WaitForSeconds(0.1f);
                }

                if (i < fbid)
                {
                    SetActive(fbBtnArr[i].gameObject);
                    yield return new WaitForSeconds(0.1f);

                    if (i == fbid - 1)
                    {
                        pointerTrans.SetParent(fbBtnArr[i].transform);
                        pointerTrans.localPosition = new Vector3(25, 100, 0);
                        SetActive(pointerTrans);
                        yield break;
                    }
                }
            }
        }

        public void ClickTaskBtn(int fbid)
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            //检查体力是否足够
            int needPower = resSvc.GetMapCfgData(fbid).power;
            if (pd.power >= needPower)
            {
                netSvc.SendMsg(new GameMsg()
                {
                    cmd = (int)CMD.ReqFBFight,
                    reqFBFight = new ReqFBFight()
                    {
                        fbid = fbid
                    }
                });
            }
            else
            {
                GameRoot.AddTips("体力不足");
            }
        }

        public void CLickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }
    }

    [Serializable]
    public class Points
    {
        public List<Transform> pointLst = new();
    }
}