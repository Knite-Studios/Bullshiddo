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

using UnityEngine;
using System.Collections;
using System.Linq;

namespace Facebook.WitAi.Utilities
{
    public static class CoroutineUtility
    {
        // Start coroutine
        public static CoroutinePerformer StartCoroutine(IEnumerator asyncMethod, bool useUpdate = false)
        {
            CoroutinePerformer performer = GetPerformer();
            performer.CoroutineBegin(asyncMethod, useUpdate);
            return performer;
        }
        // Get performer
        private static CoroutinePerformer GetPerformer()
        {
            CoroutinePerformer performer = new GameObject("Coroutine").AddComponent<CoroutinePerformer>();
            performer.gameObject.hideFlags = HideFlags.HideAndDontSave;
            return performer;
        }
        // Coroutine performer
        public class CoroutinePerformer : MonoBehaviour
        {
            // Whether currently running
            public bool IsRunning { get; private set; }

            // Settings & fields
            private bool _useUpdate;
            private IEnumerator _method;
            private Coroutine _coroutine;

            // Dont destroy
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            // Perform coroutine
            public void CoroutineBegin(IEnumerator asyncMethod, bool useUpdate)
            {
                // Cannot call twice
                if (IsRunning)
                {
                    return;
                }

                // Begin running
                IsRunning = true;

                // Use update in batch mode
                if (Application.isBatchMode)
                {
                    useUpdate = true;
                }
#if UNITY_EDITOR
                // Use update in editor mode
                if (!Application.isPlaying)
                {
                    useUpdate = true;
                    UnityEditor.EditorApplication.update += EditorUpdate;
                }
#endif

                // Set whether to use update or coroutine implementation
                _useUpdate = useUpdate;
                _method = asyncMethod;

                // Begin with initial update
                if (_useUpdate)
                {
                    CoroutineIterateUpdate();
                }
                // Begin coroutine
                else
                {
                    _coroutine = StartCoroutine(CoroutineIterateEnumerator());
                }
            }

#if UNITY_EDITOR
            // Editor iterate
            private void EditorUpdate()
            {
                CoroutineIterateUpdate();
            }
#endif
            // Runtime iterate
            private IEnumerator CoroutineIterateEnumerator()
            {
                // Wait for completion
                yield return _method;
                // Complete
                CoroutineComplete();
            }
            // Update
            private void Update()
            {
                if (_useUpdate)
                {
                    CoroutineIterateUpdate();
                }
            }
            // Batch iterate
            private void CoroutineIterateUpdate()
            {
                // Destroyed
                if (this == null || _method == null)
                {
                    CoroutineCancel();
                }
                // Continue
                else if (!MoveNext(_method))
                {
                    CoroutineComplete();
                }
            }
            // Move through queue
            private bool MoveNext(IEnumerator method)
            {
                // Move sub coroutine
                object current = method.Current;
                if (current != null && current.GetType().GetInterfaces().Contains(typeof(IEnumerator)))
                {
                    if (MoveNext(current as IEnumerator))
                    {
                        return true;
                    }
                }
                // Move this
                return method.MoveNext();
            }
            // Cancel on destroy
            private void OnDestroy()
            {
                CoroutineUnload();
            }
            // Cancel current coroutine
            public void CoroutineCancel()
            {
                CoroutineComplete();
            }
            // Completed
            private void CoroutineComplete()
            {
                // Ignore unless running
                if (!IsRunning)
                {
                    return;
                }

                // Unload
                CoroutineUnload();

                // Destroy
                if (this != null && gameObject != null)
                {
                    DestroyImmediate(gameObject);
                }
            }
            // Unload
            private void CoroutineUnload()
            {
                // Done
                IsRunning = false;

                // Complete
                if (_method != null)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.update -= EditorUpdate;
#endif
                    _method = null;
                }

                // Stop coroutine
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
            }
        }
    }
}
