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

    async void Start()
    {
        // 初期設定
        startButtton.onClickCallback = () => {
            startFlag = true;
        };

        // ステージ処理開始
        await StageLoop();
    }

    async UniTask StageLoop(){
        // マップ生成
        mapManager.generateMap("Stage2");
        int endNodeOrder = mapManager.nodeNum - 1;
        Debug.Log($"終了ノード: {endNodeOrder}");

        // マップ情報
        MapData mapData = mapManager.mapData;

        // マップピン生成
        mapManager.createPin(0);

        // チームの初期化
        // （チーム情報生成まで）
        var initInfo = createTeamInfo(0, initTeamComp);
        currentTeamInfos.Add(initInfo);

        // 弊社の情報
        var ourInfo = new OurInfo(){};

        // ステージ開始
        while (true){ 
            // デバッグ
            Debug.Log("==================");
            Debug.Log("現在チーム");
            foreach(var CI in currentTeamInfos){
                CI.printOrder();
            }
            
            // 現在チームごとに処理
            for (int i = 0; i < currentTeamInfos.Count; i++){

                // 次ノードを探索
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

                // 次ノードのリストを一旦作成
                var tmpTeamInfos = new List<TeamInfo>();

                // チーム情報を生成
                Debug.Log("次チーム");
                foreach (int order in nextOrders){
                    var nextInfo = createTeamInfo(order, new int[]{0, 0});
                    nextInfo.printOrder();
                    tmpTeamInfos.Add(nextInfo);
                }

                // 分岐ありの場合、チームアサイン処理へ
                if ( pattern == 2 ){
                    // 現チームの人員を全て次ノードへアサインする
                    await teamAssignSequence(currentInfo, tmpTeamInfos);

                    // メンバーが一人もいないチームを削除
                    var teams = deleteNonMemberTeam(tmpTeamInfos);
                    tmpTeamInfos = new List<TeamInfo>(teams);
                }

                else if (pattern == 0) {
                    tmpTeamInfos[0].teamComp[0] = currentInfo.teamComp[0];
                    tmpTeamInfos[0].teamComp[1] = currentInfo.teamComp[1];
                }

                else {
                    tmpTeamInfos[0].teamComp[0] = currentInfo.teamComp[0] + currentTeamInfos[i-1].teamComp[0];
                    tmpTeamInfos[0].teamComp[1] = currentInfo.teamComp[1] + currentTeamInfos[i-1].teamComp[1];
                }

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
                    var node = mapManager.NodesByOrder[nextTeam.nodeOrder];
                    var nodeInfo = node.GetComponent<Node>();
                    await eventManager.eventSwitch(nextTeam, nodeInfo, ourInfo, mapData);

                    // 報酬
                    eventManager.memberReward(nextTeam, mapData);
                    eventManager.skillReward(ourInfo, mapData);

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
