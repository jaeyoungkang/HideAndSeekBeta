using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Status : MonoBehaviour
    {
        public Image bgImage;
        public Text gemText;        
        public Text timeText;
        public Image[] HPImages;

        private float warnningEffectValue = 0f;
        void Start()
        {
            bShow = true;
        }

        void SetHPImages()
        {
            foreach(Image img in HPImages)
            {
                img.enabled = false;
            }

            for (int i = 0; i < GameManager.instance.maxHp; i++)
            {
                if(HPImages.Length <= i)
                {
                    print("Error: Invalid HPImage index : " + i + ", " + HPImages.Length);
                    break;
                }

                HPImages[i].enabled = true;
                Color color = HPImages[i].color;
                color.a = 0.4f;
                HPImages[i].color = color;
            }

            for (int i=0; i<GameManager.instance.playerHp; i++)
            {
                if (HPImages.Length <= i)
                {
                    print("Error: Invalid HPImage index : " + i + ", " + HPImages.Length);
                    break;
                }

                Color color = HPImages[i].color;
                color.a = 1.0f;

                HPImages[i].color = color;
            }
        }
 
        void Update()
        {
            gemText.text = GameManager.instance.dungeonGem.ToString();

            SetHPImages();
            if(GameManager.instance.playerHp < 2 )
            {
                warnningEffectValue += (Time.deltaTime*0.5f);
                if (warnningEffectValue > 0.4f) warnningEffectValue = 0;
            }
            else
            {
                warnningEffectValue = 0;
            }

            Color color = bgImage.color;
            color.r = warnningEffectValue;
            bgImage.color = color;
            
                
            timeText.text = Mathf.Floor(GameManager.instance.timeLimit).ToString();
            if (GameManager.instance.timeLimit <= 10)
            {
                timeText.color = Color.red;
                ShowWarning();
            }
            else timeText.color = Color.white;            
        }

        bool bShow = false;
        void ShowWarning()
        {
            if(bShow == false)
            {
                Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LACK_TIME), 1f, Color.red);
            }            
        }
    }
}