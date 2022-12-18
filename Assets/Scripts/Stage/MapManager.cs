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

    [SerializeField] private int[] initTeamMembers;

    Vector2 StandardPos; // 基準タイル（左上(0,0)）の中心座標
    private List<List<int>> NodeMap = new List<List<int>>();
    private List<List<int>> EdgeMap = new List<List<int>>();
    private List<List<int>> NodeOrders = new List<List<int>>();
    private List<List<int>> TeamPositions = new List<List<int>>();

    private int nodeNum;

    public List<GameObject> currentTeamObj {get; set;} = new List<GameObject>();
    public List<GameObject> nextTeamObj {get; set;} = new List<GameObject>();

    int turn = 0;

    ListUtils listUtils;
    TeamAssign teamAssign;


}
