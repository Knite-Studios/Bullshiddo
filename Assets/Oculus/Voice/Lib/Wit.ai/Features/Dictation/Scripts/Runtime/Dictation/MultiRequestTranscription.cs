/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using Facebook.WitAi.Events;
using UnityEngine;

namespace Facebook.WitAi.Dictation
{
    public class MultiRequestTranscription : MonoBehaviour
    {
        [SerializeField] private WitDictation witDictation;
        [SerializeField] private int linesBetweenActivations = 2;
        [Multiline]
        [SerializeField] private string activationSeparator = String.Empty;

        [Header("Events")]
        [SerializeField] private WitTranscriptionEvent onTranscriptionUpdated = new
            WitTranscriptionEvent();

        private StringBuilder _text;
        private string _activeText;
        private bool _newSection;

        private StringBuilder _separator;

        private void Awake()
        {
            if (!witDictation) witDictation = FindObjectOfType<WitDictation>();

            _text = new StringBuilder();
            _separator = new StringBuilder();
            for (int i = 0; i < linesBetweenActivations; i++)
            {
                _separator.AppendLine();
            }

            if (!string.IsNullOrEmpty(activationSeparator))
            {
                _separator.Append(activationSeparator);
            }
        }

        private void OnEnable()
        {
            witDictation.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);
            witDictation.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
            witDictation.VoiceEvents.OnAborting.AddListener(OnCancelled);
        }

        private void OnDisable()
        {
            _activeText = string.Empty;
            witDictation.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
            witDictation.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
            witDictation.VoiceEvents.OnAborting.RemoveListener(OnCancelled);
        }

        private void OnCancelled()
        {
            _activeText = string.Empty;
            OnTranscriptionUpdated();
        }

        private void OnFullTranscription(string text)
        {
            _activeText = string.Empty;

            if (_text.Length > 0)
            {
                _text.Append(_separator);
            }

            _text.Append(text);

            OnTranscriptionUpdated();
        }

        private void OnPartialTranscription(string text)
        {
            _activeText = text;
            OnTranscriptionUpdated();
        }

        public void Clear()
        {
            _text.Clear();
            onTranscriptionUpdated.Invoke(string.Empty);
        }

        private void OnTranscriptionUpdated()
        {
            var transcription = new StringBuilder();
            transcription.Append(_text);
            if (!string.IsNullOrEmpty(_activeText))
            {
                if (transcription.Length > 0)
                {
                    transcription.Append(_separator);
                }

                if (!string.IsNullOrEmpty(_activeText))
                {
                    transcription.Append(_activeText);
                }
            }

            onTranscriptionUpdated.Invoke(transcription.ToString());
        }
    }
}
