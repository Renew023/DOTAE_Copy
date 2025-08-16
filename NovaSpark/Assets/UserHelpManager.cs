using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class UserHelpManager : Singleton<UserHelpManager> //나중에 리팩토링 할거라서 이름 그냥 막지음
{
    public LayerMask interactLayer;

    public GameObject panelOut;
    public Image objectIcon;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI titleText; 
    public TextMeshProUGUI hpText;
    public Image hpBar_Front;

    public TextMeshProUGUI textPrefab;
    public Transform verticalParent;
    public ObjectPool<TextMeshProUGUI> textPool;

    public Collider2D selectObject;

    public bool isSelectObject = false;

    protected override void Awake()
    {
        base.Awake();
        textPool = new ObjectPool<TextMeshProUGUI>(
            () => Instantiate(textPrefab, verticalParent),
            text =>
                {
                    text.gameObject.SetActive(true);
                    StartCoroutine(release(text));
                },
            text => text.gameObject.SetActive(false)
        );
    }
    // Update is called once per frame

    private void Update()
    {
        if (isSelectObject)
        {
            Reload();
        }
        else
        {
            CheckObject();
        }
    }

    public void CheckObject()
    {
        OverlapObject();

        if (selectObject == null)
        {
            panelOut.SetActive(false);
            return;
        }
        else
        {
            Reload();
        }
    }
    public bool SelectObject()
    {
        bool isCollider = OverlapObject();
        if (!isCollider) return false;

        if (selectObject == null)
        {
            panelOut.SetActive(false);
            isSelectObject = false;
            return false;
        }
        else
        {
            Reload();
            isSelectObject = true;
            return true;
        }
    }

    private bool OverlapObject()
    {
        Vector2 curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var curSelectObject = selectObject;
        selectObject = Physics2D.OverlapCircle(curPos, 0.5f, interactLayer);
        if (selectObject == curSelectObject) return false;
        return true;
    }

    private void Reload()
    {
        panelOut.SetActive(true);
        objectIcon.sprite = selectObject.GetComponent<IDatable>()?.GetIcon();
        idText.text = selectObject.GetComponent<IDatable>()?.GetID().ToString();
        titleText.text = selectObject.GetComponent<IDatable>()?.GetName();
        hpText.text = selectObject.GetComponent<IDamageable>()?.GetHp().ToString();

        float? value = selectObject.GetComponent<IDamageable>()?.GetHpPercent();
        hpBar_Front.fillAmount = value == null ? 0f : (float)value;
    }


    public void CreateText(string text)
    {
        TextMeshProUGUI TMP = textPool.Get();
        TMP.text = text;
    }

    private IEnumerator release(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(2.0f);

        textPool.Release(text);
    }
}
