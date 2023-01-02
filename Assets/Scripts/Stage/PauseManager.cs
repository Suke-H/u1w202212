using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject PauseDialog;
    [SerializeField] CustomButton homeButton;
    [SerializeField] Canvas canvas;

    SEController SE;

    bool pauseFlag = false;

    async void Start()
    {
        SE = GameObject.Find("SE").GetComponent<SEController>();

        homeButton.onClickCallback = () => {
            // SE.playSE("click"); // SE
            pauseFlag = true;
        };

        await PauseLoop();
    }

    async public UniTask PauseLoop(){
        while(true){
            pauseFlag = false;

            // ホームボタンが押されるまで待機
            await UniTask.WaitUntil(() => (pauseFlag));

            // ダイアログ表示
            GameObject pauseDialog = Instantiate(PauseDialog) as GameObject;
            pauseDialog.transform.SetParent (canvas.transform, false);

            PauseDialog PD = pauseDialog.GetComponent<PauseDialog>();
            PD.initialize();

            // ダイアログ内のボタンが押されるまで待機
            await PD.buttonWait();

            // SE.playSE("click"); // SE

            // ダイアログ削除
            Destroy(pauseDialog);
        }
    }



}
