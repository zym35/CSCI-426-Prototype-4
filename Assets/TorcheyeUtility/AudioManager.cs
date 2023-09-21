namespace TorcheyeUtility
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        public enum SoundEffect
        {
            // Add name of the sound here
            Jump,
            ShootTongue,
            CutTongue,
            RetractTongue,
            Spikes,
            Water
        }

        public enum Music
        {
            // Add name of the music here
        }
        
        public List<AudioClip> soundEffectClips, musicClips;
        public AudioSource soundEffectSource, musicSource;
        [Tooltip("whether the first music clip auto plays and loops at the start of game")]
        public bool startPlayingMusicLoop = true;
        public float startPlayingMusicLoopVolume = 1;

        // Singleton and DontDestroy instance
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (FindObjectsOfType<AudioManager>().Length > 1)
            {
                foreach (var obj in FindObjectsOfType<AudioManager>())
                {
                    if (obj == this)
                        Destroy(gameObject);
                }
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (startPlayingMusicLoop)
                PlayMusic(0, startPlayingMusicLoopVolume);
        }

        /// <summary>
        /// Play a one-time sound effect
        /// </summary>
        /// <param name="effect">Select from AudioManager.SoundEffect</param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public void PlaySoundEffect(SoundEffect effect, float volume = 1)
        {
            var id = (int)effect;
            if (id >= soundEffectClips.Count)
            {
                Debug.LogWarning("AudioManager: no corresponding sound effect clip!");
                return;
            }
            soundEffectSource.PlayOneShot(soundEffectClips[id], volume);
        }
        
        /// <summary>
        /// Play a music
        /// </summary>
        /// <param name="music">Select from AudioManager.Music</param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public void PlayMusic(Music music, float volume = 1, bool loop = true)
        {
            var id = (int)music;
            if (id >= musicClips.Count)
            {
                Debug.LogWarning("AudioManager: no corresponding music clip!");
                return;
            }

            musicSource.clip = musicClips[id];
            musicSource.volume = volume;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }
        
        public void UnPauseMusic()
        {
            musicSource.UnPause();
        }
        
        public void StopMusic()
        {
            musicSource.Stop();
        }
    }
}