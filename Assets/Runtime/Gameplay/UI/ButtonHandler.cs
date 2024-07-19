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

        [SerializeField]
        private GameObject menuGameObject; // Reference to the Menu GameObject

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
                    director.Play();
                    director.stopped += OnDirectorStopped;

                    if (countdownUI != null)
                    {
                        menuManager.SetCountdownUIActive(true); // Activate the countdown GameObject
                        countdownUI.StartCountdown();
                    }

                    // If this button starts the main song, enable the score UI
                    if (buttonAction == ButtonAction.PlayMainSong)
                    {
                        menuManager.SetScoreUIActive(true);
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
                menuManager.SetCountdownUIActive(false); // Disable countdown UI
            }

            // Enable the menu GameObject
            if (menuGameObject != null)
            {
                menuGameObject.SetActive(true);
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
