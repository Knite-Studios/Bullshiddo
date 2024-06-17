using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Base class for behaviours that respond to UI hover events.
    /// </summary>
    [ExecuteAlways]
    public abstract class UIStateVisual : MonoBehaviour
    {
        private UIStateParentReference _uiState;
        private static readonly IUIState NoneState = new NullUIState();

        protected virtual void OnValidate()
        {
            if (_uiState.IsValid && isActiveAndEnabled)
            {
                UpdateVisual(_uiState, false);
            }
        }

        private void InternalUpdateVisual()
        {
            UpdateVisual(_uiState.IsValid && isActiveAndEnabled ? _uiState : NoneState, Application.isPlaying);
        }

        protected virtual void OnEnable()
        {
            _uiState = new UIStateParentReference(transform, InternalUpdateVisual);
            UpdateVisual(_uiState, false);
        }

        protected virtual void OnDisable()
        {
            _uiState.Dispose();
            UpdateVisual(NoneState, false);
        }

        protected abstract void UpdateVisual(IUIState uiState, bool animate);

        private class NullUIState : IUIState
        {
            public bool Focused => false;
            public bool Active => false;
            public UIStates State => UIStates.None;
        }
    }

    public struct UIStateParentReference : IDisposable, IUIState
    {
        private ParentChangedListener _parentChangeListener;
        private UIState _uiState;
        private Action _callback;

        public bool IsValid { get; private set; }
        public bool Interactable => IsValid ? _uiState.Interactable : true;
        public bool Focused => IsValid ? _uiState.Focused : false;
        public bool Active => IsValid ? _uiState.Active : false;
        public UIStates State => IsValid ? _uiState.State : UIStates.Normal;

        public UIStateParentReference(Transform child, Action callback) : this()
        {
            _callback = callback;

            _parentChangeListener = ParentChangedListener.Get(child.gameObject);
            _parentChangeListener.WhenParentWillChange += Unregister;
            _parentChangeListener.WhenParentChanged += UpdateUIStateParent;

            UpdateUIStateParent();
        }

        private void UpdateUIStateParent()
        {
            _uiState = _parentChangeListener.GetComponentInParent<UIState>();

            if (_uiState == null)
            {
                var selectable = _parentChangeListener.GetComponentInParent<Selectable>();
                if (selectable != null)
                {
                    _uiState = selectable.gameObject.AddComponent<UIState>();
                    Debug.LogWarning($"UIState added to {selectable.gameObject.name}", selectable);
                }
            }

            if (_uiState != null)
            {
                _uiState.WhenChanged += InvokeWhenChanged;
                IsValid = true;
            }
        }

        private void Unregister()
        {
            if (_uiState != null)
            {
                _uiState.WhenChanged -= InvokeWhenChanged;
                _uiState = null;
            }
            IsValid = false;
        }

        private void InvokeWhenChanged()
        {
            _callback?.Invoke();
        }

        public void Dispose()
        {
            _parentChangeListener.Dispose();
            Unregister();
        }

        private class ParentChangedListener : MonoBehaviour, IDisposable
        {
            public event Action WhenParentWillChange;
            public event Action WhenParentChanged;

            private void OnBeforeTransformParentChanged() => WhenParentWillChange?.Invoke();
            private void OnTransformParentChanged() => WhenParentChanged?.Invoke();

            public static ParentChangedListener Get(GameObject source)
            {
                var result = source.AddComponent<ParentChangedListener>();
                result.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
                return result;
            }

            public void Dispose()
            {
                if (!Application.isPlaying)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.CallbackFunction destroySelf = null;
                    destroySelf = () =>
                    {
                        UnityEditor.EditorApplication.update -= destroySelf;
                        if (this != null) DestroyImmediate(this);
                    };
                    UnityEditor.EditorApplication.update += destroySelf;
#else
                    Destroy(this);
#endif
                }
                else
                {
                    Destroy(this);
                }
            }
        }
    }
}
