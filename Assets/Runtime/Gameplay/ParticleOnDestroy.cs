using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Plays a particle system and destroys the object.
    /// </summary>
    public class ParticleOnDestroy : MonoBehaviour
    {
        [SerializeField]
        private GameObject _particleSystemPrefab;

        private void Start()
        {
            TriggerDestroy();
        }

        private void TriggerDestroy()
        {
            if (_particleSystemPrefab != null)
            {
                // Instantiate the particle system at the object's position and rotation
                Instantiate(_particleSystemPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
