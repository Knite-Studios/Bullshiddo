using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Handles collisions between two GameObjects. On collision, logs a message,
    /// triggers a falling animation, and destroys the GameObject.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        [SerializeField]
        private Vector3 fallStartPosition; // The start position for the falling animation

        [SerializeField]
        private Vector3 fallEndPosition; // The end position for the falling animation

        [SerializeField]
        private float fallDuration = 1.0f; // Duration of the falling animation

        private bool isFalling = false; // Flag to prevent multiple falls

        /// <summary>
        /// Called when this GameObject's collider first touches another collider.
        /// Logs a message and starts the fall animation if not already falling.
        /// </summary>
        /// <param name="collision">The collision data.</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!isFalling)
            {
                Debug.Log("button pressed");
                StartCoroutine(FallAndDestroy());
            }
        }

        /// <summary>
        /// Coroutine to handle the falling animation and destruction of the GameObject.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator FallAndDestroy()
        {
            isFalling = true;
            float elapsedTime = 0;

            // Perform the falling animation over the specified duration
            while (elapsedTime < fallDuration)
            {
                transform.position = Vector3.Lerp(fallStartPosition, fallEndPosition, elapsedTime / fallDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final position is set
            transform.position = fallEndPosition;
            Destroy(gameObject);
        }
    }
}
