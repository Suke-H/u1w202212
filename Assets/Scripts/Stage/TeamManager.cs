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

                // Lv計算
                int salesLv = calcLv(OurInfo.skills[0], currentTeamState.teamComp[0]);
                int engineerLv = calcLv(OurInfo.skills[1], currentTeamState.teamComp[1]);
                currentTeamState.setLv(0, salesLv);

                salesLv = calcLv(OurInfo.skills[0], nextTeamStates[teamNo].teamComp[0]);
                engineerLv = calcLv(OurInfo.skills[1], nextTeamStates[teamNo].teamComp[1]);
                nextTeamStates[teamNo].setLv(0, salesLv);

                return true;
            }

        }
        else if (sign == "minus"){
            // 次チームのメンバーが1人以上いれば、現チームに1人戻す
            if(nextTeamInfos[teamNo].teamComp[i] > 0){
                currentTeamState.plusTeam(i);
                nextTeamStates[teamNo].minusTeam(i);

                currentTeamInfo.plusMember(i);
                nextTeamInfos[teamNo].minusMember(i);

                // Lv計算
                int salesLv = calcLv(OurInfo.skills[0], currentTeamState.teamComp[0]);
                int engineerLv = calcLv(OurInfo.skills[1], currentTeamState.teamComp[1]);
                currentTeamState.setLv(0, salesLv);
                currentTeamState.setLv(1, engineerLv);

                salesLv = calcLv(OurInfo.skills[0], nextTeamStates[teamNo].teamComp[0]);
                engineerLv = calcLv(OurInfo.skills[1], nextTeamStates[teamNo].teamComp[1]);
                nextTeamStates[teamNo].setLv(0, salesLv);
                nextTeamStates[teamNo].setLv(1, engineerLv);

                return true;
            }
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

        // Lv計算
        int salesLv = calcLv(OurInfo.skills[0], currentTeamState.teamComp[0]);
        int engineerLv = calcLv(OurInfo.skills[1], currentTeamState.teamComp[1]);
        currentTeamState.setLv(0, salesLv);
        currentTeamState.setLv(1, engineerLv);

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

        // Lv計算
        int salesLv = calcLv(OurInfo.skills[0], currentTeamState.teamComp[0]);
        int engineerLv = calcLv(OurInfo.skills[1], currentTeamState.teamComp[1]);
        currentTeamState.setLv(0, salesLv);
        currentTeamState.setLv(1, engineerLv);

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

            // Lv
            salesLv = calcLv(OurInfo.skills[0], nextTeamStates[i].teamComp[0]);
            engineerLv = calcLv(OurInfo.skills[1], nextTeamStates[i].teamComp[1]);
            nextTeamStates[i].setLv(0, salesLv);
            nextTeamStates[i].setLv(1, engineerLv);
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
        customerData.printData();
        NP.initialize(customerData);

        // 座標指定
        NP.transform.localPosition = uiLocalPos;
    }

    // チーム表示（UI座標変換）
    public void destroyCustomer(){
        Destroy(nodePopup);
    }

}


