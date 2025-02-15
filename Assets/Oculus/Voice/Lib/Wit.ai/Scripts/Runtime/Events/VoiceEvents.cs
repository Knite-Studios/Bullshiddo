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
using Facebook.WitAi.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Facebook.WitAi.Events
{
    [Serializable]
    public class VoiceEvents : EventRegistry, ITranscriptionEvent, IAudioInputEvents
    {
        private const string EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS = "Activation Result Events";
        private const string EVENT_CATEGORY_MIC_EVENTS = "Mic Events";
        private const string EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS = "Activation - Deactivation Events";
        private const string EVENT_CATEGORY_TRANSCRIPTION_EVENTS = "Transcription Events";
        private const string EVENT_CATEGORY_DATA_EVENTS = "Data Events";

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip("Called when a response from Wit.ai has been received")]
        public WitResponseEvent OnResponse = new WitResponseEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip("Called when response from Wit.ai has been received from partial transcription")]
        [HideInInspector]
        public WitResponseEvent OnPartialResponse = new WitResponseEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip("Called after an on partial response to validate data.  If data.validResponse is true, service will deactivate & use the partial data as final")]
        public WitValidationEvent OnValidatePartialResponse = new WitValidationEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip(
            "Called when there was an error with a WitRequest  or the RuntimeConfiguration is not properly configured.")]
        public WitErrorEvent OnError = new WitErrorEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip(
            "Called when the activation is about to be aborted by a direct user interaction.")]
        public UnityEvent OnAborting = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip(
            "Called when the activation stopped because the network request was aborted. This can be via a timeout or call to AbortActivation.")]
        public UnityEvent OnAborted = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_RESULT_EVENTS)]
        [Tooltip(
            "Called when a a request has completed and all response and error callbacks have fired.")]
        public UnityEvent OnRequestCompleted = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_MIC_EVENTS)]
        [Tooltip("Called when the volume level of the mic input has changed")]
        public WitMicLevelChangedEvent OnMicLevelChanged = new WitMicLevelChangedEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip("Called on initial wit request option set for custom overrides")]
        public WitRequestOptionsEvent OnRequestOptionSetup = new WitRequestOptionsEvent();

        /// <summary>
        /// Called when a request is created. This happens at the beginning of
        /// an activation before the microphone is activated (if in use).
        /// </summary>
        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip(
            "Called when a request is created. This happens at the beginning of an activation before the microphone is activated (if in use)")]
        public WitRequestCreatedEvent OnRequestCreated = new WitRequestCreatedEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip("Called when the microphone has started collecting data collecting data to be sent to Wit.ai. There may be some buffering before data transmission starts.")]
        public UnityEvent OnStartListening = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip(
            "Called when the voice service is no longer collecting data from the microphone")]
        public UnityEvent OnStoppedListening = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip(
            "Called when the microphone input volume has been below the volume threshold for the specified duration and microphone data is no longer being collected")]
        public UnityEvent OnStoppedListeningDueToInactivity = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip(
            "The microphone has stopped recording because maximum recording time has been hit for this activation")]
        public UnityEvent OnStoppedListeningDueToTimeout = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip("The Deactivate() method has been called ending the current activation.")]
        public UnityEvent OnStoppedListeningDueToDeactivation = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip("Fired when recording stops, the minimum volume threshold was hit, and data is being sent to the server.")]
        public UnityEvent OnMicDataSent = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_ACTIVATION_DEACTIVATION_EVENTS)]
        [Tooltip("Fired when the minimum wake threshold is hit after an activation")]
        public UnityEvent OnMinimumWakeThresholdHit = new UnityEvent();

        [EventCategory(EVENT_CATEGORY_TRANSCRIPTION_EVENTS)]
        [FormerlySerializedAs("OnPartialTranscription")]
        [Tooltip("Message fired when a partial transcription has been received.")]
        public WitTranscriptionEvent onPartialTranscription = new WitTranscriptionEvent();

        [FormerlySerializedAs("OnFullTranscription")]
        [EventCategory(EVENT_CATEGORY_TRANSCRIPTION_EVENTS)]
        [Tooltip("Message received when a complete transcription is received.")]
        public WitTranscriptionEvent onFullTranscription = new WitTranscriptionEvent();

        [EventCategory(EVENT_CATEGORY_DATA_EVENTS)]
        public WitByteDataEvent OnByteDataReady = new WitByteDataEvent();
        [EventCategory(EVENT_CATEGORY_DATA_EVENTS)]
        public WitByteDataEvent OnByteDataSent = new WitByteDataEvent();

        #region Shared Event API - Transcription
        public WitTranscriptionEvent OnPartialTranscription => onPartialTranscription;
        public WitTranscriptionEvent OnFullTranscription => onFullTranscription;
        #endregion

        #region Shared Event API - Audio Input
        public WitMicLevelChangedEvent OnMicAudioLevelChanged => OnMicLevelChanged;
        public UnityEvent OnMicStartedListening => OnStartListening;
        public UnityEvent OnMicStoppedListening => OnStoppedListening;
        #endregion
    }
 }
