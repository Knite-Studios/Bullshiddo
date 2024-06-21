using Oculus.Interaction;
using Oculus.Interaction.Bullshiddo;
using UnityEngine;
public class HandPoseActiveState : MonoBehaviour, IActiveState
{
    [SerializeField]
    IHandReference _hand;

    [SerializeField]
    PoseRecognizer _poseRecognizer;

    [SerializeField]
    ReferenceActiveState _canHitTarget;

    public bool Active => _canHitTarget && _poseRecognizer.IsMatch(_hand);
}
