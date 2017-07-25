using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DeveloperComments : MonoBehaviour
    {
        public Text contentText;
        public Button returnBtn;
        public float speed = 20f;

        void Start()
        {
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