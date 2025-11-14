using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [Header("Vision Sense")]
        [SerializeField] private Image visionSenseKeyImage;
        [SerializeField] private Sprite visionSenseKeySpriteTriggered;
        [SerializeField] private Sprite visionSenseKeySpriteUntriggered;
        [Header("Hearing Sense")]
        [SerializeField] private Image hearingSenseKeyImage;
        [SerializeField] private Sprite hearingSenseKeySpriteTriggered;
        [SerializeField] private Sprite hearingSenseKeySpriteUntriggered;
        [Header("Throwable")]
        [SerializeField] private Image throwImage;
        [SerializeField] private Sprite throwSpriteTriggered;
        [SerializeField] private Sprite throwSpriteUntriggered;
        
        private void Awake()
        {
            // if (visionSenseKeyImage == null) 
            // {
            //     Debug.LogError("VisionSenseKeyImage is not assigned");
            //     return;
            // }
            // if (hearingSenseKeyImage == null) 
            // {
            //     Debug.LogError("HearingSenseKeyImage is not assigned");
            //     return;
            // }
            // if (throwImage == null) 
            // {
            //     Debug.LogError("ThrowImage is not assigned");
            //     return;
            // }

            //UpdateVisionSenseKeyImage(false);
            //UpdateHearingSenseKeyImage(false);
            UpdateThrowImage(false);
        }

        private void OnEnable()
        {
            EventCenter.AddListener(EventDefine.VISION_SENSE_TRIGGERED, UpdateVisionSenseKeyImage);
            EventCenter.AddListener<float, float>(EventDefine.HEARING_SENSE_ACTIVATED, UpdateHearingSenseKeyImage);
            ObjectThrower.OnThrowTriggered += UpdateThrowImage;
        }

        private void OnDisable()
        {
            EventCenter.RemoveListener(EventDefine.VISION_SENSE_TRIGGERED, UpdateVisionSenseKeyImage);
            EventCenter.RemoveListener<float, float>(EventDefine.HEARING_SENSE_ACTIVATED, UpdateHearingSenseKeyImage);
            ObjectThrower.OnThrowTriggered -= UpdateThrowImage;
        }

        public void UpdateVisionSenseKeyImage()
        {
            if (visionSenseKeyImage == null) return;
            
            visionSenseKeyImage.sprite = visionSenseKeySpriteTriggered;
            StartCoroutine(SwitchBackToUntriggered(visionSenseKeyImage, visionSenseKeySpriteUntriggered, 0.5f));
        }

        public void UpdateHearingSenseKeyImage(float targetBGMVolumePercent, float fadeTime)
        {
            if (hearingSenseKeyImage == null) return;
            
            hearingSenseKeyImage.sprite = hearingSenseKeySpriteTriggered;
            StartCoroutine(SwitchBackToUntriggered(hearingSenseKeyImage, hearingSenseKeySpriteUntriggered, 0.5f));
        }

        public void UpdateThrowImage(bool isTriggered)
        {
            if (throwImage == null) return;
            
            if (isTriggered)
            {
                throwImage.sprite = throwSpriteTriggered;
                StartCoroutine(SwitchBackToUntriggered(throwImage, throwSpriteUntriggered, 0.5f));
            }
            else
            {
                throwImage.sprite = throwSpriteUntriggered;
            }
        }

        private IEnumerator SwitchBackToUntriggered(Image image, Sprite spriteUntriggered, float delay)
        {
            yield return new WaitForSeconds(delay);
            image.sprite = spriteUntriggered;
        }
    }
}
