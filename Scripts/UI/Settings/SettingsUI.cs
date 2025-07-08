using Pelax.Audio;
using Pelax.Quality;
using Pelax.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Pelax.UI.Settings
{
    public class SettingsUI : UIPanel
    {
        public Slider MusicSlider;
        public Slider SfxSlider;
        public Slider QualitySlider;
        public AudioSource TestSfx;

        protected override void OnPreReady()
        {
            base.OnPreReady();
            MusicSlider.value = PlayerData.GetFloat(AudioManager.MusicVolume, 1);
            SfxSlider.value = PlayerData.GetFloat(AudioManager.SfxVolume, 1);
            QualitySlider.value = PlayerData.GetInt(QualityManager.QualityValue, 1);
        }

        protected override void OnReady()
        {
            base.OnReady();
            MusicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            SfxSlider.onValueChanged.AddListener(OnSfxsSliderValueChanged);
            QualitySlider.onValueChanged.AddListener(OnQualitySliderChanged);
        }

        protected override void OnNotReady()
        {
            base.OnNotReady();
            MusicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
            SfxSlider.onValueChanged.RemoveListener(OnSfxsSliderValueChanged);
            QualitySlider.onValueChanged.RemoveListener(OnQualitySliderChanged);
        }

        private void OnSfxsSliderValueChanged(float value)
        {
            AudioManager.Instance.SetSfxVolume(value);
            PlayTestSfx();
        }

        private void OnMusicSliderValueChanged(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }

        private void OnQualitySliderChanged(float value)
        {
            QualitySettings.SetQualityLevel((int)value);
            PlayerData.SetInt(QualityManager.QualityValue, (int)value);
        }

        public void PlayTestSfx()
        {
            if (TestSfx.isPlaying)
                return;
            TestSfx.Play();
        }
    }
}
