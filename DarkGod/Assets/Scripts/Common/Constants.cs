namespace Common
{
    /// <summary>
    /// 常量配置
    /// </summary>
    public class Constants
    {
        #region 颜色

        private const string ColorRed = "<color=#FF0000FF>";
        private const string ColorGreen = "<color=#00FF00FF>";
        private const string ColorBlue = "<color=#00B4FFFF>";
        private const string ColorYellow = "<color=#FFFF00FF>";
        private const string ColorEnd = "</color>";

        public static string Color(string str, TxtColor c)
        {
            return c switch
            {
                TxtColor.Red => ColorRed + str + ColorEnd,
                TxtColor.Green => ColorGreen + str + ColorEnd,
                TxtColor.Blue => ColorBlue + str + ColorEnd,
                TxtColor.Yellow => ColorYellow + str + ColorEnd,
                _ => str
            };
        }

        #endregion

        //AutoGuideNpc
        public const int NPCWiseMan = 0;
        public const int NPCGeneral = 1;
        public const int NPCArtisan = 2;

        public const int NPCTrader = 3;

        //场景名称
        public const string SceneLogin = "SceneLogin";

        //public const string SceneMainCity = "SceneMainCity";

        //场景ID
        public const int MainCityMapID = 10000;

        //背景音乐名称
        public const string BGLogin = "bgLogin";

        public const string BGMainCity = "bgMainCity";
        public const string BGHuangYe = "bgHuangYe";

        //登录按钮音效
        public const string UILoginBtn = "uiLoginBtn";

        //常规UI点击音效
        public const string UIClickBtn = "uiClickBtn";
        public const string UIExtenBtn = "uiExtenBtn";
        public const string UIOpenPage = "uiOpenPage";

        //战斗音效
        public const string AssassinHit = "assassin_Hit";
        //副本结束音效
        public const string FBLose = "fblose";
        public const string FBLogoEnter = "fbwin";
        public const string FBItemEnter = "fbitem";
        

        //屏幕标准尺寸
        public const int ScreenStandardWidth = 1334;

        public const int ScreenStandardHeight = 750;

        //摇杆点标准距离 
        public const int ScreenOPDis = 80;

        //角色移动速度
        public const int PlayerMoveSpeed = 8;
        public const int MonsterMoveSpeed = 4;

        //混合参数
        public const int BlendIdle = 0;
        public const int BlendMove = 1;

        //运动平滑加速度
        public const int AccelerSpeed = 5;

        //技能id
        public const int PlayerSkill1 = 101;
        public const int PlayerSkill2 = 102;
        public const int PlayerSkill3 = 103;
        public const int PlayerAttack1 = 111;
        public const int PlayerAttack2 = 112;
        public const int PlayerAttack3 = 113;
        public const int PlayerAttack4 = 114;

        public const int PlayerAttack5 = 115;

        //Action触发参数
        public const int ActionIdle = -1;
        public const int ActionBorn = 0;
        public const int ActionDie = 100;
        public const int DieAniLength = 5000;
        public const int ActionHit = 101;

        public const int ComboSpace = 500;

        //寻敌最远距离
        public const float searchDis = 10;
    }

    public enum TxtColor
    {
        Red,
        Green,
        Blue,
        Yellow,
    }

    public enum DamageType
    {
        None,
        AD,
        AP
    }

    public enum EntityType
    {
        None,
        Player,
        Monster,
    }

    public enum EntityState
    {
        Normal,
        BatiState, //霸体状态：不受控制，可受伤害
    }

    public enum MonsterType
    {
        None,
        Normal,
        Boss
    }
}