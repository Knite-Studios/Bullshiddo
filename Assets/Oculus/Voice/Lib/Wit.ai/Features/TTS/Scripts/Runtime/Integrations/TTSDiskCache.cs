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

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Facebook.WitAi.TTS.Data;
using Facebook.WitAi.TTS.Events;
using Facebook.WitAi.TTS.Interfaces;
using Facebook.WitAi.TTS.Utilities;
using Facebook.WitAi.Utilities;

namespace Facebook.WitAi.TTS.Integrations
{
    public class TTSDiskCache : MonoBehaviour, ITTSDiskCacheHandler
    {
        [Header("Disk Cache Settings")]
        /// <summary>
        /// The relative path from the DiskCacheLocation in TTSDiskCacheSettings
        /// </summary>
        [SerializeField] private string _diskPath = "TTS/";
        public string DiskPath => _diskPath;

        /// <summary>
        /// The cache default settings
        /// </summary>
        [SerializeField] private TTSDiskCacheSettings _defaultSettings = new TTSDiskCacheSettings();
        public TTSDiskCacheSettings DiskCacheDefaultSettings => _defaultSettings;

        /// <summary>
        /// The cache streaming events
        /// </summary>
        [SerializeField] private TTSStreamEvents _events = new TTSStreamEvents();
        public TTSStreamEvents DiskStreamEvents
        {
            get => _events;
            set { _events = value; }
        }

        // All currently performing stream requests
        private Dictionary<string, VoiceUnityRequest> _streamRequests = new Dictionary<string, VoiceUnityRequest>();

        /// <summary>
        /// Builds full cache path
        /// </summary>
        /// <param name="clipData"></param>
        /// <returns></returns>
        public string GetDiskCachePath(TTSClipData clipData)
        {
            // Disabled
            if (!ShouldCacheToDisk(clipData))
            {
                return string.Empty;
            }

            // Get directory path
            TTSDiskCacheLocation location = clipData.diskCacheSettings.DiskCacheLocation;
            string directory = string.Empty;
            switch (location)
            {
                case TTSDiskCacheLocation.Persistent:
                    directory = Application.persistentDataPath;
                    break;
                case TTSDiskCacheLocation.Temporary:
                    directory = Application.temporaryCachePath;
                    break;
                case TTSDiskCacheLocation.Preload:
                    directory = Application.streamingAssetsPath;
                    break;
            }
            if (string.IsNullOrEmpty(directory))
            {
                return string.Empty;
            }

            // Add tts cache path & clean
            directory = Path.Combine(directory, DiskPath);

            // Generate tts directory if possible
            if (location != TTSDiskCacheLocation.Preload || !Application.isPlaying)
            {
                if (!IOUtility.CreateDirectory(directory, true))
                {
                    Debug.LogError($"TTS Cache - Failed to create tts directory\nPath: {directory}\nLocation: {location}");
                    return string.Empty;
                }
            }

            // Return clip path
            return Path.Combine(directory, clipData.clipID + "." + clipData.audioType.ToString().ToLower());
        }

        /// <summary>
        /// Determine if should cache to disk or not
        /// </summary>
        /// <param name="clipData">All clip data</param>
        /// <returns>Returns true if should cache to disk</returns>
        public bool ShouldCacheToDisk(TTSClipData clipData)
        {
            return clipData != null && clipData.diskCacheSettings.DiskCacheLocation != TTSDiskCacheLocation.Stream && !string.IsNullOrEmpty(clipData.clipID);
        }

        /// <summary>
        /// Determines if file is cached on disk
        /// </summary>
        /// <param name="clipData">Request data</param>
        /// <returns>True if file is on disk</returns>
        public bool IsCachedToDisk(TTSClipData clipData)
        {
            // Get path
            string cachePath = GetDiskCachePath(clipData);
            if (string.IsNullOrEmpty(cachePath))
            {
                return false;
            }

            #if UNITY_ANDROID
            // Cannot use File.Exists with Streaming Assets on Android
            // This will try and fail to load if the file is missing and then stream it
            if (clipData.diskCacheSettings.DiskCacheLocation == TTSDiskCacheLocation.Preload && Application.isPlaying)
            {
                return true;
            }
            #endif

            // Check if file exists
            return File.Exists(cachePath);
        }

        /// <summary>
        /// Performs async load request
        /// </summary>
        public void StreamFromDiskCache(TTSClipData clipData)
        {
            // Invoke begin
            DiskStreamEvents?.OnStreamBegin?.Invoke(clipData);

            // Get file path
            string filePath = GetDiskCachePath(clipData);

            // Ensures possible
            if (!IsCachedToDisk(clipData))
            {
                string e = $"Clip not found\nPath: {filePath}";
                OnStreamComplete(clipData, e);
                return;
            }

            // Load clip async
            _streamRequests[clipData.clipID] = VoiceUnityRequest.RequestAudioClip(filePath, (path, progress) => clipData.loadProgress = progress, (path, clip, error) =>
            {
                // Apply clip
                clipData.clip = clip;
                // Call on complete
                OnStreamComplete(clipData, error);
            });
        }
        /// <summary>
        /// Cancels unity request
        /// </summary>
        public void CancelDiskCacheStream(TTSClipData clipData)
        {
            // Ignore if not currently streaming
            if (!_streamRequests.ContainsKey(clipData.clipID))
            {
                return;
            }

            // Get request
            VoiceUnityRequest request = _streamRequests[clipData.clipID];
            _streamRequests.Remove(clipData.clipID);

            // Destroy immediately
            if (request != null)
            {
                request.Unload();
            }

            // Call cancel
            DiskStreamEvents?.OnStreamCancel?.Invoke(clipData);
        }
        // On stream completion
        protected virtual void OnStreamComplete(TTSClipData clipData, string error)
        {
            // Ignore if not currently streaming
            if (!_streamRequests.ContainsKey(clipData.clipID))
            {
                return;
            }

            // Remove from list
            _streamRequests.Remove(clipData.clipID);

            // Error
            if (!string.IsNullOrEmpty(error))
            {
                DiskStreamEvents?.OnStreamError?.Invoke(clipData, error);
            }
            // Success
            else
            {
                DiskStreamEvents?.OnStreamReady?.Invoke(clipData);
            }
        }
    }
}
