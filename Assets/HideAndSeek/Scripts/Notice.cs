using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Notice : MonoBehaviour
    {
        public Text contentText;
        private float time;

        public void Show(string content, float _time, Color textColor)
        {
            contentText.text = content;
            contentText.color = textColor;
            time = _time;
            gameObject.SetActive(true);
        }

        void Start()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            if(gameObject.activeSelf && time > Mathf.Epsilon)
            {
                time -= Time.deltaTime;

                if (time <= 0)
                {
                    time = 0;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}