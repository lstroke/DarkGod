using Common;
using PEProtocol;
using UIWindow;

namespace System
{
    public class FubenSys : SystemRoot
    {
        public static FubenSys Instance;
        public FubenWnd fubenWnd;

        public override void InitSys()
        {
            base.InitSys();
            Instance = this;
            print("Init InitSys...");
        }

        public void EnterFuben()
        {
            SetFubenWndState();
        }

        public void SetFubenWndState(bool isActive = true)
        {
            fubenWnd.SetWndState(isActive);
        }

        public void RspFBFight(GameMsg msg)
        {
            print("开始副本");
            RspFBFight data = msg.rspFBFight;
            GameRoot.Instance.SetPlayerDataByFBStart(data);
            MainCitySys.Instance.CloseMainCityWnd();
            SetFubenWndState(false);
            BattleSys.Instance.StartBattle(data.fbid);
        }
    }
}