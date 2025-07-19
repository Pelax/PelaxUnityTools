using System.Collections;
using System.Collections.Generic;
using Pelax.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace Pelax.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [System.Serializable]
        public class Playlist
        {
            public List<AudioClip> Songs;
        }

        public override void Initialize()
        {
            base.Initialize();
            StartCoroutine(InitVolumes());
        }

        public const string MusicVolume = "MusicVolume";
        public const string SfxVolume = "SfxVolume";

        public AudioMixer AudioMixer;
        public AudioSource MusicAudioSource;
        public AudioSource SfxAudioSource;

        public List<Playlist> Playlists;
        public Playlist CurrentPlaylist;
        public List<AudioClip> CurrentSongs = new List<AudioClip>();

        public int MaxSfxs = 10;
        public int CurrentSfxs = 0;

        public float DefaultMusicVolume = 0.5f;
        public float DefaultSfxVolume = 0.5f;

        IEnumerator InitVolumes()
        {
            yield return null;
            SetMusicVolume(PlayerData.GetFloat(MusicVolume, DefaultMusicVolume));
            SetSfxVolume(PlayerData.GetFloat(SfxVolume, DefaultSfxVolume));
            PlayPlaylist(0, false);
        }

        public void SetMusicVolume(float value)
        {
            AudioMixer.SetFloat(MusicVolume, Mathf.Log10(value) * 20);
            PlayerData.SetFloat(MusicVolume, value);
        }

        public void SetSfxVolume(float value)
        {
            AudioMixer.SetFloat(SfxVolume, Mathf.Log10(value) * 20);
            PlayerData.SetFloat(SfxVolume, value);
        }

        public void PlaySfx(AudioClip clip)
        {
            if (CurrentSfxs < MaxSfxs)
            {
                SfxAudioSource.PlayOneShot(clip);
                CurrentSfxs++;
                StartCoroutine(DecrementSoundCount(clip.length));
            }
        }

        private IEnumerator DecrementSoundCount(float delay)
        {
            yield return new WaitForSeconds(delay);
            CurrentSfxs--;
        }

        public void PlayPlaylist(int playlistIndex, bool randomize)
        {
            if (playlistIndex > Playlists.Count - 1 || CurrentPlaylist == Playlists[playlistIndex])
                return;
            BuildSongList(playlistIndex, randomize);
            PlayNextSong();
        }

        private void PlayNextSong()
        {
            MusicAudioSource.clip = CurrentSongs[0];
            MusicAudioSource.time = 0;
            CurrentSongs.Remove(MusicAudioSource.clip);
            MusicAudioSource.Play();
        }

        public void BuildSongList(int playlistIndex, bool randomize)
        {
            // this requires to reset the current list songs
            CurrentSongs = new List<AudioClip>();
            CurrentPlaylist = Playlists[playlistIndex];
            // get the complete playlist for this level
            List<AudioClip> newSongList = new List<AudioClip>();
            newSongList.AddRange(CurrentPlaylist.Songs);
            // add all the songs that are not in the current playlist (or currently playing)
            while (newSongList.Count > 0)
            {
                AudioClip newClip = newSongList[newSongList.Count - 1];
                if (randomize)
                {
                    newClip = newSongList[Random.Range(0, newSongList.Count)];
                }
                newSongList.Remove(newClip);
                if (!CurrentSongs.Contains(newClip) && newClip != MusicAudioSource.clip)
                {
                    CurrentSongs.Insert(0, newClip);
                }
            }
            // remove all songs that are not in the new playlist from the active list of songs
            for (int i = CurrentSongs.Count - 1; i >= 0; i--)
            {
                if (!CurrentPlaylist.Songs.Contains(CurrentSongs[i]))
                {
                    CurrentSongs.RemoveAt(i);
                }
            }
        }

        void Update()
        {
            if (
                MusicAudioSource.clip != null
                && MusicAudioSource.time >= MusicAudioSource.clip.length
            )
            {
                if (
                    CurrentPlaylist.Songs.Contains(MusicAudioSource.clip)
                    && !CurrentSongs.Contains(MusicAudioSource.clip)
                )
                {
                    // if the song that ended belongs to the current playlist and it's not present atm, re-add it at the end
                    CurrentSongs.Add(MusicAudioSource.clip);
                }
                PlayNextSong();
            }
#if UNITY_EDITOR
            // shortcut to verify loop
            if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                if (MusicAudioSource.clip != null)
                {
                    MusicAudioSource.time = MusicAudioSource.clip.length - 7;
                }
            }
#endif
        }
    }
}
