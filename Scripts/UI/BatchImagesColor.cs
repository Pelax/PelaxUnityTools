using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pelax.UI
{
    public class BatchImagesColor : MonoBehaviour
    {
        public List<Image> Images;
        public Color Color;

#if UNITY_EDITOR
        void OnValidate()
        {
            foreach (var image in Images)
            {
                image.color = Color;
            }
        }
#endif
    }
}
