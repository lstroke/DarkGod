using System;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 强化升级界面
    /// </summary>
    public class StrongWnd : WindowRoot
    {
        #region UI定义

        public Image imgCurtPos;
        public Text txtStarLv;
        public Image[] imgStarLv;
        public Text propHp1;
        public Text propHp2;
        public Text propHurt1;
        public Text propHurt2;
        public Text propDef1;
        public Text propDef2;
        public Image propArr1;
        public Image propArr2;
        public Image propArr3;
        public Transform costTransRoot;
        public Text txtNeedLv;
        public Text txtCostCoin;
        public Text txtCostCrystal;
        public Text txtCoin;
        public Transform btnStrong;
        public Transform coin;

        #endregion


        public Transform posBtnTrans;
        private Image[] imgs;
        private int currentIndex;
        private PlayerData pd;
        private StrongCfg nextSc;

        protected override void InitWnd()
        {
            base.InitWnd();
            pd = GameRoot.Instance.PlayerData;
            RegClickEvts();
            ClickPosItem(0);
        }

        private void RegClickEvts()
        {
            imgs = new Image[posBtnTrans.childCount];
            for (int i = 0; i < posBtnTrans.childCount; i++)
            {
                Image img = posBtnTrans.GetChild(i).GetComponent<Image>();
                OnClick(img.gameObject, (args) =>
                {
                    ClickPosItem((int)args);
                    audioSvc.PlayUIMusic(Constants.UIClickBtn);
                }, i);
                imgs[i] = img;
            }
        }

        private void ClickPosItem(int index)
        {
            RectTransform oldTrans = imgs[currentIndex].rectTransform;
            SetSprite(imgs[currentIndex], PathDefine.ItemPlatBg);
            oldTrans.localPosition = new Vector3(0, oldTrans.localPosition.y, 0);
            oldTrans.sizeDelta = new Vector2(220, 85);

            RectTransform newTrans = imgs[index].rectTransform;
            SetSprite(imgs[index], PathDefine.ItemArrorBg);
            newTrans.localPosition = new Vector3(13, newTrans.localPosition.y, 0);
            newTrans.sizeDelta = new Vector2(253, 100);

            currentIndex = index;

            RefreshItem();
        }

        public void RefreshItem()
        {
            SetText(txtCoin, pd.coin);
            switch (currentIndex)
            {
                case 0:
                    SetSprite(imgCurtPos, PathDefine.ItemTouKui);
                    break;
                case 1:
                    SetSprite(imgCurtPos, PathDefine.ItemBody);
                    break;
                case 2:
                    SetSprite(imgCurtPos, PathDefine.ItemYaoBu);
                    break;
                case 3:
                    SetSprite(imgCurtPos, PathDefine.ItemHand);
                    break;
                case 4:
                    SetSprite(imgCurtPos, PathDefine.ItemLeg);
                    break;
                case 5:
                    SetSprite(imgCurtPos, PathDefine.ItemFoot);
                    break;
            }

            SetText(txtStarLv, pd.strongArr[currentIndex] + "星级");
            int curStarLv = pd.strongArr[currentIndex];
            for (int i = 0; i < imgStarLv.Length; i++)
            {
                SetSprite(imgStarLv[i], i < curStarLv ? PathDefine.HaveStar : PathDefine.NoStar);
            }

            int sumAddHp = resSvc.GetPropAddValPreLv(currentIndex, curStarLv, 1);
            int sumAddHurt = resSvc.GetPropAddValPreLv(currentIndex, curStarLv, 2);
            int sumAddDef = resSvc.GetPropAddValPreLv(currentIndex, curStarLv, 3);

            SetText(propHp1, "生命 +" + sumAddHp);
            SetText(propHurt1, "伤害 +" + sumAddHurt);
            SetText(propDef1, "防御 +" + sumAddDef);

            int nextStarLv = curStarLv + 1;
            if (nextStarLv != 11)
            {
                SetActive(propHp2);
                SetActive(propHurt2);
                SetActive(propDef2);
                SetActive(propArr1);
                SetActive(propArr2);
                SetActive(propArr3);
                SetActive(costTransRoot);
                SetActive(btnStrong);
                SetActive(coin);

                nextSc = resSvc.GetStrongData(currentIndex, nextStarLv);

                SetText(propHp2, "强化后 +" + nextSc.addhp);
                SetText(propHurt2, "强化后 +" + nextSc.addhurt);
                SetText(propDef2, "强化后 +" + nextSc.adddef);

                SetText(txtNeedLv,
                    Constants.Color("需要等级：" + nextSc.minlv, pd.lv >= nextSc.minlv ? TxtColor.Green : TxtColor.Red));
                SetText(txtCostCoin,
                    "需要消耗：      " +
                    Constants.Color(nextSc.coin.ToString(), pd.coin >= nextSc.coin ? TxtColor.Green : TxtColor.Red));
                SetText(txtCostCrystal,
                    Constants.Color(nextSc.crystal + "/" + pd.crystal,
                        pd.crystal >= nextSc.crystal ? TxtColor.Green : TxtColor.Red));
            }
            else
            {
                SetActive(propHp2, false);
                SetActive(propHurt2, false);
                SetActive(propDef2, false);
                SetActive(propArr1, false);
                SetActive(propArr2, false);
                SetActive(propArr3, false);
                SetActive(costTransRoot, false);
                SetActive(btnStrong, false);
                SetActive(coin, false);
            }
        }

        public void ClickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }

        public void ClickStrongBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);

            if (pd.strongArr[currentIndex] < 10)
            {
                if (pd.lv < nextSc.minlv)
                {
                    GameRoot.AddTips("角色等级不够");
                }
                else if (pd.coin < nextSc.coin)
                {
                    GameRoot.AddTips("金币不够");
                }
                else if (pd.crystal < nextSc.crystal)
                {
                    GameRoot.AddTips("水晶不够");
                }
                else
                {
                    GameMsg msg = new GameMsg()
                    {
                        cmd = (int)CMD.ReqStrong,
                        reqStrong = new ReqStrong()
                        {
                            pos = currentIndex
                        }
                    };

                    netSvc.SendMsg(msg);
                }
            }
            else
            {
                GameRoot.AddTips("当前装备已满级");
            }
        }
    }
}