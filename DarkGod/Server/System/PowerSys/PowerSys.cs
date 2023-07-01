using System;
using PEProtocol;
using Server.Cache;
using Server.Service.CfgSvc;
using Server.Service.NetSvc;
using Server.Service.TimerSvc;

namespace Server.System.PowerSys
{
    /// <summary>
    /// 体力回复系统
    /// </summary>
    public class PowerSys
    {
        private static PowerSys instance;
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;
        private TimerSvc timerSvc;

        public static PowerSys Instance
        {
            get { return instance ?? (instance = new PowerSys()); }
        }

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            timerSvc = TimerSvc.Instance;
            timerSvc.AddTimeTask(CalcPowerAdd, 100, PETimeUnit.Millisecond, 0);
            PECommon.Log("PowerSys initialized");
        }

        private void CalcPowerAdd(int tid)
        {
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.PshPower
            };
            //计算体力增长
            var onlinePlayers = cacheSvc.GetOnlinePlayers();
            foreach (var player in onlinePlayers)
            {
                ServerSession session = player.Key;
                var pd = player.Value;

                if (pd.power < PECommon.GetPowerLimit(pd.lv))
                {
                    long nowTime = timerSvc.GetNowTime();
                    if (nowTime - pd.usepower >= PECommon.PowerAddSpace)
                    {
                        pd.usepower += PECommon.PowerAddSpace;
                        pd.power++;
                        if (cacheSvc.UpdatePlayerData(pd.id, pd))
                        {
                            msg.pshPower = new PshPower()
                            {
                                power = pd.power
                            };
                        }
                        else
                        {
                            msg.err = (int)ErrorCode.UpdateDBError;
                        }

                        session.SendMsg(msg);
                    }
                }
            }
        }
    }
}