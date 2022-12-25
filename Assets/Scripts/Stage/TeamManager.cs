using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject canvas;//キャンバス
    
    // 保存用
    TeamInfo currentTeamInfo;
    TeamState currentTeamState;
    GameObject currentTeamObject;
    
    public List<TeamInfo> nextTeamInfos {get; set;} = new List<TeamInfo>();
    public List<GameObject> nextTeamObjects {get; set;} = new List<GameObject>();
    public List<TeamState> nextTeamStates {get; set;} = new List<TeamState>();
    
    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;
    

    public bool buttonFunc(int teamNo, string type, string sign){
        // 役職
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { Debug.Log("バグ！");  i = -1; }

        // プラス
        if (sign == "plus"){

            // 現チームのメンバーが1人以上いれば、次チームに1人アサイン
            if (currentTeamInfo.teamComp[i] > 0){
                currentTeamState.minusTeam(i);
                nextTeamStates[teamNo].plusTeam(i);

                currentTeamInfo.minusMember(i);
                nextTeamInfos[teamNo].plusMember(i);

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

                return true;
            }
        }

        return false;

    }

    public void assignTeams(TeamInfo currentInfo, List<TeamInfo> nextInfos){
        // 保存
        currentTeamInfo = currentInfo;
        nextTeamInfos = new List<TeamInfo>(nextInfos);

        // 現チームの処理開始

        // チーム生成
        (currentTeamObject, currentTeamState) = createTeam(-1, currentTeamInfo.teamComp);

        // チーム描画
        displayTeam(currentTeamObject, currentTeamInfo.nodePos);


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

}


