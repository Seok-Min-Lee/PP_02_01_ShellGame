using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Title : MonoBehaviour
{
    [SerializeField] private CanvasGroup signCG;
    private void Start()
    {
        if (!AudioManager.Instance.isLoadComplete)
        {
            AudioManager.Instance.Load(() =>
            {
                AudioManager.Instance.Init(volumeBGM: 1f, volumeSFX: .2f);
                AudioManager.Instance.PlayBGM(Sound.Key.Bgm);
                AudioManager.Instance.PlayBGM(Sound.Key.BgmSfx);
            });
        }

        StaticValues.isRetry = false;
        StaticValues.shellNumber = 0;
        StaticValues.shellSpeed = 0;

        signCG.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    public void OnClickStart()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_MAIN);
    }
}
