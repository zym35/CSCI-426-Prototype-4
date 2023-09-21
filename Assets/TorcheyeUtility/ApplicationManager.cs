namespace TorcheyeUtility
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public class ApplicationManager : MonoBehaviour
    {
        [Header("Application Logic")]
        [Tooltip("Reload current scene")]
        [SerializeField] private KeyCode restartGame = KeyCode.R;
        [SerializeField] private KeyCode exitGame = KeyCode.Escape;
        
        [Header("Scene Control")]
        [Tooltip("Back to the first scene if exceeds scene count")]
        [SerializeField] private KeyCode nextScene = KeyCode.RightBracket;
        [Tooltip("Back to the last scene if less than 0")]
        [SerializeField] private KeyCode previousScene = KeyCode.LeftBracket;

        [Header("Audio Volume Control")] 
        [SerializeField] private KeyCode increaseVolume = KeyCode.Equals;
        [SerializeField] private KeyCode decreaseVolume = KeyCode.Minus;
        [SerializeField] private bool autoDetectAllAudioSource = true;
        [SerializeField] private AudioSource[] audioSources;
        [SerializeField] private float increaseMultiplier = 1.5f;
        [SerializeField] private float decreaseMultiplier = 0.75f;

        private void Awake()
        {
            if (autoDetectAllAudioSource)
                audioSources = FindObjectsOfType<AudioSource>();
        }
        
        private void Update()
        {
            int activeScene = SceneManager.GetActiveScene().buildIndex;
            int totalScene = SceneManager.sceneCountInBuildSettings;

            if (Input.GetKeyDown(restartGame))
                SceneManager.LoadScene(activeScene);
            if (Input.GetKeyDown(exitGame))
                Application.Quit();
            if (Input.GetKeyDown(nextScene))
                SceneManager.LoadScene((activeScene + 1) % totalScene);
            if (Input.GetKeyDown(previousScene))
                SceneManager.LoadScene((activeScene - 1) % totalScene);

            if (Input.GetKeyDown(increaseVolume))
                foreach (AudioSource audioSource in audioSources)
                    audioSource.volume *= increaseMultiplier;
            if (Input.GetKeyDown(decreaseVolume))
                foreach (AudioSource audioSource in audioSources)
                    audioSource.volume *= decreaseMultiplier;
        }
    }
}
