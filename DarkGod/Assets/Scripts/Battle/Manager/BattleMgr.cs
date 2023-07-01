using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Controller;
using Battle.Entity;
using Battle.FSM;
using Common;
using PEProtocol;
using Service;
using UnityEngine;

namespace Battle.Manager
{
    /// <summary>
    /// 战场管理器
    /// </summary>
    public class BattleMgr : MonoBehaviour
    {
        private ResSvc resSvc;
        private AudioSvc audioSvc;

        private StateMgr stateMgr;
        private SkillMgr skillMgr;
        private MapMgr mapMgr;
        private EntityPlayer entitySelfPlayer;
        private MapCfg mc;
        [HideInInspector] public bool onBorn = false;
        [HideInInspector] public bool isPauseGame = false;

        private Dictionary<string, EntityMonster> monsterDic = new();

        public void Init(int mapId, Action cb)
        {
            resSvc = ResSvc.Instance;
            audioSvc = AudioSvc.Instance;

            //初始化管理器
            stateMgr = gameObject.AddComponent<StateMgr>();
            stateMgr.Init();
            skillMgr = gameObject.AddComponent<SkillMgr>();
            skillMgr.Init();

            //加载场景地图
            mc = resSvc.GetMapCfgData(mapId);
            resSvc.AsyncLoadScene(mc.sceneName, () =>
            {
                //初始化地图数据
                GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
                mapMgr = map.GetComponent<MapMgr>();
                mapMgr.Init(this);
                map.transform.localPosition = Vector3.zero;
                map.transform.localScale = Vector3.one;

                Camera.main.transform.position = mc.mainCamPos;
                Camera.main.transform.eulerAngles = mc.mainCamRote;

                LoadPlayer(mc);

                ActiveCurrentBatchMonster();

                audioSvc.PlayBGMusic(Constants.BGHuangYe);
                
                cb?.Invoke();
            });
            PECommon.Log("BattleMgr Initialized");
        }

        private void Update()
        {
            foreach (var item in monsterDic)
            {
                item.Value.TickAILogic();
            }

            //检测当前批次怪物是否全部死亡
            if (!onBorn && mapMgr != null)
            {
                if (monsterDic.Count == 0)
                {
                    bool isExist = mapMgr.SetNextTriggerOn();
                    onBorn = true;
                    if (!isExist)
                    {
                        //关卡结束，战斗胜利
                        EndBattle(true, entitySelfPlayer.HP);
                    }
                }
            }
        }

        public void EndBattle(bool isWin, int restHP)
        {
            isPauseGame = true;
            audioSvc.StopBGMusic();
            BattleSys.Instance.EndBattle(isWin, restHP);
        }

        private void LoadPlayer(MapCfg mc)
        {
            GameObject player = resSvc.LoadPrefab(PathDefine.AssassinBattlePlayer, true);

            player.transform.position = mc.playerBornPos;
            player.transform.eulerAngles = mc.playerBornRote;
            player.transform.localScale = Vector3.one;
            player.GetComponent<CharacterController>().enabled = true;
            PlayerController playerCtrl = player.GetComponent<PlayerController>();
            playerCtrl.Init();
            entitySelfPlayer = new EntityPlayer
            {
                stateMgr = stateMgr,
                skillMgr = skillMgr,
                battleMgr = this
            };
            entitySelfPlayer.SetController(playerCtrl);
            PlayerData pd = GameRoot.Instance.PlayerData;
            entitySelfPlayer.SetBattleProps(new BattleProps
            {
                hp = pd.hp,
                ad = pd.ad,
                ap = pd.ap,
                addef = pd.addef,
                apdef = pd.apdef,
                dodge = pd.dodge,
                pierce = pd.pierce,
                critical = pd.critical
            });
            entitySelfPlayer.Name = player.name;
            entitySelfPlayer.Idle();
        }

        public void LoadMonsterByWaveID(int wave)
        {
            for (int i = 0; i < mc.monsterLst.Count; i++)
            {
                MonsterData md = mc.monsterLst[i];
                if (md.mWave == wave)
                {
                    GameObject m = resSvc.LoadPrefab(md.mCfg.resPath, true);
                    m.transform.position = md.mBornPos;
                    m.transform.localEulerAngles = md.mBornRote;
                    m.transform.localScale = Vector3.one;

                    m.name = "m" + md.mWave + "_" + md.mIndex;

                    MonsterController mCtrl = m.GetComponent<MonsterController>();
                    mCtrl.Init();

                    EntityMonster em = new EntityMonster
                    {
                        stateMgr = stateMgr,
                        skillMgr = skillMgr,
                        battleMgr = this
                    };
                    em.SetController(mCtrl);
                    em.md = md;
                    em.Name = m.name;
                    em.SetBattleProps(md.mCfg.bps);
                    monsterDic.Add(m.name, em);
                    m.SetActive(false);
                    if (md.mCfg.mType == MonsterType.Normal)
                    {
                        GameRoot.Instance.dynamicWnd.AddHpItemInfo(m.name, mCtrl.hpRoot, em.HP);
                    }
                    else if (md.mCfg.mType == MonsterType.Boss)
                    {
                        BattleSys.Instance.playerCtrlWnd.SetBossHPBarState(true, em.HP);
                    }
                }
            }
        }

        public void ActiveCurrentBatchMonster()
        {
            TimerSvc.Instance.AddTimeTask((tid) =>
            {
                foreach (var item in monsterDic)
                {
                    item.Value.SetActive();
                    item.Value.Born();
                    TimerSvc.Instance.AddTimeTask((tid) => { item.Value.Idle(); }, 1000);
                }
            }, 500);
        }

        public List<EntityMonster> GetEntityMonsters()
        {
            return monsterDic.Values.ToList();
        }

        public EntityPlayer GetEntityPlayer()
        {
            return entitySelfPlayer;
        }

        public void RmvMonster(string key)
        {
            if (monsterDic.TryGetValue(key, out EntityMonster entityMonster))
            {
                monsterDic.Remove(key);
                GameRoot.Instance.dynamicWnd.RmvHpItemInfo(key);
            }
        }

        #region 角色控制与技能

        public void SetSelfPlayerMoveDir(Vector2 dir)
        {
            if (!entitySelfPlayer.canControl)
            {
                return;
            }

            if (entitySelfPlayer.currentAniState is AniState.Idle or AniState.Move)
            {
                entitySelfPlayer.SetDir(dir);
                if (dir == Vector2.zero)
                {
                    entitySelfPlayer.Idle();
                }
                else
                {
                    entitySelfPlayer.Move();
                }
            }
        }

        public void ReqReleaseSkill(int index)
        {
            switch (index)
            {
                case 0:
                    ReleaseNormalAtk();
                    break;
                case 1:
                    ReleaseSkill1();
                    break;
                case 2:
                    ReleaseSkill2();
                    break;
                case 3:
                    ReleaseSkill3();
                    break;
            }
        }

        private int[] comboArr =
        {
            Constants.PlayerAttack1, Constants.PlayerAttack2, Constants.PlayerAttack3, Constants.PlayerAttack4,
            Constants.PlayerAttack5
        };

        [HideInInspector] public int comboIndex;

        [HideInInspector] public double lastAtkTime;

        private void ReleaseNormalAtk()
        {
            double nowAtkTime = TimerSvc.Instance.GteNowTime();
            if (entitySelfPlayer.currentAniState == AniState.Attack)
            {
                if (entitySelfPlayer.comboQue.Count == 0 && nowAtkTime - lastAtkTime < Constants.ComboSpace &&
                    lastAtkTime != 0 && ++comboIndex < comboArr.Length)
                {
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                }
            }
            else if (entitySelfPlayer.currentAniState is AniState.Idle or AniState.Move)
            {
                comboIndex = 0;
                entitySelfPlayer.Attack(comboArr[comboIndex]);
            }

            lastAtkTime = nowAtkTime;
        }

        private void ReleaseSkill1()
        {
            entitySelfPlayer.Attack(Constants.PlayerSkill1);
        }

        private void ReleaseSkill2()
        {
            entitySelfPlayer.Attack(Constants.PlayerSkill2);
        }

        private void ReleaseSkill3()
        {
            entitySelfPlayer.Attack(Constants.PlayerSkill3);
        }

        public Vector2 GetDirInput()
        {
            return BattleSys.Instance.GetDirInput();
        }

        public bool CanRlsSkill()
        {
            return entitySelfPlayer.canRlsSkill;
        }

        #endregion
    }
}