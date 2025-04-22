using UnityEngine;
using UnityEngine.Audio;
using SoundSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

        float soundRadius = volume * 5f; // tweak this multiplier
        SoundEvents.EmitSound(position, soundRadius);        
    }
}
