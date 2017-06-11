using UnityEngine;
using System.Collections;

namespace HideAndSeek
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;
		public GameObject soundManager;
        public GameObject pageManager;
        public GameObject itemManager;
        
        void Awake ()
		{
            if (PageManager.instance == null)
                Instantiate(pageManager);

            if (GameManager.instance == null)
                Instantiate(gameManager);

            if (ItemManager.instance == null)
                Instantiate(itemManager);		

			if (SoundManager.instance == null)
				Instantiate(soundManager);
        }
	}
}