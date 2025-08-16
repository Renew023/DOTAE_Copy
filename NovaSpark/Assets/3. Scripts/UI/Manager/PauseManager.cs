using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    private float _originalTimeScale = 1f;
    private bool _isPaused = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PauseGame()
    {
        if (_isPaused) return;

        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        AudioListener.pause = true;

        _isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        Time.timeScale = _originalTimeScale;
        AudioListener.pause = false;

        _isPaused = false;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }
}
