using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public static class SoundEvents
    {
        public delegate void SoundHeard(Vector3 position, float radius);
        public static event SoundHeard OnSoundPlayed;

        public static void EmitSound(Vector3 position, float radius)
        {
            OnSoundPlayed?.Invoke(position, radius);
        }
    }
}
