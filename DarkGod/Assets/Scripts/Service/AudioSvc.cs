using UnityEngine;

namespace Service
{
    public class AudioSvc : MonoBehaviour
    {
        public static AudioSvc Instance;
        public AudioSource bgAudio;
        public AudioSource uiAudio;

        public void InitSvc()
        {
            Instance = this;
            print("Init AudioSvc...");
        }

        public void PlayBGMusic(string audioName, bool isLoop = true)
        {
            AudioClip audioClip = ResSvc.Instance.LoadAudioClip("ResAudio/" + audioName, true);
            if (bgAudio.clip == null || bgAudio.clip.name != audioClip.name)
            {
                bgAudio.clip = audioClip;
                bgAudio.loop = isLoop;
                bgAudio.Play();
            }
        }

        public void StopBGMusic()
        {
            if (bgAudio != null)
            {
                bgAudio.Stop();
            }
        }

        public void PlayUIMusic(string audioName)
        {
            AudioClip audioClip = ResSvc.Instance.LoadAudioClip("ResAudio/" + audioName, true);
            uiAudio.clip = audioClip;
            uiAudio.Play();
        }

        public void PlayCharAudio(string clipName, AudioSource charAudio)
        {
            AudioClip clip = ResSvc.Instance.LoadAudioClip("ResAudio/" + clipName, true);
            charAudio.clip = clip;
            charAudio.Play();
        }
    }
}