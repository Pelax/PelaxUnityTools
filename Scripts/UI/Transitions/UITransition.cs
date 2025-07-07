using System;
using System.Collections;
using UnityEngine;

namespace Pelax.UI.Transitions
{
    /// <summary>
    /// Base class for all UI transitions
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UITransition : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        protected virtual void Awake() { }

        protected void OnValidate()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        public enum TransitionType
        {
            Fade,
            Slide,
            Scale,
        }

        [SerializeField]
        protected float duration = 0.3f;

        [SerializeField]
        protected AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public bool IsRunning = false;

        /// <summary>
        /// Play the transition
        /// </summary>
        /// <param name="show">True to show, false to hide</param>
        /// <param name="onComplete">Callback when transition completes</param>
        public IEnumerator Play(bool show, Action onComplete = null)
        {
            IsRunning = true;
            CanvasGroup.interactable = false; // disable interaction during transition
            yield return PerformTransition(show, onComplete);
            IsRunning = false;
            CanvasGroup.interactable = true; // enable interaction after transition
        }

        protected virtual IEnumerator PerformTransition(bool show, Action onComplete = null)
        {
            yield break;
        }
    }
}
