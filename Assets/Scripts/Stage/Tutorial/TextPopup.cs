using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class TextPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textSpace;

    public void initialize(string explain){
        textSpace.text = explain;
    }

    public void moveY(float value){
        Vector3 pos = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        pos.y += value;
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;

        // this.transform.position += new Vector3(0f, value, 0f);
    }
}
