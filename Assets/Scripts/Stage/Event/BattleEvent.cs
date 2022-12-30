using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;  

public class BattleEvent : MonoBehaviour
{
    [SerializeField] GameObject BattleDialog;
    [SerializeField] GameObject SuccessDialog;
    [SerializeField] GameObject FailDialog;
    [SerializeField] GameObject canvas;//キャンバス
    [SerializeField] int PowerPerLevel;

    // -1: 選択待ち, 0: バトル辞退, 1: バトル決定
    public int battleTry {get; set;} = -1;
    // どの報酬を選択したか;
    // public int rewardNo {get; set; }

    // 自分のレベル
    int[] playerLevels = new int[]{0, 0};

    // 交渉成功率
    int negotiateSuccessRate;
    // システム完成度
    int systemCompleteRate;

    [SerializeField] MapManager mapManager;

    async public UniTask BattleEventSequence(TeamInfo teamInfo, CustomerData customerData, OurInfo ourInfo, MapData mapData)
    {
        // 初期化
        battleTry = -1;

        // Lv計算
        int salesLv = calcLv(ourInfo.skills[0], teamInfo.teamComp[0]);
        int engineerLv = calcLv(ourInfo.skills[1], teamInfo.teamComp[1]);

        playerLevels[0] = salesLv;
        playerLevels[1] = engineerLv;

        // 成功率を計算(%)
        negotiateSuccessRate = calcNegotiateRate(playerLevels[0], customerData.demandLv[0]);
        systemCompleteRate = calcSystemRate(playerLevels[1], customerData.demandLv[1]);

        // ダイアログを初期化
        var battleDialog = initializeDialog(teamInfo.teamComp, customerData);

        // バトル選択待ち
        await UniTask.WaitUntil(() => (battleTry != -1));

        if (battleTry == 1){

            // 一旦ダイアログ削除
            Destroy(battleDialog);

            // 判定
            bool result = battleJudge(negotiateSuccessRate, systemCompleteRate);

            // 成功ダイアログ
            if (result){
                GameObject successDialog = Instantiate(SuccessDialog) as GameObject;
                successDialog.transform.SetParent (canvas.transform, false);

                SuccessDialog SD = successDialog.GetComponent<SuccessDialog>();
                // MapData mapData = mapManager.mapData;
                SD.initialize(mapData.rewardParams);

                await SD.buttonWait();
                Destroy(successDialog);
            }

            // 失敗ダイアログ
            else {
                GameObject failDialog = Instantiate(FailDialog) as GameObject;
                failDialog.transform.SetParent (canvas.transform, false);

                FailDialog FD = failDialog.GetComponent<FailDialog>();
                FD.initialize();

                await FD.buttonWait();
                Destroy(failDialog);
            }
        }

        else {
            Debug.Log("辞退！！！！！");
        }

    }

    int calcLv(int skill, int number){
        int totalPower = skill * number;
        Debug.Log($"skill: {skill}, power: {totalPower}");
        return totalPower / PowerPerLevel;
    }

    int calcNegotiateRate(int playerLv, int enemyLv){
        return (playerLv - enemyLv) * 10 + 80;
    }

    int calcSystemRate(int playerLv, int enemyLv){
        return (playerLv - enemyLv) * 10 + 80;
    }

    public bool battleJudge(int percent1, int percent2){
        float rate = percent1 * percent2 * 0.0001f;

        // 0~1のランダム値
        float value = Random.value;

        Debug.Log($"value: {value}, rate: {rate}");

        if (value <= rate) { return true; }
        else { return false; }
    }

    public GameObject initializeDialog(int[] teamComp, CustomerData customer){
        // 文字
        List<string> teamArgs = new List<string>();
        teamArgs.Add(playerLevels[0].ToString());
        teamArgs.Add(playerLevels[1].ToString());
        teamArgs.Add(negotiateSuccessRate.ToString() + "%");
        teamArgs.Add(systemCompleteRate.ToString() + "%");

        teamArgs.Add(customer.customerName);
        teamArgs.Add(customer.demandLv[0].ToString());
        teamArgs.Add(customer.demandLv[1].ToString());

        // ダイアログ生成
        GameObject battleDialog = Instantiate(BattleDialog) as GameObject;
        battleDialog.transform.SetParent (canvas.transform, false);
        
        BattleDialog BD = battleDialog.GetComponent<BattleDialog>();
        BD.initialize(teamComp, false, teamArgs);

        return battleDialog;
    }

}
