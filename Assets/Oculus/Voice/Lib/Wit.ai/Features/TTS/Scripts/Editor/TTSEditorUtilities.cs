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
using Facebook.WitAi.Data.Configuration;
using Facebook.WitAi.TTS.Data;
using Facebook.WitAi.TTS.Editor.Voices;
using Facebook.WitAi.TTS.Integrations;
using Facebook.WitAi.TTS.Utilities;
using Facebook.WitAi.Utilities;
using UnityEditor;
using UnityEngine;

namespace Facebook.WitAi.TTS.Editor
{
    public static class TTSEditorUtilities
    {
        // Default TTS Setup
        public static Transform CreateDefaultSetup()
        {
            // Generate parent
            Transform parent = GenerateGameObject("TTS").transform;

            // Add TTS Service
            TTSService service = CreateService(parent);

            // Add TTS Speaker
            CreateSpeaker(parent, service);

            // Select parent
            Selection.activeObject = parent.gameObject;
            return parent;
        }

        // Default TTS Service
        public static TTSService CreateService(Transform parent = null)
        {
            // Get parent
            if (parent == null)
            {
                Transform selected = Selection.activeTransform;
                if (selected != null && selected.gameObject.scene.rootCount > 0)
                {
                    parent = Selection.activeTransform;
                }
            }
            // Ignore if found
            TTSService instance = GameObject.FindObjectOfType<TTSService>();
            if (instance != null)
            {
                // Log
                Debug.LogWarning($"TTS Service - A TTSService is already in scene\nGameObject: {instance.gameObject.name}");

                // Move into parent
                if (parent != null)
                {
                    instance.transform.SetParent(parent, true);
                }
            }

            // Generate TTSWit
            else
            {
                instance = CreateWitService(parent);
            }

            // Select & return instance
            Selection.activeObject = instance.gameObject;
            return instance;
        }

        // Default TTS Service
        private static TTSWit CreateWitService(Transform parent = null)
        {
            // Generate new TTSWit & add caches
            TTSWit ttsWit = GenerateGameObject("TTSWitService", parent).AddComponent<TTSWit>();
            ttsWit.gameObject.AddComponent<TTSRuntimeCache>();
            ttsWit.gameObject.AddComponent<TTSDiskCache>();
            Debug.Log($"TTS Service - Instantiated Service {ttsWit.gameObject.name}");

            // Refresh configuration
            WitConfiguration configuration = SetupConfiguration(ttsWit);
            if (configuration != null)
            {
                RefreshAvailableVoices(ttsWit);
            }

            // Log
            return ttsWit;
        }

        // Wit configuration
        private static WitConfiguration SetupConfiguration(TTSService instance)
        {
            // Ignore non-tts wit
            if (instance.GetType() != typeof(TTSWit))
            {
                return null;
            }
            // Already setup
            TTSWit ttsWit = instance as TTSWit;
            if (ttsWit.RequestSettings.configuration != null)
            {
                return ttsWit.RequestSettings.configuration;
            }

            // Refresh configuration list
            if (WitConfigurationUtility.WitConfigs == null)
            {
                WitConfigurationUtility.ReloadConfigurationData();
            }

            // Assign first wit configuration found
            if (WitConfigurationUtility.WitConfigs != null && WitConfigurationUtility.WitConfigs.Length > 0)
            {
                ttsWit.RequestSettings.configuration = WitConfigurationUtility.WitConfigs[0];
                Debug.Log($"TTS Service - Assigned Wit Configuration {ttsWit.RequestSettings.configuration.name}");
            }

            // Warning
            if (ttsWit.RequestSettings.configuration == null)
            {
                Debug.LogWarning($"TTS Service - Please create and assign a WitConfiguration to TTSWit");
            }

            // Return configuration
            return ttsWit.RequestSettings.configuration;
        }

        // Refresh available voices
        private static void RefreshAvailableVoices(TTSWit ttsWit)
        {
            // Fail without configuration
            if (ttsWit == null)
            {
                Debug.LogWarning($"TTS Service - Cannot refresh voices without TTS Wit Service");
                return;
            }

            // Update voices
            TTSWitVoiceUtility.UpdateVoices(ttsWit.RequestSettings.configuration, (refreshSuccess) =>
            {
                // Failed, get placeholder
                if (!refreshSuccess)
                {
                    Debug.LogWarning($"TTS Service - Cannot refresh voices\nPlease try again manually");
                    if (ttsWit.PresetVoiceSettings == null || ttsWit.PresetVoiceSettings.Length == 0)
                    {
                        TTSWitVoiceData voice = new TTSWitVoiceData()
                        {
                            name = TTSWitVoiceSettings.DEFAULT_VOICE,
                        };
                        TTSWitVoiceSettings placeholder = GetDefaultVoiceSetting(voice);
                        ttsWit.SetVoiceSettings(new TTSWitVoiceSettings[] { placeholder });
                    }
                }
                // Reset list of voices
                else
                {
                    TTSWitVoiceData[] voices = TTSWitVoiceUtility.Voices;
                    TTSWitVoiceSettings[] newSettings = new TTSWitVoiceSettings[voices.Length];
                    for (int i = 0; i < voices.Length; i++)
                    {
                        newSettings[i] = GetDefaultVoiceSetting(voices[i]);
                    }
                    ttsWit.SetVoiceSettings(newSettings);
                    Debug.Log($"TTS Service - Successfully applied {voices.Length} voices to {ttsWit.gameObject.name}");
                }

                // Refresh
                RefreshEmptySpeakers(ttsWit);
            });
        }

        // Set all blank IDs to default voice id
        private static void RefreshEmptySpeakers(TTSService service)
        {
            string defaultVoiceID = service.VoiceProvider.VoiceDefaultSettings.settingsID;
            foreach (var speaker in GameObject.FindObjectsOfType<TTSSpeaker>())
            {
                if (string.IsNullOrEmpty(speaker.presetVoiceID) || string.Equals(speaker.presetVoiceID, TTSVoiceSettings.DEFAULT_ID))
                {
                    speaker.presetVoiceID = defaultVoiceID;
                }
            }
        }

        // Get default voice settings
        private static TTSWitVoiceSettings GetDefaultVoiceSetting(TTSWitVoiceData voiceData)
        {
            TTSWitVoiceSettings result = new TTSWitVoiceSettings()
            {
                settingsID = voiceData.name.ToUpper(),
                voice = voiceData.name
            };
            // Use first style provided
            if (voiceData.styles != null && voiceData.styles.Length > 0)
            {
                result.style = voiceData.styles[0];
            }
            return result;
        }

        // Default TTS Speaker
        public static TTSSpeaker CreateSpeaker(Transform parent = null, TTSService service = null)
        {
            // Get parent
            if (parent == null)
            {
                Transform selected = Selection.activeTransform;
                if (selected != null && selected.gameObject.scene.rootCount > 0)
                {
                    parent = Selection.activeTransform;
                }
            }
            // Generate service if possible
            if (service == null)
            {
                service = CreateService(parent);
            }

            // TTS Speaker
            TTSSpeaker speaker = GenerateGameObject("TTSSpeaker", parent).AddComponent<TTSSpeaker>();
            speaker.presetVoiceID = string.Empty;

            // Audio Source
            AudioSource audio = GenerateGameObject("TTSSpeakerAudio", speaker.transform).AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            audio.spatialBlend = 0f; // Default to 2D
            speaker.AudioSource = audio;

            // Return speaker
            Debug.Log($"TTS Service - Instantiated Speaker {speaker.gameObject.name}");
            Selection.activeObject = speaker.gameObject;
            return speaker;
        }

        // Generate with specified name
        private static GameObject GenerateGameObject(string name, Transform parent = null)
        {
            Transform result = new GameObject(name).transform;
            result.SetParent(parent);
            result.localPosition = Vector3.zero;
            result.localRotation = Quaternion.identity;
            result.localScale = Vector3.one;
            return result.gameObject;
        }
    }
}
