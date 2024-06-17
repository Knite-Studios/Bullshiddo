using UnityEngine;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// Controls the visibility of a rectangle's shadow in response to Unity events.
    /// </summary>
    public class UIStateRectangleShadow : UIStateVisual
    {
        [SerializeField]
        private float _duration = 0.2f;

        [SerializeField]
        private UIStateValues<float> _shadowVisibility;

        private Rectangle _rectangle;

        protected override void OnEnable()
        {
            _rectangle = GetComponent<Rectangle>();
            base.OnEnable();
        }

        protected override void UpdateVisual(IUIState uiState, bool animate)
        {
            var shadows = _shadowVisibility.GetValue(uiState.State, 1);
            TweenRunner.Tween(_rectangle.DropShadowVisibility, shadows, _duration, x => _rectangle.DropShadowVisibility = x)
                .SetID(this)
                .Skip(!animate);
        }
    }
}