using System;
using System.Collections;
using UnityEngine;

namespace Pelax.UI.Transitions
{
    /// <summary>
    /// Slide transition
    /// </summary>
    public class SlideUITransition : UITransition
    {
        public enum SlideDirection
        {
            Up,
            Down,
            Left,
            Right,
        }

        [SerializeField]
        private SlideDirection direction = SlideDirection.Up;

        [SerializeField]
        private float slideDistance = 720f;

        [SerializeField]
        private bool slideOutOfScreen = true;

        public RectTransform rectTransform;
        private Vector2 startPosition;
        private Vector2 hiddenPosition;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            startPosition = rectTransform.anchoredPosition;
            CalculateHiddenPosition();
        }

        private void CalculateHiddenPosition()
        {
            Vector2 directionVector = Vector2.zero;

            switch (direction)
            {
                case SlideDirection.Up:
                    directionVector = Vector2.up;
                    break;
                case SlideDirection.Down:
                    directionVector = Vector2.down;
                    break;
                case SlideDirection.Left:
                    directionVector = Vector2.left;
                    break;
                case SlideDirection.Right:
                    directionVector = Vector2.right;
                    break;
            }

            hiddenPosition = startPosition + (directionVector * slideDistance);
        }

        protected override IEnumerator PerformTransition(bool show, Action onComplete = null)
        {
            Vector2 from = show ? hiddenPosition : startPosition;
            Vector2 to = show ? startPosition : (slideOutOfScreen ? hiddenPosition : startPosition);
            float time = 0f;

            rectTransform.anchoredPosition = from;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / duration);
                rectTransform.anchoredPosition = Vector2.Lerp(from, to, t);
                yield return null;
            }

            rectTransform.anchoredPosition = to;
            onComplete?.Invoke();
        }
    }
}
