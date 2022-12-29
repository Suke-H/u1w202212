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

        await CameraLoop();
    }

    async public UniTask cameraMove(string type){
        Vector3 direction;

        if (type == "Left"){ direction = new Vector3(-2.0f,0.0f,0.0f); }
        // else if (type == "Right"){ direction = new Vector3(1.0f,0.0f,0.0f); }
        else { direction = new Vector3(2.0f,0.0f,0.0f); }

        await this.transform.DOBlendableMoveBy(direction, 0.5f)
        .SetEase(moveEase) // アニメーションの種類
        .SetRelative() // 相対的
        .AsyncWaitForCompletion(); // UniTask用
    }

    async UniTask CameraLoop(){
        while (true){
            await UniTask.WaitUntil(() => (moveType != "None"));
            await cameraMove(moveType);

            moveType = "None";
        }
    }
}