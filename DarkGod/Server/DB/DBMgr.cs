using System;
using MySql.Data.MySqlClient;
using PEProtocol;
using Server.Service.TimerSvc;

namespace Server.DB
{
    public class DBMgr
    {
        private static DBMgr instance;

        public static DBMgr Instance
        {
            get { return instance ?? (instance = new DBMgr()); }
        }

        private MySqlConnection conn;

        public void Init()
        {
            conn = new MySqlConnection(
                "server=localhost;User Id=root;password=123;Database=darkgod;Charset=utf8;SslMode = none;");
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                PECommon.Log("数据库连接异常:" + e.Message, LogType.Error);
            }

            PECommon.Log("DBMgr initialized");
        }

        public PlayerData QueryPlayerData(string acct, string pwd)
        {
            PlayerData playerData = null;
            MySqlDataReader reader = null;
            bool isNew = true;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
                cmd.Parameters.AddWithValue("acct", acct);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isNew = false;
                    string _pwd = reader.GetString("pwd");
                    if (_pwd.Equals(pwd))
                    {
                        //密码正确返回玩家数据
                        playerData = new PlayerData()
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            lv = reader.GetInt32("level"),
                            exp = reader.GetInt32("exp"),
                            power = reader.GetInt32("power"),
                            coin = reader.GetInt32("coin"),
                            diamond = reader.GetInt32("diamond"),
                            crystal = reader.GetInt32("crystal"),

                            hp = reader.GetInt32("hp"),
                            ad = reader.GetInt32("ad"),
                            ap = reader.GetInt32("ap"),
                            addef = reader.GetInt32("addef"),
                            apdef = reader.GetInt32("apdef"),
                            dodge = reader.GetInt32("dodge"),
                            pierce = reader.GetInt32("pierce"),
                            critical = reader.GetInt32("critical"),

                            guideid = reader.GetInt32("guideid"),

                            usepower = reader.GetInt64("usepower"),
                            fuben = reader.GetInt32("fuben"),
                        };

                        #region Strong

                        string[] strongArrStr = reader.GetString("strong").Split('#');
                        int[] strongArr = new int[strongArrStr.Length];
                        for (int i = 0; i < strongArrStr.Length; i++)
                        {
                            strongArr[i] = Convert.ToInt32(strongArrStr[i]);
                        }

                        playerData.strongArr = strongArr;

                        #endregion

                        #region Task

                        playerData.taskArr = reader.GetString("task").Split('#');

                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                PECommon.Log("通过账号和密码查询玩家数据异常:" + e.Message, LogType.Error);
            }
            finally
            {
                reader?.Close();
                if (isNew)
                {
                    //不存在账号数据，创建新的默认账号数据并返回
                    playerData = new PlayerData()
                    {
                        id = -1,
                        name = "",
                        lv = 1,
                        exp = 0,
                        power = 150,
                        coin = 5000,
                        diamond = 500,
                        crystal = 500,

                        hp = 2000,
                        ad = 275,
                        ap = 265,
                        addef = 67,
                        apdef = 43,
                        dodge = 7,
                        pierce = 5,
                        critical = 2,
                        guideid = 1001,
                        strongArr = new int[6],

                        usepower = TimerSvc.Instance.GetNowTime(),
                        taskArr = new string[6],
                        fuben = 10001,
                    };
                    for (int i = 0; i < playerData.taskArr.Length; i++)
                    {
                        playerData.taskArr[i] = (i + 1) + "|0|0";
                    }

                    playerData.id = InsertNewAcctData(acct, pwd, playerData);
                }
            }

            return playerData;
        }

        private int InsertNewAcctData(string acct, string pwd, PlayerData pd)
        {
            int id = -1;
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand(
                        "insert into account set acct=@acct,pwd=@pwd,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond," +
                        "crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid," +
                        "strong=@strong,usepower=@usepower,task=@task,fuben=@fuben",
                        conn);
                cmd.Parameters.AddWithValue("acct", acct);
                cmd.Parameters.AddWithValue("pwd", pwd);
                cmd.Parameters.AddWithValue("name", pd.name);
                cmd.Parameters.AddWithValue("level", pd.lv);
                cmd.Parameters.AddWithValue("exp", pd.exp);
                cmd.Parameters.AddWithValue("power", pd.power);
                cmd.Parameters.AddWithValue("coin", pd.coin);
                cmd.Parameters.AddWithValue("diamond", pd.diamond);
                cmd.Parameters.AddWithValue("crystal", pd.crystal);

                cmd.Parameters.AddWithValue("hp", pd.hp);
                cmd.Parameters.AddWithValue("ad", pd.ad);
                cmd.Parameters.AddWithValue("ap", pd.ap);
                cmd.Parameters.AddWithValue("addef", pd.addef);
                cmd.Parameters.AddWithValue("apdef", pd.apdef);
                cmd.Parameters.AddWithValue("dodge", pd.dodge);
                cmd.Parameters.AddWithValue("pierce", pd.pierce);
                cmd.Parameters.AddWithValue("critical", pd.critical);

                cmd.Parameters.AddWithValue("guideid", pd.guideid);

                cmd.Parameters.AddWithValue("strong", string.Join("#", pd.strongArr));

                cmd.Parameters.AddWithValue("usepower", pd.usepower);

                cmd.Parameters.AddWithValue("task", string.Join("#", pd.taskArr));
                
                cmd.Parameters.AddWithValue("fuben", pd.fuben);

                cmd.ExecuteNonQuery();
                id = (int)cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                PECommon.Log("新建玩家数据异常:" + e.Message, LogType.Error);
            }

            return id;
        }

        public bool QueryNameData(string name)
        {
            bool exist = true;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where name = @name", conn);
                cmd.Parameters.AddWithValue("name", name);
                reader = cmd.ExecuteReader();
                exist = reader.Read();
            }
            catch (Exception e)
            {
                PECommon.Log("根据名字查找玩家数据错误：" + e.Message);
            }
            finally
            {
                reader?.Close();
            }

            return exist;
        }

        public bool UpdatePlayerData(int id, PlayerData playerData)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "update account set name = @name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal" +
                    ",hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid," +
                    "strong=@strong,usepower=@usepower,task=@task,fuben=@fuben" +
                    " where id=@id",
                    conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", playerData.name);
                cmd.Parameters.AddWithValue("level", playerData.lv);
                cmd.Parameters.AddWithValue("exp", playerData.exp);
                cmd.Parameters.AddWithValue("power", playerData.power);
                cmd.Parameters.AddWithValue("coin", playerData.coin);
                cmd.Parameters.AddWithValue("diamond", playerData.diamond);
                cmd.Parameters.AddWithValue("crystal", playerData.crystal);

                cmd.Parameters.AddWithValue("hp", playerData.hp);
                cmd.Parameters.AddWithValue("ad", playerData.ad);
                cmd.Parameters.AddWithValue("ap", playerData.ap);
                cmd.Parameters.AddWithValue("addef", playerData.addef);
                cmd.Parameters.AddWithValue("apdef", playerData.apdef);
                cmd.Parameters.AddWithValue("dodge", playerData.dodge);
                cmd.Parameters.AddWithValue("pierce", playerData.pierce);
                cmd.Parameters.AddWithValue("critical", playerData.critical);

                cmd.Parameters.AddWithValue("guideid", playerData.guideid);

                cmd.Parameters.AddWithValue("strong", string.Join("#", playerData.strongArr));

                cmd.Parameters.AddWithValue("usepower", playerData.usepower);
                
                cmd.Parameters.AddWithValue("task", string.Join("#", playerData.taskArr));
                
                cmd.Parameters.AddWithValue("fuben", playerData.fuben);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                PECommon.Log("更新玩家数据错误：" + e.Message);
                return false;
            }

            return true;
        }
    }
}