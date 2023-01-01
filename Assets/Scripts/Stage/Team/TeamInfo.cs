using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamInfo
{
    // チーム情報
    public int[] teamComp; // 構成人員

    // 現在ノード情報
    public int nodeOrder; // 番号
    public string nodeType; // 種類
    public Vector2 nodePos; // 座標

    public void plusMember(int i, int value=1){
        teamComp[i] += value;
    }

    public void minusMember(int i, int value=1){
        teamComp[i] -= value;
    }

    public void setMember(int i, int value){
        teamComp[i] = value;
    }


    public void printInfo(){
        Debug.Log($"teamComp: {teamComp[0]}, {teamComp[1]}");
        Debug.Log($"Order: {nodeOrder}, Pos: {nodePos}, Type: {nodeType}");
    }

    public void printOrder(){
        Debug.Log($"Order: {nodeOrder}");
    }   

}
