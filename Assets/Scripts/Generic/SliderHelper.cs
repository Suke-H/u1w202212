using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour, IPointerUpHandler
{
    Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();

        if (slider.name == "BGMSlider") { 
            slider.SetValueWithoutNotify(BGMController.instance.getVolume());
            slider.onValueChanged.AddListener(BGMController.instance.SoundSliderOnValueChange);
        }
        else { 
            slider.SetValueWithoutNotify(SEController.instance.getVolume()); 
            slider.onValueChanged.AddListener(SEController.instance.SoundSliderOnValueChange);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        SEController.instance.playSE("whistle");
    }
}