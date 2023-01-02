using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodePopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI customerName;
    [SerializeField] TextMeshProUGUI salesLv;
    [SerializeField] TextMeshProUGUI engineerLv;

    public void initialize(CustomerData customerData){
        this.customerName.text = customerData.customerName;
        salesLv.text = $"{customerData.demandLv[0]}";
        engineerLv.text = $"{customerData.demandLv[1]}";
    }
}
