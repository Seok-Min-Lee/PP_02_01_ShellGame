using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Main : MonoBehaviour
{
    public enum State
    {
        Wait,
        Play,
        Checkpoint,
        End
    }

    [Header("UI")]
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject finishPopup;
    [SerializeField] private Prompter prompter;

    [SerializeField] private Image[] roundFeedbacks;
    [SerializeField] private TextMeshProUGUI choiceTimerText;
    [SerializeField] private TextMeshProUGUI finishTimerText;

    [Header("Component")]
    [SerializeField] private ShellMixer shellMixer;

    [Header("Setting")]
    [SerializeField] private int roundMax;
    [SerializeField] private int choiceTime;
    [SerializeField] private int retryTime;

    private State state;
    private int roundCount = 1;
    private int rightCount = 0;
    private Coroutine coroutine = null;
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

        Init();
    }
    private void Update()
    {
        if (state == State.Wait && Input.GetKeyDown(KeyCode.Tab))
        {
            GameStart();
        }

        if (state == State.Checkpoint && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Click);
            Choose();
        }
    }
    public void OnClickStart()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Click);
            tutorialPopup.SetActive(false);

            AudioManager.Instance.PlaySFX(Sound.Key.Ready);
            prompter.Play(Prompter.Type.Ready);

            yield return new WaitForSeconds(1f);

            AudioManager.Instance.PlaySFX(Sound.Key.Go);
            prompter.Play(Prompter.Type.Go);

            yield return new WaitForSeconds(1f);

            prompter.Play(Prompter.Type.None);
            GameStart();
        }
    }
    public void OnClickRetry()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        StaticValues.isRetry = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_MAIN);
    }
    public void OnClickStop()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_TITLE);
    }
    private void GameStart()
    {
        shellMixer.Preview();
        state = State.Play;
    }
    public void Checkpoint()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            state = State.Checkpoint;

            // Start Timer
            choiceTimerText.gameObject.SetActive(true);

            for (int i = choiceTime; i > 0; i--)
            {
                if (i <= 5)
                {
                    AudioManager.Instance.PlaySFX(Sound.Key.Timer);
                }

                choiceTimerText.text = i.ToString();

                yield return new WaitForSeconds(1f);
            }

            choiceTimerText.text = "0";

            yield return new WaitForSeconds(1f);

            // When don't choose
            choiceTimerText.gameObject.SetActive(false);
            ChooseCallback(false);
        }
    }
    private void Choose()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            hit.transform.TryGetComponent<Shell>(out Shell shell))
        {
            // Stop Checkpoint Corout
            StopCoroutine(coroutine);
            choiceTimerText.gameObject.SetActive(false);

            // Choice Shell
            shell.Show(ChooseCallback);
        }
    }
    private void ChooseCallback(bool result)
    {
        if (result)
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Correct);

            rightCount++;
            roundFeedbacks[roundCount - 1].color = Color.green;

            GameCheck(2f);
        }
        else
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Wrong);

            // Show Correct Shell
            foreach (Shell s in shellMixer.Shells)
            {
                if (s.isRight)
                {
                    s.Show(GameCheck, 1.5f);
                }
            }

            roundFeedbacks[roundCount - 1].color = Color.red;
        }
    }
    private void GameCheck(float delay)
    {
        DG.Tweening.DOVirtual.DelayedCall(delay, () => 
        {
            int half = roundMax / 2 + 1;

            if (roundMax - roundCount + rightCount >= half)
            {
                if (rightCount < half)
                {
                    RoundNext();
                }
                else
                {
                    GameVictory();
                }
            }
            else
            {
                GameDefeat();
            }
        });
    }
    private void RoundNext()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Next);

            roundCount++;
            state = State.Play;

            prompter.Play(Prompter.Type.Next);

            yield return new WaitForSeconds(1.5f);

            prompter.Play(Prompter.Type.None);

            shellMixer.Preview();
        }
    }
    private void GameVictory()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Victory);

        state = State.End;
        roundCount = 1;
        rightCount = 0;

        UnityEngine.SceneManagement.SceneManager.LoadScene(ConstantValues.SCENE_REWARD);
    }
    private void GameDefeat()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            AudioManager.Instance.PlaySFX(Sound.Key.Defeat);

            state = State.End;
            roundCount = 1;
            rightCount = 0;

            finishPopup.SetActive(true);

            for (int i = retryTime; i > 0; i--)
            {
                if (i <= 5)
                {
                    AudioManager.Instance.PlaySFX(Sound.Key.Timer);
                }

                finishTimerText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            finishTimerText.text = "0";
            OnClickStop();
        }
    }
    private void Init()
    {
        // Init Fields
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        // Init UI
        prompter.Play(Prompter.Type.None);
        tutorialPopup.SetActive(true);
        finishPopup.SetActive(false);
        choiceTimerText.gameObject.SetActive(false);

        for (int i = 0; i < roundFeedbacks.Length; i++)
        {
            roundFeedbacks[i].color = new Color(0, 0, 0, 0);
        }

        // Retry
        if (StaticValues.isRetry)
        {
            OnClickStart();

            StaticValues.isRetry = false;
        }
    }
}
