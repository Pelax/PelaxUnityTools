using Unity.VisualScripting;
using UnityEngine;

namespace Pelax.UI
{
    /// <summary>
    /// Exposes methods to be used with Unity's state machines
    /// </summary>
    public class UIStateMachineHelper : MonoBehaviour
    {
        public void TogglePanel(UIPanel panel, bool enable)
        {
            panel.Toggle(enable);
        }

        public void TriggerEvent(string eventName)
        {
            CustomEvent.Trigger(gameObject, eventName);
        }
    }
}
