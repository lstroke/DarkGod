using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UIWindow
{
    /// <summary>
    /// 动态UI元素界面
    /// </summary>
    public class DynamicWnd : WindowRoot
    {
        public Animation tipsAni;
        public Text txtTips;
        public Transform hpItemRoot;
        public Animation selfDodgeAni;

        private AnimationClip clip;
        private bool isTipsShow = false;

        private Queue<string> tipsQueue = new();
        private Dictionary<string, ItemEntityHp> hpDic = new();


        protected override void InitWnd()
        {
            base.InitWnd();
            clip = tipsAni.GetClip("TipsShow");
            SetActive(txtTips, false);
        }

        #region Tips

        public void AddTips(string tips)
        {
            lock (tipsQueue)
            {
                tipsQueue.Enqueue(tips);
            }
        }

        private void Update()
        {
            lock (tipsQueue)
            {
                if (!isTipsShow && tipsQueue.Count > 0)
                {
                    string tips = tipsQueue.Dequeue();
                    SetTips(tips);
                }
            }
        }

        private void SetTips(string tips)
        {
            isTipsShow = true;
            SetText(txtTips, tips);
            SetActive(txtTips);
            tipsAni.Play();
            StartCoroutine(AniPlayDone(clip.length, () =>
            {
                isTipsShow = false;
                SetActive(txtTips, false);
            }));
        }

        private IEnumerator AniPlayDone(float sec, Action cb)
        {
            yield return new WaitForSeconds(sec);
            cb?.Invoke();
        }

        #endregion

        #region 血条

        public void AddHpItemInfo(string mName, Transform root, int hp)
        {
            if (hpDic.TryGetValue(mName, out ItemEntityHp item))
            {
                return;
            }

            GameObject go = resSvc.LoadPrefab(PathDefine.HpItemPrefab, true);
            go.transform.SetParent(hpItemRoot);
            go.transform.localPosition = new Vector3(-1000, 0, 0);
            ItemEntityHp ieh = go.GetComponent<ItemEntityHp>();
            ieh.InitItemInfo(root, hp);
            hpDic.Add(mName, ieh);
        }

        public void RmvHpItemInfo(string mName)
        {
            if (hpDic.TryGetValue(mName, out ItemEntityHp item))
            {
                Destroy(item.gameObject);
                hpDic.Remove(mName);
            }
        }

        public void RmvAllHpItemInfo()
        {
            foreach (var item in hpDic)
            {
                Destroy(item.Value.gameObject);
            }

            hpDic.Clear();
        }

        public void SetDodge(string key)
        {
            if (hpDic.TryGetValue(key, out ItemEntityHp item))
            {
                item.SetDodge();
            }
        }

        public void SetCritical(string key, int critical)
        {
            if (hpDic.TryGetValue(key, out ItemEntityHp item))
            {
                item.SetCritical(critical);
            }
        }

        public void SetHurt(string key, int hurt)
        {
            if (hpDic.TryGetValue(key, out ItemEntityHp item))
            {
                item.SetHurt(hurt);
            }
        }

        public void SetHPVal(string key, int curHP)
        {
            if (hpDic.TryGetValue(key, out ItemEntityHp item))
            {
                item.setHPVal(curHP);
            }
        }

        #endregion

        public void SetSelfDodge()
        {
            selfDodgeAni.Stop();
            selfDodgeAni.Play();
        }
    }
}