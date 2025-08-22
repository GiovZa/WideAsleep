using UnityEngine;
using UnityEngine.Audio;
using SoundSystem;
using System.Collections.Generic;

[System.Serializable]
public class NoisySurfaceSound
{
    public SurfaceType surfaceType;
    public AudioClip[] audioClips;
    [Range(0, 2)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("General Settings")]
    [SerializeField] private float soundRadiusMultiplier = 5f;

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
    }

    public void Play(AudioClip clip, Vector3 position, float volume = 1f, bool spatial = true, AudioMixerGroup mixerGroup = null)
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

        float soundRadius = volume * soundRadiusMultiplier;
        SoundEvents.EmitSound(position, soundRadius);        
    }

    public void PlayNoisySurfaceSound(SurfaceType surfaceType, Vector3 position)
    {
        if (surfaceSoundDictionary.TryGetValue(surfaceType, out NoisySurfaceSound surfaceSound))
        {
            if (surfaceSound.audioClips != null && surfaceSound.audioClips.Length > 0)
            {
                // Pick a random clip
                AudioClip clipToPlay = surfaceSound.audioClips[Random.Range(0, surfaceSound.audioClips.Length)];
                
                // Play it using the existing Play method
                Play(clipToPlay, position, surfaceSound.volume, true);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] No sound clips configured for surface type: {surfaceType}");
        }
    }
}
