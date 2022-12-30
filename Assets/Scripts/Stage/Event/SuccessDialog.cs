using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class SuccessDialog : MonoBehaviour
{
    // アイテム候補
    [SerializeField] string[] rewardTypes;
    [SerializeField] Sprite[] rewardItems;
    [SerializeField, TextArea(1,2)] string[] rewardNames;
    // [SerializeField, TextArea(1,2)] string[] rewardDescripts;

    // public List<int> rewardParams {get; set;} = new List<int>();
    // public List<string> rewardDescripts {get; set;} = new List<string>();

    [SerializeField] CustomButton[] selectButtons;
    [SerializeField] Reward[] rewards;

    bool endFlag;

    public int[] teamComp { get; set; } = new int[] { 0, 0 };
    public bool isButtonExist { get; set; }

    // [SerializeField] EventManager eventManager;
    // [SerializeField] BattleEvent battleEvent;

    EventManager eventManager;
    BattleEvent battleEvent;

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

    public string createDiscript(string rewardType, int param){
        if (rewardType == "salesMember"){
            return $"営業を\n{param}人増やす";
        }

        else if (rewardType == "engineerMember"){
            return $"エンジニアを\n{param}人増やす";
        }

        else if (rewardType == "salesSkill"){
            return $"アピール力を\n{param}増やす";
        }

        // else if (rewardType == "engineerSkill"){
        else{
            return $"技術力を\n{param}増やす";
        }

    }

    public void initialize(int[] rewardParams){
        var obj = GameObject.Find("EventManager");
        eventManager = obj.GetComponent<EventManager>();
        battleEvent = obj.GetComponent<BattleEvent>();

        // ランダムで3つ選択（重複なし）
        List<int> choices = randomChoice(rewardItems.Length, 3);
        Debug.Log($"{choices[0]}, {choices[1]}, {choices[2]}");

        // チームの初期化
        for (int i=0; i<rewards.Length; i++){
            string descript = createDiscript(rewardTypes[choices[i]], rewardParams[choices[i]]);
            rewards[i].initialize(rewardItems[choices[i]], rewardNames[choices[i]], descript); 
        }

        for (int k=0; k<4; k++){
            Debug.Log($"type: {rewardTypes[k]}, param:  {rewardParams[k]}");
        }
        Debug.Log($"{rewards.Length}, {rewardTypes.Length}");

        // 選択ボタン
        for (int j=0; j<selectButtons.Length; j++){
            selectButtons[j].onClickCallback = () => {
                // battleEvent.rewardNo = j;
                endFlag = true;

                // eventManager.rewardType = rewardTypes[j];
                // eventManager.rewardParam = rewardParams[j];
                Debug.Log($"{choices[0]}, {choices[1]}, {choices[2]}");
                Debug.Log($"{rewardTypes[0]}, {rewardTypes[1]}, {rewardTypes[2]}");
                Debug.Log($"{rewardTypes[choices[j]]}, {rewardTypes[1]}, {rewardTypes[2]}");
                eventManager.rewardType = rewardTypes[choices[j]];
                eventManager.rewardParam = rewardParams[choices[j]];
            };
        }

        // フラグ
        endFlag = false;
    }

    async public UniTask buttonWait(){
        await UniTask.WaitUntil(() => (endFlag));
    }

    
}
