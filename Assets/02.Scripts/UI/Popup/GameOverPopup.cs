using Framework.Game.Defense;
using Framework.Sound;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class GameOverPopup : PopupTemplate
    {
        public TextMeshProUGUI text_GoScore;
        public TextMeshProUGUI text_Wave;
        public GameObject obj_bounsWave;
        public TextMeshProUGUI text_bounsWave;
        public Button button_Confirm;
        public Animation anim;

        public void SetGameOver(int goScore, int waveIdx, int boundWave = 0)
        {
            //Debug.Log("goScore : " + goScore);
            anim.Play();
            text_Wave.text = waveIdx.ToString();
            text_GoScore.text = goScore.ToString();

            if (boundWave > 0)
            {
                obj_bounsWave.SetActive(true);
                text_bounsWave.text = $"+{boundWave}";
            }
            else
            {
                obj_bounsWave.SetActive(false);
            }
        }

        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            anim.Stop();

            button_Confirm.onClick.AddListener(() => OnClick_Home());
        }
        private bool goingHome = false;

        public void OnClick_Home()
        {
            if (goingHome) return;
            goingHome = true;

            SceneLoadManager.onCompleteLoadScene = () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.BGM_LOBBY);
                SceneLoadManager.onCompleteLoadScene = null;
                goingHome = false;
            };
            SceneLoadManager.Instance.SwitchingScene(2);

            GameManager.Instance.objectPoolManager.AllClear();
        }
    }
}
