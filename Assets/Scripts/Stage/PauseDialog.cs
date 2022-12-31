using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class PauseDialog : MonoBehaviour
{
    [SerializeField] CustomButton resumeButton;
    [SerializeField] CustomButton titleBackButton;

    bool resumeFlag = false;

    // アイテム候補
    public void initialize(){
        // ボタン
        resumeButton.onClickCallback = () => {
            resumeFlag = true;
        };

        titleBackButton.onClickCallback = () => {
            SceneManager.LoadScene("Title");
        };

        resumeFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (resumeFlag));
    }
}
