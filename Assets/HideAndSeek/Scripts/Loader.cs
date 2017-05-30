using UnityEngine;
using System.Collections;

namespace HideAndSeek
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;			//GameManager prefab to instantiate.
		public GameObject soundManager;         //SoundManager prefab to instantiate.
        public GameObject pageManager;
		
		void Awake ()
		{
            if (PageManager.instance == null)
                Instantiate(pageManager);

            if (GameManager.instance == null)
				Instantiate(gameManager);
			

			if (SoundManager.instance == null)
				Instantiate(soundManager);
        }
	}
}