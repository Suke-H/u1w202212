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
        startFlag = false;
        teamManager.assignTeams(current, nexts);
        // await UniTask.WaitUntil(() => startFlag);

        while (true){
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

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

            // 次ノードを格納していく
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

            // 一致判定
            var duplicates = nextOrderCandidates.GroupBy(order => order)
                            .Where(order => order.Count() > 1)
                            .Select(group => group.Key).ToList();

            Debug.Log($"合流判定：{string.Join(",", duplicates)}");

            // 合流していた場合、まとめる
            // (合流するエッジがあれば、必ずその1本だけにする)
            // NG
            //     o
            // o <
            // o - o
            //

            

            // 現在チームごとに処理
            foreach (TeamInfo currentInfo in currentTeamInfos){

                // 次ノードを探索
                var nextOrders = mapManager.searchNextOrders(currentInfo.nodeOrder);

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
                    await teamAssignSequence(currentInfo, tmpTeamInfos);

                    // メンバーが一人もいないチームを削除
                    var teams = deleteNonMemberTeam(tmpTeamInfos);
                    tmpTeamInfos = new List<TeamInfo>(teams);

                    // ピン移動
                    
                    await mapManager.movePins(currentInfo.nodeOrder, tmpTeamInfos[0].nodeOrder);

                    // 次チームごとにイベント！！！！！
                    foreach (TeamInfo team in tmpTeamInfos){
                        Debug.Log("battle");
                        team.printOrder();
                        await eventManager.eventSwitch(team);
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
