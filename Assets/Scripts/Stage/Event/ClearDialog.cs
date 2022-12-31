using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class ClearDialog : MonoBehaviour
{
    [SerializeField] CustomButton yesButton;
    [SerializeField] CustomButton noButton;

    bool endFlag;

    public void initialize(){
        // ボタン
        yesButton.onClickCallback = () => {
        };

        noButton.onClickCallback = () => {
        };

        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

}
