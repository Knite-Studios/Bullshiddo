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
using System.Collections;
using System.Collections.Generic;
using Facebook.WitAi.Interfaces;
using Facebook.WitAi.Lib;
using UnityEngine;

namespace Facebook.WitAi.Data.Entities
{
    [Serializable]
    public class WitDynamicEntities : IDynamicEntitiesProvider, IEnumerable<WitDynamicEntity>
    {
        public List<WitDynamicEntity> entities = new List<WitDynamicEntity>();

        public WitDynamicEntities()
        {

        }

        public WitDynamicEntities(IEnumerable<WitDynamicEntity> entity)
        {
            entities.AddRange(entity);
        }

        public WitDynamicEntities(params WitDynamicEntity[] entity)
        {
            entities.AddRange(entity);
        }

        public WitResponseClass AsJson
        {
            get
            {
                WitResponseClass json = new WitResponseClass();
                foreach (var entity in entities)
                {
                    json.Add(entity.entity, entity.AsJson);
                }

                return json;
            }
        }

        public override string ToString()
        {
            return AsJson.ToString();
        }

        public IEnumerator<WitDynamicEntity> GetEnumerator() => entities.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public WitDynamicEntities GetDynamicEntities()
        {
            return this;
        }

        public void Merge(IDynamicEntitiesProvider provider)
        {
            if (null == provider) return;

            entities.AddRange(provider.GetDynamicEntities());
        }

        public void Merge(IEnumerable<WitDynamicEntity> mergeEntities)
        {
            if (null == mergeEntities) return;

            entities.AddRange(mergeEntities);
        }

        public void Add(WitDynamicEntity dynamicEntity)
        {
            int index = entities.FindIndex(e => e.entity == dynamicEntity.entity);
            if(index < 0) entities.Add(dynamicEntity);
            else Debug.LogWarning($"Cannot add entity, registry already has an entry for {dynamicEntity.entity}");
        }

        public void Remove(WitDynamicEntity dynamicEntity)
        {
            entities.Remove(dynamicEntity);
        }

        public void AddKeyword(string entityName, WitEntityKeyword keyword)
        {
            var entity = entities.Find(e => entityName == e.entity);
            if (null == entity)
            {
                entity = new WitDynamicEntity(entityName);
                entities.Add(entity);
            }
            entity.keywords.Add(keyword);
        }

        public void RemoveKeyword(string entityName, WitEntityKeyword keyword)
        {
            int index = entities.FindIndex(e => e.entity == entityName);
            if (index >= 0)
            {
                entities[index].keywords.Remove(keyword);
                if(entities[index].keywords.Count == 0) entities.RemoveAt(index);
            }
        }
    }
}
