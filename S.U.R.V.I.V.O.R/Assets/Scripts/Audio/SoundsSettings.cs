using System.Runtime.Serialization;

namespace Audio
{
    [DataContract]
    public class SoundsSettings
    {
        [DataMember] public float masterVolume;
        [DataMember] public float musicVolume;
        [DataMember] public float sfxVolume;
        [DataMember] public float musicTime;
        [DataMember] public int musicIndex;
        [DataMember] public bool musicIsPaused;
    }
}
