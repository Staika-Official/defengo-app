using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Sound;
using UnityEngine.UI;
using Framework.Game.Defense;
using TMPro;

namespace Framework.UI
{
    public class PausePopup : PopupTemplate
    {
        public Slider slider_SFXVolume;
        public Slider slider_BGMVolume;
        public TextMeshProUGUI text_SFXVolume;
        public TextMeshProUGUI text_BGMVolume;

        public Button button_Home;
        public bool isPaused = false;
        //public GameObject[] soundIcons;

        public override void ActivePopup()
        {
            PauseAction = () =>
            {
                StartCoroutine(PauseTime(true));
            };
            PopUpSequence(true);
            isPaused = true;
        }

        public override void InActivePopup()
        {
            PauseAction = null;
            Time.timeScale = 1;
            isPaused = false;
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            button_Home.onClick.AddListener(() =>
            {
                InActivePopup();
                GameManager.Instance.GameOverHome();
            });

            float sfxVolume = SoundManager.Instance.audio_SFX.volume;
            float bgmVolume = SoundManager.Instance.audio_BGM.volume;

            slider_SFXVolume.value = sfxVolume;
            slider_BGMVolume.value = bgmVolume;

            text_BGMVolume.text = $"{(int)(bgmVolume * 100)}%";
            text_SFXVolume.text = $"{(int)(sfxVolume * 100)}%";

            slider_SFXVolume.onValueChanged.AddListener((sfxVol) => OnValueChangeSFX(sfxVol));
            slider_BGMVolume.onValueChanged.AddListener((bgmVol) => OnValueChangeBGM(bgmVol));

            //button_Mute.onClick.AddListener(() =>
            //{
            //    bool isBgmMute = SoundManager.Instance.audio_BGM.mute;
            //    bool isSfxMute = SoundManager.Instance.audio_SFX.mute;
            //    SoundManager.Instance.audio_BGM.mute = !isBgmMute;
            //    SoundManager.Instance.audio_SFX.mute = !isSfxMute;

            //    soundIcons[0].SetActive(SoundManager.Instance.audio_BGM.mute);
            //    soundIcons[1].SetActive(!SoundManager.Instance.audio_BGM.mute);
            //});
        }

        public void OnValueChangeBGM(float value)
        {
            SoundManager.Instance.audio_BGM.volume = value;
            text_BGMVolume.text = $"{(int)(value * 100)}%";
            SecurePlayerPrefs.SetFloat("bgmVolume", slider_BGMVolume.value);
        }

        public void OnValueChangeSFX(float value)
        {
            SoundManager.Instance.audio_SFX.volume = value;
            text_SFXVolume.text = $"{(int)(value * 100)}%";
            SecurePlayerPrefs.SetFloat("sfxVolume", slider_SFXVolume.value);
        }

        public IEnumerator PauseTime(bool isPause)
        {
            float scale = isPause ? 0 : 1;

            yield return new WaitForSeconds(0.05f);
            Time.timeScale = scale;
        }
    }
}
