using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Pin : MonoBehaviour
{
    [SerializeField] Ease moveEase;
    [SerializeField] float time;

    async public UniTask move(Vector3[] path)
    {
        await this.transform.DOPath(path, time)
            .SetEase(moveEase) // アニメーションの種類
            .AsyncWaitForCompletion(); // UniTask用

    }
}
