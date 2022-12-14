using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurInfo
{
    // スキル力
    public static int[] skills {get; private set;}

    // 合計メンバー
    public static int[] totalComp {get; private set;}

    public static void initialize(){
        skills = new int[]{10, 10};

        // skills = new int[]{30, 30};

        totalComp = new int[]{5, 5};
        // totalComp = new int[]{12, 22};
        // totalComp = new int[]{40, 35};
    }

    public static void skillUp(int no, int value){
        skills[no] += value;
    }

    public static void memberIncrease(int no, int value){
        totalComp[no] += value;
    }

    public static int totalScore(){
        int salesScore = skills[0] * totalComp[0];
        int engineerScore = skills[1] * totalComp[1];

        return salesScore + engineerScore;
    }

    
}
