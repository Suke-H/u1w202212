using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    public void initialize(int[] numbers, bool isNext){

        MemberState salesMember = Members[0].GetComponent<MemberState>();
        salesMember.initialize(color: "red", type: "sales", number: numbers[0], isNext: isNext);

        MemberState engineerMember = Members[1].GetComponent<MemberState>();
        engineerMember.initialize(color: "blue", type: "engineer", number: numbers[1], isNext: isNext);
    }

}
