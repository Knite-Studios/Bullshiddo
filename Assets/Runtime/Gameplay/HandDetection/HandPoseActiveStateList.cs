using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// A list of named IActiveStates.
    /// </summary>
    public class HandPoseActiveStateList : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IActiveState))]
        private List<MonoBehaviour> _poses;

        private Dictionary<string, DelayedFalseActiveState> _activeStates = new Dictionary<string, DelayedFalseActiveState>();

        internal IActiveState Get(string poseName)
        {
            return _poses.Find(x => x.name == poseName) as IActiveState;
        }

        public int ActiveCount()
        {
            int count = 0;
            foreach (var pose in _poses)
            {
                if ((pose as IActiveState).Active) count++;
            }
            return count;
        }

        private void Awake()
        {
            _poses.ForEach(x => _activeStates.Add(x.name, new DelayedFalseActiveState(x as IActiveState, 0.05f)));
        }

        private void Update()
        {
            foreach (var x in _activeStates) x.Value.Update();
        }

        private class DelayedFalseActiveState : IActiveState
        {
            private readonly float _delay = 0.1f;
            private readonly IActiveState _activeState;
            private float _lastActiveTime;

            public DelayedFalseActiveState(IActiveState activeState, float delay)
            {
                _activeState = activeState;
                _lastActiveTime = -1;
                _delay = delay;
                Update();
            }

            public bool Active
            {
                get
                {
                    Update();
                    return Time.time - _lastActiveTime < _delay;
                }
            }

            internal void Update()
            {
                if (_activeState.Active) _lastActiveTime = Time.time;
            }
        }
    }
}