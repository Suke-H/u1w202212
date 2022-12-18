using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TeamManager : MonoBehaviour
{
    [SerializeField] GameObject Member;
    [SerializeField] GameObject canvas;//キャンバス
    // public GameObject text;

    Camera mainCamera;

    void Start(){
        mainCamera = Camera.main;
    }

    // member表示（UI座標変換）
    public void displayMember(Vector2 pos){

        // ワールド座標 -> スクリーン座標変換
        var targetWorldPos = new Vector3(pos.x, pos.y, 0);
        var targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        // スクリーン座標 -> UIローカル座標変換
        RectTransform parentUI = this.gameObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentUI,
            targetScreenPos,
            mainCamera, // オーバーレイモードの場合はnull
            out var uiLocalPos // この変数に出力される
        );

        GameObject member = Instantiate(Member) as GameObject;
        member.transform.SetParent (canvas.transform, false);
        member.transform.localPosition = uiLocalPos;
        // member.name = $"team_{pos.y}_{pos.x}";

        Debug.Log(uiLocalPos);

    }

    
}


