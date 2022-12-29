using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class SuccessDialog : MonoBehaviour
{
    // アイテム候補
    [SerializeField] Sprite[] selectedItems;
    [SerializeField, TextArea(1,2)] string[] selectedNames;
    [SerializeField, TextArea(1,2)] string[] selectedDescripts;

    [SerializeField] CustomButton[] selectButtons;

    [SerializeField] Reward[] rewards;

    bool endFlag;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public bool isButtonExist { get; set; }

    EventManager eventManager;

    // ランダム選択
    // （0 ~ end-1）からcount個
    public List<int> randomChoice(int end, int count){
        List<int> outputs = new List<int>();

        // 初期リスト
        List<int> numbers = new List<int>();
        for (int i=0; i < end; i++) {
            numbers.Add(i);
        }

        // カウントの数だけランダム選択
        while (count-- > 0) {
            int index = Random.Range(0, numbers.Count);
            outputs.Add(numbers[index]);
            numbers.RemoveAt(index);
        }

        return outputs;
    }

    public void initialize(){

        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();

        // ランダムで3つ選択（重複なし）
        List<int> choices = randomChoice(selectedItems.Length, 3);

        Debug.Log($"{choices[0]}, {choices[1]}, {choices[2]}");

        // チームの初期化
        for (int i=0; i<rewards.Length; i++){
            int choice = choices[i];
            rewards[i].initialize(selectedItems[choice], selectedNames[choice], selectedDescripts[choice]); 
        }

        var battleEvent = GameObject.Find("EventManager").GetComponent<BattleEvent>();

        // 選択ボタン
        for (int j=0; j<selectButtons.Length; j++){
            selectButtons[j].onClickCallback = () => {
                battleEvent.rewardNo = j;
                endFlag = true;

                eventManager.rewardType = "Sales";
            };
        }

        // フラグ
        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

    
}
