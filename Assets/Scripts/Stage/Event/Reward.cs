using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    // ランダムの番号に応じて
    [SerializeField] GameObject item;
    [SerializeField] TextMeshProUGUI itemName;

    // ステージに応じて
    [SerializeField] TextMeshProUGUI descript;

    public void initialize(Sprite selectedItem, string selectedName, string selectedDescript){
        item.GetComponent<Image>().sprite = selectedItem;
        itemName.text = selectedName;
        descript.text = selectedDescript;
    }
}
