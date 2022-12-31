using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BattleDialog : MonoBehaviour
{
    // チーム情報
    [SerializeField] TextMeshProUGUI[] teamLevelTexts;
    [SerializeField] TextMeshProUGUI[] successRateTexts;

    // 敵情報
    [SerializeField] TextMeshProUGUI enemyName;
    [SerializeField] TextMeshProUGUI[] enemyLevelTexts;

    [SerializeField] CustomButton yesButton;
    [SerializeField] CustomButton noButton;

    [SerializeField] TeamUI teamUI;

    // [SerializeField] BattleEvent battleEvent;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public bool isButtonExist { get; set; }

    public void initialize(int[] teamComp, bool isButtonExist, bool lastFlag, List<string> args){

        // チームの初期化
        teamUI.initialize(teamComp, isButtonExist); 

        Debug.Log(string.Join(",", args));

        // 文字描画
        teamLevelTexts[0].text = args[0];
        teamLevelTexts[1].text = args[1];

        successRateTexts[0].text = args[2];
        successRateTexts[1].text = args[3];

        enemyName.text = args[4];

        enemyLevelTexts[0].text = args[5];
        enemyLevelTexts[1].text = args[6];

        var battleEvent = GameObject.Find("EventManager").GetComponent<BattleEvent>();
        // var battleEvent = this.gameObject.GetComponent<BattleEvent>();

        // ボタン
        yesButton.onClickCallback = () => {
            battleEvent.battleTry = 1;
        };

        if (!lastFlag){
            noButton.onClickCallback = () => {
                battleEvent.battleTry = 0;
            };
        }

        
    }

}
