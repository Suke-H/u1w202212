using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string eventType;// {get; set;}

    public CustomerData customerData;// {get; set;}

    public void initialize(string eventType){
        this.eventType = eventType;
    }

    public void initializeBattle(CustomerData customerData){
        this.customerData = customerData;
    }
}
