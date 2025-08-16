using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(DialogueManager))]
public class InteractionEvent : MonoBehaviour
{
    [Header("인스펙터 설정: 대화 이벤트 범위")]
    public DialogueEvent dialogue;

    [Space, SerializeField, Tooltip("범위에 해당하는 대화 배열 (미리보기용)")]
    private NpcDialogue[] previewDialogues;

    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        //dialogueManager = GetComponent<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("씬에 DialogueManager가 없습니다!");
    }

    private void Reset() => UpdatePreview();
    private void OnValidate() => UpdatePreview();

    private void UpdatePreview()
    {
        int start = (int)dialogue.line.x;
        int end = (int)dialogue.line.y;

        if (Application.isPlaying)
        {
            previewDialogues = DataManager.Instance.GetDialogues(start, end);
        }
#if UNITY_EDITOR
        else
        {
            // 에디터 모드 동기 로드
            var handle = Addressables.LoadAssetAsync<TextAsset>("NpcDialogue");
            handle.WaitForCompletion();

            // TextAsset 결과를 jsonAsset에 담기
            TextAsset jsonAsset = handle.Result;

            if (jsonAsset != null)
            {
                // jsonAsset.text를 사용
                var all = JsonConvert.DeserializeObject<List<NpcDialogue>>(jsonAsset.text);
                previewDialogues = all
                    .Skip(start - 1)
                    .Take(end - start + 1)
                    .ToArray();
            }
            else
            {
                previewDialogues = new NpcDialogue[0];
            }

            // 변경 사항 에디터에 반영
            EditorUtility.SetDirty(this);
        }
#else
    else
    {
        previewDialogues = new NpcDialogue[0];
    }
#endif
    }

    /// <summary>
    /// 런타임 상호작용 호출
    /// </summary>
    public void Interact()
    {
        var dialogs = DataManager.Instance.GetDialogues((int)dialogue.line.x, (int)dialogue.line.y);
        dialogueManager.ShowDialogue(dialogs);  // ShowDialogue(NpcDialogue[])

        if (dialogs != null && dialogs.Length > 0)
            dialogueManager.ShowDialogue(dialogs);  // 파라미터 타입도 NpcDialogue[]
        else
            Debug.LogWarning($"대화 데이터가 없습니다: {dialogue.line.x}~{dialogue.line.y}");
    }
}
