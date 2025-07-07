using System;
using System.Collections;
using UnityEngine;

namespace Pelax.UI.Transitions
{
    /// <summary>
    /// Scale transition
    /// </summary>
    public class ScaleUITransition : UITransition
    {
        private Vector3 startScale;
        private Vector3 hiddenScale = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();
            startScale = transform.localScale;
        }

        protected override IEnumerator PerformTransition(bool show, Action onComplete = null)
        {
            Vector3 from = show ? hiddenScale : startScale;
            Vector3 to = show ? startScale : hiddenScale;
            float time = 0f;

            transform.localScale = from;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / duration);
                transform.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }

            transform.localScale = to;
            onComplete?.Invoke();
        }
    }
}
