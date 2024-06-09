﻿/*
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
using Facebook.WitAi.Lib;
using UnityEngine;

namespace Facebook.WitAi.Configuration
{
    [Serializable]
    public abstract class WitConfigurationData
    {
        [SerializeField] public WitConfiguration witConfiguration;

        #if UNITY_EDITOR
        public void UpdateData(Action onUpdateComplete = null)
        {
            if (!witConfiguration)
            {
                onUpdateComplete?.Invoke();
                return;
            }

            var request = OnCreateRequest();
            request.onResponse += (r) => OnUpdateData(r, onUpdateComplete);
            request.Request();
        }

        protected abstract WitRequest OnCreateRequest();

        private void OnUpdateData(WitRequest request, Action onUpdateComplete)
        {
            if (request.StatusCode == 200)
            {
                UpdateData(request.ResponseData);
            }
            else
            {
                Debug.LogError(request.StatusDescription);
            }

            onUpdateComplete?.Invoke();
        }

        public abstract void UpdateData(WitResponseNode data);
        #endif
    }
}
