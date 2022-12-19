using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    public void initialize(int[] numbers, int teamNo, bool isNext){

        MemberState salesMember = Members[0].GetComponent<MemberState>();
        // salesMember.initialize(color: "red", type: "sales", number: numbers[0], isNext: isNext);
        salesMember.initialize("red", "sales", numbers[0], teamNo, isNext);

        MemberState engineerMember = Members[1].GetComponent<MemberState>();
        // engineerMember.initialize(color: "blue", type: "engineer", number: numbers[1], isNext: isNext);
        engineerMember.initialize("blue", "engineer", numbers[1], teamNo, isNext);
    }

    public GameObject getMember(int i){
        return Members[i];
    }

}
