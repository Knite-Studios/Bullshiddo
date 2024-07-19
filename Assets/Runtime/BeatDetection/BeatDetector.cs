using System.Collections.Generic;
using DequeNet;
using UnityEngine;

namespace Runtime.BeatDetection
{
    public class BeatDetector : MonoBehaviour
    {
        [Header("Audio References")]
        public AudioSource audioSource;
        public AudioClip audioClip;

        public GameObject beatCube;
        public Transform[] bassSpawnPoints = new Transform[2];
        public Transform[] lowSpawnPoints = new Transform[2];
        public Transform[] midSpawnPoints = new Transform[2];
        public Transform[] highSpawnPoints = new Transform[2];

        #region Frequency Ranges

        [Header("Bass Frequency Range")]
        [Tooltip("The lower limit of the bass frequency range. " +
                 "It means that frequencies below this value are not considered part of the bass.")]
        public int bassLowerLimit = 70;
        [Tooltip("The upper limit of the bass frequency range. " +
                 "It means that frequencies above this value are not considered part of the bass.")]
        public int bassUpperLimit = 250;

        [Header("low Frequency Range")]
        [Tooltip("The lower limit of the low frequency range. " +
                 "It means that frequencies below this value are not considered part of the low range.")]
        public int lowLowerLimit = 251;
        [Tooltip("The upper limit of the low frequency range. " +
                 "It means that frequencies above this value are not considered part of the low range.")]
        public int lowUpperLimit = 600;

        [Header("Mid Frequency Range")]
        [Tooltip("The lower limit of the mid frequency range. " +
                 "It means that frequencies below this value are not considered part of the mid range.")]
        public int midLowerLimit = 601;
        [Tooltip("The upper limit of the mid frequency range. " +
                 "It means that frequencies above this value are not considered part of the mid range.")]
        public int midUpperLimit = 2000;

        [Header("High Frequency Range")]
        [Tooltip("The lower limit of the high frequency range. " +
                 "It means that frequencies below this value are not considered part of the high range.")]
        public int highLowerLimit = 2001;
        [Tooltip("The upper limit of the high frequency range. " +
                 "It means that frequencies above this value are not considered part of the high range.")]
        public int highUpperLimit = 8000;

        #endregion

        private readonly List<int> _bandLimits = new();
        private Deque<List<float>> _beatDetector = new();
        private bool _bassDetected, _lowDetected, _midDetected, _highDetected;
        private float[] _freqAvgSpectrum = new float[4];
        private float[] _freqSpectrum = new float[1024];

        private int _maxSize;

        private void Awake()
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            var bandSize = audioSource.clip.frequency / 1024;
            _maxSize = audioSource.clip.frequency / 1024;

            _bandLimits.Clear();
            _bandLimits.Add(bassLowerLimit / bandSize);
            _bandLimits.Add(bassUpperLimit / bandSize);
            _bandLimits.Add(lowLowerLimit / bandSize);
            _bandLimits.Add(lowUpperLimit / bandSize);
            _bandLimits.Add(midLowerLimit / bandSize);
            _bandLimits.Add(midUpperLimit / bandSize);
            _bandLimits.Add(highLowerLimit / bandSize);
            _bandLimits.Add(highUpperLimit / bandSize);

            _bandLimits.TrimExcess();
            _beatDetector.Clear();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleAudio();

            // Analyze the audio to detect beats.
            GetBeat(ref _freqSpectrum, ref _freqAvgSpectrum,
                ref _bassDetected, ref _lowDetected,
                ref _midDetected, ref _highDetected);

            HandleDetection();
        }

        private void HandleDetection()
        {
            if (_bassDetected)
            {
                Debug.Log("Bass detected!");
                _bassDetected = false;
            }

            if (_lowDetected)
            {
                Debug.Log("Low detected!");
                _lowDetected = false;
            }

            if (_midDetected)
            {
                Debug.Log("Mid detected!");
                _midDetected = false;
            }

            if (_highDetected)
            {
                Debug.Log("High detected!");
                _highDetected = false;
            }
        }

        /// <summary>
        /// A function to set the booleans for beats by comparing current audio sample with statistical values of previous
        /// one's
        /// </summary>
        /// <param name="spectrum">reference to the array containing current samples and amplitudes</param>
        /// <param name="avgSpectrum">reference to the array containing average values for the sample amplitudes</param>
        /// <param name="isBass">bool to check if current value is higher than average for bass frequencies</param>
        /// <param name="isLow">bool to check if current value is higher than average for low frequencies</param>
        private void GetBeat(ref float[] spectrum, ref float[] avgSpectrum, ref bool isBass, ref bool isLow, ref bool isMid, ref bool isHigh)
        {
            var numBands = 4;
            var numChannels = audioSource.clip.channels;

            for (var numBand = 0; numBand < numBands; ++numBand)
            {
                for (var idxFFT = _bandLimits[numBand * 2]; idxFFT < _bandLimits[numBand * 2 + 1]; ++idxFFT)
                {
                    for (var channel = 0; channel < numChannels; ++channel)
                    {
                        var tempSample = new float[1024];
                        audioSource.GetSpectrumData(tempSample, channel, FFTWindow.Rectangular);
                        spectrum[numBand] += tempSample[idxFFT];
                    }
                }

                spectrum[numBand] /= (_bandLimits[numBand * 2 + 1] - _bandLimits[numBand * 2]);
            }

            if (_beatDetector.Count > 0)
            {
                FillAvgSpectrum(ref avgSpectrum, numBands, ref _beatDetector);

                var varianceSpectrum = new float[numBands];
                FillVarianceSpectrum(ref varianceSpectrum, numBands, ref _beatDetector, ref avgSpectrum);

                isBass = spectrum[0] > BeatThreshold(varianceSpectrum[0]) * avgSpectrum[0];
                isLow = spectrum[1] > BeatThreshold(varianceSpectrum[1]) * avgSpectrum[1];
                isMid = spectrum[2] > BeatThreshold(varianceSpectrum[2]) * avgSpectrum[2];
                isHigh = spectrum[3] > BeatThreshold(varianceSpectrum[3]) * avgSpectrum[3];
            }

            var fftResult = new List<float>(numBands);
            for (var index = 0; index < numBands; ++index)
                fftResult.Add(spectrum[index]);

            if (_beatDetector.Count >= _maxSize)
                _beatDetector.PopLeft();

            _beatDetector.PushRight(fftResult);
        }


        /// <summary>
        /// Function to add average values to the array
        /// </summary>
        private void FillAvgSpectrum(ref float[] avgSpectrum, int numBands, ref Deque<List<float>> fftHistory)
        {
            foreach (var iterator in fftHistory)
            {
                var fftResult = iterator;

                for (var index = 0; index < fftResult.Count; ++index)
                    avgSpectrum[index] += fftResult[index];
            }

            for (var index = 0; index < numBands; ++index)
                avgSpectrum[index] /= fftHistory.Count;
        }


        /// <summary>
        /// Function to add variance values to the array
        /// </summary>
        private void FillVarianceSpectrum(ref float[] varianceSpectrum, int numBands, ref Deque<List<float>> fftHistory, ref float[] avgSpectrum)
        {
            foreach (var iterator in fftHistory)
            {
                var fftResult = iterator;

                for (var index = 0; index < fftResult.Count; ++index)
                    varianceSpectrum[index] += (fftResult[index] - avgSpectrum[index]) * (fftResult[index] - avgSpectrum[index]);
            }

            for (var index = 0; index < numBands; ++index)
                varianceSpectrum[index] /= fftHistory.Count;
        }

        /// <summary>
        /// Function to get the threshold value for the sample
        /// </summary>
        /// <param name="variance">variance for the sample</param>
        /// <returns>float threshold</returns>
        private float BeatThreshold(float variance)
            => -15f * variance + 1.55f;

        private void ToggleAudio()
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.Play();
        }

        public void ChangeClip(AudioClip clip)
        {
            if (audioSource.isPlaying) audioSource.Pause();

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
