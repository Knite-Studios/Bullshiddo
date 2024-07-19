using System.Collections;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Handles the UI that appears before a track starts.
    /// </summary>
    public class CountdownUI : MonoBehaviour
    {
        static readonly WaitForSeconds _oneSecond = new WaitForSeconds(1);

        [SerializeField]
        private GameObject _countdownParent;

        [SerializeField]
        private GameObject _countdownIcon;

        [SerializeField]
        private TextMeshProUGUI _countdownText;

        [SerializeField]
        private int _countdown;

        private bool _countingDown;

        public event System.Action OnCountdownFinished;

        public void StartCountdown()
        {
            if (!_countingDown)
            {
                Debug.Log("Starting Countdown...");
                StartCoroutine(CountdownRoutine());
            }
        }

        private IEnumerator CountdownRoutine()
        {
            _countingDown = true;
            _countdownParent.SetActive(true);
            for (int i = _countdown; i > 0; i--)
            {
                _countdownText.SetText(i.ToString());
                Debug.Log($"Countdown: {i}");
                yield return _oneSecond;
            }
            _countdownText.SetText("GO");
            Debug.Log("Countdown finished: GO");
            yield return _oneSecond;
            _countdownParent.SetActive(false);
            _countingDown = false;
            OnCountdownFinished?.Invoke();
        }
    }
}