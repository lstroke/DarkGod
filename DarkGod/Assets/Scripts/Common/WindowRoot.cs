using System;
using Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common
{
    /// <summary>
    /// UI界面基类
    /// </summary>
    public class WindowRoot : MonoBehaviour
    {
        protected ResSvc resSvc;
        protected AudioSvc audioSvc;
        protected NetSvc netSvc;
        protected TimerSvc timerSvc;

        public void SetWndState(bool isActive = true)
        {
            SetActive(gameObject, isActive);
            if (isActive)
            {
                InitWnd();
            }
            else
            {
                ClearWnd();
            }
        }

        public bool GetWndState()
        {
            return gameObject.activeSelf;
        }

        protected virtual void InitWnd()
        {
            resSvc = ResSvc.Instance;
            audioSvc = AudioSvc.Instance;
            netSvc = NetSvc.Instance;
            timerSvc = TimerSvc.Instance;
        }

        protected virtual void ClearWnd()
        {
            resSvc = null;
            audioSvc = null;
            netSvc = null;
            timerSvc = null;
        }

        #region Tool Functions

        protected void SetActive(GameObject go, bool isActive = true)
        {
            go.SetActive(isActive);
        }

        protected void SetActive(Transform go, bool isActive = true)
        {
            go.gameObject.SetActive(isActive);
        }

        protected void SetActive(RectTransform go, bool isActive = true)
        {
            go.gameObject.SetActive(isActive);
        }

        protected void SetActive(Image go, bool isActive = true)
        {
            go.gameObject.SetActive(isActive);
        }

        protected void SetActive(Text go, bool isActive = true)
        {
            go.gameObject.SetActive(isActive);
        }

        protected void SetText(Text txt, string context = "")
        {
            txt.text = context;
        }

        protected void SetText(Transform trans, string context = "")
        {
            trans.GetComponent<Text>().text = context;
        }

        protected void SetText(Text txt, float num)
        {
            SetText(txt, num.ToString());
        }

        protected void SetText(Transform trans, float num)
        {
            SetText(trans, num.ToString());
        }

        protected void SetSprite(Image img, string path)
        {
            Sprite sp = resSvc.LoadSprite(path, true);
            img.sprite = sp;
        }

        protected T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
            }

            return t;
        }

        protected Transform FindTrans(Transform trans, string FindName)
        {
            return trans ? trans.Find(FindName) : transform.Find(FindName);
        }

        protected T FindComponent<T>(Transform trans, string FindName) where T : Component
        {
            return FindTrans(trans, FindName).GetComponent<T>();
        }

        #endregion

        #region Click Evts

        protected void OnClick(GameObject go, Action<object> cb, object args)
        {
            PEListener listener = GetOrAddComponent<PEListener>(go);
            listener.onClick = cb;
            listener.args = args;
        }

        protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
        {
            PEListener listener = GetOrAddComponent<PEListener>(go);
            listener.onClickDown = cb;
        }

        protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
        {
            PEListener listener = GetOrAddComponent<PEListener>(go);
            listener.onClickUp = cb;
        }

        protected void OnDrag(GameObject go, Action<PointerEventData> cb)
        {
            PEListener listener = GetOrAddComponent<PEListener>(go);
            listener.onDrag = cb;
        }

        #endregion
    }
}