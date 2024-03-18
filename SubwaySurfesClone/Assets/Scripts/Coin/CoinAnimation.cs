using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinAnimation : MonoBehaviour
{
    public float rotationDuration = 1f;
    private Tween coinTween;

    private void Start()
    {
        RotateCoin();
    }

    private void OnDestroy()
    {
        if (coinTween != null)
        {
            coinTween.Kill();
        }
    }

    void RotateCoin()
    {
        if (transform == null)
        {
            return;
        }

        coinTween = transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}
