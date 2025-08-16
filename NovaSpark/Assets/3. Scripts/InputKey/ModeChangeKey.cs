using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModeChangeKey : MonoBehaviour
{
    private KeyManager keyManager;
    [field : SerializeField] public KeyMode curKeyMode { get; private set; } = KeyMode.Normal;

    private void Start()
    {
        keyManager = GetComponentInParent<KeyManager>();
    }
    public void OnBuilding(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            curKeyMode = KeyMode.Build == curKeyMode ? KeyMode.Normal : KeyMode.Build;
            keyManager.ModChange(curKeyMode);
        }
    }
}
