using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class ClearDialog : MonoBehaviour
{
    [SerializeField] CustomButton yesButton;
    [SerializeField] CustomButton noButton;

    bool selectFlag;
    public bool nextFlag {get; set;}

    public void initialize(){
        // ボタン
        yesButton.onClickCallback = () => {
            nextFlag = true;
            selectFlag = true;
        };

        if (noButton != null){
            noButton.onClickCallback = () => {
                nextFlag = false;
                selectFlag = true;
                };
        }

        selectFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (selectFlag));
    }

}
