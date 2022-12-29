using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class EventManager : MonoBehaviour
{
    BattleEvent battleEvent;

    // イベントにかかわるチーム情報管理
    int skill = 5;
    int[] enemyLevels = new int[]{3, 4};

    // 報酬
    public string rewardType {get; set;} = "None";

    public void memberReward(TeamInfo info){
        if (rewardType == "None") { return; }

        if (rewardType == "Sales") {
            info.teamComp[0] += 2;
        }

        else if (rewardType == "Engineer"){
            info.teamComp[1] += 2;
        }

        rewardType = "None";
    }

    void Start()
    {
        battleEvent = this.GetComponent<BattleEvent>();
    }

    async public UniTask eventSwitch(TeamInfo teamInfo){

        await battleEvent.BattleEventSequence(teamInfo);

    }

}
