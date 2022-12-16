using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    [SerializeField] GameObject BattleDialog; 


    void Start()
    {
        
    }

    public void battleEvent(TeamState teamState){
        GameObject battleDialog = Instantiate(BattleDialog) as GameObject;
    }

    void Update()
    {
        
    }
}
