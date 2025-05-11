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

    public void Play(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        bool spatial = true,
        AudioMixerGroup mixerGroup = null,
        float playDuration = -1f         // ← new parameter: how long to play
    )
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

        // if a positive duration was passed in, clamp it to clip.length
        float destroyAfter = (playDuration > 0f)
            ? Mathf.Min(playDuration, clip.length)
            : clip.length;

        Destroy(tempGO, destroyAfter);

        float soundRadius = volume * 5f;
        SoundEvents.EmitSound(position, soundRadius);        
    }
}
