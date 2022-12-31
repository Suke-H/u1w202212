using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurInfo
{
    // スキル力
    public static int[] skills {get; set;}

    // 合計メンバー
    public static int[] totalComp {get; set;}

    public static void initialize(){
        skills = new int[]{10, 10};
        totalComp = new int[]{5, 5};

    }

    public static void skillUp(int no, int value){
        skills[no] += value;
    }

    public static void memberIncrease(int no, int value){
        totalComp[no] += value;
    }
}
