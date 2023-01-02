using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStore
{
    public static string stageName {get; set;} = "Tutorial";
    // public static string stageName {get; set;} = "test";
    // public static string stageName {get; set;} = "Stage-1";

    public static int getStageNo(){
        string[] arr = stageName.Split('-');
        int stageNo = int.Parse(arr[1]);

        return stageNo;
    }

    public static void advanceStage(){
        int stageNo = getStageNo();

        stageNo++;
        stageName = $"Stage-{stageNo}";
    }
    
}
