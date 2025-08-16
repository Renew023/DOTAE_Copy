using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartPopupType
{
    StartPanel,
    NickNamePanel,
    LobbyPanel,
    RoomDetailPanel,
    GameDescriptionPanel,
    None
}

public class StartPopupUI : Singleton<StartPopupUI>
{
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup nickNamePanel;
    [SerializeField] private CanvasGroup lobbyPanel;
    [SerializeField] private CanvasGroup roomDetailPanel;
    [SerializeField] private CanvasGroup gameDescriptionPanel;
    [SerializeField] private CanvasGroup patchNotePanel;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowRoomSetting(StartPopupType type)
    {
        GroupSetActive(startPanel, type == StartPopupType.StartPanel || 
                                   type == StartPopupType.NickNamePanel ||
                                   type == StartPopupType.GameDescriptionPanel);
        GroupSetActive(nickNamePanel, type == StartPopupType.NickNamePanel);
        GroupSetActive(lobbyPanel, type == StartPopupType.LobbyPanel ||
                                   type == StartPopupType.RoomDetailPanel);
        GroupSetActive(roomDetailPanel, type == StartPopupType.RoomDetailPanel);
        GroupSetActive(gameDescriptionPanel, type == StartPopupType .GameDescriptionPanel);
    }

    public void PatchNotePanelOpen()
    {
        GroupSetActive(patchNotePanel, true);
    }

    public void PatchNotePanelClose()
    {
        GroupSetActive(patchNotePanel, false);
    }

    private void GroupSetActive(CanvasGroup data, bool isTrue)
    {
        if (isTrue)
        {
            data.alpha = 1;
            data.interactable = true;
            data.blocksRaycasts = true;
        }
        else
        {
            data.alpha = 0;
            data.interactable = false;
            data.blocksRaycasts = false;
        }

        if(data.TryGetComponent<IUIPanel>(out var panel))
        {
            panel.Init();
        }
    }
}
