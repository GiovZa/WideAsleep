using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public float volume = 1f;
    public bool loop = false;
    public bool spatial = true;
}
