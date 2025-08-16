using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum KeyMode
{
    Normal, //기본 공격
    Build, //빌딩 및 카메라 이동

}

public class KeyManager : MonoBehaviour //TODO : StateMachine으로 바꾸기 전에 쓰는 임시 버전.
{
    public PlayerInput playerInput;

    private Dictionary<string, InputAction> actionDict = new();
    private Dictionary<string, InputActionMap> actionMap = new();
    
    public PlayerKey playerKey;
    public QuickSlotKey quickSlotKey;
    public SkillSlotKey skillSlotKey;
    public UIPanelKey uiPanelKey;
    public BuildingKey buildingKey;
    public ModeChangeKey modeChangeKey;
    public void ModChange(KeyMode keyMode)
    {
        switch(keyMode)
        {
            case KeyMode.Normal:
                actionDict[InputNames.Player_Attack].Enable();
                buildingKey.enabled = false;
                actionMap["TempBuild"].Disable();
                break;
            case KeyMode.Build:
                actionDict[InputNames.Player_Attack].Disable();
                buildingKey.enabled = true;
                actionMap["TempBuild"].Enable();
                break;
        }
    }

    private void Awake()
    {
        Initialzed();
        ModChange(modeChangeKey.curKeyMode);
    }

    private void Initialzed()
    {

        foreach (var map in playerInput.actions.actionMaps)
        {
            map.Enable();
            actionMap[map.name] = map;
            //Debug.Log(map.name);
            foreach (var action in map.actions)
            {
                string key = $"{map.name}/{action.name}"; // 고유 키 보장
                if (!actionDict.ContainsKey(key))
                {
                    actionDict[key] = action;
                }
                else
                {
                    //Debug.LogWarning($"중복된 액션 이름 발견: {key}");
                }
            }
        }
    }
}

public static class InputNames
{
    public const string Build_Building = "Build/Building";
    public const string Player_Attack = "Player/Attack";
}