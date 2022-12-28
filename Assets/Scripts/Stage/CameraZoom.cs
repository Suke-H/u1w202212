using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CameraZoom : MonoBehaviour
{
    Transform tf; //Main CameraのTransform
    Camera cam; //Main CameraのCamera

    [SerializeField] Ease moveEase;
    
    async void Start()
    {
        tf = this.gameObject.GetComponent<Transform>(); //Main CameraのTransformを取得する。
        cam = this.gameObject.GetComponent<Camera>(); //Main CameraのCameraを取得する。

        await CameraLoop();
    }

    async public UniTask cameraMove(string type){
        Debug.Log("ん＞");
        
        Vector3 direction;
        if (type == "Up"){ direction = new Vector3(0.0f,2.0f,0.0f); }
        else if (type == "Down"){ direction = new Vector3(0.0f,-2.0f,0.0f); }
        else if (type == "Left"){ direction = new Vector3(-2.0f,0.0f,0.0f); }
        // else if (type == "Right"){ direction = new Vector3(1.0f,0.0f,0.0f); }
        else { direction = new Vector3(2.0f,0.0f,0.0f); }

        // float direction = zoomIn ? -1f : 1f;
        
        await this.transform.DOBlendableMoveBy(direction, 0.5f)
        .SetEase(moveEase) // アニメーションの種類
        .SetRelative() // 相対的
        .AsyncWaitForCompletion(); // UniTask用
    }

    async UniTask CameraLoop(){

        while (true){
            await UniTask.WaitUntil(() => Input.anyKey);

            if(Input.GetKey(KeyCode.UpArrow)) { //上キーが押されていれば
                await cameraMove("Up"); //カメラを上へ移動。
            }
            else if(Input.GetKey(KeyCode.DownArrow)) { //下キーが押されていれば
                await cameraMove("Down"); //カメラを下へ移動。
            }
            if(Input.GetKey(KeyCode.LeftArrow)) { //左キーが押されていれば
                Debug.Log("左");
                await cameraMove("Left"); //カメラを左へ移動。
            }
            else if(Input.GetKey(KeyCode.RightArrow)) { //右キーが押されていれば
                Debug.Log("右");
                await cameraMove("Right"); //カメラを右へ移動。
            }
        }

    }

    // async void Update()
    // {
    //     /* カメラズーム */

    //     // if(Input.GetKey(KeyCode.I)) { //Iキーが押されていれば
    //     //     cam.orthographicSize = cam.orthographicSize - 1.0f; //ズームイン。
    //     // }
    //     // else if(Input.GetKey(KeyCode.O)) { //Oキーが押されていれば
    //     //     cam.orthographicSize = cam.orthographicSize + 1.0f; //ズームアウト。
    //     // }

    //     /* カメラ移動 */

    //     if(Input.GetKey(KeyCode.UpArrow)) { //上キーが押されていれば
    //         await cameraMove("Up"); //カメラを上へ移動。
    //     }

    //     else if(Input.GetKey(KeyCode.DownArrow)) { //下キーが押されていれば
    //         await cameraMove("Down"); //カメラを下へ移動。
    //     }
    //     if(Input.GetKey(KeyCode.LeftArrow)) { //左キーが押されていれば
    //         Debug.Log("左");
    //         await cameraMove("left"); //カメラを左へ移動。
    //     }
    //     else if(Input.GetKey(KeyCode.RightArrow)) { //右キーが押されていれば
    //         Debug.Log("右");
    //         await cameraMove("Right"); //カメラを右へ移動。
    //     }
    // }
}