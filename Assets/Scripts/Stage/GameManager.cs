using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{

    // private bool startFlag = false;
    public bool startFlag { get; set; } = false;

    // TeamManager teamManager;

    List<TeamState> CurrentTeamStates = new List<TeamState>();
    EventManager eventManager;

    async void Start()
    {
        // teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();

        // await UniTask.WaitUntil(() => teamManager.isInitialized); // フラグが上がるまで待機
        
        await EntireLoop();
        
    }

    void eventSwitch(TeamState teamState){
        eventManager.battleEvent(teamState);
    }

    async UniTask EntireLoop(){
        while(true){

            // await memberSelectTurn(teamState);

            await UniTask.WaitUntil(() => startFlag); // フラグが上がるまで待機

            foreach(TeamState teamState in CurrentTeamStates){
                eventSwitch(teamState);
            }

        }

    }
}
