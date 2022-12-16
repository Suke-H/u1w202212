using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TeamManager : MonoBehaviour
{
    // [SerializeField] CustomButton AssignButton;
    [SerializeField] CustomButton startButton;

    GameManager gameManager;
    MapGenerator mapGenerator;

    public bool isInitialized { get; set; } = false;

    public List<TeamState> currentTeamStates {get; set;} = new List<TeamState>();
    public List<TeamState> nextTeamStates {get; set;} = new List<TeamState>();

    public List<GameObject> currentTeamObj;
    public List<GameObject> nextTeamObj;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

        startButton.onClickCallback = () => { 
            gameManager.startFlag = true;
        };

        var members = new Dictionary<string, int>();
        members.Add("sales", 4);
        members.Add("design", 4);
        members.Add("program", 4);

        TeamState initialTeamState = new TeamState(){members=members, mas=0}; 
        currentTeamStates.Add(initialTeamState);

        currentTeamObj = mapGenerator.currentTeamObj;
        nextTeamObj = mapGenerator.nextTeamObj;

        isInitialized = true;
    }

    public void updateMemberDisplay(int source, int target, string memberType){
        var text = transform.Find("Child1/Child2").gameObject;
    }

    // public void moveMember(TeamState source, TeamState target, string memberType){
    public void moveMember(int source, int target, string memberType){
        // source.members[memberType] -= 1;
        // target.members[memberType] += 1;
        currentTeamStates[source].members[memberType] -= 1;
        nextTeamStates[target].members[memberType] += 1;
    }

    public int searchC(int mas){
        int i=0; 
        foreach (TeamState teamState in currentTeamStates){
            if (teamState.mas == mas) { return i; }
            i++;
        }

        return -1;
    }

    public int searchN(int mas){
        int i=0; 
        foreach (TeamState teamState in nextTeamStates){
            if (teamState.mas == mas) { return i; }
            i++;
        }

        return -1;
    }

    async public UniTask memberSelectTurn(){
        
        while (gameManager.startFlag){
            // assignボタンを推したらCurrent/Next TeamStates内の対応するTeamStateの人数を増減
            // sourceの人数が0じゃなかったら注意出してほしい
        }

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "program");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "program");

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "sales");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "sales");

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "design");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(1)], "design");

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "program");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "program");

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "sales");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "sales");

        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "design");
        // moveMember(AllTeamStates[searchC(0)], AllTeamStates[searchN(2)], "design");

        moveMember(searchC(0), searchN(1), "program");
        moveMember(searchC(0), searchN(1), "program");

        moveMember(searchC(0), searchN(1), "sales");
        moveMember(searchC(0), searchN(1), "sales");

        moveMember(searchC(0), searchN(1), "design");
        moveMember(searchC(0), searchN(1), "design");

        moveMember(searchC(0), searchN(2), "program");
        moveMember(searchC(0), searchN(2), "program");

        moveMember(searchC(0), searchN(2), "sales");
        moveMember(searchC(0), searchN(2), "sales");

        moveMember(searchC(0), searchN(2), "design");
        moveMember(searchC(0), searchN(2), "design");

    }

    
}

public class TeamState{

    public Dictionary<string, int> members;
    public int mas;

}
