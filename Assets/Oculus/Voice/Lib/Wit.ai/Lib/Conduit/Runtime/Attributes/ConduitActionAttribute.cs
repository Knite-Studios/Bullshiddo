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

using System.Collections.Generic;
using System.Linq;

namespace Meta.Conduit
{
    using System;

    /// <summary>
    /// Marks the method as a callback for voice commands. The method will be mapped to an intent and invoked whenever
    /// that intent is resolved by the backend.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConduitActionAttribute : Attribute
    {
        /// <summary>
        /// The intent name matching this method. If left blank, the method name will be used to infer the intent name.
        /// </summary>
        public string Intent { get; private set; }

        /// <summary>
        /// The minimum confidence value for intent matching
        /// </summary>
        public float MinConfidence { get; protected set; }
        // Default minimum confidence
        protected const float DEFAULT_MIN_CONFIDENCE = 0.9f;

        /// <summary>
        /// The maximum confidence value for intent matching
        /// </summary>
        public float MaxConfidence { get; protected set; }
        // Default maximum confidence
        protected const float DEFAULT_MAX_CONFIDENCE = 1.0f;

        /// <summary>
        /// Additional aliases to refer to the intent this method represent.
        /// </summary>
        public List<string> Aliases { get; private set; }

        /// <summary>
        /// If true, this will be called for partial responses
        /// instead of full responses.  It will also contain a VoiceSession
        /// parameter which can be used to 'validate' a partial response so
        /// the VoiceSession treats the response as final & deactivates.
        /// </summary>
        public bool ValidatePartial { get; private set; }

        /// <summary>
        /// Triggers a method to be executed if it matches a voice command's intent.
        /// </summary>
        /// <param name="intent">The name of the intent to match.</param>
        protected ConduitActionAttribute(string intent = "", params string[] aliases)
        {
            this.Intent = intent;
            this.Aliases = aliases.ToList();
        }

        /// <summary>
        /// Triggers a method to be executed if it matches a voice command's intent.
        /// </summary>
        /// <param name="intent">The name of the intent to match.</param>
        /// <param name="minConfidence">The minimum confidence value (0-1) needed to match.</param>
        /// <param name="maxConfidence">The maximum confidence value(0-1) needed to match.</param>
        /// <param name="aliases">Other names to refer to this intent.</param>
        protected ConduitActionAttribute(string intent = "", float minConfidence = DEFAULT_MIN_CONFIDENCE, float maxConfidence = DEFAULT_MAX_CONFIDENCE, bool validatePartial = false, params string[] aliases)
        {
            this.Intent = intent;
            this.MinConfidence = minConfidence;
            this.MaxConfidence = maxConfidence;
            this.ValidatePartial = validatePartial;
            this.Aliases = aliases.ToList();
        }
    }
}
