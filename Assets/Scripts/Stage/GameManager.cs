using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

public class GameManager : MonoBehaviour
{

    // 初期情報
    [SerializeField] int[] initTeamComp;

    // ボタン、フラグ系
    [SerializeField] CustomButton startButtton;
    public bool startFlag { get; set; } = false;
    // public bool isAssignComplete {get; set;} = false;

    // マネージャー陣
    [SerializeField] MapManager mapManager;
    [SerializeField] TeamManager teamManager;
    [SerializeField] EventManager eventManager;

    // チーム情報
    private List<TeamInfo> currentTeamInfos = new List<TeamInfo>();
    private List<TeamInfo> nextTeamInfos = new List<TeamInfo>();

    // チーム情報の生成
    public TeamInfo createTeamInfo(int order, int[] teamComp){
        var (pos, nodeType) = mapManager.getTeamNodeInfo(order);
        
        TeamInfo teamInfo = new TeamInfo(){teamComp=teamComp, 
        nodeOrder=order, nodeType=nodeType, nodePos=pos};

        return teamInfo;
    }

    // チームアサイン待ちシークエンス
    async public UniTask teamAssignSequence(TeamInfo current, List<TeamInfo> nexts){
        // startFlag = false;
        teamManager.assignTeams(current, nexts);

        // 現チームの全ての人員を割り振り出来ていなかったらループ
        while (true){
            startFlag = false;
            await UniTask.WaitUntil(() => startFlag);

            if (current.teamComp[0] == 0 & current.teamComp[1] == 0){
                break;
            }

        }

        teamManager.destroyTeams();
    }

    // 無人チームを削除
    public List<TeamInfo> deleteNonMemberTeam(List<TeamInfo> teams){
        int i = 0;
        while (true){
            if (i >= teams.Count){ break; }

            var comp = teams[i].teamComp;
            if (comp[0] == 0 & comp[1] == 0){
                teams.RemoveAt(i);
                Debug.Log($"{i}削除");
                continue;
            }
            i++;
        }

        return teams;
    }

    public void margeCurrentInfo(){
        var margeGroup = margeJudge();

        foreach (List<int> group in margeGroup){
            foreach (int index in group){

            }
        }
    }

    // 現ノードたちがに合流（マージ）するものがあるか判定
    // (前提のマップ仕様として、合流するエッジがあれば、必ずその1本だけにする)
    //
    // OK
    // o
    //   ＼  
    // o -  o
    //
    // NG
    //     o
    // o <
    // o - o
    //
    public List<List<int>> margeJudge(){

        // 
        var margeGroup = new List<List<int>>();

        /* 現ノード達からの全ての次ノード候補の番号をリスト化 */
        var nextOrderCandidates = new List<int>(){};
        foreach (TeamInfo currentInfo in currentTeamInfos){

            // 次ノードを探索
            var nextOrders = mapManager.searchNextOrders(currentInfo.nodeOrder);

            // 分岐があった場合、無視(-1を挿入)
            if (nextOrders.Count > 1){
                nextOrderCandidates.Add(-1);
            }

            // 分岐がない1本道の場合、ノード番号を追加
            else {
                nextOrderCandidates.Add(nextOrders[0]);
            }
        }

        /* /////////////////////////// */

        // GroupByを使用して、要素ごとにグループ化する
        var groups = nextOrderCandidates.Select((x, i) => new { Value = x, Index = i })
                                        .GroupBy(x => x.Value);

        // グループを回して、各グループ内の要素とインデックスを出力する
        foreach (var group in groups)
        {
            Debug.Log($"Value: {group.Key}");
            // -1なら終了
            if (group.Key == -1) { break; }

            // 同じ要素のインデックスを格納していく
            List<int> margeOneGroup = new List<int>();
            foreach (var item in group)
            {
                Debug.Log($"Index: {item.Index}");
                margeOneGroup.Add(item.Index);
            }

            // 重複があればマージグループとして追加
            if (margeOneGroup.Count > 1){
                margeGroup.Add(margeOneGroup);
            }
        }

        return margeGroup;
    }

    async void Start()
    {
        // 初期設定
        startButtton.onClickCallback = () => {
            // if (condition){
                startFlag = true;
            // }
        };

        // ステージ処理開始
        await StageLoop();
    }

    async UniTask StageLoop(){
        // マップ生成
        mapManager.generateMap("Stage2");
        int endNodeOrder = mapManager.nodeNum - 1;
        Debug.Log($"終了ノード: {endNodeOrder}");

        // マップピン生成
        mapManager.createPin(0);

        // チームの初期化
        // （チーム情報生成まで）
        var initInfo = createTeamInfo(0, initTeamComp);
        currentTeamInfos.Add(initInfo);

        // ステージ開始
        while (true){ 
            // デバッグ
            Debug.Log("==================");
            Debug.Log("現在チーム");
            foreach(var CI in currentTeamInfos){
                CI.printOrder();
            }

            // 合流チームがいないか判定
            // var margeGroup = margeJudge();
            // ListUtils listUtils =new ListUtils();
            // listUtils.printMap(margeGroup, "marge");


            // 現在チームごとに処理
            // foreach (TeamInfo currentInfo in currentTeamInfos){
            for (int i = 0; i < currentTeamInfos.Count; i++){

                // 次ノードを探索
                // var nextOrders = mapManager.searchNextOrders(currentInfo.nodeOrder);
                var nextOrders = mapManager.searchNextOrders(currentTeamInfos[i].nodeOrder);

                // 0. 分岐なし
                // 1. 分岐なし、合流あり
                // 2. 分岐あり
                int pattern;

                // 分岐がなかったら合流判定
                if (nextOrders.Count == 1){
                    if (i != currentTeamInfos.Count - 1){
                        var nearCurrentOrders = mapManager.searchNextOrders(currentTeamInfos[i+1].nodeOrder);

                        // 合流するならiを1つスキップ
                        if (nextOrders[0] == nearCurrentOrders[0]) { 
                            pattern = 1;
                            i++;
                            }
                        else { pattern = 0; }
                    }

                    else { pattern = 0; }
                }

                // 分岐があったら2
                else { pattern = 2; }

                // 現在ノードの情報
                var currentInfo = currentTeamInfos[i];
                // if (pattern == 1) { var nearCurrentInfo = currentTeamInfos[i-1]; }

                // 次ノードのリストを一旦作成
                var tmpTeamInfos = new List<TeamInfo>();

                // チーム情報を生成
                Debug.Log("次チーム");
                foreach (int order in nextOrders){
                    var nextInfo = createTeamInfo(order, new int[]{0, 0});
                    nextInfo.printOrder();
                    tmpTeamInfos.Add(nextInfo);
                }

                // 次チームの数が2つ以上なら、チームアサイン処理へ
                if (tmpTeamInfos.Count >= 2){
                    // 現チームの人員を全て次ノードへアサインする
                    await teamAssignSequence(currentInfo, tmpTeamInfos);

                    // メンバーが一人もいないチームを削除
                    var teams = deleteNonMemberTeam(tmpTeamInfos);
                    tmpTeamInfos = new List<TeamInfo>(teams);
                }

                // 増えたチーム分、現ノードにマップピン追加
                // for (int i=0; i<tmpTeamInfos.Count-1; i++){
                //     mapManager.createPin(currentInfo.nodeOrder);
                // }

                // 必要なピンの数
                int pinCount = tmpTeamInfos.Count;

                // 次チームごとにイベント!
                foreach (TeamInfo nextTeam in tmpTeamInfos){
                    // ログ
                    Debug.Log("battle");
                    nextTeam.printOrder();

                    // ピン移動
                    if (pattern == 1){
                        await UniTask.WhenAll(
                            mapManager.movePins(currentInfo.nodeOrder, nextTeam.nodeOrder),
                            mapManager.movePins(currentTeamInfos[i-1].nodeOrder, nextTeam.nodeOrder)
                        );
                        mapManager.deletePin(currentInfo.nodeOrder);
                        mapManager.deletePin(currentTeamInfos[i-1].nodeOrder);
                        mapManager.createPin(nextTeam.nodeOrder);
                    }

                    else {
                        await mapManager.movePins(currentInfo.nodeOrder, nextTeam.nodeOrder);

                        mapManager.deletePin(currentInfo.nodeOrder);
                        mapManager.createPin(nextTeam.nodeOrder);
                    }

                    // イベント開始
                    await eventManager.eventSwitch(nextTeam);

                    // 次ノードが残っていたら現ノードにピンを追加
                    if (--pinCount > 0){
                        Debug.Log($"pinCount: {pinCount}");
                        mapManager.createPin(currentInfo.nodeOrder);
                    }
                }

                nextTeamInfos.AddRange(tmpTeamInfos);

            }

            // 次チームを現在チームに
            currentTeamInfos = new List<TeamInfo>(nextTeamInfos);
            nextTeamInfos = new List<TeamInfo>();

            if (currentTeamInfos[0].nodeOrder == mapManager.nodeNum - 1){
                Debug.Log("ごーーーーーる");
                break;
            }

        }
    }

}
