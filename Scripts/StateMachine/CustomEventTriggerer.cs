using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Pelax.StateMachine
{
    public class CustomEventTriggerer : MonoBehaviour
    {
        public void TriggerEvent(string eventName)
        {
            CustomEvent.Trigger(gameObject, eventName);
        }
    }
}
