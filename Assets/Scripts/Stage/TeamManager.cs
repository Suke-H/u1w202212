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

    public void assignTeams(int currentNodeOrder, int[] currentComp){
        if (firstFlag){
            mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
            teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
            firstFlag = false;
        }

        initialTeams();

        // List<int> currentOrders = mapGenerator.currentOrders;
        // currentTeamComp = currentComp;
        
        // List<int> nextOrders = mapGenerator.nextOrders;
        // for (int j=0; j<nextOrders.Count; j++){
        //     nextTeamComps.Add(new int[]{0, 0});
        // }

        // 現ノードの番号から次ノードの番号を検索
        List<int> nextOrders = mapGenerator.searchNextOrders(currentNodeOrder);

        // // 現ノードにチーム生成
        // int i = -1;
        // foreach (int order in currentOrders){
        //     var (pos, nodeType) = mapGenerator.getTeamNodeInfo(order);
        //     teamManager.displayTeam(pos, nodeType, i, "current");
        //     i++;
        // }

        // Debug.Log(currentComp);
        // Debug.Log(currentTeam);
        // Array.Copy(currentComp, currentTeam.teamComp, currentComp.Length);


        // 現ノードにチーム生成
        var (pos, nodeType) = mapGenerator.getTeamNodeInfo(currentNodeOrder);
        currentTeam = teamManager.displayTeam(pos, nodeType, -1, currentComp);

        // 次ノードにチーム生成
        nextTeams = new List<TeamState>(); // リセット
        int i = 0;
        foreach (int order in nextOrders){
            (pos, nodeType) = mapGenerator.getTeamNodeInfo(order);
            var nextTeam = teamManager.displayTeam(pos, nodeType, i, new int[]{0, 0});
            nextTeams.Add(nextTeam);
            i++;
        }

        
    }

    public void initialTeams(){
        // nextTeams = new List<GameObject>();
        nextTeams = new List<TeamState>();
    }

    int[] defaultArr = new int[]{0, 0};

    // チーム表示（UI座標変換）
    public TeamState displayTeam(Vector2 pos, string nodeType, int teamNo, int[] comp){

        Debug.Log("ここにはきてるんよね？");
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
        // if (teamNo == -1){ teamState.initialize(comp, teamNo); }
        // else { teamState.initialize(new int[]{0, 0}, teamNo); }
        teamState.initialize(comp, teamNo); 

        return teamState;
    }

}


