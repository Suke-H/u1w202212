using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  

public class GameManager : MonoBehaviour
{
    public bool startFlag { get; set; } = false;

    // 初期情報
    [SerializeField] int[] initTeamComp;

    // ボタン
    [SerializeField] CustomButton startButtton;

    // マネージャー陣
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] TeamManager teamManager;
    [SerializeField] EventManager eventManager;

    // チーム情報
    private List<TeamInfo> currentTeamInfos = new List<TeamInfo>();
    private List<TeamInfo> nextTeamInfos = new List<TeamInfo>();

    // チーム情報の生成
    public TeamInfo createTeamInfo(int order, int[] teamComp){
        var (pos, nodeType) = mapGenerator.getTeamNodeInfo(order);
        
        TeamInfo teamInfo = new TeamInfo(){teamComp=teamComp, 
        nodeOrder=order, nodeType=nodeType, nodePos=pos};

        return teamInfo;
    }

    // チームアサイン待ちシークエンス
    async public UniTask teamAssignSequence(TeamInfo current, List<TeamInfo> nexts){
        startFlag = false;
        teamManager.assignTeams(current, nexts);
        // await UniTask.WaitUntil(() => startFlag);
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
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
            startFlag = true;
        };

        // ステージ処理開始
        await StageLoop();
    }

    // async UniTask eventSwitch(int[] teamComp){
    //     eventManager.eventSwitch(teamComp);
    // }

    async UniTask StageLoop(){
        // マップ生成
        mapGenerator.generateMap("Stage2");
        int endNodeOrder = mapGenerator.nodeNum - 1;
        Debug.Log($"終了ノード: {endNodeOrder}");

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

            // 現在チームごとに処理
            foreach (TeamInfo currentInfo in currentTeamInfos){

                // 次ノードを探索
                var nextOrders = mapGenerator.searchNextOrders(currentInfo.nodeOrder);

                // チーム情報を生成
                Debug.Log("次チーム");
                foreach (int order in nextOrders){
                    var nextInfo = createTeamInfo(order, new int[]{0, 0});
                    nextInfo.printOrder();
                    nextTeamInfos.Add(nextInfo);
                }

                // 次チームの数が2つ以上なら、チームアサイン処理へ
                if (nextTeamInfos.Count >= 2){
                    await teamAssignSequence(currentInfo, nextTeamInfos);

                    // メンバーが一人もいないチームを削除
                    var teams = deleteNonMemberTeam(nextTeamInfos);
                    nextTeamInfos = new List<TeamInfo>(teams);

                    // 次チームごとにイベント！！！！！
                    foreach (TeamInfo team in nextTeamInfos){
                        Debug.Log("battle");
                        team.printOrder();
                        await eventManager.eventSwitch(team);
                    }

                } 

                // // 次チームごとにイベント開始
                // foreach(TeamState teamState in CurrentTeamStates){
                //     eventSwitch(teamState);
                //     await eventSwitch(new int[]{8, 8});

                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            }

            // 次チームを現在チームに
            currentTeamInfos = new List<TeamInfo>(nextTeamInfos);
            nextTeamInfos = new List<TeamInfo>();

            if (currentTeamInfos[0].nodeOrder == 8){
                Debug.Log("ごーーーーーる");
                break;
            }



        }
    }

}
