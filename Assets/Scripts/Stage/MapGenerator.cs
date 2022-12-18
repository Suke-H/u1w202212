using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;  
using System;
using System.IO;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] float gridSize;
    [SerializeField] private GameObject nodeBase;
    [SerializeField] private GameObject[] nodeTypes;
    [SerializeField] private GameObject Team;

    [SerializeField] private int[] initMemberNum;

    Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
    private List<List<int>> NodeMap = new List<List<int>>();
    private List<List<int>> EdgeMap = new List<List<int>>();
    private List<List<int>> NodeOrders = new List<List<int>>();
    private List<List<int>> TeamPositions = new List<List<int>>();

    private int nodeNum;

    private List<Color> colors = new List<Color>();

    // public bool isMapGenerated = false;

    public List<GameObject> currentTeamObj {get; set;} = new List<GameObject>();
    public List<GameObject> nextTeamObj {get; set;} = new List<GameObject>();

    int turn = 0;

    void Start()
    {
        // 色の追加
        colors.Add(new Color(46/255f,82/255f,143/255f,1.0f)); // 青
        colors.Add(new Color(220/255f,20/255f,60/255f,1.0f)); // 赤
        colors.Add(new Color(146/255f,208/255f,80/255f,1.0f)); // 緑
        
        // 本オブジェクトの位置を基準とする
        StandardPos = new Vector2(this.transform.position.x, this.transform.position.y);

        // マップ生成
        // generateMap();
        NodeMap = read2DListFromCSV("Stage2/NodeMap"); // ノード行列
        EdgeMap = read2DListFromCSV("Stage2/EdgeMap"); // エッジリスト
        (NodeOrders, nodeNum) = ordering(); // ノード番号リスト
        // チームリスト

        printMap(NodeMap, "NodeMap");
        printMap(EdgeMap, "EdgeMap");
        printMap(NodeOrders, "NodeOrders");

        // isMapGenerated = true;

        // マップ描画
        drawMap();

        // 現在チームに追加
        Vector2Int position = searchNode(0);
        if (position.x != -1){
            GameObject team = Instantiate(Team) as GameObject;
            team.transform.parent = this.transform; // Supplyの子にする
            team.transform.position = GetTeamPostion(position.x, position.y);
            team.name = $"team_{position.y}_{position.x}";

            currentTeamObj.Add(team);
        }

        List<Vector2Int> positions = searchNextNodes();

        foreach(Vector2Int pos in positions){
            GameObject team = Instantiate(Team) as GameObject;
            team.transform.parent = this.transform; // Supplyの子にする
            team.transform.position = GetTeamPostion(pos.x, pos.y);
            team.name = $"team_{pos.y}_{pos.x}";

            nextTeamObj.Add(team);
        }
    }

    public void printMap(List<List<int>> oriList, string text){
        Debug.Log(text);

        foreach (List<int> row in oriList){
            Debug.Log(string.Join(", ",row.Select(obj => obj.ToString())));
        }
    }

    public List<List<int>> initTeamPositions(){

        // Listを0で初期化
        List<List<int>> tmpList = initList(nodeNum, initMemberNum.Length, 0);

        // 初期位置に初期チーム配置
        for(int i=0; i<initMemberNum.Length; i++){
            tmpList[0][i] = initMemberNum[i];
        }

        return tmpList;
    }

    public Vector2Int searchNode(int order)
    {
        for (int y = 0; y < NodeOrders.Count; y++){
            for (int x = 0; x < NodeOrders[0].Count; x++){
                if (NodeOrders[y][x] == order){
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.Log("エラーが来るぞ！！！");
        return new Vector2Int(-1, -1);
    }

    public List<Vector2Int> searchNextNodes(){

        List<Vector2Int> nodes = new List<Vector2Int>();

        for (int y = 0; y < NodeMap.Count; y++){
            if (NodeMap[y][turn+1] != -1){
                nodes.Add(new Vector2Int(turn+1, y));
            }
        }

        return nodes;
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
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/2);
    }

    public List<List<int>> initList(int x, int y, int value){

        List<List<int>> tmpList = new List<List<int>>();

        for(int i=0; i<y; i++){
            List<int> tmp = new List<int>();
            for(int j=0; j<x; j++){ tmp.Add(value);}
            tmpList.Add(tmp);
        }

        return tmpList;
    }

    public (List<List<int>>, int) ordering()
    {
        // Listを-1で初期化
        List<List<int>> tmpList = initList(NodeMap[0].Count, NodeMap.Count, -1);

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

    public List<string[]> readCSV(string path)
    {
        //ResourcesからCSVを読み込むのに必要
        TextAsset csvFile;

        //読み込んだCSVファイルを格納
        List<string[]> csvDatas = new List<string[]>();

        /* Resouces/CSV下のCSV読み込み */
        csvFile = Resources.Load(path) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1){
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        return csvDatas;

    }

    public List<List<int>> read2DListFromCSV(string path)
    {
        //一時入力用で毎回初期化する構造体とリスト
        List<List<int>> tmpList = new List<List<int>>();

        // CSVの読み込み
        List<string[]> csvDatas = readCSV(path);

        // List<List<int>>形式に移す
        for (int i = 0; i < csvDatas.Count; i++)
        {
            // 一行ずつ
            List<int> tmpRow = new List<int>();
            for (int j=0; j<csvDatas[i].Length; j++){
                tmpRow.Add(int.Parse(csvDatas[i][j]));
            }

            // 追加
            tmpList.Add(tmpRow);
        }

        return tmpList;
    }

    // public void generateMap()
    // {
    //     NodeMap.Add(new List<int>() { -1, -1, 1, 4, -1 });
    //     NodeMap.Add(new List<int>() { 0, 1, 2, 1, 1 });
    //     NodeMap.Add(new List<int>() { -1, 1, 3, -1, -1 });
        
    //     EdgeMap.Add(new List<int>(){ -1, 2, 1, 4 });
    //     EdgeMap.Add(new List<int>(){ 1, 1, 1, 1 });
    //     EdgeMap.Add(new List<int>(){ 3, 1, 5, -1 });
    // }

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
            Vector2Int pos0 = searchNode(edge[0]);
            Vector2Int pos1 = searchNode(edge[1]);
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

