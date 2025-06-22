using TMPro;
using UnityEngine;

namespace Pelax.Utils
{
    public class StatsDisplay : MonoBehaviour
    {
        public TMP_Text FPSText;
        public float updateInterval = 0.5f;

        private float accum = 0;
        private float frames = 0;
        private float timeleft;

        private void Start()
        {
            timeleft = updateInterval;
        }

        private void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            frames++;

            if (timeleft <= 0)
            {
                float fps = accum / frames;
                FPSText.text = fps.ToString("f0") + "fps";

                if (fps >= 55)
                {
                    FPSText.color = Color.green;
                }
                else if (fps >= 35)
                {
                    FPSText.color = Color.yellow;
                }
                else
                {
                    FPSText.color = Color.red;
                }

                timeleft = updateInterval;
                accum = 0;
                frames = 0;

            }
        }
    }
}