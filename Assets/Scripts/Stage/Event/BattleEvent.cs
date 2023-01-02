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

    [SerializeField] SEController SE;

    // -1: 選択待ち, 0: バトル辞退, 1: バトル決定
    public int battleTry {get; set;} = -1;
    // 成功したか
    public bool successFlag {get; set;} = false;

    // 自分のレベル
    int[] playerLevels = new int[]{0, 0};

    // 交渉成功率
    int negotiateSuccessRate;
    // システム完成度
    int systemCompleteRate;

    [SerializeField] MapManager mapManager;
    [SerializeField] Tutorial tutorial;

    void Start(){
        SE = GameObject.Find("SE").GetComponent<SEController>();
    }

    async public UniTask BattleEventSequence(TeamInfo teamInfo, CustomerData customerData, MapData mapData, bool lastFlag)
    {
        // 初期化
        battleTry = -1;
        successFlag = false;

        // Lv計算
        int salesLv = calcLv(OurInfo.skills[0], teamInfo.teamComp[0]);
        int engineerLv = calcLv(OurInfo.skills[1], teamInfo.teamComp[1]);

        playerLevels[0] = salesLv;
        playerLevels[1] = engineerLv;

        // 成功率を計算(%)
        negotiateSuccessRate = calcNegotiateRate(playerLevels[0], customerData.demandLv[0]);
        systemCompleteRate = calcSystemRate(playerLevels[1], customerData.demandLv[1]);

        // ダイアログを初期化
        var battleDialog = initializeDialog(teamInfo.teamComp, lastFlag, customerData);

        // チュートリアル（ルール1）
        if (StageStore.stageName == "Tutorial"){
            BattleDialog BD = battleDialog.GetComponent<BattleDialog>();
            BD.setActive(false);
            await tutorial.TutoRule1();
            BD.setActive(true);
        }

        // チュートリアル（最後）
        if (lastFlag & StageStore.stageName == "Tutorial"){
            BattleDialog BD = battleDialog.GetComponent<BattleDialog>();
            BD.setActive(false);
            await tutorial.TutoEnd();
            BD.setActive(true);
        }

        // バトル選択待ち
        await UniTask.WaitUntil(() => (battleTry != -1));

        /* 商談開始 */
        if (battleTry == 1){

            // 一旦ダイアログ削除
            Destroy(battleDialog);

            // 成功判定
            bool result = battleJudge(negotiateSuccessRate, systemCompleteRate);

            /* 成功 */
            if (result){
                SE.playSE("success"); // SE

                GameObject successDialog = Instantiate(SuccessDialog) as GameObject;
                successDialog.transform.SetParent (canvas.transform, false);

                SuccessDialog SD = successDialog.GetComponent<SuccessDialog>();
                SD.initialize(mapData.rewardParams);

                // チュートリアル（その2-1）
                if (StageStore.stageName == "Tutorial"){
                    SD.setActive(false);
                    await tutorial.TutoRule2_1();
                    SD.setActive(true);
                }

                await SD.buttonWait();
                Destroy(successDialog);

                successFlag = true;
            }

            /* 失敗 */
            else {
                SE.playSE("shock"); // SE

                // 最終ノードならこの処理を飛ばす
                if (!lastFlag){
                    GameObject failDialog = Instantiate(FailDialog) as GameObject;
                    failDialog.transform.SetParent (canvas.transform, false);

                    FailDialog FD = failDialog.GetComponent<FailDialog>();
                    FD.initialize();

                    await FD.buttonWait();
                    Destroy(failDialog);
                }

                successFlag = false;
            }
        }

        /* 商談辞退 */
        else {
            successFlag = false;
        }

    }

    int calcLv(int skill, int number){
        int totalPower = skill * number;
        return totalPower / PowerPerLevel;
    }

    int calcNegotiateRate(int playerLv, int enemyLv){
        // return (playerLv - enemyLv) * 10 + 80;
        float d = (float)(playerLv - enemyLv) / (float)(playerLv + enemyLv) * 20f;
        return (int)(d*10) + 80;
    }

    int calcSystemRate(int playerLv, int enemyLv){
        // return (playerLv - enemyLv) * 10 + 80;
        float d = (float)(playerLv - enemyLv) / (float)(playerLv + enemyLv) * 20f;
        return (int)(d*10) + 80;
    }

    public bool battleJudge(int percent1, int percent2){
        float rate1 = percent1 * 0.01f;
        float rate2 = percent2 * 0.01f;

        // 0~1のランダム値
        float value1 = Random.value;
        float value2 = Random.value;

        if (value1 <= rate1 & value2 <= rate2) { return true; }
        else { return false; }
    }

    public GameObject initializeDialog(int[] teamComp, bool lastFlag, CustomerData customer){
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
        BD.initialize(teamComp, false, lastFlag, teamArgs);

        return battleDialog;
    }

}
