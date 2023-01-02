using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Tutorial : MonoBehaviour
{
    [SerializeField, TextArea(1,4)] string[] explains;
    [SerializeField] GameObject TextPopup;
    [SerializeField] Canvas canvas;
    [SerializeField] CustomButton[] Buttons;

    int textNo = 0;
    int endNo;

    bool[] endFlags; 

    void Start(){
        endNo = explains.Length;

        if (StageStore.stageName == "Tutorial"){
            endFlags = new bool[]{false, false, false, false, false, false, false};
        }
        else{
            endFlags = new bool[]{true, true, true, true, true, true, true};
        }
    }

    public void setButtonsActive(bool flag){
        foreach (CustomButton button in Buttons){
            button.setActive(flag);
        }
    }

    async public UniTask Popup(float moveValue=0f){
        // 言うことなくなったら終わり
        if (textNo >= endNo){ return; }

        // チュートリアルじゃなくても終わり
        if (StageStore.stageName != "Tutorial") { return; }

        // クリアダイアログ表示
        GameObject textPopup = Instantiate(TextPopup) as GameObject;
        textPopup.transform.SetParent (canvas.transform, false);

        TextPopup TP = textPopup.GetComponent<TextPopup>();
        TP.initialize(explains[textNo]);

        // y移動
        TP.moveY(moveValue);
        textNo++;

        // クリック待ち
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));

        // ダイアログ削除
        Destroy(textPopup);
    }

    // 冒頭
    async public UniTask TutoStart(){
        if (endFlags[0]) { return; }
        setButtonsActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        await Popup();
        await Popup();
        await Popup();
        await Popup();

        setButtonsActive(true);
        endFlags[0] = true;
    }

    // ルール1
    async public UniTask TutoRule1(){
        if (endFlags[1]) { return; }
        // setButtonsActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await Popup();
        await Popup(270f);
        await Popup(270f);
        await Popup(270f);
        await Popup(270f);
        await Popup(270f);
        await Popup(270f);

        // setButtonsActive(true);
        endFlags[1] = true;
    }

    // ルール2-1
    async public UniTask TutoRule2_1(){
        if (endFlags[2]) { return; }
        // setButtonsActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await Popup();
        await Popup(270f);

        // setButtonsActive(true);
        endFlags[2] = true;
    }

    // ルール2-2
    async public UniTask TutoRule2_2(){
        if (endFlags[3]) { return; }
        setButtonsActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await Popup(200f);
        await Popup(200f);
        await Popup(200f);

        setButtonsActive(true);
        endFlags[3] = true;
    }

    // ルール3
    async public UniTask TutoRule3(){
        if (endFlags[4]) { return; }
        setButtonsActive(false);

        await Popup();
        await Popup();
        await Popup();
        await Popup();
        await Popup();
        await Popup();

        setButtonsActive(true);
        endFlags[4] = true;
    }

    // 終わりフェイク
    async public UniTask TutoFakeEnd(){
        if (endFlags[5]) { return; }
        setButtonsActive(false);

        await Popup();

        setButtonsActive(true);
        endFlags[5] = true;
    }

    // ほんとの終わり
    async public UniTask TutoEnd(){
        if (endFlags[6]) { return; }
        // setButtonsActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await Popup();
        await Popup();
        await Popup(270f);

        // setButtonsActive(true);
        endFlags[6] = true;
    }

    
}
