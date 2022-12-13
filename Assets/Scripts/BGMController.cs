using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    // シングルトン設定
    static public BGMController instance;

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

    private static AudioSource BGM;
    private static string beforeState = "TitleOrStageSelect";
    static public float volume = 0.7f;

    // BGM
    [SerializeField] AudioClip BGMOnTitle;
    [SerializeField] AudioClip BGMOnStage;
    // [SerializeField] AudioClip BGMOnPuzzle123;
    // [SerializeField] AudioClip BGMOnPuzzle4;

    void Start()
    {
        Debug.Log("どう？");
        BGM = this.GetComponent<AudioSource>();
        BGMChange(BGMOnTitle);
        BGM.volume = volume;

        //シーンが切り替わった時に呼ばれるメソッドを登録
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    public float getVolume(){
        return volume;
    }

    // public void playBGM(){
    //     BGM.Play(); // 再生
    // }
    //
    // public void stopBGM(){
    //     BGM.Stop(); // 停止
    // }

    void BGMChange(AudioClip BGMClip){
        BGM.Stop(); // 停止
        BGM.clip = BGMClip;
        BGM.Play(); // 再生
    }

    void OnActiveSceneChanged(Scene beforeScene, Scene afterScene){
        // シーン状態はTitleOrStage, Puzzle123, Puzzle4の3つ
        string afterState;

        //  
        if (afterScene.name == "Stage")
        {
            afterState = "Stage";
        }

        else {
            afterState = "TitleOrStageSelect";
        }

        Debug.Log($"{beforeState} -> {afterState}");

        ////// BGM遷移開始 //////

        // 前がステージ1,2,3の場合
        if (beforeState == "Stage"){
            if (afterState == "TitleOrStage"){ BGMChange(BGMOnTitle); } // タイトルorステージ選択 
            //else{} // 同じ状態ならBGMはそのまま
        }

        // 前がタイトルorステージ選択の場合
        else{
            if (afterState == "Stage"){ BGMChange(BGMOnStage); } // ステージ
            //else{} // 同じ状態ならBGMはそのまま
        }

        // 保存
        beforeState = afterState;
    }

    public void SoundSliderOnValueChange(float newSliderValue)
	{
		// 音楽の音量をスライドバーの値に変更
        volume = newSliderValue;
        BGM.volume = volume;
	}
}

