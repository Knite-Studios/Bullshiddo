using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.Bullshiddo
{
    public class ButtonHandler : MonoBehaviour
    {
        public enum ButtonAction
        {
            PlayMainSong,
            StartTutorial,
            QuitGame
        }

        [Header("Action Settings")]
        [SerializeField]
        private ButtonAction buttonAction; // The action this button performs

        [SerializeField]
        private PlayableDirector director; // Reference to the PlayableDirector for this button

        [SerializeField]
        private MenuManager menuManager; // Reference to the MenuManager

        [SerializeField]
        private CountdownUI countdownUI; // Reference to the CountdownUI script

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collision detected with: {collision.gameObject.name}");

            // Perform the specified action
            if (menuManager != null)
            {
                menuManager.SetMenuElementsActive(false);

                if (buttonAction == ButtonAction.QuitGame)
                {
                    QuitGame();
                    return;
                }

                if (director != null)
                {
                    if (countdownUI != null)
                    {
                        countdownUI.OnCountdownFinished += () =>
                        {
                            PlayDirectorWithAudio();
                        };
                        menuManager.SetCountdownUIActive(true); // Activate the countdown GameObject
                        countdownUI.StartCountdown();
                    }
                    else
                    {
                        PlayDirectorWithAudio();
                    }

                    // If this button starts the main song, enable the score and combo UI
                    if (buttonAction == ButtonAction.PlayMainSong)
                    {
                        menuManager.SetScoreUIActive(true);
                        menuManager.SetComboUIActive(true);
                    }
                }
            }
        }

        private void PlayDirectorWithAudio()
        {
            Debug.Log("Playing PlayableDirector with audio");
            director.Play();
            director.stopped += OnDirectorStopped;

            // Ensure AudioSource is active and playing
            foreach (var output in director.playableAsset.outputs)
            {
                if (output.outputTargetType == typeof(AudioSource))
                {
                    var audioSource = director.GetGenericBinding(output.sourceObject) as AudioSource;
                    if (audioSource != null)
                    {
                        Debug.Log($"Found AudioSource: {audioSource.name}, playing audio");
                        audioSource.Play();

                        // Ensure AudioSource is routed to the correct AudioMixerGroup
                        if (audioSource.outputAudioMixerGroup == null)
                        {
                            Debug.LogWarning("AudioSource is not assigned to an AudioMixerGroup");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("AudioSource not found or not assigned correctly");
                    }
                }
            }
        }

        private void OnDirectorStopped(PlayableDirector stoppedDirector)
        {
            // Enable the menu elements again
            if (menuManager != null)
            {
                menuManager.SetMenuElementsActive(true);
                menuManager.SetScoreUIActive(false); // Disable score UI if it was enabled
                menuManager.SetComboUIActive(false); // Disable combo UI if it was enabled
                menuManager.SetCountdownUIActive(false); // Disable countdown UI
            }

            stoppedDirector.stopped -= OnDirectorStopped;
        }

        private void QuitGame()
        {
            Debug.Log("Quit game triggered");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
