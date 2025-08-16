using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancePopupUI : PopupUI
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button popup;
    private Coroutine closeRoutine;

    public override void ShowPanel()
    {
        base.ShowPanel();

        popup.onClick.AddListener(OnClickPopup);
        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(DelayedClose());
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(2f);
        HidePanel();
    }

    private void OnClickPopup()
    {
        HidePanel();
    }

    public override void HidePanel()
    {
        base.HidePanel();
        
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
            closeRoutine = null;
        }

        popup.onClick.RemoveListener(OnClickPopup);
    }

    public void SetMessage(string msg)
    {
        if (messageText) messageText.text = msg;
    }
}