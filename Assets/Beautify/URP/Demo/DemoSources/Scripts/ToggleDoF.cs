using UnityEngine;
using Beautify.Universal;
using Input = Beautify.Demos.InputProxy;

namespace Beautify.Demos {

    public class ToggleDoF : MonoBehaviour {

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                // toggles DoF state by overriding the volume property
                bool state = BeautifySettings.settings.depthOfField.value;
                BeautifySettings.settings.depthOfField.Override(!state);
            }
        }

    }
}
