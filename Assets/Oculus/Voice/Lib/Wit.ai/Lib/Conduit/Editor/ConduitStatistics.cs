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
using System.Collections.Generic;
using Meta.Wit.LitJson;
using UnityEngine;

namespace Meta.Conduit.Editor
{
    /// <summary>
    /// Aggregates and persists Conduit statistics.
    /// </summary>
    internal class ConduitStatistics
    {
        private const string ConduitSuccessfulGenerationsKey = "ConduitSuccessfulGenerations";
        private const string ConduitSignatureFrequencyKey = "ConduitSignatureFrequency";
        private const string ConduitIncompatibleSignatureFrequencyKey = "ConduitIncompatibleSignatureFrequency";
        private readonly IPersistenceLayer _persistenceLayer;
        
        public ConduitStatistics(IPersistenceLayer persistenceLayer)
        {
            _persistenceLayer = persistenceLayer;
            Load();
        }
        
        /// <summary>
        /// A randomly generated ID representing at telemetry report. 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Number of successful generations since last reset.
        /// </summary>
        public int SuccessfulGenerations { set; get; }

        /// <summary>
        /// Holds the frequency of method signatures.
        /// Key is signatures in the form: [ReturnTypeId]-[TypeId]:[FrequencyOfType],[TypeId]:[FrequencyOfType].
        /// Value is the number of times this signature was encountered in the last extraction process.
        /// </summary>
        public Dictionary<string, int> SignatureFrequency { get; private set; } = new Dictionary<string, int>();

        /// <summary>
        /// Similar to <see cref="SignatureFrequency"/> but for incompatible methods.
        /// </summary>
        public Dictionary<string, int> IncompatibleSignatureFrequency { get; private set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// Adds the supplied frequencies to the current collection.
        /// </summary>
        /// <param name="sourceFrequencies">The frequencies to add.</param>
        public void AddFrequencies(Dictionary<string, int> sourceFrequencies)
        {
            AddFrequencies(this.SignatureFrequency, sourceFrequencies);
        }
        
        /// <summary>
        /// Adds the supplied incompatible method frequencies to the current collection.
        /// </summary>
        /// <param name="sourceFrequencies">The frequencies to add.</param>
        public void AddIncompatibleFrequencies(Dictionary<string, int> sourceFrequencies)
        {
            AddFrequencies(this.IncompatibleSignatureFrequency, sourceFrequencies);
        }

        /// <summary>
        /// Merges two frequency dictionaries.
        /// </summary>
        /// <param name="targetFrequencies">The frequencies we are adding to.</param>
        /// <param name="sourceFrequencies">The frequencies to add.</param>
        private void AddFrequencies(Dictionary<string, int> targetFrequencies, Dictionary<string, int> sourceFrequencies)
        {
            foreach (var entry in sourceFrequencies)
            {
                if (!targetFrequencies.ContainsKey(entry.Key))
                {
                    targetFrequencies[entry.Key] = entry.Value;
                }
                else
                {
                    targetFrequencies[entry.Key] += entry.Value;
                }
            }
        }

        /// <summary>
        /// Persists the statistics to local storage.
        /// </summary>
        public void Persist()
        {
            try
            {
                var json = JsonMapper.ToJson(this.SignatureFrequency);
                _persistenceLayer.SetString(ConduitSignatureFrequencyKey, json);

                json = JsonMapper.ToJson(this.IncompatibleSignatureFrequency);
                _persistenceLayer.SetString(ConduitIncompatibleSignatureFrequencyKey, json);
            
                _persistenceLayer.SetInt(ConduitSuccessfulGenerationsKey, SuccessfulGenerations);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to persist Conduit statistics. {e}");
            }
        }

        /// <summary>
        /// Loads the statistics from local storage.
        /// </summary>
        public void Load()
        {
            try
            {
                SuccessfulGenerations = _persistenceLayer.HasKey(ConduitSuccessfulGenerationsKey)
                    ? _persistenceLayer.GetInt(ConduitSuccessfulGenerationsKey)
                    : 0;

                if (_persistenceLayer.HasKey(ConduitSignatureFrequencyKey))
                {
                    var json = _persistenceLayer.GetString(ConduitSignatureFrequencyKey);
                    SignatureFrequency = JsonMapper.ToObject<Dictionary<string, int>>(json);
                }
                else
                {
                    SignatureFrequency = new Dictionary<string, int>();
                }

                if (_persistenceLayer.HasKey(ConduitIncompatibleSignatureFrequencyKey))
                {
                    var json = _persistenceLayer.GetString(ConduitIncompatibleSignatureFrequencyKey);
                    IncompatibleSignatureFrequency = JsonMapper.ToObject<Dictionary<string, int>>(json);
                }
                else
                {
                    IncompatibleSignatureFrequency = new Dictionary<string, int>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load Conduit statistics. {e}");
            }
        }
    }
}
