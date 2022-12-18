using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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