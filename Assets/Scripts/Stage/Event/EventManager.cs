using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class EventManager : MonoBehaviour
{
    BattleEvent battleEvent;

    // イベントにかかわるチーム情報管理
    // int skill = 5;
    // int[] enemyLevels = new int[]{3, 4};

    // 報酬
    public string rewardType {get; set;} = "None";
    public int rewardParam {get; set;} = 0;

    public void memberReward(TeamInfo teamInfo, OurInfo ourInfo, MapData mapData){
        if (rewardType == "salesMember") {
            teamInfo.teamComp[0] += mapData.rewardParams[0];
            ourInfo.totalComp[0] += mapData.rewardParams[0];

            rewardType = "None";
        }

        else if (rewardType == "engineerMember"){
            teamInfo.teamComp[1] += mapData.rewardParams[1];
            ourInfo.totalComp[1] += mapData.rewardParams[1];

            rewardType = "None";
        }

    }

    public void skillReward(OurInfo info, MapData mapData){
        if (rewardType == "salesSkill") {
            info.skills[0] += mapData.rewardParams[2];
            rewardType = "None";
        }

        else if (rewardType == "engineerSkill"){
            info.skills[1] += mapData.rewardParams[3];
            rewardType = "None";
        }

    }

    void Start()
    {
        battleEvent = this.GetComponent<BattleEvent>();
    }

    async public UniTask eventSwitch(TeamInfo teamInfo, Node nodeInfo, OurInfo ourInfo, MapData mapData, bool lastFlag){

        Debug.Log($"event: {nodeInfo.eventType}");

        if (nodeInfo.eventType == "battle"){
            await battleEvent.BattleEventSequence(teamInfo, nodeInfo.customerData, ourInfo, mapData, lastFlag);

        }

    }

}
