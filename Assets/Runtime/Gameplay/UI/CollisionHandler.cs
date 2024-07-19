using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Handles collisions between button GameObjects to start gameplay, manage UI visibility, and play directors.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField]
        private GameObject playButton; // Reference to the Play button
        [SerializeField]
        private GameObject tutorialButton; // Reference to the Tutorial button
        [SerializeField]
        private GameObject quitButton; // Reference to the Quit button

        [Header("Directors")]
        [SerializeField]
        private PlayableDirector tutorialDirector; // Reference to the tutorial PlayableDirector
        [SerializeField]
        private PlayableDirector mainSongDirector; // Reference to the main song PlayableDirector

        [Header("Countdown UI")]
        [SerializeField]
        private CountdownUI countdownUI; // Reference to the CountdownUI

        [Header("Other UI Elements")]
        [SerializeField]
        private GameObject logoAndMetaSlot; // Reference to the combined logo and meta slot GameObject
        [SerializeField]
        private GameObject scoreUI; // Reference to the score UI canvas

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collision detected with: {collision.gameObject.name}");

            // Check which button was pressed
            if (collision.gameObject == playButton)
            {
                Debug.Log("Play button pressed");
                PlayMainSong();
            }
            else if (collision.gameObject == tutorialButton)
            {
                Debug.Log("Tutorial button pressed");
                StartTutorial();
            }
            else if (collision.gameObject == quitButton)
            {
                Debug.Log("Quit button pressed");
                QuitGame();
            }
        }

        private void StartTutorial()
        {
            // Hide all buttons and logo/meta slot
            SetMenuElementsActive(false);

            // Start countdown and play tutorial director
            StartCountdownAndPlay(tutorialDirector);
        }

        private void PlayMainSong()
        {
            // Hide all buttons and logo/meta slot
            SetMenuElementsActive(false);

            // Start countdown and play main song director
            StartCountdownAndPlay(mainSongDirector);

            // Enable score UI
            if (scoreUI != null) scoreUI.SetActive(true);
        }

        private void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void StartCountdownAndPlay(PlayableDirector director)
        {
            if (countdownUI != null)
            {
                countdownUI.OnCountdownFinished += () =>
                {
                    director.Play();
                    director.stopped += OnDirectorStopped;
                };
                countdownUI.StartCountdown();
            }
            else
            {
                director.Play();
                director.stopped += OnDirectorStopped;
            }
        }

        private void OnDirectorStopped(PlayableDirector director)
        {
            // Enable logo/meta slot
            if (logoAndMetaSlot != null) logoAndMetaSlot.SetActive(true);

            // Re-enable all buttons
            SetMenuElementsActive(true);

            // Disable score UI if it was enabled
            if (scoreUI != null) scoreUI.SetActive(false);

            director.stopped -= OnDirectorStopped;
        }

        private void SetMenuElementsActive(bool isActive)
        {
            if (playButton != null) playButton.SetActive(isActive);
            if (tutorialButton != null) tutorialButton.SetActive(isActive);
            if (quitButton != null) quitButton.SetActive(isActive);
            if (logoAndMetaSlot != null) logoAndMetaSlot.SetActive(isActive);
        }
    }
}
