using PENet;

namespace PEProtocol
{
    /// <summary>
    /// 客户端服务器公用工具类
    /// </summary>
    public class PECommon
    {
        public static readonly int PowerAddSpace = 5 * 60 * 1000; //毫秒

        public static void Log(string msg = "", LogType logType = LogType.Log)
        {
            LogLevel lv = (LogLevel)logType;
            PETool.LogMsg(msg, lv);
        }

        public static int GetFightProps(PlayerData pd)
        {
            return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
        }

        public static int GetPowerLimit(int lv)
        {
            return (lv - 1) / 10 * 150 + 150;
        }

        public static int GetExpUpValueByLv(int lv)
        {
            return 100 * lv * lv;
        }
        
        public static void CalcExp(PlayerData pd, int addExp)
        {
            int curLv = pd.lv;
            int curExp = pd.exp;
            curExp += addExp;
            while (true)
            {
                int upNeedExp = GetExpUpValueByLv(curLv);
                if (curExp >= upNeedExp)
                {
                    curLv++;
                    curExp -= upNeedExp;
                }
                else
                {
                    pd.lv = curLv;
                    pd.exp = curExp;
                    break;
                }
            }
        }
    }

    public enum LogType
    {
        Log,
        Warn,
        Error,
        Info
    }
}