using System;
using System.Collections.Generic;
using System.Xml;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using LogType = PEProtocol.LogType;
using Random = System.Random;

namespace Service
{
    /// <summary>
    /// 资源加载服务
    /// </summary>
    public class ResSvc : MonoBehaviour
    {
        public static ResSvc Instance;
        private Action prgCB = null;


        public void InitSvc()
        {
            print("Init ResSvc...");
            InitRDNameCfg(PathDefine.RDNameCfg);
            InitMonsterDicCfg(PathDefine.MonsterCfg);
            InitMapCfg(PathDefine.MapCfg);
            InitGuideCfg(PathDefine.GuideCfg);
            InitStrongCfg(PathDefine.StrongCfg);
            InitTaskRewardCfg(PathDefine.TaskRewardCfg);
            InitSkillActionCfg(PathDefine.SkillActionCfg);
            InitSkillCfg(PathDefine.SkillCfg);
            InitSkillMoveCfg(PathDefine.SkillMoveCfg);
            Instance = this;
        }

        private void Update()
        {
            if (prgCB != null)
            {
                prgCB();
            }
        }

        public void AsyncLoadScene(string sceneName, Action loaded)
        {
            GameRoot.Instance.loadingWnd.SetWndState();
            var sceneAsync = SceneManager.LoadSceneAsync(sceneName);
            prgCB = () =>
            {
                float val = sceneAsync.progress;
                GameRoot.Instance.loadingWnd.SetProgress(val);
                if (val == 1)
                {
                    loaded?.Invoke();
                    prgCB = null;
                    sceneAsync = null;
                    GameRoot.Instance.loadingWnd.SetWndState(false);
                }
            };
        }

        private Dictionary<string, AudioClip> adDic = new();

        public AudioClip LoadAudioClip(string path, bool cache = false)
        {
            AudioClip ac;
            if (!adDic.TryGetValue(path, out ac))
            {
                ac = Resources.Load<AudioClip>(path);
                if (cache)
                {
                    adDic[path] = ac;
                }
            }

            return ac;
        }

        private Dictionary<string, GameObject> goDic = new();

        public GameObject LoadPrefab(string path, bool cache = false)
        {
            GameObject prefab;
            if (!goDic.TryGetValue(path, out prefab))
            {
                prefab = Resources.Load<GameObject>(path);
                if (cache)
                {
                    goDic[path] = prefab;
                }
            }

            GameObject go = null;
            if (prefab) go = Instantiate(prefab);
            return go;
        }

        private Dictionary<string, Sprite> spDic = new();

        public Sprite LoadSprite(string path, bool cache = false)
        {
            Sprite sp;
            if (!spDic.TryGetValue(path, out sp))
            {
                sp = Resources.Load<Sprite>(path);
                if (cache)
                {
                    spDic[path] = sp;
                }
            }

            return sp;
        }

        #region InitCfgs

        #region 随机名字读取配置

        private List<string> surnameList = new();
        private List<string> manList = new();
        private List<string> womanList = new();

        private void InitRDNameCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "surname":
                                surnameList.Add(e.InnerText);
                                break;
                            case "man":
                                manList.Add(e.InnerText);
                                break;
                            case "woman":
                                womanList.Add(e.InnerText);
                                break;
                        }
                    }
                }
            }
        }

        public string GetRDNameData(bool man = true)
        {
            Random rd = new Random();
            string rdName = surnameList[PETools.RDInt(0, surnameList.Count - 1, rd)];
            if (man)
            {
                rdName += manList[PETools.RDInt(0, manList.Count - 1, rd)];
            }
            else
            {
                rdName += womanList[PETools.RDInt(0, womanList.Count - 1, rd)];
            }

            return rdName;
        }

        #endregion

        #region 地图

        private Dictionary<int, MapCfg> mapCfgDataDic = new();

        private void InitMapCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    MapCfg mc = new MapCfg()
                    {
                        ID = ID,
                        monsterLst = new List<MonsterData>()
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "mapName":
                                mc.mapName = e.InnerText;
                                break;
                            case "sceneName":
                                mc.sceneName = e.InnerText;
                                break;
                            case "power":
                                mc.power = int.Parse(e.InnerText);
                                break;
                            case "mainCamPos":
                            {
                                string[] valArr = e.InnerText.Split(",");
                                mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "mainCamRote":
                            {
                                string[] valArr = e.InnerText.Split(",");
                                mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornPos":
                            {
                                string[] valArr = e.InnerText.Split(",");
                                mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornRote":
                            {
                                string[] valArr = e.InnerText.Split(",");
                                mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "monsterLst":
                            {
                                string[] valArr = e.InnerText.Split("#");
                                for (int waveIndex = 1; waveIndex < valArr.Length; waveIndex++)
                                {
                                    string[] tempArr = valArr[waveIndex].Split('|');
                                    for (int j = 1; j < tempArr.Length; j++)
                                    {
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData md = new MonsterData
                                        {
                                            ID = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfgData(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]),
                                                float.Parse(arr[3])),
                                            mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                            mLevel = int.Parse(arr[5])
                                        };
                                        mc.monsterLst.Add(md);
                                    }
                                }
                            }
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
        }

        public MapCfg GetMapCfgData(int id)
        {
            return mapCfgDataDic.TryGetValue(id, out MapCfg data) ? data : null;
        }

        #endregion

        #region 自动引导配置

        private Dictionary<int, AutoGuideCfg> guideTaskDic = new();

        private void InitGuideCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    AutoGuideCfg agc = new AutoGuideCfg()
                    {
                        ID = ID
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "npcID":
                                agc.npcID = int.Parse(e.InnerText);
                                break;
                            case "dilogArr":
                                agc.dilogArr = e.InnerText;
                                break;
                            case "actID":
                                agc.actID = int.Parse(e.InnerText);
                                break;
                            case "coin":
                                agc.coin = int.Parse(e.InnerText);
                                break;
                            case "exp":
                                agc.exp = int.Parse(e.InnerText);
                                break;
                        }
                    }

                    guideTaskDic[ID] = agc;
                }
            }
        }

        public AutoGuideCfg GetAutoGuideData(int id)
        {
            if (guideTaskDic.TryGetValue(id, out AutoGuideCfg agc))
            {
                return agc;
            }

            return null;
        }

        #endregion

        #region 任务奖励配置

        private Dictionary<int, TaskRewardCfg> taskRewardDic = new();

        private void InitTaskRewardCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
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
                            case "taskName":
                                trc.taskName = e.InnerText;
                                break;
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
            }
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

        #region 强化升级配置

        private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new();

        private void InitStrongCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
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
                        strongDic[sc.pos] = new();
                    }

                    strongDic[sc.pos][sc.starlv] = sc;
                }
            }
        }

        public StrongCfg GetStrongData(int pos, int starlv)
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

        public int GetPropAddValPreLv(int pos, int starlv, int type)
        {
            int val = 0;
            if (strongDic.TryGetValue(pos, out Dictionary<int, StrongCfg> dic))
            {
                for (int i = 0; i <= starlv; i++)
                {
                    if (dic.TryGetValue(i, out StrongCfg sc))
                    {
                        val += type switch
                        {
                            1 => sc.addhp,
                            2 => sc.addhurt,
                            3 => sc.adddef,
                            _ => 0
                        };
                    }
                }
            }

            return val;
        }

        #endregion

        #region 技能配置

        private Dictionary<int, SkillCfg> skillDic = new();

        private void InitSkillCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    SkillCfg sc = new SkillCfg()
                    {
                        ID = ID,
                        skillMoveLst = new List<int>(),
                        skillActionLst = new List<int>(),
                        skillDamageLst = new List<int>(),
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "skillName":
                                sc.skillName = e.InnerText;
                                break;
                            case "cdTime":
                                sc.cdTime = int.Parse(e.InnerText);
                                break;
                            case "skillTime":
                                sc.skillTime = int.Parse(e.InnerText);
                                break;
                            case "aniAction":
                                sc.aniAction = int.Parse(e.InnerText);
                                break;
                            case "fx":
                                sc.fx = e.InnerText;
                                break;
                            case "isCombo":
                                sc.isCombo = "1".Equals(e.InnerText);
                                break;
                            case "isCollide":
                                sc.isCollide = "1".Equals(e.InnerText);
                                break;
                            case "isBreak":
                                sc.isBreak = "1".Equals(e.InnerText);
                                break;
                            case "dmgType":
                                sc.dmgType = (DamageType)int.Parse(e.InnerText);
                                break;
                            case "skillMoveLst":
                                string[] skillMoveStrArr = e.InnerText.Split('|');
                                foreach (var skillMoveId in skillMoveStrArr)
                                {
                                    sc.skillMoveLst.Add(int.Parse(skillMoveId));
                                }

                                break;
                            case "skillActionLst":
                                string[] skillActionStrArr = e.InnerText.Split('|');
                                foreach (var id in skillActionStrArr)
                                {
                                    sc.skillActionLst.Add(int.Parse(id));
                                }

                                break;
                            case "skillDamageLst":
                                string[] skillDamageStrArr = e.InnerText.Split('|');
                                foreach (var damage in skillDamageStrArr)
                                {
                                    sc.skillDamageLst.Add(int.Parse(damage));
                                }

                                break;
                        }
                    }

                    skillDic.Add(ID, sc);
                }
            }
        }

        public SkillCfg GetSkillCfgData(int id)
        {
            return skillDic.TryGetValue(id, out SkillCfg data) ? data : null;
        }

        #endregion

        #region 技能移动配置

        private Dictionary<int, SkillMoveCfg> skillMoveDic = new();

        private void InitSkillMoveCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    SkillMoveCfg smc = new SkillMoveCfg()
                    {
                        ID = ID
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "delayTime":
                                smc.delayTime = int.Parse(e.InnerText);
                                break;
                            case "moveTime":
                                smc.moveTime = int.Parse(e.InnerText);
                                break;
                            case "moveDis":
                                smc.moveDis = float.Parse(e.InnerText);
                                break;
                        }
                    }

                    skillMoveDic.Add(ID, smc);
                }
            }
        }

        public SkillMoveCfg GetSkillMoveCfgData(int id)
        {
            return skillMoveDic.TryGetValue(id, out SkillMoveCfg data) ? data : null;
        }

        #endregion

        #region 技能移动配置

        private Dictionary<int, SkillActionCfg> skillActionDic = new();

        private void InitSkillActionCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    SkillActionCfg sac = new SkillActionCfg()
                    {
                        ID = ID
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "delayTime":
                                sac.delayTime = int.Parse(e.InnerText);
                                break;
                            case "radius":
                                sac.radius = float.Parse(e.InnerText);
                                break;
                            case "angle":
                                sac.angle = int.Parse(e.InnerText);
                                break;
                        }
                    }

                    skillActionDic.Add(ID, sac);
                }
            }
        }

        public SkillActionCfg GetSkillActionCfgData(int id)
        {
            return skillActionDic.TryGetValue(id, out SkillActionCfg data) ? data : null;
        }

        #endregion

        #region 怪物配置

        private Dictionary<int, MonsterCfg> monsterDic = new();

        private void InitMonsterDicCfg(string path)
        {
            TextAsset xml = Resources.Load<TextAsset>(path);
            if (!xml)
            {
                PECommon.Log("xml file" + path + " not found", LogType.Error);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.text);
                XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    var ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                    MonsterCfg mc = new MonsterCfg
                    {
                        ID = ID,
                        bps = new BattleProps()
                    };
                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "mName":
                                mc.mName = e.InnerText;
                                break;
                            case "mType":
                                mc.mType = (MonsterType)int.Parse(e.InnerText);
                                break;
                            case "isStop":
                                mc.isStop = "1".Equals(e.InnerText);
                                break;
                            case "resPath":
                                mc.resPath = e.InnerText;
                                break;
                            case "skillID":
                                mc.skillID = int.Parse(e.InnerText);
                                break;
                            case "atkDis":
                                mc.atkDis = float.Parse(e.InnerText);
                                break;
                            case "hp":
                                mc.bps.hp = int.Parse(e.InnerText);
                                break;
                            case "ad":
                                mc.bps.ad = int.Parse(e.InnerText);
                                break;
                            case "ap":
                                mc.bps.ap = int.Parse(e.InnerText);
                                break;
                            case "addef":
                                mc.bps.addef = int.Parse(e.InnerText);
                                break;
                            case "apdef":
                                mc.bps.apdef = int.Parse(e.InnerText);
                                break;
                            case "dodge":
                                mc.bps.dodge = int.Parse(e.InnerText);
                                break;
                            case "pierce":
                                mc.bps.pierce = int.Parse(e.InnerText);
                                break;
                            case "critical":
                                mc.bps.critical = int.Parse(e.InnerText);
                                break;
                        }
                    }

                    monsterDic.Add(ID, mc);
                }
            }
        }

        public MonsterCfg GetMonsterCfgData(int id)
        {
            return monsterDic.TryGetValue(id, out MonsterCfg data) ? data : null;
        }

        #endregion

        #endregion
    }
}