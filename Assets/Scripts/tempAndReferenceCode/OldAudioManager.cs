// using UnityEngine;

// public class AudioManager : MonoBehaviour
// {
//     public static AudioManager Instance;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     public void Play(SoundEvent soundEvent, Vector3 position)
//     {
//         GameObject tempGO = new GameObject("TempAudio");
//         tempGO.transform.position = position;
//         var source = tempGO.AddComponent<AudioSource>();
//         source.clip = soundEvent.clip;
//         source.volume = soundEvent.volume;
//         source.loop = soundEvent.loop;
//         source.spatialBlend = soundEvent.spatial ? 1f : 0f;
//         if (soundEvent.mixerGroup != null)
//             source.outputAudioMixerGroup = soundEvent.mixerGroup;
//         source.Play();
//         if (!soundEvent.loop) Destroy(tempGO, soundEvent.clip.length);
//     }
// }
