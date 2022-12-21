using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public int teamNo { get; set; }

    // public TeamCond teamCond = new TeamCond(teamComp=)

    public void initialize(int[] teamComp, int teamNo){

        // チーム構成
        Array.Copy(teamComp, this.teamComp, teamComp.Length);

        // チームNo
        this.teamNo = teamNo;

        // メンバー生成
        MemberState salesMember = Members[0].GetComponent<MemberState>();
        salesMember.initialize("red", "sales", teamComp[0], teamNo);

        MemberState engineerMember = Members[1].GetComponent<MemberState>();
        engineerMember.initialize("blue", "engineer", teamComp[1], teamNo);
    }

    public void plusTeam(int No){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.plusMembers();
    }

    public void minusTeam(int No){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.minusMembers();
    }

    // public GameObject getMember(int i){
    //     return Members[i];
    // }

}
