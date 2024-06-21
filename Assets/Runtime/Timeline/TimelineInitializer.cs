using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Evaluates a timeline in Start
    /// </summary>
    public class TimelineInitializer : MonoBehaviour
    {
        void Start() => GetComponent<PlayableDirector>().Evaluate();
    }
}
