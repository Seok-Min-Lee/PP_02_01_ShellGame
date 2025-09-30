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
        Checkpoint
    }

    [SerializeField] private GameObject readyPopup;
    [SerializeField] private GameObject goPopup;
    [SerializeField] private GameObject nextPopup;
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject finishPopup;

    [SerializeField] private Image[] roundFeedbacks;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private ShellMixer shellMixer;
    [SerializeField] private int roundMax;

    private State state;
    private int roundCount = 1;
    private int rightCount = 0;
    private Coroutine coroutine = null;
    private void Start()
    {
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
            Choose();
        }
    }
    public void OnClickStart()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            tutorialPopup.SetActive(false);
            readyPopup.SetActive(true);

            yield return new WaitForSeconds(1f);

            readyPopup.SetActive(false);
            goPopup.SetActive(true);

            yield return new WaitForSeconds(1f);

            goPopup.SetActive(false);
            GameStart();
        }
    }
    public void OnClickRetry()
    {
        StaticValues.isRetry = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
    public void OnClickStop()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
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
            timerText.gameObject.SetActive(true);

            for (int i = 10; i > 0; i--)
            {
                timerText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            timerText.text = "0";

            yield return new WaitForSeconds(1f);

            // When don't choose
            timerText.gameObject.SetActive(false);
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
            timerText.gameObject.SetActive(false);

            // Choice Shell
            shell.Show(ChooseCallback);
        }
    }
    private void ChooseCallback(bool result)
    {
        if (result)
        {
            rightCount++;
            roundFeedbacks[roundCount - 1].color = Color.green;

            GameCheck(false);
        }
        else
        {
            // Show Correct Shell
            foreach (Shell s in shellMixer.Shells)
            {
                if (s.isRight)
                {
                    s.Show(GameCheck);
                }
            }

            roundFeedbacks[roundCount - 1].color = Color.red;
        }
    }
    private void GameCheck(bool value)
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
    }
    private void RoundNext()
    {
        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            roundCount++;
            state = State.Play;

            nextPopup.SetActive(true);

            yield return new WaitForSeconds(1f);

            nextPopup.SetActive(false);

            shellMixer.Preview();
        }
    }
    private void GameVictory()
    {
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        finishPopup.SetActive(true);
    }
    private void GameDefeat()
    {
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        finishPopup.SetActive(true);
    }
    private void Init()
    {
        // Init Fields
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        // Init UI
        readyPopup.SetActive(false);
        goPopup.SetActive(false);
        nextPopup.SetActive(false);
        tutorialPopup.SetActive(true);
        finishPopup.SetActive(false);
        timerText.gameObject.SetActive(false);

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
