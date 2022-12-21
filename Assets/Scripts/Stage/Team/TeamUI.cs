using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TeamUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI salesNumberText;
    [SerializeField] TextMeshProUGUI engineerNumberText;
    [SerializeField] GameObject[] plusButtons;
    [SerializeField] GameObject[] minusButtons;

    

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public bool isButtonExist { get; set; }

    public void initialize(int[] teamComp, bool isButtonExist){

        // チーム構成
        Array.Copy(teamComp, this.teamComp, teamComp.Length);
        // 文字
        salesNumberText.text = teamComp[0].ToString();
        engineerNumberText.text =  teamComp[1].ToString();

        // ボタン
        this.isButtonExist = isButtonExist;
        if (!this.isButtonExist){
            plusButtons[0].SetActive(false);
            minusButtons[0].SetActive(false);
            plusButtons[1].SetActive(false);
            minusButtons[1].SetActive(false);
        }

        
    }

}
