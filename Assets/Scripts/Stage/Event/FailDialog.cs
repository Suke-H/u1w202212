using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class FailDialog : MonoBehaviour
{
    [SerializeField, TextArea(1,4)] string[] flavors;
    [SerializeField] TextMeshProUGUI flavorText;

    [SerializeField] CustomButton nextButton;

    bool endFlag;

    public void initialize(){
        // ランダムでフレーバーテキストを選択
        int choice = Random.Range(0, flavors.Length);
        Debug.Log($"choice: {choice}");
        flavorText.text = flavors[choice];

        // ボタン
        nextButton.onClickCallback = () => {
            endFlag = true;
        };

        // フラグ
        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }
}
