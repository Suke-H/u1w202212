using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class Test : MonoBehaviour
{
    [SerializeField] private CustomButton Button100;
    [SerializeField] private CustomButton Button200;
    [SerializeField] private CustomButton Button300;
    
    void Start()
    {
        Button100.onClickCallback = () => { 
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking (100);
        };
        
        Button200.onClickCallback = () => { 
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking (200);
        };
        
        Button300.onClickCallback = () => { 
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking (300);
        };
    }

}
