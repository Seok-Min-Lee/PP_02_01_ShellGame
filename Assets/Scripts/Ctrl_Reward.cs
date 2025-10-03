using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Reward : MonoBehaviour
{
    [SerializeField] private Image timerGuage;
    [SerializeField] private int timeLimit;


    private float timer = 0f;
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
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timerGuage.fillAmount = (timeLimit - timer) / timeLimit;

        if (timer > timeLimit)
        {
            OnClickHome();
            timer = 0f;
        }

        if (Input.anyKey)
        {
            timer = 0f;
        }
    }

    public void OnClickHome()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_TITLE);
    }
}
