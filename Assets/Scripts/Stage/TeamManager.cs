using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject canvas;//キャンバス

    // チーム
    // int[] currentTeamComp;
    // List<int[]> nextTeamComps = new List<int[]>();

    // GameObject currentTeam;
    TeamState currentTeam;
    // List<GameObject> nextTeams = new List<GameObject>();
    public List<TeamState> nextTeams {get; set;} = new List<TeamState>();
    // List<TeamState> nextTeamStates = new List<TeamState>();

    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;

    MapGenerator mapGenerator;
    TeamManager teamManager;
    bool firstFlag = true;

    public void buttonFunc(int teamNo, string type, string sign){
        // 役職
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { Debug.Log("バグ！");  i = -1; }

        // プラスマイナス
        if (sign == "plus"){
            // currentTeamComp[i] -= 1;
            // nextTeamComps[teamNo][i] += 1;
            currentTeam.teamComp[i] -= 1;
            nextTeams[teamNo].teamComp[i] += 1;
        }
        else if (sign == "minus"){
            currentTeam.teamComp[i] += 1;
            nextTeams[teamNo].teamComp[i] -= 1;
        }
        else {
            Debug.Log("+でも-でもない何か");
        }

        var team = currentTeam.GetComponent<TeamState>();
        var member = team.getMember(i).GetComponent<MemberState>();
        // member.updateNumber(currentTeamComp[i]);
        member.updateNumber(currentTeam.teamComp[i]);

        // Debug.Log($"{currentTeamComp[0]}, {currentTeamComp[1]}");
        // Debug.Log($"{nextTeamComps[0][0]}, {nextTeamComps[0][1]}");
        // Debug.Log($"{nextTeamComps[1][0]}, {nextTeamComps[1][1]}");
        
    }

    public void assignTeams(int[] currentComp){
        if (firstFlag){
            mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
            teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
            firstFlag = false;
        }

        initialTeams();

        List<int> currentOrders = mapGenerator.currentOrders;
        // currentTeamComp = currentComp;
        
        List<int> nextOrders = mapGenerator.nextOrders;
        // for (int j=0; j<nextOrders.Count; j++){
        //     nextTeamComps.Add(new int[]{0, 0});
        // }

        // 現ノードにチーム生成
        int i = -1;
        foreach (int order in currentOrders){
            var (pos, nodeType) = mapGenerator.getTeamNodeInfo(order);
            teamManager.displayTeam(pos, nodeType, i, "current");
            i++;
        }

        // 次ノードにチーム生成
        foreach (int order in nextOrders){
            var (pos, nodeType) = mapGenerator.getTeamNodeInfo(order);
            teamManager.displayTeam(pos, nodeType, i, "next");
            i++;
        }

        Debug.Log(currentComp);
        Debug.Log(currentTeam);
        Array.Copy(currentComp, currentTeam.teamComp, currentComp.Length);
    }

    public void initialTeams(){
        // nextTeams = new List<GameObject>();
        nextTeams = new List<TeamState>();
    }

    // チーム表示（UI座標変換）
    public void displayTeam(Vector2 pos, string nodeType, int teamNo, string condition){
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

        // チームオブジェクト生成し、場所指定
        GameObject team = Instantiate(Team) as GameObject;
        team.transform.SetParent (canvas.transform, false);
        team.transform.localPosition = uiLocalPos;

        // チームの初期化
        TeamState teamState = team.GetComponent<TeamState>();
        // if (condition == "current"){ teamState.initialize(nodeType, currentTeamComp, teamNo, false); }
        if (condition == "current"){ teamState.initialize(currentTeam.teamComp, teamNo, false); }
        else if (condition == "next"){ teamState.initialize(new int[]{0, 0}, teamNo, true); }
        // if (condition == "current"){ teamState.initialize(nodeType, currentTeam.teamComp, teamNo, false); }
        // else if (condition == "next"){ teamState.initialize(nodeType, new int[]{0, 0}, teamNo, true);
        else { Debug.Log("気をつけろ！"); }
        

        // チームを格納して管理
        // if (condition == "current"){ currentTeam = team; }
        // else if (condition == "next"){ nextTeams.Add(team); }
        // else { Debug.Log("気をつけろ！"); }
        if (condition == "current"){ currentTeam = teamState; }
        else if (condition == "next"){ nextTeams.Add(teamState); }
        else { Debug.Log("気をつけろ！"); }
    }
    

}


