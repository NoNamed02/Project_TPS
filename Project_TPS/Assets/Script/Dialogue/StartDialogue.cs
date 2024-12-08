using PixelCrushers.DialogueSystem;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    public Transform actor;
    public Transform conversant;
    public SetDialogueForNPC NPC;
    private PlayerMovement _player;
    void Start()
    {
        _player = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_player.isAiming)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.white, 1f);
        LayerMask layerMask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, layerMask))
        {
            Debug.Log($"Hit object: {hit.collider.name}, Tag: {hit.collider.tag}");
            if (hit.collider.CompareTag("NPC"))
            {
                _player.isTalk = true;
                Transform conversant = hit.collider.transform;
                NPC = hit.collider.GetComponent<SetDialogueForNPC>();
                if (!NPC.doTalk)
                {
                    DialogueManager.StartConversation(NPC.conversationName, actor, conversant);
                    //NPC.doTalk = true;
                }
            }
        }
        else
        {
            Debug.LogError("Conversation name is not set!");
        }
    }
    public void TalkEnd()
    {
        _player.isTalk = false;
        _player.SetMouseCursorOff();
    }
}
