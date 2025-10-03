using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct ColorPair
{
    public Color first;
    public Color second;
}

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(CanvasGroup))]
public class Prompter : MonoBehaviour
{
    public enum Type
    {
        None,
        Ready,
        Go,
        Next,
    }

    [SerializeField] private ColorPair[] colors;
    [SerializeField] private Type type;

    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(Type type)
    {
        if (type == Type.None)
        {
            gameObject.SetActive(false);
            return;
        }

        text.text = type.ToString().ToUpper();
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;

        ColorPair colorPair = colors[(int)type - 1];
        text.colorGradient = new VertexGradient(
            colorPair.first, 
            colorPair.first, 
            colorPair.second, 
            colorPair.second
        );

        switch (type)
        {
            case Type.Ready:
                gameObject.SetActive(true);
                Contract();
                break;
            case Type.Go:
                gameObject.SetActive(true);
                Expand();
                break;
            case Type.Next:
                gameObject.SetActive(true);
                Yoyo();
                break;
        }
    }
    private void Contract()
    {
        Vector3 scale = Vector3.one * .8f;

        transform.DOScale(scale, 1f);
    }
    private void Expand()
    {
        Vector3 scale = Vector3.one * 10f;

        transform.DOScale(scale, 1f);
        canvasGroup.DOFade(0f, 1f);
    }
    private void Yoyo()
    {
        Sequence seq = DOTween.Sequence();

        canvasGroup.alpha = 0f;
        seq.Append(canvasGroup.DOFade(1f, .2f));
        seq.Append(transform.DOScale(1.2f, .2f).SetLoops(4, LoopType.Yoyo));
    }
}
