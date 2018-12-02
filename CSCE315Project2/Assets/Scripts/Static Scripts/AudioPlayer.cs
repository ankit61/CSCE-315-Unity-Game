using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound {

    public static class AudioPlayer {

        public static void PlayRandomDeathSound(AudioSource source)
        {
            int randomIndex = Random.Range(0, Constants.DEATH_AUDIO_CLIPS.Count); // Max is exclusive
            //randomIndex = 2;
            Debug.Log(Constants.DEATH_AUDIO_CLIPS[randomIndex]);
            AudioClip clip = (AudioClip)Resources.Load(Constants.DEATH_AUDIO_CLIPS[randomIndex]);
            source.clip = clip;
            source.volume = 0.5f;
            source.Play();
        }

        public static void PlaySound(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.volume = 0.5f;
            source.Play();
        }
    }
}
