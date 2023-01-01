using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamState : MonoBehaviour
{
    [SerializeField] GameObject[] Members;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public int teamNo { get; set; }

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

    public void plusTeam(int No, int value=1){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.plusMembers(value);
        teamComp[No] += value;
    }

    public void minusTeam(int No, int value=1){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.minusMembers(value);
        teamComp[No] -= value;
    }

    public void setTeamMember(int No, int value){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.setMembers(value);
        teamComp[No] = value;
    }

    public void setLv(int No, int level){
        MemberState ms = Members[No].GetComponent<MemberState>();
        ms.setLv(level);
    }

}
