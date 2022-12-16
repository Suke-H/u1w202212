using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class MapGenerator : MonoBehaviour
{
    [SerializeField] float gridSize;
    [SerializeField] private GameObject nodeBase;
    [SerializeField] private GameObject[] nodeTypes;
    [SerializeField] private GameObject Team;

    Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
    private List<List<int>> projectNodes = new List<List<int>>();
    private List<List<int>> projectEdges = new List<List<int>>();
    private List<List<int>> orderNodes = new List<List<int>>();

    private List<Color> colors = new List<Color>();

    public bool isMapGenerated = false;

    int turn = 0;

    void Start()
    {
        colors.Add(new Color(46/255f,82/255f,143/255f,1.0f)); // 青
        colors.Add(new Color(220/255f,20/255f,60/255f,1.0f)); // 赤
        colors.Add(new Color(146/255f,208/255f,80/255f,1.0f)); // 緑
        
        StandardPos = new Vector2(this.transform.position.x, this.transform.position.y);
        generateMap();
        isMapGenerated = true;
        drawMap();

        // Vector2Int position = searchNode(0);
        // if (position.x != -1){
        //     GameObject team = Instantiate(Team) as GameObject;
        //     team.transform.parent = this.transform; // Supplyの子にする
        //     team.transform.position = GetTeamPostion(position.x, position.y);
        //     team.name = $"team_{position.y}_{position.x}";
        // }

        // List<Vector2Int> positions = searchNextNodes();

        // foreach(Vector2Int pos in positions){
        //     GameObject team = Instantiate(Team) as GameObject;
        //     team.transform.parent = this.transform; // Supplyの子にする
        //     team.transform.position = GetTeamPostion(pos.x, pos.y);
        //     team.name = $"team_{pos.y}_{pos.x}";
        // }

    }
    
    public Vector2 GetActualPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    public Vector2 GetActualPostionByFloat(float x, float y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    // public Vector2 GetTeamPostion(int x, int y){
    //     return new Vector2
    //         (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/2);
    // }

    public void ordering()
    {
        // -1で初期化
        for (int y = 0; y < projectNodes.Count; y++) {
            List<int> row = new List<int>();
            
            for (int x = 0; x < projectNodes[0].Count; x++) {
                row.Add(-1);
            }
            projectNodes.Add(row);
        }

        int count = 0;
        
        for (int x = 0; x < projectNodes[0].Count; x++) {
            for (int y = 0; y < projectNodes.Count; y++) {
                if (projectNodes[y][x] != -1)
                {
                    orderNodes[y][x] = count;
                    count++;
                }
            }
        }
    }

    // public Vector2Int searchNode(int order)
    // {
    //     for (int y = 0; y < projectNodes.Count; y++){
    //         for (int x = 0; x < projectNodes[0].Count; x++){
    //             if (projectNodes[y][x] == order)
    //             {
    //                 return new Vector2Int(x, y);
    //             }
    //         }
    //     }

    //     return new Vector2Int(-1, -1);
    // }

    // public List<Vector2Int> searchNextNodes(){

    //     List<Vector2Int> nodes = new List<Vector2Int>();

    //     for (int y = 0; y < projectNodes.Count; y++){
    //         if (projectNodes[y][turn+1] != -1){
    //             nodes.Add(new Vector2Int(turn+1, y));
    //         }
    //     }

    //     return nodes;
    // }

    async public UniTask generateMap()
    {
        projectNodes.Add(new List<int>() { -1, -1, 1, 4, -1 });
        projectNodes.Add(new List<int>() { 0, 1, 2, 1, 1 });
        projectNodes.Add(new List<int>() { -1, 1, 3, -1, -1 });
        
        projectEdges.Add(new List<int>(){ -1, 2, 1, 4 });
        projectEdges.Add(new List<int>(){ 1, 1, 1, 1 });
        projectEdges.Add(new List<int>(){ 3, 1, 5, -1 });
    }

    void drawLine(int x, int y, Vector3[] positions){
        GameObject lineObj = new GameObject();
        lineObj.name = $"{y}_{x}_line";
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

    void drawMap()
    {
        for(int y=0; y<projectNodes.Count; y++){
            for (int x=0; x<projectNodes[0].Count; x++){

                int type = projectNodes[y][x];

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
        
        for(int y=0; y<projectEdges.Count; y++){
            for (int x=0; x<projectEdges[0].Count; x++){

                Vector3[] positions;
                
                if (projectEdges[y][x] == 1)
                {
                    positions = new Vector3[]{
                        GetActualPostion(x, y),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 2)
                {
                    positions = new Vector3[]{
                        GetActualPostion(x, y+1),
                        GetActualPostionByFloat(x+1/3f, y+0f),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 3)
                {
                    positions = new Vector3[]{
                        GetActualPostion(x, y-1),
                        GetActualPostionByFloat(x+1/3f, y+0f),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 4)
                {
                    positions = new Vector3[]{
                        GetActualPostion(x, y),
                        GetActualPostionByFloat(x+1/3f, y+1f),
                        GetActualPostion(x+1, y+1)
                    };
                }
                
                else if (projectEdges[y][x] == 5)
                {
                    positions = new Vector3[]{
                        GetActualPostion(x, y),
                        GetActualPostionByFloat(x+1/3f, y-1f),
                        GetActualPostion(x+1, y-1)
                    };
                }

                else
                {
                    continue;
                }
                
            // 線の描画
            drawLine(x, y, positions);
                
            }
        }
        
    }

    void Update()
    {
        
    }
}
