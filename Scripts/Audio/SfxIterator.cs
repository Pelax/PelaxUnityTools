using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelax.Audio
{
    public class SfxIterator : MonoBehaviour
    {
        public List<AudioClip> Sfxs;

        [Tooltip("If left null, SoundsManager PlaySfx mehod is used")]
        public AudioSource AudioSource;
        private int sfxIndex = -1; // when playing random, use this as "last played"

        public void PlayNext()
        {
            sfxIndex++;
            if (sfxIndex > Sfxs.Count - 1)
            {
                sfxIndex = 0;
            }
            PlayIndex(sfxIndex);
        }

        public void PlayRandom()
        {
            int index = Random.Range(0, Sfxs.Count);
            if (Sfxs.Count > 1)
            {
                // we have at least 2, makes sure we don't play the same on twice in a row
                while (index == sfxIndex)
                {
                    index = Random.Range(0, Sfxs.Count);
                }
            }
            PlayIndex(index);
            sfxIndex = index;
        }

        private void PlayIndex(int index)
        {
            if (AudioSource == null)
            {
                AudioManager.Instance.PlaySfx(Sfxs[index]);
            }
            else
            {
                AudioSource.clip = Sfxs[index];
                AudioSource.Play();
            }
        }

        internal void Stop()
        {
            AudioSource.Stop();
        }
    }
}
