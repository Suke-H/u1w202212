using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemberState : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CustomButton plusButton;
    [SerializeField] CustomButton minusButton;

    [SerializeField] Image frame;
    [SerializeField] TextMeshProUGUI kakeru;

    public string type { get; protected set; }
    public int number { get; protected set; } = 0;

    ColorPallet pallet = new ColorPallet();
    private Color defaultColor;

    void Start(){
        plusButton.onClickCallback = () => {
            number++;
            numberText.text = number.ToString();
        };

        minusButton.onClickCallback = () => {
            if (number > 0){ 
                number--; 
                numberText.text = number.ToString();
            }
        };

        // defaultColor = pallet.red;
    }

    public void initialize(string color="blue", string type="engineer", int number=0){

        kakeru.text = "×";

        // 色
        if (color=="blue"){
            frame.color = pallet.blueFloat;
            kakeru.color = pallet.blueInt;
            typeText.color = pallet.blueInt;
        }

        else if (color=="red"){
            frame.color = pallet.redFloat;
            // kakeru.color = pallet.redInt;
            // typeText.color = pallet.redInt;
            kakeru.color = pallet.redFloat;
            typeText.color = pallet.redFloat;
        }


        // 職種
        this.type = type;
        if (this.type == "engineer"){
            typeText.text = "エンジニア";
        }
        else if (this.type == "sales"){
            typeText.text = "営業";
            Debug.Log("は？？");
        }

        // 数
        this.number = number;
        numberText.text = this.number.ToString();
    }

    // public void Update(){

    // }
}
