using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DeveloperComments : MonoBehaviour
    {
        public Text contentTextEng;
        public Text contentTextKor;
        private Text contentText;
        public Button returnBtn;
        public float speed = 20f;

        void Start()
        {
            contentTextEng.gameObject.SetActive(false);
            contentTextKor.gameObject.SetActive(false);
            if (LocalizationManager.instance.locallanguage == SystemLanguage.Korean) contentText = contentTextKor;
            else contentText = contentTextEng;

            contentText.gameObject.SetActive(true);

            Vector3 pos = contentText.transform.localPosition;
            pos.y = 0;
            contentText.transform.localPosition = pos;
            returnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        void Update()
        {
            contentText.transform.localPosition += new Vector3(0.0f, 1, 0.0f);
        }
    }
}