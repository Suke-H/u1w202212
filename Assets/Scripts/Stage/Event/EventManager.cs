using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class EventManager : MonoBehaviour
{
    BattleEvent battleEvent;

    int[] enemyLevels = new int[]{3, 4};

    // 報酬
    public string rewardType {get; set;} = "None";
    public int rewardParam {get; set;} = 0;

    public void memberReward(TeamInfo teamInfo, MapData mapData){
        if (rewardType == "salesMember") {
            teamInfo.teamComp[0] += mapData.rewardParams[0];
            OurInfo.totalComp[0] += mapData.rewardParams[0];

            rewardType = "None";
        }

        else if (rewardType == "engineerMember"){
            teamInfo.teamComp[1] += mapData.rewardParams[1];
            OurInfo.totalComp[1] += mapData.rewardParams[1];

            rewardType = "None";
        }

    }

    public void skillReward(MapData mapData){
        if (rewardType == "salesSkill") {
            OurInfo.skills[0] += mapData.rewardParams[2];
            rewardType = "None";
        }

        else if (rewardType == "engineerSkill"){
            OurInfo.skills[1] += mapData.rewardParams[3];
            rewardType = "None";
        }

    }

    void Start()
    {
        battleEvent = this.GetComponent<BattleEvent>();
    }

    async public UniTask eventSwitch(TeamInfo teamInfo, Node nodeInfo, MapData mapData, bool lastFlag){
        Debug.Log($"event: {nodeInfo.eventType}");

        if (nodeInfo.eventType == "battle" || nodeInfo.eventType == "boss"){
            await battleEvent.BattleEventSequence(teamInfo, nodeInfo.customerData, mapData, lastFlag);
        }

        

    }

}
