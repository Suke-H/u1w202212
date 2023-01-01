using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;

public class OverDialog : MonoBehaviour
{
    [SerializeField, TextArea(1,4)] string[] flavors;
    [SerializeField] TextMeshProUGUI flavorText;

    [SerializeField] CustomButton nextButton;
    bool endFlag;

    public void initialize(){
        // ランダムでフレーバーテキストを選択
        int choice = Random.Range(0, flavors.Length);
        flavorText.text = flavors[choice];

        // ボタン
        nextButton.onClickCallback = () => {
            endFlag = true;
            SceneManager.LoadScene("Title");
        };

        // フラグ
        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }
}
