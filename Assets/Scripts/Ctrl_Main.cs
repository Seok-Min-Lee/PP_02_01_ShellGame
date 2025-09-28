using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    [SerializeField] private ShellMixer shellMixer;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            shellMixer.Launch();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            shellMixer.Shuffle(10);
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼
        {
            shellMixer.Choose();
        }
    }
}
