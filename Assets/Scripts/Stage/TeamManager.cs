using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject NodePopup;
    [SerializeField] GameObject canvas;//キャンバス

    [SerializeField] GameObject Whistle;
    [SerializeField] GameObject Balance;

    SEController SE;
    
    // 保存用
    TeamInfo currentTeamInfo;
    TeamState currentTeamState;
    GameObject currentTeamObject;

    // 保存用（パターン2）
    TeamInfo currentTeamInfo2;
    TeamState currentTeamState2;
    GameObject currentTeamObject2;

    GameObject nodePopup;
    
    public List<TeamInfo> nextTeamInfos {get; set;} = new List<TeamInfo>();
    public List<GameObject> nextTeamObjects {get; set;} = new List<GameObject>();
    public List<TeamState> nextTeamStates {get; set;} = new List<TeamState>();

    // ボタン
    public List<GameObject> WhistleInstances {get; set;} = new List<GameObject>();
    public GameObject BalanceInstance {get; set;}
    
    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;

    void Start(){
        mainCamera = Camera.main;

        SE = GameObject.Find("SE").GetComponent<SEController>();
    }

    public void OutOfFocus(){
        if (currentTeamObject != null){
            currentTeamObject.SetActive(false);
        }   
        foreach (GameObject team in nextTeamObjects){
            team.SetActive(false);
        }

        foreach (var whistle in WhistleInstances){
            if (whistle != null){
                whistle.SetActive(false);
            }
        }

        if (BalanceInstance != null){
            BalanceInstance.SetActive(false);
        }
    }

    public void InFocus(){
        if (currentTeamObject != null){
            currentTeamObject.SetActive(true);
        }   
        foreach (GameObject team in nextTeamObjects){
            team.SetActive(true);
        }

        foreach (var whistle in WhistleInstances){
            if (whistle != null){
                whistle.SetActive(true);
            }
        }

        if (BalanceInstance != null){
            BalanceInstance.SetActive(true);
        }

    }

    int calcLv(int skill, int number){
        int totalPower = skill * number;
        return totalPower / 10;
    }

    // レベルを更新
    public void updateLevel(TeamState teamState){
        int salesLv = calcLv(OurInfo.skills[0], teamState.teamComp[0]);
        int engineerLv = calcLv(OurInfo.skills[1], teamState.teamComp[1]);
        teamState.setLv(0, salesLv);
        teamState.setLv(1, engineerLv);
    }

    // 全てのチームから全人員を持ってくる
    public void whistleFunc(int teamNo){
        for (int type=0; type<2; type++){
            // 現在の値
            int nextReal = nextTeamInfos[teamNo].teamComp[type];
            // 全人数
            int total = currentTeamInfo.teamComp[type];
            total += nextTeamInfos[0].teamComp[type];
            total += nextTeamInfos[1].teamComp[type];

            // 差分
            // int delta = OurInfo.totalComp[type] - nextReal;
            int delta = total - nextReal;

            // ホイッスルを鳴らしたチームに総動員
            nextTeamStates[teamNo].plusTeam(type, delta);
            nextTeamInfos[teamNo].plusMember(type, delta);

            // それ以外は0人に
            currentTeamState.minusTeam(type, currentTeamInfo.teamComp[type]);
            currentTeamInfo.minusMember(type, currentTeamInfo.teamComp[type]);

            nextTeamStates[1-teamNo].minusTeam(type, nextTeamInfos[1-teamNo].teamComp[type]);
            nextTeamInfos[1-teamNo].minusMember(type, nextTeamInfos[1-teamNo].teamComp[type]);
        }

        // Lv再計算
        updateLevel(currentTeamState);
        updateLevel(nextTeamStates[teamNo]);
        updateLevel(nextTeamStates[1-teamNo]);
    }

    // 次チームを均等にする（奇数だったら[0]を1つ多めに）
    public void balanceFunc(){
        for(int type=0; type<2; type++){
            // 合計値
            // int total = OurInfo.totalComp[type];
            int total = currentTeamInfo.teamComp[type];
            total += nextTeamInfos[0].teamComp[type];
            total += nextTeamInfos[1].teamComp[type];

            // 目標の値
            int[] goals = new int[] {total/2 + total%2, total/2};

            for (int teamNo=0; teamNo<2; teamNo++){
                // 現在の値
                int nextReal = nextTeamInfos[teamNo].teamComp[type];
                // 差分
                int delta = goals[teamNo] - nextReal;

                Debug.Log($"total: {total}, nextReal: {nextReal}, delta: {delta}");

                currentTeamState.minusTeam(type, delta);
                nextTeamStates[teamNo].plusTeam(type, delta);

                currentTeamInfo.minusMember(type, delta);
                nextTeamInfos[teamNo].plusMember(type, delta);
            }
        }

        // Lv再計算
        updateLevel(currentTeamState);
        updateLevel(nextTeamStates[0]);
        updateLevel(nextTeamStates[1]);            

    }

    public bool buttonFunc(int teamNo, string type, string sign){
        // 役職
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { i = -1; }

        // プラス
        if (sign == "plus"){
            // 現チームのメンバーが1人以上いれば、次チームに1人アサイン
            if (currentTeamInfo.teamComp[i] > 0){
                currentTeamState.minusTeam(i);
                nextTeamStates[teamNo].plusTeam(i);

                currentTeamInfo.minusMember(i);
                nextTeamInfos[teamNo].plusMember(i);
            }

            // いなければ他チームから持ってくる
            else if (nextTeamInfos[1-teamNo].teamComp[i] > 0){
                nextTeamStates[1-teamNo].minusTeam(i);
                nextTeamStates[teamNo].plusTeam(i);

                nextTeamInfos[1-teamNo].minusMember(i);
                nextTeamInfos[teamNo].plusMember(i);
            }

            // Lv再計算
            updateLevel(currentTeamState);
            updateLevel(nextTeamStates[teamNo]);
            updateLevel(nextTeamStates[1-teamNo]);

            return true;
        }

        // マイナス
        else if (sign == "minus"){
            // 次チームのメンバーが1人以上いれば、現チームに1人戻す
            if(nextTeamInfos[teamNo].teamComp[i] > 0){
                currentTeamState.plusTeam(i);
                nextTeamStates[teamNo].minusTeam(i);

                currentTeamInfo.plusMember(i);
                nextTeamInfos[teamNo].minusMember(i);
            }

            // Lv再計算
            updateLevel(currentTeamState);
            updateLevel(nextTeamStates[teamNo]);

            return true;
        }

        return false;
    }

    public void assignCurrentTeams(TeamInfo currentInfo){
        if (currentTeamObject != null){
            Destroy(currentTeamObject);
        }

        if (currentTeamObject2 != null){
            Destroy(currentTeamObject2);
        }

        // 保存
        currentTeamInfo = currentInfo;
        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);
        // Lv再計算
        updateLevel(currentTeamState);
        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos, dx: -1.8f, dy: 0f);
    }

    public void assignCurrentUnionTeams(TeamInfo current1, TeamInfo current2){
        if (currentTeamObject != null){
            Destroy(currentTeamObject);
        }

        if (currentTeamObject2 != null){
            Destroy(currentTeamObject2);
        }

        // 保存
        currentTeamInfo = current1;
        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);
        // Lv再計算
        updateLevel(currentTeamState);
        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos, dx: -1.8f, dy: 0f);

        // 保存
        currentTeamInfo2 = current2;
        // チーム生成
        (currentTeamObject2, currentTeamState2) = createTeam(-1, currentTeamInfo2.teamComp);
        // Lv再計算
        updateLevel(currentTeamState2);
        // チーム描画
        displayTeam(currentTeamObject2, currentTeamInfo2.nodePos, dx: -1.8f, dy: 0f);
    }

    public void assignTeams(TeamInfo currentInfo, List<TeamInfo> nextInfos){
        if (currentTeamObject != null){
            Destroy(currentTeamObject);
        }

        if (currentTeamObject2 != null){
            Destroy(currentTeamObject2);
        }

        // 保存
        currentTeamInfo = currentInfo;
        nextTeamInfos = new List<TeamInfo>(nextInfos);

        /* 現チームの処理開始 */

        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);
        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos, dx: -1.8f, dy: 0f);
        // Lv更新
        updateLevel(currentTeamState);

        /* 次チームの処理開始 */

        nextTeamObjects = new List<GameObject>(); // リセット
        nextTeamStates = new List<TeamState>(); // リセット

        for (int i=0; i<nextTeamInfos.Count; i++){
            // チーム生成
            var (nextTeamObject, nextTeamState) = createTeam(i, new int[]{0, 0});
            // チーム描画
            displayTeam(nextTeamObject, nextTeamInfos[i].nodePos, dx: 1.8f, dy: 0f);
            // 追加
            nextTeamObjects.Add(nextTeamObject);
            nextTeamStates.Add(nextTeamState);
            // Lv更新
            updateLevel(nextTeamStates[i]);
        }

        // ボタン生成
        createButtons();
    }

    public (GameObject, TeamState) createTeam(int teamNo, int[] comp){
        // チームオブジェクト生成し、場所指定
        GameObject team = Instantiate(Team) as GameObject;
        team.transform.SetParent (canvas.transform, false);

        // チームの初期化
        TeamState teamState = team.GetComponent<TeamState>();
        teamState.initialize(comp, teamNo); 

        return (team, teamState);
    }

    // ワールド座標をUI座標に変換
    public Vector2 WorldToUI(Vector2 pos, float dx=0f, float dy=0f){

        // ワールド座標 -> スクリーン座標変換
        var targetWorldPos = new Vector3(pos.x+dx, pos.y+dy, 0);
        var targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        // スクリーン座標 -> UIローカル座標変換
        RectTransform parentUI = this.gameObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentUI,
            targetScreenPos,
            mainCamera, // オーバーレイモードの場合はnull
            out var uiLocalPos // この変数に出力される
        );

        return uiLocalPos;
    }

    // チーム表示
    public void displayTeam(GameObject team, Vector2 pos, float dx=0f, float dy=0f){
        team.transform.localPosition = WorldToUI(pos, dx, dy);
    }

    public void destroyTeams(){
        Destroy(currentTeamObject);

        foreach (GameObject team in nextTeamObjects){
            Destroy(team);
        }

        nextTeamObjects = new List<GameObject>();
        nextTeamStates = new List<TeamState>();
    }

    // 顧客情報表示
    public void displayCustomer(Vector2 pos, CustomerData customerData){
        // ダイアログ表示
        nodePopup = Instantiate(NodePopup) as GameObject;
        nodePopup.transform.SetParent (canvas.transform, false);

        NodePopup NP = nodePopup.GetComponent<NodePopup>();
        // customerData.printData();
        NP.initialize(customerData);

        // 座標指定
        NP.transform.localPosition = WorldToUI(pos, dx: 0f, dy: -1.2f);
    }

    // チーム表示（UI座標変換）
    public void destroyCustomer(){
        Destroy(nodePopup);
    }

    public void createButtons(){
        // 天秤ボタン生成
        GameObject balance = Instantiate(Balance) as GameObject;
        balance.transform.SetParent (canvas.transform, false);
        balance.transform.localPosition = WorldToUI(currentTeamInfo.nodePos,
                                        dx: 0f, dy: 1.5f); // 座標指定
        // ボタンの関数
        CustomButton balanceButton = balance.GetComponent<CustomButton>();
        balanceButton.onClickCallback = () => {
            SE.playSE("balance");
            balanceFunc();
        };

        BalanceInstance = balance; // 保存

        for (int i=0; i<2; i++){
            // ホイッスルボタン生成
            GameObject whistle = Instantiate(Whistle) as GameObject;
            whistle.transform.SetParent (canvas.transform, false);
            whistle.transform.localPosition = WorldToUI(nextTeamInfos[i].nodePos, 
                                            dx: 0f, dy: 1.5f); // 座標指定

            // ボタンの関数
            CustomButton whistleButton = whistle.GetComponent<CustomButton>();
            int index = i;
            whistleButton.onClickCallback = () => {
                SE.playSE("whistle");
                whistleFunc(index);
            };

            WhistleInstances.Add(whistle); // 保存
        }
    }

    public void destroyButtons(){
        foreach (var whistle in WhistleInstances){
            Destroy(whistle);
        }
        Destroy(BalanceInstance);
    }

}


