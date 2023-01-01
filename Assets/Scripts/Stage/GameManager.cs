using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 初期情報
    // [SerializeField] int[] initTeamComp;

    // ボタン、フラグ系
    [SerializeField] CustomButton startButtton;
    public bool startFlag { get; set; } = false;
    public bool clearFlag {get; set;} = false;

    // マネージャー陣
    [SerializeField] MapManager mapManager;
    [SerializeField] TeamManager teamManager;
    [SerializeField] EventManager eventManager;
    [SerializeField] BattleEvent battleEvent;
    [SerializeField] CameraMove cameraMove;

    // BGM, SE
    [SerializeField] BGMController BGM;

    // ダイアログ
    [SerializeField] GameObject ClearDialog;
    [SerializeField] GameObject OverDialog;
    [SerializeField] GameObject canvas;//キャンバス

    // チーム情報
    private List<TeamInfo> currentTeamInfos = new List<TeamInfo>();
    private List<TeamInfo> nextTeamInfos = new List<TeamInfo>();

    // ステージ情報
    private string stageName;

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

    public void OutOfFocus(){
        startButtton.setActive(false);
    }

    public void InFocus(){
        startButtton.setActive(true);
    }

    async public UniTask stageOver(){
        // ダイアログ表示
        GameObject overDialog = Instantiate(OverDialog) as GameObject;
        overDialog.transform.SetParent (canvas.transform, false);

        OverDialog OD = overDialog.GetComponent<OverDialog>();
        OD.initialize();

        // ランキング
        int totalMemberNum = OurInfo.totalComp[0] + OurInfo.totalComp[1];
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking (totalMemberNum);

        await OD.buttonWait();
        Destroy(overDialog);
    }

    async public UniTask stageClear(){
        // ダイアログ表示
        // GameObject clearDialog = Instantiate(ClearDialog) as GameObject;
        // clearDialog.transform.SetParent (canvas.transform, false);

        // ClearDialog CD = clearDialog.GetComponent<ClearDialog>();
        // CD.initialize();

        // await CD.buttonWait();
        // Destroy(clearDialog);

        // チュートリアルであれば終了
        if (stageName == "Tutorial"){
            SceneManager.LoadScene("Title");
        }

        // 通常のゲーム
        else {
            string[] arr = stageName.Split('-');
            int stageNo = int.Parse(arr[1]);

            // ラストステージなら終了
            if (stageNo == 3){
                SceneManager.LoadScene("Clear");
            }

            // 次のステージへ
            else {
                stageNo++;
                stageName = $"Stage-{stageNo}";
                StageStore.stageName = stageName;

                SceneManager.LoadScene("Stage");
            }
        }
    }

    // 現ノードたちがに合流（マージ）するものがあるか判定
    // 
    // *前提のマップ仕様
    // 1.ノードAからノードBへ合流するエッジがあれば、Aからのエッジは必ずその1本だけにする
    // 2.合流エッジは2本のみ。3本以上にはしない
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
        // ボタン
        startButtton.onClickCallback = () => {
            startFlag = true;
        };

        // ステージ名取得
        stageName = StageStore.stageName;
        Debug.Log($"stagename: {stageName}");

        // マップ生成
        mapManager.generateMap(stageName);

        // BGM
        BGM.BGMChange("Normal");

        // 最初のみ弊社情報を初期化
        if (stageName == "Stage-1"){
            OurInfo.initialize();
        }

        // ステージ処理開始
        await StageLoop();

        // ゲームエンド処理
        if (battleEvent.successFlag){ await stageClear(); }
        else { await stageOver(); }
    }

    async UniTask StageLoop(){

        // カメラ移動用の過去位置
        var pastPos = mapManager.searchNodePos(0);

        // 終了ノードの番号
        int endNodeOrder = mapManager.nodeNum - 1;
        Debug.Log($"終了ノード: {endNodeOrder}");

        // マップ情報
        MapData mapData = mapManager.mapData;

        // マップピン生成
        mapManager.createPin(0);

        // チームの初期化
        // （チーム情報生成まで）
        var initInfo = createTeamInfo(0, OurInfo.totalComp);
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
            for (int i = 0; i < currentTeamInfos.Count; i++){

                // カメラ移動
                var currentPos = mapManager.searchNodePos(currentTeamInfos[i].nodeOrder);
                await cameraMove.cameraMovePos(currentPos, pastPos);
                pastPos.x = currentPos.x;
                pastPos.y = currentPos.y;
                // Debug.Log($"past: ({pastPos.x},{pastPos.y}), current: ({currentPos.x},{currentPos.y})");

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
                Debug.Log($"次チーム: {nextOrders.Count}");
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

                    teamManager.assignCurrentTeams(currentInfo);
                }

                else {
                    tmpTeamInfos[0].teamComp[0] = currentInfo.teamComp[0] + currentTeamInfos[i-1].teamComp[0];
                    tmpTeamInfos[0].teamComp[1] = currentInfo.teamComp[1] + currentTeamInfos[i-1].teamComp[1];

                    teamManager.assignCurrentTeams(currentInfo);
                }

                // 必要なピンの数
                int pinCount = tmpTeamInfos.Count;

                // 行動開始のボタン押し待ち
                await UniTask.WaitUntil(() => (startFlag));
                startFlag = false;

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

                    // 最終ノードの処理
                    var lastFlag = (nextTeam.nodeOrder == endNodeOrder);
                    if (lastFlag){
                        BGM.BGMChange("Last"); // 最終ノードのBGM
                    }

                    // イベント開始
                    var node = mapManager.NodesByOrder[nextTeam.nodeOrder];
                    var nodeInfo = node.GetComponent<Node>();
                    await eventManager.eventSwitch(nextTeam, nodeInfo, mapData, lastFlag);

                    // 報酬を反映
                    eventManager.memberReward(nextTeam, mapData);
                    eventManager.skillReward(mapData);

                    // 最終ノードだったらステージ終了
                    if (lastFlag){ break; }

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

            // 終了ノードにたどり着いたらクリア
            if (currentTeamInfos[0].nodeOrder == endNodeOrder){
                Debug.Log("ごーーーーーる");
                break;
            }

        }
    }

}
