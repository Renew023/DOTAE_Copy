using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class QuestLog : Singleton<QuestLog>
{
    public QuestBox txtPrefab; //퀘스트 존재
    public ObjectPool<QuestBox> textPool;
    public Dictionary<int, QuestBox> onTextView = new();
    public Transform contentTransform;

    protected override void Awake()
    {
        base.Awake();
        textPool = new ObjectPool<QuestBox>(
            () => Instantiate(txtPrefab, contentTransform),
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false)
        );
    }
    
    public void ResetQuestLog()
    {
        foreach(var quest in onTextView)
        {
            quest.Value.titleText.text = "" + DialogManager.Instance.dialogCash.questName;
            quest.Value.descriptionText.text = "\n\n" + DialogManager.Instance.dialogCash.questDescription;
            quest.Value.questMission.text = "";
        }
    }

    public void CreateQuestLog(int key)
    {
        onTextView[key] = textPool.Get();
        //Key 기반해서 Text 소환 끝.
        onTextView[key].titleText.text = "" + DialogManager.Instance.dialogCash.questName;
        onTextView[key].descriptionText.text = "\n\n" + DialogManager.Instance.dialogCash.questDescription;

        //string stringQuest = string.Join(",", DialogManager.Instance.dialogCash.);
        //onTextView[key].questMission.text = 
    }

    public void RemoveQuestLog(int key)
    {
        if (onTextView.ContainsKey(key))
        {
            textPool.Release(onTextView[key]);
            onTextView.Remove(key);
        }
    }
}