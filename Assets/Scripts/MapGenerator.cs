using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] float gridSize;
    [SerializeField] private GameObject panelPrefab;
    Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
    private List<List<int>> projectNodes = new List<List<int>>();
    private List<List<int>> projectEdges = new List<List<int>>();

    void Start()
    {
        StandardPos = new Vector2(this.transform.position.x, this.transform.position.y);
        generateMap();
        drawMap();
    }
    
    public Vector2 GetActualPostion(int x, int y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    public Vector2 GetActualPostionByFloat(float x, float y){
        return new Vector2
            (StandardPos.x + x*gridSize, StandardPos.y - y*gridSize);
    }

    void generateMap()
    {
        projectNodes.Add(new List<int>() { -1, -1, 1, 1, -1 });
        projectNodes.Add(new List<int>() { 1, 1, 1, 1, 1 });
        projectNodes.Add(new List<int>() { -1, 1, 1, -1, -1 });
        
        projectEdges.Add(new List<int>(){ -1, 2, 1, 4 });
        projectEdges.Add(new List<int>(){ 1, 1, 1, 1 });
        projectEdges.Add(new List<int>(){ 3, 1, 5, -1 });
    }

    void drawMap()
    {
        for(int y=0; y<projectNodes.Count; y++){
            for (int x=0; x<projectNodes[0].Count; x++){
                // int tileNum = TILE_TYPE[panelMap[y][x]];
                // GameObject tile = Instantiate(panelPrefabs[tileNum]) as GameObject; //タイル生成

                if (projectNodes[y][x] != -1){
                    GameObject tile = Instantiate(panelPrefab) as GameObject;
                    tile.transform.parent = this.transform; // Supplyの子にする
                    tile.transform.position = GetActualPostion(x, y);
                    tile.name = $"{y}_{x}";
                }
            }
        }
        
        for(int y=0; y<projectEdges.Count; y++){
            for (int x=0; x<projectEdges[0].Count; x++){

                // Vector3 pos1;
                // Vector3 pos2;
                Vector3[] positions;
                
                if (projectEdges[y][x] == 1)
                {
                    // pos1 = GetActualPostion(x, y);
                    // pos2 = GetActualPostion(x+1, y);
                    positions = new Vector3[]{
                        GetActualPostion(x, y),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 2)
                {
                    // pos1 = GetActualPostion(x, y+1);
                    // pos2 = GetActualPostion(x+1, y);
                    positions = new Vector3[]{
                        GetActualPostion(x, y+1),
                        GetActualPostionByFloat(x+1/3f, y+1f),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 3)
                {
                    // pos1 = GetActualPostion(x, y-1);
                    // pos2 = GetActualPostion(x+1, y);
                    positions = new Vector3[]{
                        GetActualPostion(x, y-1),
                        GetActualPostionByFloat(x+1/3f, y+0f),
                        GetActualPostion(x+1, y)
                    };
                }
                
                else if (projectEdges[y][x] == 4)
                {
                    // pos1 = GetActualPostion(x, y);
                    // pos2 = GetActualPostion(x+1, y+1);
                    positions = new Vector3[]{
                        GetActualPostion(x, y),
                        GetActualPostionByFloat(x+1/3f, y+1f),
                        GetActualPostion(x+1, y+1)
                    };
                }
                
                else if (projectEdges[y][x] == 5)
                {
                    // pos1 = GetActualPostion(x, y);
                    // pos2 = GetActualPostion(x+1, y-1);
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
                
                GameObject lineObj = new GameObject();
                lineObj.name = $"{y}_{x}_line";
                lineObj.transform.parent = this.transform; // Supplyの子にする
                var line =lineObj.AddComponent<LineRenderer>();
                // line.SetPosition(0, pos1);
                // line.SetPosition(1, pos2);

                // 線の太さ
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;

                // 線の色
                line.material = new Material(Shader.Find("Sprites/Default"));
                // line.startColor = Color.red;
                // line.endColor = Color.red;
                line.startColor = new Color(46/255f,82/255f,143/255f,1.0f);
                line.endColor = new Color(46/255f,82/255f,143/255f,1.0f);

                // 点の数を指定する
                line.positionCount = positions.Length;

                // 線を引く場所を指定する
                line.SetPositions(positions);
            }
        }
        
        
        
    }

    void Update()
    {
        
    }
}
