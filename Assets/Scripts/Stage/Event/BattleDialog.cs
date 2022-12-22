using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] teamLevelTexts;
    [SerializeField] TextMeshProUGUI[] enemyLevelTexts;
    [SerializeField] TextMeshProUGUI[] successRateTexts;

    [SerializeField] CustomButton yesButton;
    [SerializeField] CustomButton noButton;

    [SerializeField] TeamUI teamUI;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public bool isButtonExist { get; set; }

    public void initialize(int[] teamComp, bool isButtonExist, List<string> args){

        // チームの初期化
        teamUI.initialize(teamComp, isButtonExist); 

        Debug.Log(string.Join(",", args));

        // 文字描画
        teamLevelTexts[0].text = args[0];
        teamLevelTexts[1].text = args[1];
        enemyLevelTexts[0].text = args[2];
        enemyLevelTexts[1].text = args[3];

        successRateTexts[0].text = args[4];
        successRateTexts[1].text = args[5];

        var battleEvent = GameObject.Find("EventManager").GetComponent<BattleEvent>();

        // ボタン
        yesButton.onClickCallback = () => {
            battleEvent.battleTry = 1;
        };

        noButton.onClickCallback = () => {
            battleEvent.battleTry = 0;
        };
        
    }

}
