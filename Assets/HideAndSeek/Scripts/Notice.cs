using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Notice : MonoBehaviour
    {
        public static Notice instance = null;

        GameObject noticeImage;
        Text contentText;
        
        bool isShowing = false;

        string text;
        float time = 2f;
        Color color;


        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void InitUI()
        {
            noticeImage = GameObject.Find("NoticeImage");
            contentText = GameObject.Find("NoticeContentText").GetComponent<Text>();            
        }

        public void Show(string content, float _time, Color textColor)
        {
            isShowing = true;
            color = textColor;
            text = content;
            time = _time;
            SetImage();
        }

        void SetImage()
        {
            contentText.text = text;
            contentText.color = color;
        }

        void Update()
        {
            if(noticeImage == null) return;
            noticeImage.SetActive(isShowing);

            if (isShowing)
            {
                SetImage();
                time -= Time.deltaTime;

                if (time <= 0)
                {
                    isShowing = false;
                    time = 0;                    
                }
            }
        }
    }
}