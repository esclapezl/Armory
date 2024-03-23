using System;
using UnityEngine;

namespace Sounds
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] public AudioClip[] soundClips;
        [NonSerialized] private AudioSource _audioSource;

        void Start()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySound(int clipIndex, float volume = 1f, float pitch = 1f)
        {
            _audioSource.clip = soundClips[clipIndex];
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.Play();
        }
    }
}