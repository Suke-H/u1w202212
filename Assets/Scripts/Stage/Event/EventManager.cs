using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] GameObject BattleDialog; 


    void Start()
    {
        
    }

    public void battleEvent(){
        // GameObject battleDialog = Instantiate(BattleDialog) as GameObject;
        Debug.Log("バトルだよ！");
    }

    void Update()
    {
        
    }
}
