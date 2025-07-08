using Pelax.Utils;
using UnityEngine;

namespace Pelax.Quality
{
    public class QualityManager : Singleton<QualityManager>
    {
        public const string QualityValue = "QualityValue";

        public int DefaultQuality = 2; // 0 to 5, this is mid quality

        public override void Initialize()
        {
            base.Initialize();
            var savedQuality = PlayerData.GetInt(QualityValue, DefaultQuality);
            QualitySettings.SetQualityLevel(savedQuality);
        }
    }
}
