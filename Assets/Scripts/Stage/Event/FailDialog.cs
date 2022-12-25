using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FailDialog : MonoBehaviour
{
    [SerializeField, TextArea(1,4)] string[] flavors;
    [SerializeField] TextMeshProUGUI flavorText;

    [SerializeField] CustomButton nextButton;

    public void initialize(){
        // ランダムでフレーバーテキストを選択
        int choice = Random.Range(0, flavors.Length);
        Debug.Log($"choice: {choice}");
        flavorText.text = flavors[choice];

        // ボタン
        nextButton.onClickCallback = () => {
            Debug.Log("戻る");
        };
    }
}
