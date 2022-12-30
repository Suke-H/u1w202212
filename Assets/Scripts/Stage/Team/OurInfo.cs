using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurInfo
{
    // スキル力
    public int[] skills {get; set;} = new int[]{10, 10};

    public void skillUp(int no, int value){
        skills[no] += value;
    }   
}
