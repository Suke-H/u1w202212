using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string eventType;// {get; set;}
    public CustomerData customerData;// {get; set;}
    public Vector2 pos;

    Camera mainCamera;
    TeamManager teamManager;

    public void initialize(string eventType){
        this.eventType = eventType;

        mainCamera = Camera.main;
        pos.x = this.transform.position.x;
        pos.y = this.transform.position.y;

        teamManager = GameObject.Find("TeamManager").GetComponent<TeamManager>();
    }

    public void initializeBattle(CustomerData customerData){
        this.customerData = customerData;
    }

    void OnMouseEnter(){
        if (eventType == "battle"){
            // customerData.printData();
            teamManager.displayCustomer(pos, customerData);
        }
        
    }

    void OnMouseExit(){
        if (eventType == "battle"){
            teamManager.destroyCustomer();
        }
    }
}
