using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CameraMove : MonoBehaviour
{
    Transform tf; //Main CameraのTransform
    Camera cam; //Main CameraのCamera

    [SerializeField] Ease moveEase;
    [SerializeField] CustomButton leftButton;
    [SerializeField] CustomButton rightButton;
    [SerializeField] CustomButton upButton;
    [SerializeField] CustomButton downButton;
    [SerializeField] CustomButton positionButton;

    [SerializeField] GameManager gameManager;
    [SerializeField] TeamManager teamManager;

    [SerializeField] float gridSize;

    Vector2Int standardPos = new Vector2Int(0, 0);
    Vector2Int cameraPos = new Vector2Int(0, 0);

    bool initFlag = true;

    string moveType = "None";
    
    async void Start()
    {
        tf = this.gameObject.GetComponent<Transform>(); //Main CameraのTransformを取得する。
        cam = this.gameObject.GetComponent<Camera>(); //Main CameraのCameraを取得する。

        // 初期設定
        leftButton.onClickCallback = () => {
            moveType = "Left";
        };

        rightButton.onClickCallback = () => {
            moveType = "Right";
        };

        upButton.onClickCallback = () => {
            moveType = "Up";
        };

        downButton.onClickCallback = () => {
            moveType = "Down";
        };

        positionButton.onClickCallback = () => {
            moveType = "Reposit";
        };

        await CameraLoop();
    }

    // async public UniTask cameraAutoMove(Vector2Int currentPos, Vector2Int pastPos){
    async public UniTask cameraAutoMove(Vector2Int currentPos){

        // 初回のみ初期化
        if (initFlag){
            standardPos.x = currentPos.x;
            standardPos.y = currentPos.y;
            cameraPos.x = currentPos.x;
            cameraPos.y = currentPos.y;
            initFlag = false;
        }

        moveType = "Auto";

        // var delta = currentPos - pastPos;
        var delta = currentPos - cameraPos;
        Vector3 direction = new Vector3(delta.x*gridSize, -delta.y*gridSize, 0f);

        await this.transform.DOBlendableMoveBy(direction, 0.5f)
        .SetEase(moveEase) // アニメーションの種類
        .SetRelative() // 相対的
        .AsyncWaitForCompletion(); // UniTask用

        // 保存
        standardPos.x = currentPos.x;
        standardPos.y = currentPos.y;
        cameraPos.x = currentPos.x;
        cameraPos.y = currentPos.y;
    }

    async public UniTask cameraMove(string type){
        Vector3 direction;

        if (type == "Left"){ 
            cameraPos.x -= 1;
            direction = new Vector3(-gridSize,0.0f,0.0f);
        }
        else if (type == "Right"){ 
            cameraPos.x += 1;
            direction = new Vector3(gridSize,0.0f,0.0f); 
        }
        else if (type == "Up"){ 
            cameraPos.y -= 1;
            direction = new Vector3(0.0f,gridSize,0.0f); 
        }
        else { 
            cameraPos.y += 1;
            direction = new Vector3(0.0f,-gridSize,0.0f); }

        await this.transform.DOBlendableMoveBy(direction, 0.5f)
        .SetEase(moveEase) // アニメーションの種類
        .SetRelative() // 相対的
        .AsyncWaitForCompletion(); // UniTask用
    }

    // 元の位置に戻す
    async public UniTask repositCamera(){
        var delta = standardPos - cameraPos;

        Vector3 direction = new Vector3(delta.x*gridSize, -delta.y*gridSize, 0f);

        await this.transform.DOBlendableMoveBy(direction, 0.5f)
        .SetEase(moveEase) // アニメーションの種類
        .SetRelative() // 相対的
        .AsyncWaitForCompletion(); // UniTask用

        // 保存
        cameraPos.x = standardPos.x;
        cameraPos.y = standardPos.y;
    }

    public bool isInFocus(){
        return (cameraPos.x == standardPos.x & cameraPos.y == standardPos.y);
    }


    async UniTask CameraLoop(){
        while (true){
            // カメラの位置と現在位置がずれていたら位置修正ボタン表示
            if (isInFocus()){
                gameManager.InFocus();
                teamManager.InFocus();
                positionButton.setActive(false); 
            }
            else{ 
                positionButton.setActive(true); 
            }

            // ボタン押し or カメラ自動移動待ち
            await UniTask.WaitUntil(() => (moveType != "None"));

            if (moveType == "Reposit"){
                await repositCamera(); 
            }
            
            else if(moveType != "Auto"){
                gameManager.OutOfFocus();
                teamManager.OutOfFocus();
                await cameraMove(moveType); 
            }

            moveType = "None";
        }
    }
}