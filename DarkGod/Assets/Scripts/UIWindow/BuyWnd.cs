using Common;
using PEProtocol;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 购买交易窗口
    /// </summary>
    public class BuyWnd : WindowRoot
    {
        public Text txtInfo;
        public Button btnSure;
        private int buyType; // 0购买体力，1购买金币

        protected override void InitWnd()
        {
            base.InitWnd();

            RefreshUI();
        }

        public void SetBuyType(int type)
        {
            buyType = type;
        }

        private void RefreshUI()
        {
            btnSure.interactable = true;
            switch (buyType)
            {
                case 0:
                    //购买体力
                    SetText(txtInfo,
                        "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" +
                        Constants.Color("100体力", TxtColor.Green) + "？");
                    break;
                case 1:
                    //购买金币
                    SetText(txtInfo,
                        "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" +
                        Constants.Color("1000金币", TxtColor.Green) + "？");
                    break;
            }
        }

        public void ClickSureBtn()
        {
            btnSure.interactable = false;
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.ReqBuy,
                reqBuy = new ReqBuy()
                {
                    type = buyType
                }
            };
            
            netSvc.SendMsg(msg);
        }

        public void ClickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }
    }
}