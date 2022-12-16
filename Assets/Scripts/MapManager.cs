using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class MapManager : MonoBehaviour
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

    int turn = 0;

    MapGenerator mapGenerator;

    public Vector2 GetTeamPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize + gridSize/2);
    }

    public Vector2Int searchNode(int order)
    {
        for (int y = 0; y < projectNodes.Count; y++){
            for (int x = 0; x < projectNodes[0].Count; x++){
                if (projectNodes[y][x] == order)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public List<Vector2Int> searchNextNodes(){

        List<Vector2Int> nodes = new List<Vector2Int>();

        for (int y = 0; y < projectNodes.Count; y++){
            if (projectNodes[y][turn+1] != -1){
                nodes.Add(new Vector2Int(turn+1, y));
            }
        }

        return nodes;
    }


    async void Start()
    {

        // 自動生成待ち
        // await mapGenerator.generateMap();
        await UniTask.WaitUntil(() => mapGenerator.isMapGenerated); // フラグが上がるまで待機

        Vector2Int position = searchNode(0);
        if (position.x != -1){
            GameObject team = Instantiate(Team) as GameObject;
            team.transform.parent = this.transform; // Supplyの子にする
            team.transform.position = GetTeamPostion(position.x, position.y);
            team.name = $"team_{position.y}_{position.x}";
        }

        List<Vector2Int> positions = searchNextNodes();

        foreach(Vector2Int pos in positions){
            GameObject team = Instantiate(Team) as GameObject;
            team.transform.parent = this.transform; // Supplyの子にする
            team.transform.position = GetTeamPostion(pos.x, pos.y);
            team.name = $"team_{pos.y}_{pos.x}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
