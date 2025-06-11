using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Sound
{
    [CreateAssetMenu(menuName = "SoundData")]
    public class SoundData : ScriptableObject
    {
        public AudioClip audioClip;
        public SoundType soundType;
        [Range(0, 1f)]
        public float volume;
    }
}