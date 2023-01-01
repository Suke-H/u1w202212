using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject NodePopup;
    // [SerializeField] GameObject whistle;
    [SerializeField] GameObject canvas;//キャンバス
    
    // 保存用
    TeamInfo currentTeamInfo;
    TeamState currentTeamState;
    GameObject currentTeamObject;

    GameObject nodePopup;
    
    public List<TeamInfo> nextTeamInfos {get; set;} = new List<TeamInfo>();
    public List<GameObject> nextTeamObjects {get; set;} = new List<GameObject>();
    public List<TeamState> nextTeamStates {get; set;} = new List<TeamState>();
    
    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;

    public void OutOfFocus(){
        if (currentTeamObject != null){
            currentTeamObject.SetActive(false);
        }   
        foreach (GameObject team in nextTeamObjects){
            team.SetActive(false);
        }
    }

    public void InFocus(){
        if (currentTeamObject != null){
            currentTeamObject.SetActive(true);
        }   
        foreach (GameObject team in nextTeamObjects){
            team.SetActive(true);
        }
    }

    int calcLv(int skill, int number){
        int totalPower = skill * number;
        return totalPower / 10;
    }

    // public void CtoN(TeamState currentState, TeamState nextState,  int type, int value=1){
    //     currentState.minusTeam(type, value);
    //     nextState.plusTeam(type, value);

    //     currentInfo.minusMember(type, value);
    //     nextInfo.plusMember(type, value);
    // }

    // public void NtoC(){

    // }

    // レベルを更新
    public void updateLevel(TeamState teamState){
        int salesLv = calcLv(OurInfo.skills[0], teamState.teamComp[0]);
        int engineerLv = calcLv(OurInfo.skills[1], teamState.teamComp[1]);
        teamState.setLv(0, salesLv);
        teamState.setLv(1, engineerLv);
    }

    // 現在チームから全ての人員を持ってくる
    public void whistle(int teamNo){
        int[] nums = new int[]{currentTeamState.teamComp[0], currentTeamState.teamComp[1]};

        for (int i=0; i<2; i++){
            currentTeamState.minusTeam(i, nums[i]);
            nextTeamStates[teamNo].plusTeam(i, nums[i]);

            currentTeamInfo.minusMember(i, nums[i]);
            nextTeamInfos[teamNo].plusMember(i, nums[i]);
        }
    }

    // 次チームを均等にする（奇数だったら[0]を1つ多めに）
    public void balance(){

        for(int type=0; type<2; type++){
            int total = currentTeamState.teamComp[type]
                        + nextTeamStates[0].teamComp[type]
                        + nextTeamStates[1].teamComp[type];

            // 目標の値
            int[] goals = new int[] {total/2 + total%2, total/2};
            Debug.Log($"type: {type}, goals: ({goals[0]},{goals[1]})");

            for (int teamNo=0; teamNo<2; teamNo++){
                // 現在の値
                int nextReal = nextTeamInfos[teamNo].teamComp[type];
                // Debug.Log($"type: {type}, goals: ({goals[0]},{goals[1]})");

                currentTeamState.minusTeam(type, goals[teamNo]-nextReal);
                nextTeamStates[teamNo].plusTeam(type, goals[teamNo]-nextReal);

                currentTeamInfo.minusMember(type, goals[teamNo]-nextReal);
                nextTeamInfos[teamNo].plusMember(type, goals[teamNo]-nextReal);
            }
        }            

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

            // // いなければ他チームから持ってくる
            // else {
            //     nextTeamStates[teamNo].minusTeam(i);
            //     nextTeamStates[teamNo].plusTeam(i);

            //     nextTeamInfos[teamNo].minusMember(i);
            //     nextTeamInfos[teamNo].plusMember(i);
            // }

            // Lv再計算
            updateLevel(currentTeamState);
            updateLevel(nextTeamStates[teamNo]);

            return true;

        }
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

        // 保存
        currentTeamInfo = currentInfo;

        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);

        // Lv再計算
        updateLevel(currentTeamState);

        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos);
    }

    public void assignTeams(TeamInfo currentInfo, List<TeamInfo> nextInfos){
        if (currentTeamObject != null){
            Destroy(currentTeamObject);
        }

        // 保存
        currentTeamInfo = currentInfo;
        nextTeamInfos = new List<TeamInfo>(nextInfos);

        // 現チームの処理開始

        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);

        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos);

        // Lv更新
        updateLevel(currentTeamState);

        // 次チームの処理開始

        nextTeamObjects = new List<GameObject>(); // リセット
        nextTeamStates = new List<TeamState>(); // リセット

        for (int i=0; i<nextTeamInfos.Count; i++){
            // チーム生成
            var (nextTeamObject, nextTeamState) = createTeam(i, new int[]{0, 0});
            // チーム描画
            displayTeam(nextTeamObject, nextTeamInfos[i].nodePos);

            // 追加
            nextTeamObjects.Add(nextTeamObject);
            nextTeamStates.Add(nextTeamState);

            // Lv更新
            updateLevel(nextTeamStates[i]);
        }

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

    // チーム表示（UI座標変換）
    public void displayTeam(GameObject team, Vector2 pos){
        mainCamera = Camera.main;

        // ワールド座標 -> スクリーン座標変換
        var targetWorldPos = new Vector3(pos.x, pos.y, 0);
        var targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        // スクリーン座標 -> UIローカル座標変換
        RectTransform parentUI = this.gameObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentUI,
            targetScreenPos,
            mainCamera, // オーバーレイモードの場合はnull
            out var uiLocalPos // この変数に出力される
        );

        team.transform.localPosition = uiLocalPos;
    }

    public void destroyTeams(){
        Destroy(currentTeamObject);

        foreach (GameObject team in nextTeamObjects){
            Destroy(team);
        }

        nextTeamObjects = new List<GameObject>();
        nextTeamStates = new List<TeamState>();
    }

    // チーム表示（UI座標変換）
    public void displayCustomer(Vector2 pos, CustomerData customerData){
        mainCamera = Camera.main;

        // ワールド座標 -> スクリーン座標変換
        var targetWorldPos = new Vector3(pos.x, pos.y-1.2f, 0);
        var targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        // スクリーン座標 -> UIローカル座標変換
        RectTransform parentUI = this.gameObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentUI,
            targetScreenPos,
            mainCamera, // オーバーレイモードの場合はnull
            out var uiLocalPos // この変数に出力される
        );

        // ダイアログ表示
        nodePopup = Instantiate(NodePopup) as GameObject;
        nodePopup.transform.SetParent (canvas.transform, false);

        NodePopup NP = nodePopup.GetComponent<NodePopup>();
        // customerData.printData();
        NP.initialize(customerData);

        // 座標指定
        NP.transform.localPosition = uiLocalPos;
    }

    // チーム表示（UI座標変換）
    public void destroyCustomer(){
        Destroy(nodePopup);
    }

}


