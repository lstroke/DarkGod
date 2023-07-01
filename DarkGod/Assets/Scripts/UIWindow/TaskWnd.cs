using System;
using System.Collections.Generic;
using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 任务奖励界面
    /// </summary>
    public class TaskWnd : WindowRoot
    {
        public Transform scrollTrans;
        private PlayerData pd;
        private List<TaskRewardData> trdLst = new();

        protected override void InitWnd()
        {
            base.InitWnd();
            pd = GameRoot.Instance.PlayerData;
            RefreshUI();
        }

        public void RefreshUI()
        {
            trdLst.Clear();
            List<TaskRewardData> finishLst = new();
            List<TaskRewardData> todoLst = new();
            List<TaskRewardData> doneLst = new();

            foreach (var taskStr in pd.taskArr)
            {
                string[] taskInfo = taskStr.Split("|");
                TaskRewardData trd = new TaskRewardData()
                {
                    ID = int.Parse(taskInfo[0]),
                    prgs = int.Parse(taskInfo[1]),
                    taked = "1".Equals(taskInfo[2])
                };
                if (trd.taked)
                {
                    doneLst.Add(trd);
                }
                else
                {
                    TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trd.ID);
                    if (trd.prgs == trc.count)
                    {
                        finishLst.Add(trd);
                    }
                    else
                    {
                        todoLst.Add(trd);
                    }
                }
            }

            trdLst.AddRange(finishLst);
            trdLst.AddRange(todoLst);
            trdLst.AddRange(doneLst);

            for (int i = 0; i < scrollTrans.childCount; i++)
            {
                Destroy(scrollTrans.GetChild(i).gameObject);
            }

            foreach (var trd in trdLst)
            {
                GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab,true);
                go.transform.SetParent(scrollTrans);

                TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trd.ID);

                SetText(FindTrans(go.transform, "txtName"), trc.taskName);
                SetText(FindTrans(go.transform, "txtPrg"), trd.prgs + "/" + trc.count);
                SetText(FindTrans(go.transform, "txtExp"), "奖励：    经验" + trc.exp);
                SetText(FindTrans(go.transform, "txtCoin"), "金币" + trc.coin);

                Image imgPrg = FindComponent<Image>(go.transform, "prgBar/prgVal");
                imgPrg.fillAmount = trd.prgs * 1f / trc.count;
                Button btnTake = FindComponent<Button>(go.transform, "btnTake");
                btnTake.onClick.AddListener(() => { ClickTakeBtn(trd.ID, btnTake); });
                Transform transComp = FindTrans(go.transform, "imgComp");
                if (trd.taked)
                {
                    btnTake.interactable = false;
                    SetActive(transComp);
                }
                else
                {
                    SetActive(transComp, false);
                    btnTake.interactable = trd.prgs == trc.count;
                }
            }
        }

        private void ClickTakeBtn(int tid, Button takebtn)
        {
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.ReqTaskReward,
                reqTaskReward = new ReqTaskReward()
                {
                    rid = tid
                }
            };
            netSvc.SendMsg(msg);
            takebtn.interactable = false;
        }

        public void ClickCloseBtn()
        {
            audioSvc.PlayUIMusic(Constants.UIClickBtn);
            SetWndState(false);
        }
    }
}