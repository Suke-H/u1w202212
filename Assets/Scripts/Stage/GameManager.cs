using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{

    // private bool startFlag = false;
    public bool startFlag { get; set; } = false;

    // TeamManager teamManager;
    MapGenerator mapGenerator;
    // TeamAssign teamAssign;

    List<TeamState> CurrentTeamStates = new List<TeamState>();
    EventManager eventManager;

    async public UniTask teamAssignSequence(){

        List<int> nextNodeOrders = mapGenerator.nextNodeOrders;

        if (nextNodeOrders.Count > 1){

            // チーム割り振り変更処理
            while (!startFlag){
                Debug.Log("チーム割り振りを変更");
                // teamAssign.changeTeamAssign()
            }
        }

    }

    async void Start()
    {

        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

        mapGenerator.generateMap("Stage2");

        // teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        // await UniTask.WaitUntil(() => teamManager.isInitialized); // フラグが上がるまで待機

        
        // await EntireLoop();
        
    }

    void eventSwitch(TeamState teamState){
        eventManager.battleEvent(teamState);
    }

    async UniTask EntireLoop(){
        while(true){

            // await memberSelectTurn(teamState);

            await teamAssignSequence();
            // await UniTask.WaitUntil(() => startFlag); // フラグが上がるまで待機

            foreach(TeamState teamState in CurrentTeamStates){
                eventSwitch(teamState);
            }

        }

    }
}
