using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FinishDialog : MonoBehaviour
{
    [SerializeField] CustomButton yesButton;

    public bool endFlag {get; set;} = false;

    public void initialize(){
        // ボタン
        yesButton.onClickCallback = () => {
            endFlag = true;
        };

        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

}
