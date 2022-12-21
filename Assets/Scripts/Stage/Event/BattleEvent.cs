using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleEvent : MonoBehaviour
{
    [SerializeField] GameObject BattleDialog;

    [SerializeField] GameObject canvas;//キャンバス

    [SerializeField] int PowerPerLevel;

    int skill = 5;
    int[] enemyLevels = new int[]{3, 4};

    void Start()
    {
        // Lv計算
        int salesLv = calcLv(skill, 4);
        int engineerLv = calcLv(skill, 4);

        // 成功率を計算
        int negotiateRate = calcNegotiateRate(salesLv, enemyLevels[0]);
        int systemRate = calcSystemRate(engineerLv, enemyLevels[1]);

        // 文字
        List<string> teamArgs = new List<string>();
        teamArgs.Add(salesLv.ToString());
        teamArgs.Add(engineerLv.ToString());
        teamArgs.Add(enemyLevels[0].ToString());
        teamArgs.Add(enemyLevels[1].ToString());
        teamArgs.Add(negotiateRate.ToString() + "%");
        teamArgs.Add(systemRate.ToString() + "%");

        // ダイアログ生成
        GameObject battleDialog = Instantiate(BattleDialog) as GameObject;
        battleDialog.transform.SetParent (canvas.transform, false);
        
        BattleDialog BD = battleDialog.GetComponent<BattleDialog>();
        BD.initialize(new int[]{4, 4}, false, teamArgs);
        
    }

    public void BattleEventSequence(int[] teamComp){
        // ダイアログ生成
        GameObject battleDialog = Instantiate(BattleDialog) as GameObject;
        battleDialog.transform.SetParent (canvas.transform, false);

        // 
        int engineerLv = calcLv(skill, teamComp[0]);
    }

    int calcLv(int skill, int number){
        int totalPower = skill * number;
        return totalPower / PowerPerLevel;
    }

    int calcNegotiateRate(int playerLv, int enemyLv){
        return (playerLv - enemyLv) * 10 + 80;
    }

    int calcSystemRate(int playerLv, int enemyLv){
        return (playerLv - enemyLv) * 10 + 50;
    }

}
