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

    public string type { get; set; }
    public int number { get; set; } = 0;
    public int teamNo { get; set; } // -1は現チーム 0,1,2..は次チーム

    ColorPallet pallet = new ColorPallet();
    private Color defaultColor;

    CustomButton plusSpec;
    CustomButton minusSpec;

    TeamManager teamManager;

    void Start(){
        teamManager =  GameObject.Find("TeamManager").GetComponent<TeamManager>();
    }

    public void plusMembers(){
        this.number++;
        numberText.text = this.number.ToString();
    }

    public void minusMembers(){
        this.number--;
        numberText.text = this.number.ToString();
    }

    public void initialize(string color, string type, int number, int teamNo){
        // 色
        if (color == "blue"){
            frame.color = pallet.blue;
            typeText.color = pallet.blue;
            numberText.color = pallet.blue;
        }

        else if (color == "red"){
            frame.color = pallet.red;
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

        // チームNo
        this.teamNo = teamNo;

        // 次ノードの場合
        plusSpec = plusButton.GetComponent<CustomButton>();
        minusSpec = minusButton.GetComponent<CustomButton>();

        if (teamNo != -1){
            plusSpec.onClickCallback = () => {
                // number++;
                // numberText.text = number.ToString();
                // teamManager.buttonFunc(this.teamNo, this.type, "plus");
                
                bool result = teamManager.buttonFunc(this.teamNo, this.type, "plus");
                numberText.text = this.number.ToString();
                // if (result){
                //     plusMembers();
                // }
            };

            minusSpec.onClickCallback = () => {
                // if (number > 0){ 
                //     number--; 
                //     numberText.text = number.ToString();
                //     teamManager.buttonFunc(this.teamNo, this.type, "minus");
                // }

                bool result = teamManager.buttonFunc(this.teamNo, this.type, "minus");
                numberText.text = this.number.ToString();
                // if (result){
                //     minusMembers();
                // }
            };
        }

        else {
            plusButton.SetActive(false);
            minusButton.SetActive(false);
        }
    }

}
