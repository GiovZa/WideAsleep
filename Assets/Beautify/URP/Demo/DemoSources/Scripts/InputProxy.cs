using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Beautify.Demos {

    public static class InputProxy {

        public static bool GetKeyDown(KeyCode keyCode) {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return false;
            switch (keyCode) {
                case KeyCode.J: return keyboard.jKey.wasPressedThisFrame;
                case KeyCode.T: return keyboard.tKey.wasPressedThisFrame;
                case KeyCode.B: return keyboard.bKey.wasPressedThisFrame;
                case KeyCode.C: return keyboard.cKey.wasPressedThisFrame;
                case KeyCode.N: return keyboard.nKey.wasPressedThisFrame;
                case KeyCode.F: return keyboard.fKey.wasPressedThisFrame;

                case KeyCode.Alpha1: return keyboard.digit1Key.wasPressedThisFrame;
                case KeyCode.Alpha2: return keyboard.digit2Key.wasPressedThisFrame;
                case KeyCode.Alpha3: return keyboard.digit3Key.wasPressedThisFrame;
                case KeyCode.Alpha4: return keyboard.digit4Key.wasPressedThisFrame;
                case KeyCode.Alpha5: return keyboard.digit5Key.wasPressedThisFrame;
                case KeyCode.Alpha6: return keyboard.digit6Key.wasPressedThisFrame;
                case KeyCode.Alpha7: return keyboard.digit7Key.wasPressedThisFrame;
                case KeyCode.Alpha8: return keyboard.digit8Key.wasPressedThisFrame;
                case KeyCode.Alpha9: return keyboard.digit9Key.wasPressedThisFrame;
                case KeyCode.Alpha0: return keyboard.digit0Key.wasPressedThisFrame;
            }
            return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return UnityEngine.Input.GetKeyDown(keyCode);
#else
            return false;
#endif
        }

        public static bool GetMouseButtonDown(int button) {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            if (mouse == null) return false;
            switch (button) {
                case 0: return mouse.leftButton.wasPressedThisFrame;
                case 1: return mouse.rightButton.wasPressedThisFrame;
                case 2: return mouse.middleButton.wasPressedThisFrame;
                default: return false;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            return UnityEngine.Input.GetMouseButtonDown(button);
#else
            return false;
#endif
        }
    }
}


