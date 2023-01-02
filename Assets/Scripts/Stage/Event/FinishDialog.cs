using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FinishDialog : MonoBehaviour
{
    [SerializeField] CustomButton yesButton;
    [SerializeField] TextMeshProUGUI flavorText;

    public bool endFlag {get; set;} = false;

    public void initialize(string textType){
        // ボタン
        yesButton.onClickCallback = () => {
            endFlag = true;
        };

        endFlag = false;

        if (textType == "Tutorial"){ flavorText.text = "研修終了！"; }
        else if (textType == "GiveUp"){ flavorText.text = "定時で上がります！"; }
        else if (textType == "AllClear"){ flavorText.text = "お疲れ様でした！"; }
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

}
