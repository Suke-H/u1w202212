using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemberState : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] GameObject plusButton;
    [SerializeField] GameObject minusButton;
    

    [SerializeField] Image frame;
    [SerializeField] TextMeshProUGUI kakeru;

    public string type { get; protected set; }
    public int number { get; protected set; } = 0;

    ColorPallet pallet = new ColorPallet();
    private Color defaultColor;

    CustomButton plusSpec;
    CustomButton minusSpec;

    // void Start(){
    //     plusButton.onClickCallback = () => {
    //         number++;
    //         numberText.text = number.ToString();
    //     };

    //     minusButton.onClickCallback = () => {
    //         if (number > 0){ 
    //             number--; 
    //             numberText.text = number.ToString();
    //         }
    //     };
    // }

    public void initialize(string color="blue", string type="engineer", int number=0, bool isNext=true){
        // 色
        if (color == "blue"){
            frame.color = pallet.blue;
            kakeru.color = pallet.blue;
            typeText.color = pallet.blue;
            numberText.color = pallet.blue;
        }

        else if (color == "red"){
            frame.color = pallet.red;
            kakeru.color = pallet.red;
            typeText.color = pallet.red;
            numberText.color = pallet.red;
        }

        // 職種
        this.type = type;
        if (this.type == "engineer"){ typeText.text = "エンジニア"; }
        else if (this.type == "sales"){ typeText.text = "営業"; }

        // 数
        this.number = number;
        numberText.text = this.number.ToString();

        // 次ノードの場合
        plusSpec = plusButton.GetComponent<CustomButton>();
        minusSpec = minusButton.GetComponent<CustomButton>();

        if (isNext){
            plusSpec.onClickCallback = () => {
                number++;
                numberText.text = number.ToString();
            };

            minusSpec.onClickCallback = () => {
                if (number > 0){ 
                    number--; 
                    numberText.text = number.ToString();
                }
            };
        }

        else {
            plusButton.SetActive(false);
            minusButton.SetActive(false);
        }
    }

}
