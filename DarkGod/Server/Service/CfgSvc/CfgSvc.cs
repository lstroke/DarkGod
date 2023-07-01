using System;
using System.Collections.Generic;
using System.Xml;
using PEProtocol;

namespace Server.Service.CfgSvc
{
    /// <summary>
    /// 配置数据服务
    /// </summary>
    public class CfgSvc
    {
        private static CfgSvc instance;

        public static CfgSvc Instance
        {
            get { return instance ?? (instance = new CfgSvc()); }
        }

        public void Init()
        {
            InitGuideCfg();
            InitStrongCfg();
            InitTaskRewardCfg();
            InitMapCfg();
            PECommon.Log("CfgSvc initialized");
        }

        #region 任务引导配置

        private Dictionary<int, GuideCfg> guideDic = new Dictionary<int, GuideCfg>();

        private void InitGuideCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Config\guide.xml");
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                GuideCfg agc = new GuideCfg()
                {
                    ID = ID
                };
                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "coin":
                            agc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            agc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }

                guideDic[ID] = agc;
            }

            PECommon.Log("GuideCfg initialized");
        }

        public GuideCfg GetGuideCfg(int id)
        {
            if (guideDic.TryGetValue(id, out GuideCfg agc))
            {
                return agc;
            }

            return null;
        }

        #endregion

        #region 任务奖励配置

        private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();

        private void InitTaskRewardCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Config\taskreward.xml");
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskRewardCfg trc = new TaskRewardCfg()
                {
                    ID = ID
                };
                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "count":
                            trc.count = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            trc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            trc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }

                taskRewardDic[ID] = trc;
            }

            PECommon.Log("TaskRewardCfg initialized");
        }

        public TaskRewardCfg GetTaskRewardCfg(int id)
        {
            if (taskRewardDic.TryGetValue(id, out TaskRewardCfg trc))
            {
                return trc;
            }

            return null;
        }

        #endregion

        #region 装备强化配置

        private Dictionary<int, Dictionary<int, StrongCfg>> strongDic =
            new Dictionary<int, Dictionary<int, StrongCfg>>();

        private void InitStrongCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Config\strong.xml");
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sc = new StrongCfg()
                {
                    ID = ID
                };
                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            sc.pos = val;
                            break;
                        case "starlv":
                            sc.starlv = val;
                            break;
                        case "addhp":
                            sc.addhp = val;
                            break;
                        case "addhurt":
                            sc.addhurt = val;
                            break;
                        case "adddef":
                            sc.adddef = val;
                            break;
                        case "minlv":
                            sc.minlv = val;
                            break;
                        case "coin":
                            sc.coin = val;
                            break;
                        case "crystal":
                            sc.crystal = val;
                            break;
                    }
                }

                if (!strongDic.TryGetValue(sc.pos, out Dictionary<int, StrongCfg> dic))
                {
                    strongDic[sc.pos] = new Dictionary<int, StrongCfg>();
                }

                strongDic[sc.pos][sc.starlv] = sc;
            }

            PECommon.Log("StrongCfg initialized");
        }

        public StrongCfg GetStrongCfg(int pos, int starlv)
        {
            if (strongDic.TryGetValue(pos, out Dictionary<int, StrongCfg> dic))
            {
                if (dic.TryGetValue(starlv, out StrongCfg sc))
                {
                    return sc;
                }
            }

            return null;
        }

        #endregion

        #region 地图配置

        private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();

        private void InitMapCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Config\map_v4.xml");
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg()
                {
                    ID = ID
                };
                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                    }
                }

                mapCfgDataDic.Add(ID, mc);
            }
        }

        public MapCfg GetMapCfgData(int id)
        {
            return mapCfgDataDic.TryGetValue(id, out MapCfg data) ? data : null;
        }

        #endregion
    }

    public class BaseData<T>
    {
        public int ID;
    }

    public class GuideCfg : BaseData<GuideCfg>
    {
        public int coin; //完成奖励金币
        public int exp; //完成奖励经验
    }

    public class StrongCfg : BaseData<StrongCfg>
    {
        public int pos;
        public int starlv;
        public int addhp;
        public int addhurt;
        public int adddef;
        public int minlv;
        public int coin;
        public int crystal;
    }

    public class TaskRewardCfg : BaseData<TaskRewardCfg>
    {
        public int count;
        public int exp;
        public int coin;
    }

    public class TaskRewardData : BaseData<TaskRewardData>
    {
        public int prgs;
        public bool taked;
    }

    public class MapCfg : BaseData<MapCfg>
    {
        public int power;
        public int coin;
        public int exp;
        public int crystal;
    }
}