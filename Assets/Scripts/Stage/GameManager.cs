using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{
    public bool startFlag { get; set; } = false;
    [SerializeField] int[] initTeamComp;
    [SerializeField] CustomButton startButtton;

    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] TeamManager teamManager;
    [SerializeField] EventManager eventManager;

    // TeamManager teamManager;
    // MapGenerator mapGenerator;
    // EventManager eventManager;

    List<TeamState> CurrentTeamStates = new List<TeamState>();

    // チーム割り振り変更シークエンス
    async public UniTask teamAssignSequence(int currentNodeOrder){
        teamManager.assignTeams(currentNodeOrder, initTeamComp);

        await UniTask.WaitUntil(() => startFlag);
    }

    async void Start()
    {
        // mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        mapGenerator.generateMap("Stage2");

        // teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        // eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();

        startButtton.onClickCallback = () => {
            startFlag = true;
        };

        await EntireLoop();
    }

    async UniTask eventSwitch(int[] teamComp){
        eventManager.eventSwitch(teamComp);
    }

    async UniTask EntireLoop(){

        await eventSwitch(new int[]{8, 8});

        // while(true){

        // for (int i=0; i<5; i++){

        //     int currentNodeOrder = i; 

        //     var nextOrders = mapGenerator.searchNextOrders(currentNodeOrder);
        //     if (nextOrders.Count >= 2){
        //         await teamAssignSequence(currentNodeOrder);
        //     }

        //     startFlag = false;

            // foreach(TeamState teamState in CurrentTeamStates){
            //     eventSwitch(teamState);
            // }

        // }
        // }

    }
}
