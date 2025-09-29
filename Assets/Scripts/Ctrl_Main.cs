using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    public enum State
    {
        Wait,
        Play,
        Checkpoint
    }
    [SerializeField] private ShellMixer shellMixer;
    [SerializeField] private int roundMax;

    private State state;
    private int roundCount = 1;
    private int rightCount = 0;
    private void Start()
    {
        state = State.Wait;
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
    public void GameStart()
    {
        shellMixer.Preview();

        state = State.Play;

        Debug.Log("GameStart");
    }
    public void Checkpoint()
    {
        state = State.Checkpoint;
        Debug.Log("Checkpoint");
    }

    public void Choose()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            hit.transform.TryGetComponent<Shell>(out Shell shell))
        {
            shell.Show(ChooseCallback);
        }
    }
    public void ChooseCallback(bool result)
    {
        if (result)
        {
            rightCount++;
            RoundSuccess();
        }
        else
        {
            RoundFail();
        }
        
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
    public void RoundSuccess()
    {
        Debug.Log("RoundSuccess");
    }
    public void RoundFail()
    {
        Debug.Log("RoundFail");

    }
    public void RoundNext()
    {
        roundCount++;
        state = State.Play;

        Debug.Log("RoundNext");

        shellMixer.Preview();
    }
    public void GameVictory()
    {
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        Debug.Log("GameVictory");
    }
    public void GameDefeat()
    {
        state = State.Wait;
        roundCount = 1;
        rightCount = 0;

        Debug.Log("GameDefeat");
    }
}
