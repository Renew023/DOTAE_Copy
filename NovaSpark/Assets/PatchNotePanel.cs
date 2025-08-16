using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatchNotePanel : MonoBehaviour
{
    private CanvasGroup CanvasGroup;
    [SerializeField] private Button closeButton;

    void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        closeButton.onClick.AddListener(() =>
        {
            StartPopupUI.Instance.PatchNotePanelClose();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
