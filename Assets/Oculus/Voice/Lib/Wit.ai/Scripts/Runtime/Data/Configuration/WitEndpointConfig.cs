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

namespace Facebook.WitAi.Configuration
{
    [Serializable]
    public class WitEndpointConfig
    {
        private static WitEndpointConfig defaultEndpointConfig = new WitEndpointConfig();

        public string uriScheme;
        public string authority;
        public int port;

        public string witApiVersion;

        public string speech;
        public string message;
        public string dictation;

        public string UriScheme => string.IsNullOrEmpty(uriScheme) ? WitRequest.URI_SCHEME : uriScheme;
        public string Authority =>
            string.IsNullOrEmpty(authority) ? WitRequest.URI_AUTHORITY : authority;
        public int Port => port <= 0 ? WitRequest.URI_DEFAULT_PORT : port;
        public string WitApiVersion => string.IsNullOrEmpty(witApiVersion)
            ? WitRequest.WIT_API_VERSION
            : witApiVersion;

        public string Speech =>
            string.IsNullOrEmpty(speech) ? WitRequest.WIT_ENDPOINT_SPEECH : speech;

        public string Message =>
            string.IsNullOrEmpty(message) ? WitRequest.WIT_ENDPOINT_MESSAGE : message;

        public string Dictation => string.IsNullOrEmpty(dictation) ? WitRequest.WIT_ENDPOINT_DICTATION : dictation;

        public static WitEndpointConfig GetEndpointConfig(WitConfiguration witConfig)
        {
            return witConfig && null != witConfig.endpointConfiguration
                ? witConfig.endpointConfiguration
                : defaultEndpointConfig;
        }
    }
}
