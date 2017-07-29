using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class EscapePage : MonoBehaviour
    {
        public Button YesBtn, NoBtn;
        public Text content;
                
        void Start()
        {
            content.text = LocalizationManager.instance.GetLocalUIString(UI_STRING.QUIT);

            YesBtn.GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.YES_BTN);
            NoBtn.GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.NO_BTN);
            YesBtn.onClick.AddListener(GameManager.instance.QuitGame);
            NoBtn.onClick.AddListener(close);
        }

        void close()
        {
            GameManager.instance.BacktoPreState();
        }        
    }
}