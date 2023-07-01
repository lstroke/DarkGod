using System;
using Service;
using UnityEngine;

namespace Common
{
    public class PlayMusic : MonoBehaviour
    {
        private AudioSvc audioSvc;

        private void Start()
        {
            audioSvc = AudioSvc.Instance;
        }

        public void PlayUIMusic(string audioName)
        {
            audioSvc.PlayUIMusic(audioName);
        }
    }
}