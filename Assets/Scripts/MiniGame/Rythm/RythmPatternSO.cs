using UnityEngine;

[System.Serializable]
public struct RythmInfo
{
    public RythmType Type;
    public bool isRest;
}
[CreateAssetMenu(fileName = "New RythmPatternSO", menuName = "MiniGameSO/RythmPatternSO")]
public class RythmPatternSO : ScriptableObject
{
    [SerializeField] private RythmInfo[] _rythmList;
    public RythmInfo[] RythmList => _rythmList;
}
