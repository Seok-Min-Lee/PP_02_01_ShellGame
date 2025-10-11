using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingWindow : MonoBehaviour
{
    public Ctrl_Main ctrl { get; private set; }

    public void Init(Ctrl_Main ctrl)
    {
        this.ctrl = ctrl;
        gameObject.SetActive(true);
    }
    
    public void OnClickStart(int num)
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        int shellLevel, speedLevel, shuffleLevel;
        switch (num)
        {
            case 0:
                shellLevel = 3;
                speedLevel = 1;
                shuffleLevel = 5;
                break;
            case 1:
                shellLevel = 4;
                speedLevel = 3;
                shuffleLevel = 10;
                break;
            case 2:
                shellLevel = 5;
                speedLevel = 5;
                shuffleLevel = 15;
                break;
            default:
                return;
        }

        ctrl.PlayGame(shellLevel, speedLevel, shuffleLevel);
    }
}
