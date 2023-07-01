using Common;
using PEProtocol;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle.Manager
{
    /// <summary>
    /// 地图管理器
    /// </summary>
    public class MapMgr : MonoBehaviour
    {
        public DoorData[] doorArr;
        private BattleMgr battleMgr;
        private int waveIndex = 1;

        public void Init(BattleMgr battle)
        {
            PECommon.Log("MapMgr Initialized");
            battleMgr = battle;
            battleMgr.LoadMonsterByWaveID(waveIndex);
        }

        public bool SetNextTriggerOn()
        {
            waveIndex++;
            foreach (var doorData in doorArr)
            {
                if (doorData.triggerWave == waveIndex)
                {
                    setDoor(doorData, true);
                    return true;
                }
            }

            return false;
        }

        public void TriggerMonsterBorn(DoorData doorData, int waveIdx)
        {
            setDoor(doorData, false);
            battleMgr.LoadMonsterByWaveID(waveIdx);
            battleMgr.onBorn = false;
            battleMgr.ActiveCurrentBatchMonster();
        }

        private void setDoor(DoorData doorData, bool state)
        {
            doorData.boxCollider.isTrigger = state;
            foreach (var go in doorData.effectGo)
            {
                go.SetActive(!state);
            }
        }
    }
}