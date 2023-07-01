using Battle;
using Battle.Manager;
using Common;
using PEProtocol;
using UIWindow;
using UnityEngine;

namespace System
{
    /// <summary>
    /// 战斗业务系统
    /// </summary>
    public class BattleSys : SystemRoot
    {
        public static BattleSys Instance;

        public PlayerCtrlWnd playerCtrlWnd;
        public BattleEndWnd battleEndWnd;

        [HideInInspector] public BattleMgr battleMgr;

        private int fbid;
        private double startTime;

        public override void InitSys()
        {
            base.InitSys();
            Instance = this;
            print("Init BattleSys...");
        }

        public void StartBattle(int mapId)
        {
            fbid = mapId;
            GameObject go = new GameObject
            {
                name = "BattleRoot"
            };
            go.transform.SetParent(GameRoot.Instance.transform);
            battleMgr = go.AddComponent<BattleMgr>();
            battleMgr.Init(mapId, () => { startTime = timerSvc.GteNowTime(); });
            SetPlayerCtrlWndState();
        }

        public void EndBattle(bool isWin, int restHP)
        {
            playerCtrlWnd.SetWndState(false);
            GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();
            playerCtrlWnd.SetBossHPBarState(false);

            if (isWin)
            {
                double endTime = timerSvc.GteNowTime();
                GameMsg msg = new GameMsg()
                {
                    cmd = (int)CMD.ReqFBFightEnd,
                    reqFbFightEnd = new ReqFBFightEnd()
                    {
                        win = isWin,
                        fbid = fbid,
                        restHP = restHP,
                        costTime = (int)((endTime - startTime) / 1000),
                    }
                };
                netSvc.SendMsg(msg);
            }
            else
            {
                SetBattleEndWndState(FBEndType.Lose);
            }
        }

        public void RspFightEnd(GameMsg msg)
        {
            RspFBFightEnd data = msg.rspFbFightEnd;
            GameRoot.Instance.SetPlayerDataByFBEnd(data);

            battleEndWnd.SetBattleEndData(data.fbid, data.costTime, data.restHP);
            SetBattleEndWndState(FBEndType.Win);
        }

        public void DestroyBattle()
        {
            GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();
            SetPlayerCtrlWndState(false);
            SetBattleEndWndState(FBEndType.None, false);
            Destroy(battleMgr.gameObject);
        }

        public void SetBattleEndWndState(FBEndType fbEndType, bool isActive = true)
        {
            battleEndWnd.SetEndType(fbEndType);
            battleEndWnd.SetWndState(isActive);
        }

        public void SetPlayerCtrlWndState(bool isActive = true)
        {
            playerCtrlWnd.SetWndState(isActive);
        }

        public void SetMoveDir(Vector2 dir)
        {
            battleMgr.SetSelfPlayerMoveDir(dir);
        }

        public void ReqReleaseSkill(int index)
        {
            battleMgr.ReqReleaseSkill(index);
        }

        public Vector2 GetDirInput()
        {
            return playerCtrlWnd.currentDir;
        }
    }
}