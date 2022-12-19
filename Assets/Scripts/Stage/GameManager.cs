using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{

    // private bool startFlag = false;
    public bool startFlag { get; set; } = false;

    TeamManager teamManager;
    MapGenerator mapGenerator;
    // TeamAssign teamAssign;

    List<TeamState> CurrentTeamStates = new List<TeamState>();
    EventManager eventManager;

    async public UniTask teamAssignSequence(){

        List<int> nextOrders = mapGenerator.nextOrders;

        // 次ノードが2つ以上なら
        if (nextOrders.Count >= 2){

            // チームアサイン
            teamManager.assignTeams();

            // チーム割り振り変更処理
            Debug.Log("チーム割り振りを変更");
            // while (!startFlag){
                // teamAssign.changeTeamAssign()
            // }
        }

    }

    async void Start()
    {
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        mapGenerator.generateMap("Stage2");

        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();

        await EntireLoop();
    }

    void eventSwitch(TeamState teamState){
        eventManager.battleEvent(teamState);
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
