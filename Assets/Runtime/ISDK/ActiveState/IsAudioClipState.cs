using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    public class IsAudioClipState : MonoBehaviour, IActiveState
    {
        public List<AudioClip> Clips;

        public bool Active => Clips.TrueForAll(x => x.loadState == AudioDataLoadState.Loaded);
    }
}
