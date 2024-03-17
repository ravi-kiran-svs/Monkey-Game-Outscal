using System;
using UnityEngine;
using ServiceLocator.Utilities;

namespace ServiceLocator.Sound {
    public class SoundService {
        private SoundScriptableObject soundScriptableObject;
        private AudioSource audioEffects;
        private AudioSource backgroundMusic;

        private void Start() {
            PlaybackgroundMusic(SoundType.BackgroundMusic, true);
        }

        public SoundService(SoundScriptableObject sso, AudioSource ae, AudioSource bgm) {
            soundScriptableObject = sso;
            audioEffects = ae;
            backgroundMusic = bgm;
            Start();
        }

        public void PlaySoundEffects(SoundType soundType, bool loopSound = false) {
            AudioClip clip = GetSoundClip(soundType);
            if (clip != null) {
                audioEffects.loop = loopSound;
                audioEffects.clip = clip;
                audioEffects.PlayOneShot(clip);
            } else
                Debug.LogError("No Audio Clip selected.");
        }

        private void PlaybackgroundMusic(SoundType soundType, bool loopSound = false) {
            AudioClip clip = GetSoundClip(soundType);
            if (clip != null) {
                backgroundMusic.loop = loopSound;
                backgroundMusic.clip = clip;
                backgroundMusic.Play();
            } else
                Debug.LogError("No Audio Clip selected.");
        }

        private AudioClip GetSoundClip(SoundType soundType) {
            Sounds sound = Array.Find(soundScriptableObject.audioList, item => item.soundType == soundType);
            if (sound.audio != null)
                return sound.audio;
            return null;
        }
    }
}