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
    /// <summary>
    /// Represents a method parameter/argument in the manifest.
    /// </summary>
    internal class ManifestParameter
    {
        /// <summary>
        /// Called via JSON reflection, need preserver or it will be stripped on compile
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public ManifestParameter() { }

        /// <summary>
        /// This is the parameter name as exposed to the backend (slot or role)
        /// </summary>
        public string Name
        {
            get => name;
            set => name = ConduitUtilities.DelimitWithUnderscores(value).ToLower();
        }
        private string name;

        /// <summary>
        /// This is the technical name of the parameter in the actual method in codebase.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// A fully qualified name exposed to the backend for uniqueness.
        /// </summary>
        public string QualifiedName { get; set; }

        /// <summary>
        /// This is the data type of the parameter, exposed as an entity type.
        /// </summary>
        public string EntityType
        {
            get
            {
                var lastPeriod = QualifiedTypeName.LastIndexOf('.');
                if (lastPeriod < 0)
                {
                    return string.Empty;
                }
                var entityName = QualifiedTypeName.Substring(lastPeriod + 1);

                // Identify whether it's a nested type
                var lastPlus = entityName.LastIndexOf('+');

                if (lastPlus < 0)
                {
                    return entityName;
                }

                return entityName.Substring(lastPlus + 1);
            }
        }

        /// <summary>
        /// The assembly containing the data type.
        /// </summary>
        public string TypeAssembly { get; set; }

        /// <summary>
        /// The fully qualified name of the parameter data type.
        /// </summary>
        public string QualifiedTypeName { get; set; }

        /// <summary>
        /// Additional names by which the backend can refer to this parameter.
        /// </summary>
        public List<string> Aliases { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ManifestParameter other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + name.GetHashCode();
            hash = hash * 31 + InternalName.GetHashCode();
            hash = hash * 31 + QualifiedName.GetHashCode();
            hash = hash * 31 + TypeAssembly.GetHashCode();
            hash = hash * 31 + QualifiedTypeName.GetHashCode();
            hash = hash * 31 + Aliases.GetHashCode();
            return hash;
        }

        private bool Equals(ManifestParameter other)
        {
            return Equals(this.InternalName, other.InternalName) && Equals(this.QualifiedName, other.QualifiedName) &&
                   Equals(this.EntityType, other.EntityType) && this.Aliases.SequenceEqual(other.Aliases) &&
                   Equals(this.TypeAssembly, other.TypeAssembly) &&
                   Equals(this.QualifiedTypeName, other.QualifiedTypeName);
        }
    }
}
