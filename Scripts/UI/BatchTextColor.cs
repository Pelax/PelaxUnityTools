using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pelax.UI
{
    public class BatchTextColor : MonoBehaviour
    {
        public List<TMP_Text> Texts;
        public Color Color;

#if UNITY_EDITOR
        void OnValidate()
        {
            foreach (var text in Texts)
            {
                text.color = Color;
            }
        }
#endif
    }
}
