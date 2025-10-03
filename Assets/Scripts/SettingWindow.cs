using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingWindow : MonoBehaviour
{
    private const int SHELL_NUMBER_MIN = 3;
    private const int SHELL_NUMBER_MAX = 5;
    private const int SHELL_SPEED_MIN = 1;
    private const int SHELL_SPEED_MAX = 10;

    [SerializeField] TextMeshProUGUI ShellText;
    [SerializeField] TextMeshProUGUI SpeedText;

    public Ctrl_Main ctrl { get; private set; }

    private int shellLevel = 0;
    private int speedLevel = 0;
    public void Init(Ctrl_Main ctrl)
    {
        this.ctrl = ctrl;

        //
        shellLevel = SHELL_NUMBER_MIN;
        speedLevel = SHELL_SPEED_MIN;
        ShellText.text = shellLevel.ToString();
        SpeedText.text = speedLevel.ToString();

        gameObject.SetActive(true);
    }
    public void OnClickNextShell()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        shellLevel = Mathf.Min(shellLevel + 1, SHELL_NUMBER_MAX);
        ShellText.text = shellLevel.ToString();
    }
    public void OnClickPreviousShell()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        shellLevel = Mathf.Max(shellLevel - 1, SHELL_NUMBER_MIN);
        ShellText.text = shellLevel.ToString();
    }
    public void OnClickNextSpeed()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        speedLevel = Mathf.Min(speedLevel + 1, SHELL_SPEED_MAX);
        SpeedText.text = speedLevel.ToString();
    }
    public void OnClickPreviousSpeed()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        speedLevel = Mathf.Max(speedLevel - 1, SHELL_SPEED_MIN);
        SpeedText.text = speedLevel.ToString();
    }
    public void OnClickStart()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        ctrl.PlayGame(shellLevel, speedLevel);
    }
    public void OnClickDefault()
    {
        AudioManager.Instance.PlaySFX(Sound.Key.Click);

        shellLevel = SHELL_NUMBER_MIN;
        speedLevel = SHELL_SPEED_MIN;

        ShellText.text = shellLevel.ToString();
        SpeedText.text = speedLevel.ToString();
    }
}
