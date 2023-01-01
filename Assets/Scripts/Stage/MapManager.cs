using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;  
using System;
using System.IO;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] private float gridSize;
    [SerializeField] private GameObject nodeBase;
    [SerializeField] private GameObject[] nodeTypes;
    [SerializeField] private GameObject Team;

    public List<List<int>> NodeMap {get; protected set;} = new List<List<int>>(); 
    public List<List<int>> EdgeMap {get; protected set;} = new List<List<int>>();
    public List<List<int>> OrderMap {get; protected set;} = new List<List<int>>();

    public List<GameObject> NodesByOrder {get; set;} = new List<GameObject>();

    public MapData mapData {get; set;}

    [SerializeField] GameObject PIN;
    [SerializeField] float pinMoveTime;
    Dictionary<int, GameObject> PinMap = new Dictionary<int, GameObject>();

    public Vector2 StandardPos {get; protected set;} // 基準タイル（左上(0,0)）の中心座標

    public int nodeNum {get; set;}
    private List<Color> colors = new List<Color>();

    Dictionary<int, string> nodeTypeDict = new Dictionary<int, string>();

    ListUtils listUtils = new ListUtils();
    ColorPallet pallet = new ColorPallet();
    TeamManager teamManager;

    int pinCount = 1;

    void Start(){
        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        nodeTypeDict.Add(0, "none");  
        nodeTypeDict.Add(1, "battle");  
        nodeTypeDict.Add(2, "happening");  
        nodeTypeDict.Add(3, "money"); 
        nodeTypeDict.Add(4, "refresh"); 
        nodeTypeDict.Add(5, "boss"); 
    }

    /* 主関数 */
    public void generateMap(string stageName){
        colors.Add(pallet.blue); // 青
        colors.Add(pallet.red); // 赤
        colors.Add(pallet.green); // 緑

        // 本オブジェクトの位置を基準とする
        StandardPos = new Vector2(this.transform.position.x, this.transform.position.y);

        // マップ生成
        NodeMap = listUtils.read2DListFromCSV($"{stageName}/NodeMap"); // ノード行列
        EdgeMap = listUtils.read2DListFromCSV($"{stageName}/EdgeMap"); // エッジリスト

        // マップ詳細情報
        mapData = Resources.Load<MapData>($"{stageName}/Map");

        (OrderMap, nodeNum) = orderingNodes(); // ノード番号リスト

        // マップ描画
        drawMap();
    }

    /* 座標 */

    public (Vector2, string) getTeamNodeInfo(int order){
        Vector2Int posXY = searchNodePos(order);
        Vector2 pos = GetTeamPostion(posXY.x, posXY.y);

        string nodeType = nodeTypeDict[NodeMap[posXY.y][posXY.x]];

        return (pos, nodeType);
    }

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
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/3);
            // (StandardPos.x + x*gridSize - gridSize/2, StandardPos.y - y*gridSize);
    }

    public Vector2 GetPinPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/6);
    }

    public Vector3[] GetPathPositions(Vector2Int pos0, Vector2Int pos1){
        int xSpan = Math.Abs(pos1.x - pos0.x);
        Vector3[] positions = new Vector3[]{
                        GetActualPostion(pos0.x, pos0.y),
                        // GetActualPostionByFloat(pos0.x+xSpan/3f, pos1.y+0f),
                        GetActualPostion(pos1.x, pos1.y)
                    };

        return positions;
    }

    /* マップ系処理 */

    public (List<List<int>>, int) orderingNodes()
    {
        // Listを-1で初期化
        List<List<int>> tmpList = listUtils.initList(NodeMap[0].Count, NodeMap.Count, -1);

        int count = 0;
        
        // 「左から→上から」の順で番号を振っていく
        for (int x = 0; x < NodeMap[0].Count; x++) {
            for (int y = 0; y < NodeMap.Count; y++) {
                if (NodeMap[y][x] != -1)
                {
                    tmpList[y][x] = count;
                    count++;
                }
            }
        }

        return (tmpList, count);
    }

    public Vector2Int searchNodePos(int order)
    {
        for (int y = 0; y < OrderMap.Count; y++){
            for (int x = 0; x < OrderMap[0].Count; x++){
                if (OrderMap[y][x] == order){
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.Log("エラーが来るぞ！！！");
        return new Vector2Int(-1, -1);
    }

    public List<int> searchNextOrders(int currentOrder){
        List<int> nodes = new List<int>();

        for (int y = 0; y < EdgeMap.Count; y++){
            if (EdgeMap[y][0] == currentOrder){
                nodes.Add(EdgeMap[y][1]);
            }
        }

        return nodes;
    }

    /* 描画処理 */

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
        for (int order=0; order<nodeNum; order++){
            var pos = searchNodePos(order);
            int type = NodeMap[pos.y][pos.x];

            // ノード
            GameObject node = Instantiate(nodeBase) as GameObject;
            node.transform.parent = this.transform; // Supplyの子にする
            node.transform.position = GetActualPostion(pos.x, pos.y);
            node.name = $"{pos.y}_{pos.x}";
            node.GetComponent<SpriteRenderer>().color = colors[pos.y];

            // ノードの種類の絵
            if (type != 0){
                GameObject nodeType = Instantiate(nodeTypes[type-1]) as GameObject;
                nodeType.transform.parent = node.transform; // nodeの子にする
                nodeType.transform.position = GetActualPostion(pos.x, pos.y);
            }

            // ノード情報
            Node nodeInfo = node.GetComponent<Node>();
            nodeInfo.initialize(nodeTypeDict[type]);
            if (nodeTypeDict[type] == "battle"){
                int ID = Array.IndexOf(mapData.nodeOrders, order);
                var customer = mapData.customerDatas[ID];
                nodeInfo.initializeBattle(customer);
            }

            NodesByOrder.Add(node);
        }

        /* エッジの描画 */
        foreach (List<int> edge in EdgeMap){
            Vector2Int pos0 = searchNodePos(edge[0]);
            Vector2Int pos1 = searchNodePos(edge[1]);
            int xSpan = Math.Abs(pos1.x - pos0.x);

            Debug.Log($"{edge[0]}: ({pos0.x},{pos0.y}), {edge[1]}: ({pos1.x},{pos1.y})");

            Vector3[] positions = GetPathPositions(pos0, pos1);

            drawLine(edge[0], edge[1], pos1.y, positions);
        }
    }

    /* ピン */

    // ピンの生成
    // 初期位置、分岐時に増殖
    // リストに(order, pinオブジェクト)を追加

    public void createPin(int order){
        var pos = searchNodePos(order);
        var currentPos = GetActualPostion(pos.x, pos.y);

        GameObject pinObj = Instantiate(PIN) as GameObject;
        pinObj.transform.parent = this.transform; // Supplyの子にする
        pinObj.transform.position = currentPos;
        pinObj.transform.name = $"pin_{order}_{pinCount++}";

        PinMap.Add(order, pinObj);
    }

    // ピンの削除
    public void deletePin(int order){
        // オブジェクト取り出し
        var pinObj = PinMap[order];

        // リストから削除
        PinMap.Remove(order);

        // オブジェクト自体削除
        Destroy(pinObj);
    }

    // ピンの移動
    async public UniTask movePins(int currentOrder, int nextOrder){
        // 現地点
        var currentPos = searchNodePos(currentOrder);

        // 次地点
        var nextPos = searchNodePos(nextOrder);

        // Pinオブジェクト取り出し
        GameObject pinObj = PinMap[currentOrder];
        var pin = pinObj.GetComponent<Pin>();

        // 動かす
        var path = GetPathPositions(currentPos, nextPos);
        await pin.move(path);
        
        // マップから削除して、追加
        // PinMap.Remove(currentOrder);

        // PinMap.Add(nextOrder, pinObj);

    }

}

