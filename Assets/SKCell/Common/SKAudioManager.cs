using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Animations;

namespace SKCell
{
    [AddComponentMenu("SKCell/Misc/SKAudioManager")]
    public class SKAudioManager : SKMonoSingleton<SKAudioManager>
    {
        public AudioSource musicAudioSource;

        private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();      
        private Dictionary<string, AudioSource> audioSourceDict = new Dictionary<string, AudioSource>();      

        public float musicVolume = 1;
        public float soundVolume = 1;

        private string MUSIC_PATH = "AudioClip/Music/";
        private string SOUND_PATH = "AudioClip/Sound/";

        private Dictionary<AudioSource, float> musicSources = new Dictionary<AudioSource, float>();
        private Dictionary<AudioSource, float> soundSources = new Dictionary<AudioSource, float>();

        private Transform root;
        private void Start()
        {
            root = new GameObject("AudioRoot").transform;
            DontDestroyOnLoad(root.gameObject);
        }

        public AudioSource PlayMusic(string id, Action action = null, bool loop = false, float volume = 1f, float pitch = 1f, float damp = 0f)
        {
            AudioSource audioSource = new GameObject("Audio").AddComponent<AudioSource>();
            audioSource.transform.SetParent(root);
            musicSources.Add(audioSource, volume);
            audioSource.clip = GetAudioClip(id, 1);
            audioSource.clip.LoadAudioData();
            audioSource.loop = loop;
            audioSource.pitch = pitch;
            audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
            if (damp > 0f)
                SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, damp, (f) =>
                {
                    audioSource.volume = f * musicVolume * volume;
                });
            else
                audioSource.volume = musicVolume * volume;

            float oTimeScale = Time.timeScale;
            Time.timeScale = 1;
            audioSource.Play();
            if (!loop)
                SKUtils.InvokeAction(audioSource.clip.length, () => {
                    Destroy(audioSource.gameObject);
                    musicSources.Remove(audioSource);
                });
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public AudioSource PlaySound(string id, Action action = null, bool loop = false, float volume = 1f, float pitch = 1f, float damp =0f)
        {
            AudioSource audioSource = new GameObject("Audio").AddComponent<AudioSource>();
            audioSource.transform.SetParent(root);
            soundSources.Add(audioSource, volume);
            audioSource.clip = GetAudioClip(id, 1);
            audioSource.clip.LoadAudioData();
            audioSource.loop = loop;
            audioSource.pitch = pitch;
            audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
            if(damp > 0f)
                SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, damp, (f) =>
                {
                    audioSource.volume = f * soundVolume * volume;
                });
            else
                audioSource.volume = soundVolume * volume;

            float oTimeScale = Time.timeScale;
            Time.timeScale = 1;
            audioSource.Play();
            if (!loop)
                SKUtils.InvokeAction(audioSource.clip.length, () => { 
                    Destroy(audioSource.gameObject);
                    SKUtils.RemoveKeyInDictionary(soundSources, audioSource);
                });
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public AudioSource PlayIdentifiableSound(string fileName, string id, bool loop = false, float volume = 1, float damp = 0.5f)
        {
            if (audioSourceDict.ContainsKey(id))
            {
                return null;
            }

            AudioSource audioSource = new GameObject("Audio").AddComponent<AudioSource>();
            audioSource.transform.SetParent(root);
            soundSources.Add(audioSource, volume);
            audioSourceDict.Add(id, audioSource);

            audioSource.clip = GetAudioClip(fileName, 1);
            if (!audioSource.clip)
                return null;

            audioSource.clip.LoadAudioData();
            audioSource.loop = loop;
            audioSource.volume = soundVolume * volume;
            SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, damp, (f) =>
            {
                audioSource.volume = f * soundVolume * volume;
            });
            float oTimeScale = Time.timeScale;
            Time.timeScale = 1;
            audioSource.Play();
            if (!loop)
                SKUtils.InvokeAction(audioSource.clip.length, () => {
                    Destroy(audioSource.gameObject);
                    SKUtils.RemoveKeyInDictionary(soundSources, audioSource);
                });
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public void StopIdentifiableSound(string id, float dampTime = 0.15f)
        {
            if (!audioSourceDict.ContainsKey(id))
                return;
            AudioSource audioSource = audioSourceDict[id];
            SKUtils.RemoveKeyInDictionary(audioSourceDict, id);
            SKUtils.RemoveKeyInDictionary(soundSources, audioSource);

            float oVolume = audioSource.volume;
            SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, dampTime, (f) =>
            {
                audioSource.volume = oVolume * (1 - f) * soundVolume;
            }, (f) =>
            {
                 Destroy(audioSource.gameObject);
            });
        }

        public AudioSource PlayIdentifiableMusic(string fileName, string id, bool loop = false, float volume = 1, float damp = 0.5f)
        {
            if (audioSourceDict.ContainsKey(id))
            {
                return null;
            }

            AudioSource audioSource = new GameObject("Audio").AddComponent<AudioSource>();
            audioSource.transform.SetParent(root);
            musicSources.Add(audioSource, volume);
            audioSourceDict.Add(id, audioSource);

            audioSource.clip = GetAudioClip(fileName, 1);
            if (!audioSource.clip)
                return null;

            audioSource.clip.LoadAudioData();
            audioSource.loop = loop;
            audioSource.volume = musicVolume * volume;
            SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, damp, (f) =>
            {
                audioSource.volume = f * musicVolume * volume;
            });
            float oTimeScale = Time.timeScale;
            Time.timeScale = 1;
            audioSource.Play();
            if (!loop)
                SKUtils.InvokeAction(audioSource.clip.length, () => {
                    Destroy(audioSource.gameObject);
                    SKUtils.RemoveKeyInDictionary(musicSources, audioSource);
                });
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public void StopIdentifiableMusic(string id, float dampTime = 0.15f)
        {
            if (!audioSourceDict.ContainsKey(id))
                return;
            AudioSource audioSource = audioSourceDict[id];
            SKUtils.RemoveKeyInDictionary(audioSourceDict, id);
            SKUtils.RemoveKeyInDictionary(musicSources, audioSource);

            float oVolume = audioSource.volume;
            SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, dampTime, (f) =>
            {
                if(audioSource)
                audioSource.volume = oVolume * (1 - f) * musicVolume;
            }, (f) =>
            {
                Destroy(audioSource.gameObject);
            });
        }

        private AudioClip GetAudioClip(string id, int Type)
        {
            if (!audioClipDict.ContainsKey(id))
            {
                string path = null;
                switch (Type)
                {
                    case 0:
                        path = MUSIC_PATH;
                        break;
                    case 1:
                        path = SOUND_PATH;
                        break;
                }
                AudioClip ac = Resources.Load(path + id) as AudioClip;

                if (ac == null)
                {
                    SKUtils.EditorLogError(path + id + "Load audio clip failed!");
                }
                audioClipDict.Add(id, ac);
            }
            return audioClipDict[id];
        }

        public void SetSoundVolume(float vol01)
        {
            soundVolume = vol01;
            foreach (var item in soundSources)
            {
                item.Key.volume = item.Value * soundVolume;
            }
        }

        public void SetMusicVolume(float vol01)
        {
            musicVolume = vol01;
            foreach (var item in musicSources)
            {
                item.Key.volume = item.Value * musicVolume;
            }
        }

    }


}