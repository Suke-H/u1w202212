using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;  
using System;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] float gridSize;
    [SerializeField] private GameObject nodeBase;
    [SerializeField] private GameObject[] nodeTypes;
    [SerializeField] private GameObject Team;

    [SerializeField] private int[] initTeamMembers;

    Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
    private List<List<int>> NodeMap = new List<List<int>>();
    private List<List<int>> EdgeMap = new List<List<int>>();
    private List<List<int>> NodeOrders = new List<List<int>>();
    private List<List<int>> TeamPositions = new List<List<int>>();

    public List<int> currentNodeOrders {get; set;} = new List<int>();
    public List<int> nextNodeOrders {get; set;} = new List<int>();

    private int nodeNum;

    private List<Color> colors = new List<Color>();

    public List<GameObject> currentTeamObj {get; set;} = new List<GameObject>();
    public List<GameObject> nextTeamObj {get; set;} = new List<GameObject>();

    int turn = 0;

    ListUtils listUtils;
    TeamAssign teamAssign;

    /* 主関数 */
    public void generateMap(string stageName){
        listUtils = new ListUtils();
        teamAssign = new TeamAssign();

        // 色の追加
        colors.Add(new Color(46/255f,82/255f,143/255f,1.0f)); // 青
        colors.Add(new Color(220/255f,20/255f,60/255f,1.0f)); // 赤
        colors.Add(new Color(146/255f,208/255f,80/255f,1.0f)); // 緑
        
        // 本オブジェクトの位置を基準とする
        StandardPos = new Vector2(this.transform.position.x, this.transform.position.y);

        // マップ生成
        NodeMap = listUtils.read2DListFromCSV($"{stageName}/NodeMap"); // ノード行列
        EdgeMap = listUtils.read2DListFromCSV($"{stageName}/EdgeMap"); // エッジリスト
        (NodeOrders, nodeNum) = listUtils.orderingNodes(NodeMap); // ノード番号リスト

        // マップ描画
        drawMap();

        // チーム初期化
        currentNodeOrders.Add(0);
        nextNodeOrders = listUtils.searchNextNodeOrders(EdgeMap, 0);

        // 初期チーム描画
        // initTeamDisplay();
    }

    // public void initTeamDisplay(){
    //     // 現在チームに追加
    //     Vector2Int position = listUtils.searchNodePos(NodeOrders, 0);
    //     if (position.x != -1){
    //         currentTeamObj.Add(createTeamObject(position));
    //     }

    //     // 次チームに追加
    //     List<Vector2Int> positions = listUtils.searchNextNodes(NodeMap, EdgeMap, 0);
    //     foreach(Vector2Int pos in positions){
    //         nextTeamObj.Add(createTeamObject(pos));
    //     }
    // }

    // public GameObject createTeamObject(Vector2Int pos){
    //     GameObject team = Instantiate(Team) as GameObject;
    //     team.transform.parent = this.transform; // 本オブジェクトの子にする
    //     team.transform.position = GetTeamPostion(pos.x, pos.y);
    //     team.name = $"team_{pos.y}_{pos.x}";

    //     return team;
    // }

    public Vector2 GetActualPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    public Vector2 GetActualPostionByFloat(float x, float y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    public Vector2 GetTeamPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/2);
    }


    public void drawLine(int current, int next, int y, Vector3[] positions){
        GameObject lineObj = new GameObject();
        lineObj.name = $"{current}_{next}_line";
        lineObj.transform.parent = this.transform; // Supplyの子にする
        var line = lineObj.AddComponent<LineRenderer>();

        // 線の太さ
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        // 線の色
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = colors[y];
        line.endColor = colors[y];

        // 点の数を指定する
        line.positionCount = positions.Length;

        // 線を引く場所を指定する
        line.SetPositions(positions);
    }

    public void drawMap()
    {
        /* ノードの描画 */
        for(int y=0; y<NodeMap.Count; y++){
            for (int x=0; x<NodeMap[0].Count; x++){

                int type = NodeMap[y][x];

                if (type != -1){
                    GameObject node = Instantiate(nodeBase) as GameObject;
                    node.transform.parent = this.transform; // Supplyの子にする
                    node.transform.position = GetActualPostion(x, y);
                    node.name = $"{y}_{x}";
                    node.GetComponent<SpriteRenderer>().color = colors[y];

                    if (type != 0){
                        GameObject nodeType = Instantiate(nodeTypes[type-1]) as GameObject;
                        nodeType.transform.parent = node.transform; // nodeの子にする
                        nodeType.transform.position = GetActualPostion(x, y);
                    }

                }
            }
        }

        /* エッジの描画 */
        foreach (List<int> edge in EdgeMap){
            Vector2Int pos0 = listUtils.searchNodePos(NodeOrders, edge[0]);
            Vector2Int pos1 = listUtils.searchNodePos(NodeOrders, edge[1]);
            int xSpan = Math.Abs(pos1.x - pos0.x);
            Debug.Log($"edge: {edge[0]}-{edge[1]}, pos0: ({pos0.x}, {pos0.y}), pos1: ({pos1.x}, {pos1.y})");

            Vector3[] positions = new Vector3[]{
                        GetActualPostion(pos0.x, pos0.y),
                        GetActualPostionByFloat(pos0.x+xSpan/3f, pos1.y+0f),
                        GetActualPostion(pos1.x, pos1.y)
                    };

            drawLine(edge[0], edge[1], pos1.y, positions);
        }
    }

}

