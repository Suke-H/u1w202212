using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class Test : MonoBehaviour
{
    
    void Start()
    {
        // // クラスのNCMBObjectを作成
        // NCMBObject testClass = new NCMBObject("TestClass");
        //
        // // オブジェクトに値を設定
        // testClass["message"] = "Hello, NCMB!";
        //
        // // データストアへの登録
        // testClass.SaveAsync();

        // Type == Number の場合
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking (500);
    }

}
