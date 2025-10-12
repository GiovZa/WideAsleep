using UnityEngine;
using UnityEngine.Audio;
using SoundSystem;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class NoisySurfaceSound
{
    public SurfaceType surfaceType;
    public AudioClip[] audioClips;
    [Range(0, 2)]
    public float volume = 1f;
    [Range(0, 30)]
    public float soundRadius = 5f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup SFXMixerGroup;
    public AudioMixerGroup UIMixerGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float crossfadeDuration = 1.0f;

    [Header("Noisy Surface Sounds")]
    public List<NoisySurfaceSound> noisySurfaceSounds;
    private Dictionary<SurfaceType, NoisySurfaceSound> surfaceSoundDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize the dictionary for fast lookups
        surfaceSoundDictionary = new Dictionary<SurfaceType, NoisySurfaceSound>();
        foreach (var sound in noisySurfaceSounds)
        {
            if (sound != null && !surfaceSoundDictionary.ContainsKey(sound.surfaceType))
            {
                surfaceSoundDictionary.Add(sound.surfaceType, sound);
            }
        }

        // Ensure there is a BGM source
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f; // 2D sound
        }
    }

    #region BGM Management
    public void PlayBGM(AudioClip clip, bool loop = true, bool crossfade = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] BGM clip is null.");
            return;
        }

        if (bgmSource.isPlaying && bgmSource.clip == clip)
        {
            // The requested BGM is already playing.
            return;
        }

        bgmSource.loop = loop;

        if (crossfade && bgmSource.isPlaying)
        {
            StartCoroutine(CrossfadeBGM(clip));
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    private IEnumerator CrossfadeBGM(AudioClip newClip)
    {
        float timer = 0f;
        float startVolume = bgmSource.volume;

        // Fade out
        while (timer < crossfadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, timer / crossfadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        bgmSource.Stop();

        // Start new clip
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        timer = 0f;
        while (timer < crossfadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, timer / crossfadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        bgmSource.volume = startVolume; // Ensure it ends at the correct volume
    }
    #endregion

    #region Play sounds for both Player and AI
    /// <summary>
    /// Plays a sound that can be heard by both the player and the AI.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The world position to play the sound at.</param>
    /// <param name="volume">The audible volume for the player.</param>
    /// <param name="soundRadius">The radius for AI detection, normally it would be 5m.</param>
    /// <param name="spatial">Whether the sound is 3D.</param>
    /// <param name="mixerGroup">The audio mixer group to use.</param>
    public void Play(AudioClip clip, Vector3 position, float volume = 1f, float soundRadius = 5f, bool spatial = true, AudioMixerGroup mixerGroup = null)
    {
        if (clip == null) return;

        // Play the sound for the player
        PlaySoundForPlayerOnly(clip, position, volume, spatial, mixerGroup);

        // Emit the sound event for the AI
        SoundEvents.EmitSound(position, soundRadius);
    }

    public void PlayRandomSound(AudioClip[] clips, Vector3 position, float volume = 1f, float soundRadius = 5f, bool spatial = true, AudioMixerGroup mixerGroup = null)
    {
        int rand = Random.Range(0, clips.Length);
        Play(clips[rand], position, volume, soundRadius, spatial, mixerGroup);
    }

    #endregion

    #region Play sounds for Player only
    /// <summary>
    /// Plays a sound that can only be heard by the player, not the AI.
    /// </summary>
    public void PlaySoundForPlayerOnly(AudioClip clip, Vector3 position, float volume = 1f, bool spatial = true, AudioMixerGroup mixerGroup = null)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatial ? 1f : 0f;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        Destroy(tempGO, clip.length);
    }

    public void PlayRandomSoundForPlayerOnly(AudioClip[] clips, Vector3 position, float volume = 1f, bool spatial = true, AudioMixerGroup mixerGroup = null)
    {
        if (clips == null || clips.Length == 0) return;
        int rand = Random.Range(0, clips.Length);

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clips[rand];
        source.volume = volume;
        source.spatialBlend = spatial ? 1f : 0f;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        Destroy(tempGO, clips[rand].length);
    }
    #endregion

    public void PlayNoisySurfaceSound(NoisySurface surface, Vector3 position)
    {
        if (surface == null) return;

        if (surfaceSoundDictionary.TryGetValue(surface.surfaceType, out NoisySurfaceSound surfaceSound))
        {
            if (surfaceSound.audioClips != null && surfaceSound.audioClips.Length > 0)
            {
                // Pick a random clip from the dictionary
                AudioClip clipToPlay = surfaceSound.audioClips[Random.Range(0, surfaceSound.audioClips.Length)];

                // Check if we should use the override values from the surface component
                float volume = surface.overrideDefaults ? surface.volume : surfaceSound.volume;
                float radius = surface.overrideDefaults ? surface.soundRadius : surfaceSound.soundRadius;

                // Play it using the existing Play method with the determined values
                Play(clipToPlay, position, volume, radius, true, SFXMixerGroup);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] No sound clips configured for surface type: {surface.surfaceType}");
        }
    }
}
