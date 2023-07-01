using System;
using PENet;

//网络通信协议（服务端客户端共用）
namespace PEProtocol
{
    [Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTaskReward reqTaskReward;
        public RspTaskReward rspTaskReward;
        public PshTaskPrg pshTaskPrg;

        public ReqFBFight reqFBFight;
        public RspFBFight rspFBFight;

        public ReqFBFightEnd reqFbFightEnd;
        public RspFBFightEnd rspFbFightEnd;
    }

    #region 登录

    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pwd;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }

    #endregion

    #region 玩家数据

    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge; //闪避概率
        public int pierce; //穿透比率
        public int critical; //暴击概率

        public int guideid; //当前任务id号

        public int[] strongArr;

        public long usepower;

        public string[] taskArr; //1|1|0# id|已完成次数|是否领取奖励

        public int fuben; //副本进度
    }

    [Serializable]
    public class ReqRename
    {
        public string name;
    }

    [Serializable]
    public class RspRename
    {
        public string name;
    }

    #endregion

    #region 引导相关

    [Serializable]
    public class ReqGuide
    {
        public int guideid;
    }

    [Serializable]
    public class RspGuide
    {
        public int guideid;
        public int coin;
        public int lv;
        public int exp;
    }

    #endregion

    #region 强化相关

    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }

    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int adddef;
        public int[] strongArr;
    }

    #endregion

    #region 聊天相关

    [Serializable]
    public class SndChat
    {
        public string chat;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string chat;
    }

    #endregion

    #region 资源交易相关

    [Serializable]
    public class ReqBuy
    {
        public int type;
    }

    [Serializable]
    public class RspBuy
    {
        public int type;
        public int diamond;
        public int coin;
        public int power;
    }

    #endregion

    #region 体力回复

    [Serializable]
    public class PshPower
    {
        public int power;
    }

    #endregion

    #region 任务相关

    [Serializable]
    public class ReqTaskReward
    {
        public int rid;
    }

    [Serializable]
    public class RspTaskReward
    {
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }

    [Serializable]
    public class PshTaskPrg
    {
        public string[] taskArr;
    }

    #endregion

    #region 副本相关

    [Serializable]
    public class ReqFBFight
    {
        public int fbid;
    }

    [Serializable]
    public class RspFBFight
    {
        public int fbid;
        public int power;
        public long usepower;
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public bool win;
        public int fbid;
        public int restHP;
        public int costTime;
    }

    [Serializable]
    public class RspFBFightEnd
    {
        public bool win;
        public int fbid;
        public int restHP;
        public int costTime;

        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fuben;
    }

    #endregion

    public enum CMD
    {
        None = 0,

        //登录相关 100
        ReqLogin = 101,
        RspLogin = 102,

        ReqRename = 103,
        RspRename = 104,

        //主城相关
        ReqGuide = 201,
        RspGuide = 202,

        ReqStrong = 203,
        RspStrong = 204,

        SndChat = 205,
        PshChat = 206,

        ReqBuy = 207,
        RspBuy = 208,

        PshPower = 209,

        ReqTaskReward = 210,
        RspTaskReward = 211,
        PshTaskPrg = 212,

        ReqFBFight = 301,
        RspFBFight = 302,

        ReqFBFightEnd = 303,
        RspFBFightEnd = 304,
    }

    public enum ErrorCode
    {
        None, //没有错误
        ClientDataError, //客户端数据异常
        UpdateDBError, //更新数据库错误
        AcctIsOnline, //账号已上线
        WrongPwd, //密码错误
        NameIsExist, //名字已存在

        ExceedMaxLevel,
        LackLevel,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower,
    }

    public class SvcCfg
    {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}