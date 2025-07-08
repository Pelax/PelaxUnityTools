using System.Collections;
using System.Linq;
using Pelax.UI.Transitions;
using Pelax.Utils;
using UnityEngine;

namespace Pelax.UI
{
    /// <summary>
    /// Base class for all UI screens with transition support
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        [Header("Transitions")]
        [SerializeField]
        private UITransition[] transitions;

        private Coroutine activeTransition;

        [SerializeField]
        private bool isReady;

        private bool isDestroyed;

        void OnValidate()
        {
            // find all transitions in this object and add them
            transitions = GetComponentsInChildren<UITransition>(false)
                .Where(t => t.enabled)
                .ToArray();
        }

        /// <summary>
        /// Most UIs are hidden by default at start if they were active
        /// </summary>
        void Awake()
        {
            // if not ready at start, disable silently
            if (Time.time == 0 && !isReady)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Toggle the UI with optional transition
        /// </summary>
        public void Toggle(bool enable)
        {
            if (enable == isReady || isDestroyed)
                return;

            // enable object to play transition
            gameObject.SetActive(true);
            if (enable)
            {
                OnPreReady();
            }

            if (activeTransition != null)
            {
                StopCoroutine(activeTransition);
                activeTransition = null;
            }

            activeTransition = StartCoroutine(ToggleRoutine(enable));
        }

        private IEnumerator ToggleRoutine(bool enable)
        {
            if (enable)
            {
                yield return PlayTransition(true);
                isReady = true;
                OnReady();
            }
            else
            {
                isReady = false;
                yield return PlayTransition(false);
                OnNotReady();
                gameObject.SetActive(false);
            }

            activeTransition = null;
        }

        private IEnumerator PlayTransition(bool show)
        {
            if (transitions == null || transitions.Length == 0)
                yield break;

            var routines = new IEnumerator[transitions.Length];
            for (int i = 0; i < transitions.Length; i++)
            {
                routines[i] = transitions[i].Play(show);
            }

            // Start all transitions
            foreach (var routine in routines)
            {
                if (routine != null)
                    StartCoroutine(routine);
            }

            // Wait for all transitions to complete if needed
            bool anyTransitionRunning;
            do
            {
                anyTransitionRunning = false;
                foreach (var transition in transitions)
                {
                    if (transition != null && transition.IsRunning)
                    {
                        anyTransitionRunning = true;
                        break;
                    }
                }
                yield return null;
            } while (anyTransitionRunning);
        }

        /// <summary>
        /// Used to setup UIs before they transition in
        /// </summary>
        protected virtual void OnPreReady() { }

        /// <summary>
        /// UI is already visible and interactable
        /// </summary>
        protected virtual void OnReady() { }

        /// <summary>
        /// UI is hidden and not interactable
        /// </summary>
        protected virtual void OnNotReady() { }

        /// <summary>
        /// UI is being destroyed and new Toggle calls should be ignored
        /// </summary>
        protected virtual void OnDestroy()
        {
            isDestroyed = true;
        }
    }
}
