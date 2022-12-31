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

    // 共通
    static public float volume = 0.7f;
    static public string beforeBGMName = "Init";

    // BGM
    [SerializeField] AudioClip normalBGM;
    [SerializeField] AudioClip lastBGM;

    void Start()
    {
        BGM = this.GetComponent<AudioSource>();
        BGMChange("Normal");
        BGM.volume = volume;
    }

    public float getVolume(){
        return volume;
    }

    public void setVolume(){
        BGM.volume = volume;
    }

    public void BGMChange(string bgmName){
        if (bgmName == "Normal" && beforeBGMName != bgmName){
            BGM.Stop(); // 停止
            BGM.clip = normalBGM;

            setVolume();

            BGM.Play(); // 再生
        }

        else if (bgmName == "Last" && beforeBGMName != bgmName){
            BGM.Stop(); // 停止
            BGM.clip = lastBGM;

            setVolume();

            // 音量下げる
            BGM.volume = volume * 0.35f;

            BGM.Play(); // 再生
        }
        
        beforeBGMName = bgmName;
    }

    public void SoundSliderOnValueChange(float newSliderValue)
	{
		// 音楽の音量をスライドバーの値に変更
        volume = newSliderValue;
        BGM.volume = volume;
	}
}

