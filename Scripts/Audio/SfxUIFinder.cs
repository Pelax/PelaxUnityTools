using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Pelax.Audio
{
    public class SfxUIFinder : MonoBehaviour
    {
        public List<Button> AllButtons;
        public List<Toggle> AllToggles;
        public AudioSource SfxButtonAudioSource;
        public AudioSource SfxToggleAudioSource;

        void Awake()
        {
            foreach (var button in AllButtons)
            {
                button.onClick.AddListener(() =>
                {
                    SfxButtonAudioSource.Play();
                });
            }
            foreach (var toggle in AllToggles)
            {
                toggle.onValueChanged.AddListener(
                    (value) =>
                    {
                        SfxToggleAudioSource.Play();
                    }
                );
            }
        }

        void OnDestroy()
        {
            foreach (var button in AllButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (var toggle in AllToggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Find all UI elements")]
        void FindAllUIElements()
        {
            AllButtons = FindObjectsByType<Button>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                )
                .ToList();
            AllToggles = FindObjectsByType<Toggle>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                )
                .ToList();
        }
#endif
    }
}
