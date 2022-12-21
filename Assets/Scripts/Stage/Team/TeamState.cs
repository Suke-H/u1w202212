using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };

    public void initialize(int[] numbers, int teamNo){
        Array.Copy(numbers, teamComp, numbers.Length);

        MemberState salesMember = Members[0].GetComponent<MemberState>();
        salesMember.initialize("red", "sales", numbers[0], teamNo);

        MemberState engineerMember = Members[1].GetComponent<MemberState>();
        engineerMember.initialize("blue", "engineer", numbers[1], teamNo);
    }

    public GameObject getMember(int i){
        return Members[i];
    }

}
