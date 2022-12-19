// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Cysharp.Threading.Tasks;  

// public class MapManager : MonoBehaviour
// {
    
//     private float gridSize;
//     // [SerializeField] private GameObject nodeBase;
//     // [SerializeField] private GameObject[] nodeTypes;
//     private GameObject Team;

//     Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
//     private List<List<int>> NodeMap = new List<List<int>>();
//     private List<List<int>> EdgeMap = new List<List<int>>();
//     private List<List<int>> NodeOrders = new List<List<int>>();
//     private List<List<int>> TeamPositions = new List<List<int>>();

//     // public List<GameObject> currentTeamObj {get; set;} = new List<GameObject>();
//     // public List<GameObject> nextTeamObj {get; set;} = new List<GameObject>();

//     int turn = 0;
//     private int nodeNum;

//     ListUtils listUtils = new ListUtils();
//     // TeamAssign teamAssign;

//     MapGenerator mg;
//     TeamManager teamManager;

//     public Vector2 GetTeamPostion(int x, int y){
//         return new Vector2
//             (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/2);
//     }

//     // MapGeneratorのプロパティを引継ぎ
//     public void tranferProperty(){
//         mg = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

//         (gridSize, Team) = mg.getSerializeField();
//         StandardPos = mg.StandardPos;
//         NodeMap = mg.NodeMap;
//         EdgeMap = mg.EdgeMap;
//         NodeOrders = mg.NodeOrders;
//         TeamPositions = mg.TeamPositions;
//     }

//     public void initTeamDisplay(){
//         Vector2Int posXY = listUtils.searchNodePos(NodeOrders, 0);
//         var pos = GetTeamPostion(posXY.x, posXY.y);
//         teamManager.displayTeam(pos);
//     }

// }
