using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Pin : MonoBehaviour
{

    [SerializeField] Ease moveEase;
    [SerializeField] float time;

    // async public UniTask move(Vector2 currentPos, Vector2 nextPos)
    // {
    //     Vector3 currentPos3 = new Vector3(currentPos.x, currentPos.y, 0f);
    //     Vector3 nextPos3 = new Vector3(nextPos.x, nextPos.y, 0f);
    //     Vector3 direction = nextPos3 - currentPos3;

    //     await this.transform.DOBlendableMoveBy(direction, time)
    //         .SetEase(moveEase) // アニメーションの種類
    //         .SetRelative() // 相対的
    //         .AsyncWaitForCompletion(); // UniTask用
    // }

    async public UniTask move(Vector3[] path)
    {

        await this.transform.DOPath(path, time)
            .SetEase(moveEase) // アニメーションの種類
            .AsyncWaitForCompletion(); // UniTask用
    }
}
