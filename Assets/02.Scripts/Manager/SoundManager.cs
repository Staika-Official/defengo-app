using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

namespace Framework.Sound
{
    public enum SoundType
    {
        BGM,
        SFX
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public AudioSource audio_BGM;
        public AudioSource audio_SFX;

        public SoundData soundData_LobbyBGM;

        public Dictionary<SoundKey, SoundData> dic_SoundData = new();

        public bool isBgmMute = false;
        public bool isSfxMute = false;

        public float sfxVolume;
        public float bgmVolume;

        private void Start()
        {
            if(Instance is null)
            {
                Instance = this;
                audio_BGM.clip = soundData_LobbyBGM.audioClip;
                audio_BGM.Play();

                audio_BGM.volume = SecurePlayerPrefs.GetFloat("bgmVolume", 1);
                audio_SFX.volume = SecurePlayerPrefs.GetFloat("sfxVolume", 1);
            }
        }

        public void SetBgmVolume(float value)
        {
            audio_BGM.volume = value;
        }

        public void SetSfxVolume(float value)
        {
            audio_SFX.volume = value;
        }

        public void PlaySound(SoundKey soundKey)
        {
            if (dic_SoundData.ContainsKey(soundKey))
            {
                SoundData soundData = dic_SoundData[soundKey];

                switch (soundData.soundType)
                {
                    case SoundType.BGM:
                        audio_BGM.clip = soundData.audioClip;
                        audio_BGM.Play();
                        break;
                    case SoundType.SFX:
                        audio_SFX.PlayOneShot(soundData.audioClip);
                        break;
                }
            }
            else
            {
                Debug.Log("Not SoundData ContainsKey");
            }
        }
    }
}