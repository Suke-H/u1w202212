using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{
    public bool startFlag { get; set; } = false;
    [SerializeField] int[] initTeamComp;
    [SerializeField] CustomButton startButtton;

    TeamManager teamManager;
    MapGenerator mapGenerator;

    List<TeamState> CurrentTeamStates = new List<TeamState>();
    EventManager eventManager;

    // チーム割り振り変更シークエンス
    async public UniTask teamAssignSequence(int currentNodeOrder){
        teamManager.assignTeams(currentNodeOrder, initTeamComp);

        await UniTask.WaitUntil(() => startFlag);
    }

    async void Start()
    {
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        mapGenerator.generateMap("Stage2");

        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();

        startButtton.onClickCallback = () => {
            startFlag = true;
        };

        await EntireLoop();
    }

    void eventSwitch(){
        eventManager.battleEvent();
    }

    async UniTask EntireLoop(){

        // while(true){

        for (int i=0; i<5; i++){

            int currentNodeOrder = i; 

            var nextOrders = mapGenerator.searchNextOrders(currentNodeOrder);
            if (nextOrders.Count >= 2){
                await teamAssignSequence(currentNodeOrder);
            }

            startFlag = false;

            // foreach(TeamState teamState in CurrentTeamStates){
            //     eventSwitch(teamState);
            // }

        // }
        }

    }
}
