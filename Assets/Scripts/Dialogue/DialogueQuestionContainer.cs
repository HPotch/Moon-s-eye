using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueQuestionContainer : MonoBehaviour
{
    [SerializeField] private GameObject _dialogueQuestionPrefab;
    private int _selected = 0;
    private Vector3 _startLocalPos;

    private void Awake()
    {
        _startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        GameManager.Instance.dqc ??= this;
        ControllerScroll();
    }

    private void LateUpdate()
    {
        CameraController camControl = GameManager.Instance.camcontrol;
        transform.localPosition = _startLocalPos;
        if (camControl) transform.position += new Vector3(0f, camControl.CamOffsetY, 0f);
    }
    
    private void ControllerScroll()
    {
        GameManager gm = GameManager.Instance;
        if (gm.currentInputMode != GameManager.InputMode.Controller || transform.childCount <= 0) return;

        Gamepad gp = Gamepad.current;
        
        var prevSelected = _selected;
        if (gm.scrollUpKeys.Any(Input.GetKeyDown)) _selected--;
        if (gm.scrollDownKeys.Any(Input.GetKeyDown)) _selected++;
        
        if (gp.dpad.up.wasPressedThisFrame ||
            gp.leftStick.up.wasPressedThisFrame ||
            gp.rightStick.up.wasPressedThisFrame) _selected--;
        if (gp.dpad.down.wasPressedThisFrame || 
            gp.leftStick.down.wasPressedThisFrame ||
            gp.rightStick.down.wasPressedThisFrame) _selected++;
        if (prevSelected == _selected) return;
        
        if (_selected < 0) _selected = transform.childCount - 1;
        _selected %= transform.childCount;
        
        int i = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<DialogueQuestion>().Selected = i == _selected;
            i++;
        }
    }

    public void SpawnDialogueQuestion(Classes.Question question, NPCDialogue dialogue, float waitTime = 0f)
    {
        GameObject dqp = Instantiate(_dialogueQuestionPrefab, transform);
        DialogueQuestion dq = dqp.GetComponent<DialogueQuestion>();
        dq.Question = question;
        dq.NPCDialogue = dialogue;
        dq.dqc = this;
        dq.waitTime = waitTime;
        if (transform.childCount == 1 && GameManager.Instance.currentInputMode == GameManager.InputMode.Controller) dq.Selected = true;
    }

    public void Empty()
    {
        foreach (Transform child in transform)
        {
            DialogueQuestion dq = child.GetComponent<DialogueQuestion>();
            dq?.OnOff(false);
        }
        _selected = 0;
    }
}
