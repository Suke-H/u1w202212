using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FinishDialog : MonoBehaviour
{
    [SerializeField] CustomButton yesButton;
    // [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] Image ClearText;
    [SerializeField] Sprite[] TextContents;

    public bool endFlag {get; set;} = false;

    public void initialize(string textType){
        // ボタン
        yesButton.onClickCallback = () => {
            endFlag = true;
        };

        endFlag = false;

        // if (textType == "Tutorial"){ flavorText.text = "研修終了！"; }
        // else if (textType == "GiveUp"){ flavorText.text = "定時で上がります！"; }
        // else if (textType == "AllClear"){ flavorText.text = "お疲れ様でした！"; }

        if (textType == "Tutorial"){ ClearText.sprite = TextContents[0]; }
        else if (textType == "GiveUp"){ ClearText.sprite = TextContents[1]; }
        else if (textType == "AllClear"){ ClearText.sprite = TextContents[2]; }
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

}
