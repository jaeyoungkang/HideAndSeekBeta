using UnityEngine;
using System.Collections;

namespace HideAndSeek
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource[] efxSources;
        public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
		public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
		
		
		void Awake ()
		{
			//Check if there is already an instance of SoundManager
			if (instance == null)
				//if not, set it to this.
				instance = this;
			//If instance already exists:
			else if (instance != this)
				//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
				Destroy (gameObject);
			
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad (gameObject);
		}
		
		
		public void PlaySingle(AudioClip clip)
		{
			if (clip == null)
				return;
			
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            foreach(AudioSource efxSource in efxSources)
            {
                if (efxSource.isPlaying) continue;

                efxSource.pitch = randomPitch;
                efxSource.clip = clip;
                efxSource.Play();
                break;
            }
		}

        public void RandomizeSfx(params AudioClip[] clips)
		{
			if (clips.Length == 0)
				return;
			
			int randomIndex = Random.Range(0, clips.Length);
            PlaySingle(clips[randomIndex]);
		}
    }
}
