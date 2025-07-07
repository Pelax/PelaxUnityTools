using System;
using System.Collections;
using UnityEngine;

namespace Pelax.UI.Transitions
{
    /// <summary>
    /// Fade transition using CanvasGroup
    /// </summary>
    public class FadeTransition : UITransition
    {
        protected override IEnumerator PerformTransition(bool show, Action onComplete = null)
        {
            float startAlpha = show ? 0f : 1f;
            float endAlpha = show ? 1f : 0f;
            float time = 0f;

            if (show)
                CanvasGroup.alpha = startAlpha;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / duration);
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }

            CanvasGroup.alpha = endAlpha;
            onComplete?.Invoke();
        }
    }
}
