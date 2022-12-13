using UnityEngine;

public class StageManager : MonoBehaviour
{
    // シングルトン設定ここから
    static public StageManager instance;

    private void Awake()
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // シングルトン設定ここまで
    public static string stageName = "Stage1";
    public static int stageNo = 1;

    public static void advanceStage()
    {
        stageNo += 1;
        stageName = $"Stage{stageNo}";
    }

    public static void resetStage()
    {
        stageNo = 1;
        stageName = "Stage1";
    }

    public static string getStageName(){
        return (stageName != null) ? stageName : "Stage1";
    }

    public static int getStageNo(){
        return (stageNo != 0) ? stageNo : 1;
    }

    // public static void setStageName(string name){
    //     stageName = name;
    // }

}

