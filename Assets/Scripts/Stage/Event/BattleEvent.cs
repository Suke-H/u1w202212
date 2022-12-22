using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;  

public class BattleEvent : MonoBehaviour
{
    [SerializeField] GameObject BattleDialog;
    [SerializeField] GameObject canvas;//キャンバス
    [SerializeField] int PowerPerLevel;

    // -1: 選択待ち, 0: バトル辞退, 1: バトル決定
    public int battleTry {get; set;} = -1;

    // EventManagerに集約
    int skill = 5;
    int[] enemyLevels = new int[]{3, 4};

    async public UniTask BattleEventSequence(int[] teamComp)
    {
        // 初期化
        battleTry = -1;

        // Lv計算
        int salesLv = calcLv(skill, teamComp[0]);
        int engineerLv = calcLv(skill, teamComp[1]);

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
        BD.initialize(teamComp, false, teamArgs);

        // バトル選択待ち
        await UniTask.WaitUntil(() => (battleTry != -1));

        if (battleTry == 1){
            Debug.Log("商談開始！！！！！");
        }

        else {
            Debug.Log("辞退！！！！！");
        }
        
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
