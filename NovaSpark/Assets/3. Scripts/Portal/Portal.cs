using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    public GameObject targetObj;

    public GameObject toObj;

    Animator animator;
    private InputAction interactAction;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
            targetObj = collision.gameObject;
            animator.SetBool("gate", true);

            var pi = targetObj.GetComponent<PlayerInput>();
            if (pi == null) return;

            // 액션 가져오기
            interactAction = pi.actions.FindAction("Interact");

            if (interactAction != null)
            {
                interactAction.performed += OnInteractPerformed;
                interactAction.Enable();   
            }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        animator.SetBool("gate", false);

        if(interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
        }

        interactAction = null;
        targetObj = null;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(targetObj != null)
        {
            StartCoroutine(PortalRoutine());
        }
    }

    IEnumerator PortalRoutine()
    {
        yield return null;
       // targetObj.GetComponent<Player>().isControl = false;
        yield return new WaitForSeconds(0.5f);
        targetObj.transform.position = toObj.transform.position;
        yield return new WaitForSeconds(0.5f);
       // targetObj.GetComponent<Player>().isControl = true;
    }
}
