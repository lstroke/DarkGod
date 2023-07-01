using System;
using Battle.Manager;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 地图触发数据类
    /// </summary>
    public class DoorData : MonoBehaviour
    {
        public MapMgr mapMgr;
        public BoxCollider boxCollider;
        public GameObject[] effectGo;

        public int triggerWave;

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (mapMgr != null)
                {
                    mapMgr.TriggerMonsterBorn(this, triggerWave);
                }
            }
        }
    }
}