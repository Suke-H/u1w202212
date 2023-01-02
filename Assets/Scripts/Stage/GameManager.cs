using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using NCMB;

public class GameManager : MonoBehaviour
{
    // ボタン、フラグ系
    [SerializeField] CustomButton startButtton;
    public bool startFlag { get; set; } = false;
    public bool clearFlag { get; set;} = false;
    public bool whistleFlag { get; set;} = false;

    // アクティブ切替したいボタン
    [SerializeField] CustomButton[] Buttons;

    // マネージャー陣
    [SerializeField] MapManager mapManager;
    [SerializeField] TeamManager teamManager;
    [SerializeField] EventManager eventManager;
    [SerializeField] BattleEvent battleEvent;
    [SerializeField] CameraMove cameraMove;
    [SerializeField] Tutorial tutorial;

    // BGM, SE
    [SerializeField] BGMController BGM;
    [SerializeField] SEController SE;

    // ダイアログ
    [SerializeField] GameObject ClearDialog;
    [SerializeField] GameObject OverDialog;
    [SerializeField] GameObject FinishDialog;
    [SerializeField] GameObject canvas;//キャンバス

    // チーム情報
    private List<TeamInfo> currentTeamInfos = new List<TeamInfo>();
    private List<TeamInfo> nextTeamInfos = new List<TeamInfo>();

    [SerializeField] TextMeshProUGUI[] levelTexts;

    // ステージ情報
    private string stageName;

    // チーム情報の生成
    public TeamInfo createTeamInfo(int order, int[] teamComp){
        var (pos, nodeType) = mapManager.getTeamNodeInfo(order);
        
        TeamInfo teamInfo = new TeamInfo(){teamComp=new int[]{teamComp[0], teamComp[1]}, 
        nodeOrder=order, nodeType=nodeType, nodePos=pos};

        return teamInfo;
    }

    // チームアサイン待ちシークエンス
    async public UniTask teamAssignSequence(TeamInfo current, List<TeamInfo> nexts){
        teamManager.assignTeams(current, nexts);

        // チュートリアル（ルールその3）
        await tutorial.TutoRule3();

        // 入力待ち
        while (true){
            startFlag = false;
            await UniTask.WaitUntil(() => startFlag);

            // 現チームの全ての人員を割り振り出来ていなかったらループ
            if (current.teamComp[0] == 0 & current.teamComp[1] == 0){
                break;
            }
        }

        teamManager.destroyTeams();
        teamManager.destroyButtons();
    }

    // 無人チームを削除
    public List<TeamInfo> deleteNonMemberTeam(List<TeamInfo> teams){
        int i = 0;
        while (true){
            if (i >= teams.Count){ break; }

            var comp = teams[i].teamComp;
            if (comp[0] == 0 & comp[1] == 0){
                teams.RemoveAt(i);
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

    async public UniTask gameEnd(string endType){
        // 終了ダイアログ表示
        GameObject finishDialog = Instantiate(FinishDialog) as GameObject;
        finishDialog.transform.SetParent (canvas.transform, false);

        FinishDialog FD = finishDialog.GetComponent<FinishDialog>();
        FD.initialize(endType);

        // ランキング
        if (endType != "Tutorial"){
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking (OurInfo.totalScore());
        }

        // ボタン押し待ち
        await FD.buttonWait();
        
        // タイトルへ 
        SceneManager.LoadScene("Title");
    }

    async public UniTask stageOver(){
        // ゲームオーバーダイアログ表示
        GameObject overDialog = Instantiate(OverDialog) as GameObject;
        overDialog.transform.SetParent (canvas.transform, false);

        OverDialog OD = overDialog.GetComponent<OverDialog>();
        OD.initialize();

        // ランキング
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking (OurInfo.totalScore());

        // ボタン押し待ち
        await OD.buttonWait();

        // タイトルへ
        SceneManager.LoadScene("Title");
    }

    async public UniTask stageClear(){
        // SE
        SE.playSE("congrat");

        // チュートリアルであれば終了
        if (stageName == "Tutorial"){
            await gameEnd("Tutorial");
        }

        // 通常のゲーム
        else {
            // ラストステージなら終了
            if (StageStore.getStageNo() == 3){
            // if (StageStore.getStageNo() == 1){
                await gameEnd("AllClear");
            }

            // 次のステージへ
            else {
                // クリアダイアログ表示
                GameObject clearDialog = Instantiate(ClearDialog) as GameObject;
                clearDialog.transform.SetParent (canvas.transform, false);

                ClearDialog CD = clearDialog.GetComponent<ClearDialog>();
                CD.initialize();

                // ボタン押し待ち
                await CD.buttonWait();
                Destroy(clearDialog);

                // 辞退なら終了
                if (!CD.nextFlag){
                    await gameEnd("GiveUp");
                }

                // 次のステージへ
                StageStore.advanceStage();
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
            SE.playSE("click");
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
        // if (stageName == "Stage-1" || stageName == "Tutorial" || stageName == "test"){
        if (stageName == "Stage-1" || stageName == "Tutorial" || stageName == "Stage-2" || stageName == "Stage-3"){
            OurInfo.initialize();
        }

        // スキル表示
        levelTexts[0].text = $"{OurInfo.skills[0]}";
        levelTexts[1].text = $"{OurInfo.skills[1]}";

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

        // チュートリアル（最初）
        await tutorial.TutoStart();

        // ステージ開始
        while (true){ 
            // デバッグ
            Debug.Log("==================");
            Debug.Log("現在チーム");
            // foreach(var CI in currentTeamInfos){
            //     CI.printOrder();
            // }

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
                else { 
                    pattern = 2; 
                }

                // カメラ移動
                // if (pattern != 2){
                Vector2Int currentPos = mapManager.searchNodePos(currentTeamInfos[i].nodeOrder);
                // await cameraMove.cameraAutoMove(currentPos, pastPos);
                await cameraMove.cameraAutoMove(currentPos);
                pastPos.x = currentPos.x;
                pastPos.y = currentPos.y;
                // }

                // 現在ノードの情報
                var currentInfo = currentTeamInfos[i];

                // 次ノードのリストを一旦作成
                var tmpTeamInfos = new List<TeamInfo>();

                // チーム情報を生成
                foreach (int order in nextOrders){
                    var nextInfo = createTeamInfo(order, new int[]{0, 0});
                    // nextInfo.printOrder();
                    tmpTeamInfos.Add(nextInfo);
                }

                // 分岐ありの場合、チームアサイン処理へ
                if ( pattern == 2 ){

                    // カメラ（分岐が⤴→か⤵→かで変更）
                    int currentY = mapManager.searchNodePos(currentInfo.nodeOrder).y;
                    int nextY = mapManager.searchNodePos(tmpTeamInfos[1].nodeOrder).y;

                    // ⤵→の場合、カメラを1つ下げる
                    if (currentY+1 == nextY){
                        await cameraMove.cameraMove("Down");
                    }

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

                    teamManager.assignCurrentUnionTeams(currentTeamInfos[i-1], currentInfo);
                }

                // 必要なピンの数
                int pinCount = tmpTeamInfos.Count;

                // 行動開始のボタン押し待ち
                await UniTask.WaitUntil(() => (startFlag));
                startFlag = false;

                // 次チームごとにイベント!
                foreach (TeamInfo nextTeam in tmpTeamInfos){
                    // ログ
                    // nextTeam.printOrder();

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

                    /* イベント開始 */

                    // ボタンを一旦無効化
                    foreach (CustomButton button in Buttons){
                        button.setActive(false);
                    }

                    var node = mapManager.NodesByOrder[nextTeam.nodeOrder];
                    var nodeInfo = node.GetComponent<Node>();
                    await eventManager.eventSwitch(nextTeam, nodeInfo, mapData, lastFlag);

                    // 報酬を反映
                    eventManager.memberReward(nextTeam, mapData);
                    eventManager.skillReward(mapData);

                    // チュートリアル（ルールその2-2）
                    await tutorial.TutoRule2_2();

                    // スキル表示
                    levelTexts[0].text = $"{OurInfo.skills[0]}";
                    levelTexts[1].text = $"{OurInfo.skills[1]}";

                    // 最終ノードだったらステージ終了
                    if (lastFlag){ break; }

                    // ボタンを有効に戻す
                    foreach (CustomButton button in Buttons){
                        button.setActive(true);
                    }

                    // 次ノードが残っていたら現ノードにピンを追加
                    if (--pinCount > 0){
                        mapManager.createPin(currentInfo.nodeOrder);
                    }
                }

                // チュートリアル（終わりフェイク）
                if (pattern == 2) { await tutorial.TutoFakeEnd(); }

                nextTeamInfos.AddRange(tmpTeamInfos);
            }

            // 次チームを現在チームに
            currentTeamInfos = new List<TeamInfo>(nextTeamInfos);
            nextTeamInfos = new List<TeamInfo>();

            // 終了ノードにたどり着いたらクリア
            if (currentTeamInfos[0].nodeOrder == endNodeOrder){
                break;
            }

        }
    }

}
