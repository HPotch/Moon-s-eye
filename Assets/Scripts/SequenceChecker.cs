using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private List<int> sequence = new List<int>();
    
    private Piano _piano;

    private void Start()
    {
        GetPiano();
    }

    private void Update()
    {
        if (_piano is null) GetPiano();
        else if (_piano.CheckSequence(sequence)) print("Found Match!");
    }

    private void GetPiano()
    {
        _piano = GameManager.Instance.piano;
    }
}
