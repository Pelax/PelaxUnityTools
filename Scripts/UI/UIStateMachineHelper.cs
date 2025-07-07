using UnityEngine;

namespace Pelax.UI
{
    /// <summary>
    /// Exposes the UIPanel Toggle method to be used in Unity's state machines
    /// </summary>
    public class UIStateMachineHelper : MonoBehaviour
    {
        public void TogglePanel(UIPanel panel, bool enable)
        {
            panel.Toggle(enable);
        }
    }
}
