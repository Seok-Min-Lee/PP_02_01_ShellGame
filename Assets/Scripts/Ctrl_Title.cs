using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Title : MonoBehaviour
{
    [SerializeField] private CanvasGroup signCG;
    private void Start()
    {
        signCG.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    public void OnClickStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_MAIN);
    }
}
