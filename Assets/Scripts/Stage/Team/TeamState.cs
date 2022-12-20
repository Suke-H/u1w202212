using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    // public string nodeType { get; protected set; }
    // int[] currentTeamComp;
    // List<int[]> nextTeamComps = new List<int[]>();
    public int[] teamComp { get; set; } = new int[] { 0, 0 };

    // public void initialize(string nodeType, int[] numbers, int teamNo, bool isNext){
    public void initialize(int[] numbers, int teamNo, bool isNext){
        // this.nodeType = nodeType;
        // teamComp = numbers;
        Array.Copy(numbers, teamComp, numbers.Length);

        MemberState salesMember = Members[0].GetComponent<MemberState>();
        salesMember.initialize("red", "sales", numbers[0], teamNo, isNext);

        MemberState engineerMember = Members[1].GetComponent<MemberState>();
        engineerMember.initialize("blue", "engineer", numbers[1], teamNo, isNext);
    }

    public GameObject getMember(int i){
        return Members[i];
    }

    // public void set

}
