using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour, IPointerUpHandler
{
    Slider slider;
    // private BGMController BGM;
    // private SEController SE;

    private void Awake() {
        slider = GetComponent<Slider>();
        // BGM = GameObject.Find("BGM").GetComponent<BGMController>();
        // SE = GameObject.Find("SE").GetComponent<SEController>();

        if (slider.name == "BGMSlider") { 
            // slider.SetValueWithoutNotify(BGM.getVolume());
            // slider.onValueChanged.AddListener(BGM.SoundSliderOnValueChange);
            slider.SetValueWithoutNotify(BGMController.instance.getVolume());
            slider.onValueChanged.AddListener(BGMController.instance.SoundSliderOnValueChange);
        }
        else { 
            // slider.SetValueWithoutNotify(SE.getVolume()); 
            // slider.onValueChanged.AddListener(SE.SoundSliderOnValueChange);
            slider.SetValueWithoutNotify(SEController.instance.getVolume()); 
            slider.onValueChanged.AddListener(SEController.instance.SoundSliderOnValueChange);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        // SE.playSE("button");
        SEController.instance.playSE("button");
    }
}