using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const float FADE_DURATION_SCENE_TRANSITION = 0.5f;

        private static string settingsPath;
        private static SoundsSettings soundsSettings;

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        private List<AudioClip> musicSounds;
        private int musicIndex;
        private bool musicIsPaused;

        public event Action<AudioClip> WhenMusicChanged;

        public AudioMixer Mixer => mixer;
        public bool MusicIsPaused => musicIsPaused;
        public AudioClip CurrentMusic => musicSource.clip;
        public float GetMusicTime() => musicSource.time;
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
                musicSounds = Resources.LoadAll<AudioClip>("Audio/Music").ToList();
                settingsPath = System.IO.Path.Join(Application.persistentDataPath, "soundsSettings.xml");
                LoadSettingsOrCreatDefault();
            }
        }

        private void Start()
        {
            WriteSoundSettings();
        }

        private void OnEnable()
        {
            SceneTransition.StartLoad += OnStartLoadSceneTransition;
            SceneTransition.EndLoad += OnEndLoadSceneTransition;
            SceneTransition.StartUnload += OnStartUnloadSceneTransition;
            SceneTransition.EndUnload += OnEndUnloadSceneTransition;
        }

        private void OnDisable()
        {
            SceneTransition.StartLoad -= OnStartLoadSceneTransition;
            SceneTransition.EndLoad -= OnEndLoadSceneTransition;
            SceneTransition.StartUnload -= OnStartUnloadSceneTransition;
            SceneTransition.EndUnload -= OnEndUnloadSceneTransition;
        }

        private void Update()
        {
            if (!musicSource.isPlaying && !musicIsPaused && musicSounds.Count > 0)
            {
                musicIndex = musicSource.clip == null ? 0 : (musicSounds.Count + musicIndex + 1) % musicSounds.Count;
                PlayMusic(musicIndex);
            }

            UpdateSoundSettings();
        }

        private void OnDestroy()
        {
            SaveSettings();
        }

        public void PlayOneShotSFX(AudioClip audioClip)
        {
            sfxSource.PlayOneShot(audioClip);
        }

        public void PlaySFX(AudioClip audioClip)
        {
            sfxSource.Stop();
            sfxSource.clip = audioClip;
            sfxSource.Play();
        }

        public void NextMusic(float transitionDuration = 0)
        {
            StartCoroutine(SmoothSwitchingMusic((musicSounds.Count + musicIndex + 1) % musicSounds.Count,
                transitionDuration));
        }

        public void PreviousMusic(float transitionDuration = 0)
        {
            StartCoroutine(SmoothSwitchingMusic((musicSounds.Count + musicIndex - 1) % musicSounds.Count,
                transitionDuration));
        }

        public void PauseMusic()
        {
            musicIsPaused = true;
            musicSource.Pause();
        }

        public void UnPauseMusic()
        {
            musicIsPaused = false;
            musicSource.UnPause();
        }

        public void SetMusicTime(float time)
        {
            musicSource.time = Mathf.Clamp(time, 0, musicSource.clip.length - 0.01f);
        }

        private void PlayMusic(int index)
        {
            ResetSource(musicSource);
            musicSource.clip = musicSounds[index];
            WhenMusicChanged?.Invoke(musicSource.clip);
            musicSource.Play();
            musicIsPaused = false;
        }

        private void LoadSettingsOrCreatDefault()
        {
            try
            {
                soundsSettings = File.Exists(settingsPath)
                    ? Serializer.ReadObject<SoundsSettings>(settingsPath)
                    : GetDefaultSoundsSettings();
            }
            catch (Exception)
            {
                soundsSettings = GetDefaultSoundsSettings();
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            if (soundsSettings != null)
            {
                Serializer.WriteObject(settingsPath, soundsSettings);
            }
        }

        private void UpdateSoundSettings()
        {
            mixer.GetFloat("MasterVolume", out var masterVolume);
            mixer.GetFloat("MusicVolume", out var musicVolume);
            mixer.GetFloat("SFXVolume", out var sfxVolume);
            soundsSettings.masterVolume = Mathf.Pow(10, masterVolume / 20);
            soundsSettings.musicVolume = Mathf.Pow(10, musicVolume / 20);
            soundsSettings.sfxVolume = Mathf.Pow(10, sfxVolume / 20);
            soundsSettings.musicIndex = musicIndex;
            soundsSettings.musicTime = musicSource.time;
            soundsSettings.musicIsPaused = musicIsPaused;
        }

        private void WriteSoundSettings()
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(soundsSettings.masterVolume) * 20);
            mixer.SetFloat("MusicVolume", Mathf.Log10(soundsSettings.musicVolume) * 20);
            mixer.SetFloat("SFXVolume", Mathf.Log10(soundsSettings.sfxVolume) * 20);
            musicIndex = soundsSettings.musicIndex;
            PlayMusic(musicIndex);
            SetMusicTime(soundsSettings.musicTime);
            if (soundsSettings.musicIsPaused)
                PauseMusic();
            else
                UnPauseMusic();
        }

        private SoundsSettings GetDefaultSoundsSettings()
        {
            return new SoundsSettings()
            {
                sfxVolume = Mathf.Pow(10, -10f / 20),
                musicVolume = Mathf.Pow(10, -20f / 20),
                masterVolume = Mathf.Pow(10, 0),
                musicIndex = 0,
                musicTime = 0,
                musicIsPaused = false
            };
        }

        private void ResetSource(AudioSource audioSource)
        {
            audioSource.time = 0;
            audioSource.timeSamples = 0;
        }

        private IEnumerator StartSmoothSoundFading(AudioSource audioSource, float duration, float startVolume,
            float targetVolume)
        {
            float currentTime = 0;
            var currentVol = startVolume;
            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                yield return null;
            }
        }

        private IEnumerator SmoothSwitchingMusic(int index, float transitionDuration)
        {
            StartCoroutine(StartSmoothSoundFading(musicSource, transitionDuration, 1, 0.0001f));
            yield return new WaitForSeconds(transitionDuration);
            musicSource.Stop();
            musicIndex = index;
            PlayMusic(musicIndex);
            StartCoroutine(StartSmoothSoundFading(musicSource, transitionDuration, 0.0001f, 1));
        }

        private void SmoothPlaySound(AudioSource audioSource, AudioClip audioClip, float transitionDuration)
        {
            ResetSource(audioSource);
            audioSource.clip = audioClip;
            audioSource.Play();
            StartCoroutine(StartSmoothSoundFading(audioSource, transitionDuration, 0.0001f, 1));
        }

        private IEnumerator SmoothStopSound(AudioSource audioSource, float transitionDuration)
        {
            StartCoroutine(StartSmoothSoundFading(audioSource, transitionDuration, 1, 0.0001f));
            yield return new WaitForSeconds(transitionDuration);
            audioSource.Stop();
        }

        private void OnStartLoadSceneTransition()
        {
            StartCoroutine(StartLoadSceneTransition());
        }

        private IEnumerator StartLoadSceneTransition()
        {
            StartCoroutine(StartSmoothSoundFading(musicSource, FADE_DURATION_SCENE_TRANSITION, 1, 0.0001f));
            yield return new WaitForSeconds(FADE_DURATION_SCENE_TRANSITION);
            PauseMusic();
            SmoothPlaySound(sfxSource, Sounds.GetUISoundEffect(UISoundEffectsType.LoadScene),
                FADE_DURATION_SCENE_TRANSITION);
        }

        private void OnEndLoadSceneTransition()
        {
            StartCoroutine(SmoothStopSound(sfxSource, FADE_DURATION_SCENE_TRANSITION));
        }

        private void OnStartUnloadSceneTransition()
        {
            SmoothPlaySound(sfxSource, Sounds.GetUISoundEffect(UISoundEffectsType.UnloadScene),
                FADE_DURATION_SCENE_TRANSITION);
        }

        private void OnEndUnloadSceneTransition()
        {
            StartCoroutine(EndUnloadSceneTransition());
        }

        private IEnumerator EndUnloadSceneTransition()
        {
            StartCoroutine(StartSmoothSoundFading(sfxSource, FADE_DURATION_SCENE_TRANSITION, 1, 0.0001f));
            yield return new WaitForSeconds(FADE_DURATION_SCENE_TRANSITION);
            sfxSource.Stop();
            ResetSource(sfxSource);
            UnPauseMusic();
            StartCoroutine(StartSmoothSoundFading(musicSource, FADE_DURATION_SCENE_TRANSITION, 0.0001f, 1));
            sfxSource.volume = 1;
        }
    }
}