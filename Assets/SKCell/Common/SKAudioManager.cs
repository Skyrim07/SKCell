using UnityEngine;
using System.Collections.Generic;
using System;

namespace SKCell
{
    [AddComponentMenu("SKCell/Misc/SKAudioManager")]
    public class SKAudioManager : SKMonoSingleton<SKAudioManager>
    {
        public AudioSource musicAudioSource;

        private List<AudioSource> unusedSoundAudioSourceList;   
        private List<AudioSource> usedSoundAudioSourceList;     
        private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();      
        private Dictionary<string, AudioSource> audioSourceDict = new Dictionary<string, AudioSource>();      

        private float musicVolume = 1;
        private float soundVolume = 1;

        private string musicVolumePrefs = "MusicVolume";
        private string soundVolumePrefs = "SoundVolume";

        private string MUSIC_PATH = "AudioClip/Music/";
        private string SOUND_PATH = "AudioClip/Sound/";
        private string BGM_PATH = "AudioClip/BGM/";

        public AudioSource audioSource = new AudioSource();

        public List<string> music_ids = new List<string>();
        public List<string> BGMs = new List<string>();

        int BGMhasPlayed = 0;

        protected override void Awake()
        {
            base.Awake();
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            unusedSoundAudioSourceList = new List<AudioSource>();
            usedSoundAudioSourceList = new List<AudioSource>();
        }

        protected void Start()
        {
            //if (PlayerPrefs.HasKey(musicVolumePrefs))
            //{
            //    musicVolume = PlayerPrefs.GetFloat(musicVolumePrefs);
            //}
            //if (PlayerPrefs.HasKey(soundVolumePrefs))
            //{
            //    musicVolume = PlayerPrefs.GetFloat(soundVolumePrefs);
            //}
        }

        public void Initialize()
        {
            LoadClips();
        }

        public AudioSource PlayMusic(string id, bool loop = true, int type = 2, float volume = 1f)
        {
            musicAudioSource.clip = GetAudioClip(id, type);
            musicAudioSource.clip.LoadAudioData();
            musicAudioSource.loop = loop;
            musicAudioSource.Play();
            SKUtils.StartProcedure(SKCurve.LinearIn, 0.5f, (f) =>
            {
                musicAudioSource.volume = f * musicVolume;
            });
            return musicAudioSource;
        }
        public AudioSource StopMusic()
        {
            float oVolume = musicAudioSource.volume;
            SKUtils.StartProcedure(SKCurve.LinearIn, 0.5f, (f) =>
            {
                musicAudioSource.volume = oVolume * (1 - f);
            });
            return musicAudioSource;
        }

        public void ChangeMusic(bool change_bgm = false)
        {
            if (musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
                BGMhasPlayed = (BGMhasPlayed + 1) % BGMs.Count;
                PlayMusic(BGMs[BGMhasPlayed]);
            }
            else if (change_bgm == true)
            {
                BGMhasPlayed = (BGMhasPlayed + 1) % BGMs.Count;
            }
            else
            {
                PlayMusic(BGMs[BGMhasPlayed]);
            }
        }

        public AudioSource PlaySound(string id, Action action = null, bool loop = false, float volume = 1f, float pitch = 1f, float damp =0f)
        {
                AddAudioSource();
                audioSource = UnusedToUsed();
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
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public AudioSource PlayIdentifiableSound(string fileName, string id, bool loop = false, float volume = 1, float damp = 0.5f)
        {
            if (audioSourceDict.ContainsKey(id))
            {
                return null;
                //StopIdentifiableSound(id);
            }

            AudioSource audioSource = AddIdentifiableAudioSource(id);
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
            audioSource.Play();
            float oTimeScale = Time.timeScale;
            Time.timeScale = 1;
            audioSource.Play();
            Time.timeScale = oTimeScale;
            return audioSource;
        }

        public void StopIdentifiableSound(string id, float dampTime = 0.15f)
        {
            RemoveIdentifiableAudioSource(id, dampTime);
        }

        public void StopSound()
        {
            SKUtils.EditorLogNormal(audioSource.GetInstanceID());

            if (audioSource != null)
            {
                audioSource.Stop();
            }
            
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
                    case 2:
                        path = BGM_PATH;
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

        private void LoadClips()
        {
            var keys = SKCSVReader.instance.CollectKey1("AudioClip");
            foreach (var key in keys)
            {
                int type = SKCSVReader.instance.GetInt("AudioClip", key.ToString(), "Type");
                if (type == 2)
                {
                    GetAudioClip(key, type);
                    BGMs.Add(key);
                }
            }
            GetMusic_list(keys);
            foreach (var music_id in music_ids)
            {
                GetAudioClip(music_id, 0);
            }
        }

        private void GetMusic_list(List<string> keys)
        {
            List<string> music_list = new List<string>();
            foreach (var key in keys)
            {
                int type = SKCSVReader.instance.GetInt("AudioClip", key.ToString(), "Type");
                if (type == 0)
                {
                    music_list.Add(key);
                }
            }
            int count = music_list.Count;
            for (int i = 0; i < count; i++)
            {
                int ran = UnityEngine.Random.Range(0, music_list.Count - 1);
                music_ids.Add(music_list[ran]);
                music_list.RemoveAt(ran);
            }
        }

        private AudioSource AddAudioSource()
        {
            if (unusedSoundAudioSourceList.Count != 0)
            {
                return UnusedToUsed();
            }
            else
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                unusedSoundAudioSourceList.Add(audioSource);
                return audioSource;
            }
        }

        private AudioSource AddIdentifiableAudioSource(string id)
        {
            GameObject go = new GameObject("AudioAgent");
            go.transform.SetParent(transform);
            AudioSource audioSource = go.AddComponent<AudioSource>();
            SKUtils.InsertOrUpdateKeyValueInDictionary(audioSourceDict, id, audioSource);

            return audioSource;
        }

        private void RemoveIdentifiableAudioSource(string id, float dampenTime = 0.15f)
        {
            if (!audioSourceDict.ContainsKey(id))
                return;
            AudioSource audioSource = audioSourceDict[id];
            SKUtils.RemoveKeyInDictionary(audioSourceDict, id);

            if (audioSource == null)
                return;

            float oVolume = audioSource.volume;
            SKUtils.StartProcedureUnscaled(SKCurve.LinearIn, dampenTime, (f) =>
            {
                audioSource.volume = oVolume * (1 - f);
            }, (f) =>
            {
               // Destroy(audioSource.gameObject);
            });
        }


        private AudioSource UnusedToUsed()
        {
            AudioSource audioSource = unusedSoundAudioSourceList[0];
            unusedSoundAudioSourceList.RemoveAt(0);
            usedSoundAudioSourceList.Add(audioSource);
            return audioSource;
        }

        public void ChangeMusicVolume(float volume)
        {
            musicVolume = volume;
            musicAudioSource.volume = volume;

            PlayerPrefs.SetFloat(musicVolumePrefs, volume);
        }
        public void ChangeSoundVolume(float volume)
        {
            soundVolume = volume;
            for (int i = 0; i < unusedSoundAudioSourceList.Count; i++)
            {
                unusedSoundAudioSourceList[i].volume = volume;
            }
            for (int i = 0; i < usedSoundAudioSourceList.Count; i++)
            {
                usedSoundAudioSourceList[i].volume = volume;
            }

            PlayerPrefs.SetFloat(soundVolumePrefs, volume);
        }
    }
}