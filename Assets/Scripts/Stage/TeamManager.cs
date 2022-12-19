using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Team;
    [SerializeField] GameObject canvas;//キャンバス

    // チーム
    // List<int[]> currentTeamComps = new List<int[]>();
    int[] currentTeamComp;
    List<int[]> nextTeamComps = new List<int[]>();
    // List<GameObject> currentTeams = new List<GameObject>();
    GameObject currentTeam;
    List<GameObject> nextTeams = new List<GameObject>();

    ColorPallet pallet = new ColorPallet();
    Camera mainCamera;

    MapGenerator mapGenerator;
    TeamManager teamManager;
    bool firstFlag = true;

    public void plus(int teamNo, string type){
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { Debug.Log("バグ！");  i = -1; }

        currentTeamComp[i] -= 1;
        nextTeamComps[teamNo][i] += 1;

    }

    public void minus(int teamNo, string type){
        int i;
        if (type == "sales"){ i = 0; }
        else if (type == "engineer"){ i = 1; }
        else { Debug.Log("バグ！");  i = -1; }

        currentTeamComp[i] += 1;
        nextTeamComps[teamNo][i] -= 1;
    }

    public void assignTeams(){

        initialTeams();

        if (firstFlag){
            mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
            teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
            firstFlag = false;
        }

        // Vector2Int posXY = listUtils.searchNodePos(NodeOrders, 0);
        // var pos = GetTeamPostion(posXY.x, posXY.y);

        // 現ノードにチーム生成
        List<int> currentOrders = mapGenerator.currentOrders;
        foreach (int order in currentOrders){
            var pos = mapGenerator.getTeamPositionByOrder(order);
            teamManager.displayTeam(pos, "current");
        }

        // 次ノードにチーム生成
        List<int> nextOrders = mapGenerator.nextOrders;
        foreach (int order in nextOrders){
            var pos = mapGenerator.getTeamPositionByOrder(order);
            teamManager.displayTeam(pos, "next");
        }
    }

    public void initialTeams(){
        // currentTeams = new List<GameObject>();
        nextTeams = new List<GameObject>();
    }

    // チーム表示（UI座標変換）
    public void displayTeam(Vector2 pos, string condition){
    // public void displayTeam(Vector2 pos, string condition, int[] currentCond){
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
        if (condition == "current"){ teamState.initialize(new int[]{0, 0}, true); }
        else if (condition == "next"){ teamState.initialize(new int[]{0, 0}, true); }
        else { Debug.Log("気をつけろ！"); }
        

        // チームを格納して管理
        if (condition == "current"){ currentTeam = team; }
        // if (condition == "current"){ currentTeams.Add(team); }
        else if (condition == "next"){ nextTeams.Add(team); }
        else { Debug.Log("気をつけろ！"); }
    }

}


