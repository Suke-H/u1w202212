using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour, IPointerUpHandler
{
    Slider slider;
    private BGMController BGM;
    private SEController SE;

    private void Awake() {
        slider = GetComponent<Slider>();
        BGM = GameObject.Find("BGM").GetComponent<BGMController>();
        SE = GameObject.Find("SE").GetComponent<SEController>();

        // if (slider.name == "BGMSlider") { slider.value = BGM.getVolume(); }
        // else { slider.value = SE.getVolume(); }

        if (slider.name == "BGMSlider") { 
            slider.SetValueWithoutNotify(BGM.getVolume());
            slider.onValueChanged.AddListener(BGM.SoundSliderOnValueChange);
        }
        else { 
            slider.SetValueWithoutNotify(SE.getVolume()); 
            slider.onValueChanged.AddListener(SE.SoundSliderOnValueChange);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        SE.playSE("quote");
    }
}