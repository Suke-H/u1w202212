using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class SEController : MonoBehaviour
{
    // シングルトン設定
    static public SEController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // シングルトン設定ここまで

    private AudioSource SE;
    static public float volume = 0.5f;
    [SerializeField] AudioClip[] SEClips;
    [SerializeField] string[] SENames;
    [SerializeField] float[] SEVolumeRates;

    void Start(){
        SE = this.GetComponent<AudioSource>();
        SE.volume = volume;

        // await TestLoop();
    }

    async UniTask TestLoop(){
        while (true){
            await UniTask.WaitUntil(() => Input.anyKey);

            if (Input.GetKey(KeyCode.A)){ playSE(SENames[0]);}
            else if (Input.GetKey(KeyCode.B)){ playSE(SENames[1]);}
            else if (Input.GetKey(KeyCode.C)){ playSE(SENames[2]);}
            else if (Input.GetKey(KeyCode.D)){ playSE(SENames[3]);}
            else if (Input.GetKey(KeyCode.E)){ playSE(SENames[4]);}
            else if (Input.GetKey(KeyCode.F)){ playSE(SENames[5]);}

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }

    void Update(){
            


        
    }

    public float getVolume(){
        return volume;
    }

    public void playSE(string name){
        List<string> SENamesList = new List<string>(SENames);
        int num = SENamesList.IndexOf(name);
        SE.volume = volume * SEVolumeRates[num];
        SE.PlayOneShot(SEClips[num]);
    }

    public void SoundSliderOnValueChange(float newSliderValue)
    {
        // 音楽の音量をスライドバーの値に変更
        volume = newSliderValue;
        SE.volume = volume;
    }

    

}