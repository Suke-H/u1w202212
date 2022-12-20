using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{
    public bool startFlag { get; set; } = false;
    [SerializeField] int[] initTeamComp;

    TeamManager teamManager;
    MapGenerator mapGenerator;

    List<TeamState> CurrentTeamStates = new List<TeamState>();
    EventManager eventManager;

    // private List<TeamState> currentTeams;
    // private List<TeamState> nextTeams;

    public void updateNextTeams(){
        

    }

    async public UniTask teamAssignSequence(){

        List<int> nextOrders = mapGenerator.nextOrders;

        // 次ノードが2つ以上なら
        if (nextOrders.Count >= 2){

            // foreach(TeamState currentTeam in currentTeams){
                // チームアサイン
                teamManager.assignTeams(initTeamComp);
            // }

            // チーム割り振り変更処理
            Debug.Log("チーム割り振りを変更");
            // while (!startFlag){
                // teamAssign.changeTeamAssign()
            // }
        }

        // nextTeams = new List<TeamState>(teamManager.nextTeams);

    }

    async void Start()
    {
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        mapGenerator.generateMap("Stage2");

        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();

        await EntireLoop();
    }

    void eventSwitch(){
        eventManager.battleEvent();
    }

    async UniTask EntireLoop(){

        // while(true){

            await teamAssignSequence();

            // foreach(TeamState teamState in CurrentTeamStates){
            //     eventSwitch(teamState);
            // }

        // }

    }
}
