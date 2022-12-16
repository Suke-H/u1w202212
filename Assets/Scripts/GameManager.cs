using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{

    [SerializeField] CustomButton startButton;
    bool startFlag = false;

    List<TeamState> AllTeamState = new List<TeamState>();
    EventManager eventManager;

    void Start()
    {
        var members = new Dictionary<string, int>();
        members.Add("sales", 4);
        members.Add("design", 4);
        members.Add("program", 4);

        TeamState initialTeamState = new TeamState(){members=members, panel=0}; 
        AllTeamState.Add(initialTeamState);

        startButton.onClickCallback = () => { 
            startFlag = true;
        };
    }

    void eventSwitch(TeamState teamState){
        eventManager.battleEvent(teamState);
    }

    async UniTask memberSelectTurn(TeamState teamState){

        int currentPanel = teamState.panel;
        

        while(!startFlag){

        }
    }

    async UniTask EntireLoop(){
        while(true){

            // await UniTask.WaitUntil(() => startFlag); // フラグが上がるまで待機
            foreach(TeamState teamState in AllTeamState){
                await memberSelectTurn(teamState);
            }
            

            foreach(TeamState teamState in AllTeamState){
                eventSwitch(teamState);
            }

        }

    }
}
