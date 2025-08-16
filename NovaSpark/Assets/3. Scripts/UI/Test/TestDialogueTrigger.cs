using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(InteractionEvent))]
public class TestClickTrigger : MonoBehaviour
{
    private InteractionEvent interactionEvent;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        interactionEvent = GetComponent<InteractionEvent>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (interactionEvent == null)
            Debug.LogError("InteractionEvent 필요");
        if (dialogueManager == null)
            Debug.LogError("DialogueManager 필요");
    }

    private void OnMouseDown()
    {
        // 대화 UI가 켜져 있으면 다음, 꺼져 있으면 시작
        if (dialogueManager.IsActive)
            dialogueManager.NextDialogue();
        else
            interactionEvent.Interact();
    }
}
