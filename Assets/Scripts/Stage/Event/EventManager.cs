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

    void Start()
    {
        battleEvent = this.GetComponent<BattleEvent>();
    }

    async public UniTask eventSwitch(int[] teamComp){
    // public void eventSwitch(int[] teamComp, string nodeType){

        battleEvent.BattleEventSequence(teamComp);

    }

}
