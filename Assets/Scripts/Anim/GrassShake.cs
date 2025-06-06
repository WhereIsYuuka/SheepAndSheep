using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;

public class GrassShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private List<RectTransform> grassTransforms;

    void Awake()
    {
        grassTransforms = new();
        foreach (Transform child in transform)
        {
            RectTransform rectTransform = child as RectTransform;
            if (rectTransform.CompareTag("Grass"))
            {
                grassTransforms.Add(rectTransform);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(ShakeGrassLoop());
    }

    private IEnumerator ShakeGrassLoop()
    {
        while (true)
        {
            ShakeGrass();
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.5f, 3f));
        }
    }

    public void ShakeGrass()
    {
        foreach (Transform grassTransform in grassTransforms)
        {
            ShakeSingleGrass(grassTransform);
        }
    }

    private void ShakeSingleGrass(Transform grassTransform)
    {
        grassTransform.DOKill();
        grassTransform.DOScale(new Vector3(0.8f, 1.2f, 1f), shakeDuration)
                .SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
    }
}