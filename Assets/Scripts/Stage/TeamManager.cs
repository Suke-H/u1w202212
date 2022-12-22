using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject canvas;//キャンバス
    
    TeamState currentTeamState;
    public List<TeamState> nextTeamStates {get; set;} = new List<TeamState>();
    GameObject currentTeamObject;
    public List<GameObject> nextTeamObjects {get; set;} = new List<GameObject>();

    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;

    MapGenerator mapGenerator;
    bool firstFlag = true;

    public void buttonFunc(int teamNo, string type, string sign){
        // 役職
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { Debug.Log("バグ！");  i = -1; }

        // プラスマイナス
        if (sign == "plus"){
            currentTeamState.minusTeam(i);
            nextTeamStates[teamNo].plusTeam(i);
        }
        else if (sign == "minus"){
            currentTeamState.plusTeam(i);
            nextTeamStates[teamNo].minusTeam(i);
        }
        else {
            Debug.Log("+でも-でもない何か");
        }

    }

    public void assignTeams(int currentNodeOrder, int[] currentComp){
        if (firstFlag){
            mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
            firstFlag = false;
        }
        

        // 現ノードの情報
        var (pos, nodeType) = mapGenerator.getTeamNodeInfo(currentNodeOrder);
        
        TeamInfo currentTeamInfo = new TeamInfo(){teamComp=currentComp, 
        nodeOrder=currentNodeOrder, nodeType=nodeType, nodePos=pos};

        currentTeamInfo.printInfo();
        
        // チーム生成
        // (currentTeamObject, currentTeamState) = createTeam(-1, currentComp);

        // チーム描画
        // displayTeam(currentTeamObject, pos);

        // 現ノードの番号から次ノードの番号を検索
        List<int> nextOrders = mapGenerator.searchNextOrders(currentNodeOrder);

        // 次ノードにチーム生成
        List<TeamInfo> nextTeamInfos = new List<TeamInfo>();

        nextTeamStates = new List<TeamState>(); // リセット
        for (int i=0; i<nextOrders.Count; i++){
            (pos, nodeType) = mapGenerator.getTeamNodeInfo(nextOrders[i]);

            TeamInfo nextTeamInfo = new TeamInfo(){teamComp=new int[]{0, 0}, 
            nodeOrder=nextOrders[i], nodeType=nodeType, nodePos=pos};

            nextTeamInfos.Add(nextTeamInfo);

            // チーム生成
            // var (nextTeamObject, nextTeamState) = createTeam(i, new int[]{0, 0});
            // チーム描画
            // displayTeam(nextTeamObject, pos);

            // 追加
            // nextTeamObjects.Add(nextTeamObject);
            // nextTeamStates.Add(nextTeamState);
        }
        
    }

    public (GameObject, TeamState) createTeam(int teamNo, int[] comp){

        // チームオブジェクト生成し、場所指定
        GameObject team = Instantiate(Team) as GameObject;
        team.transform.SetParent (canvas.transform, false);
        // team.transform.localPosition = uiLocalPos;

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

}


