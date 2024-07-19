using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.Bullshiddo
{
    public class MenuManager : MonoBehaviour
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
        public PlayableDirector mainSongDirector; // Reference to the main song PlayableDirector
        [SerializeField]
        public PlayableDirector tutorialDirector; // Reference to the tutorial PlayableDirector

        [Header("Other UI Elements")]
        [SerializeField]
        private GameObject logoAndMetaSlot; // Reference to the combined logo and meta slot GameObject
        [SerializeField]
        private GameObject scoreUI; // Reference to the score UI canvas
        [SerializeField]
        private GameObject countdownUI; // Reference to the countdown GameObject

        private void Start()
        {
            if (countdownUI != null)
            {
                countdownUI.SetActive(false); // Ensure countdown is turned off at start
            }

            if (scoreUI != null)
            {
                scoreUI.SetActive(false); // Ensure score UI is turned off at start
            }
        }

        public void SetMenuElementsActive(bool isActive)
        {
            if (playButton != null) playButton.SetActive(isActive);
            if (tutorialButton != null) tutorialButton.SetActive(isActive);
            if (quitButton != null) quitButton.SetActive(isActive);
            if (logoAndMetaSlot != null) logoAndMetaSlot.SetActive(isActive);
        }

        public void SetScoreUIActive(bool isActive)
        {
            if (scoreUI != null) scoreUI.SetActive(isActive);
        }

        public void SetCountdownUIActive(bool isActive)
        {
            if (countdownUI != null) countdownUI.SetActive(isActive);
        }
    }
}
