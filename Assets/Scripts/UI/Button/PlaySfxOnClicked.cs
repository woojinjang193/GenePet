using UnityEngine;
using UnityEngine.UI;

public class PlaySfxOnClicked : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null) { Debug.LogWarning("버튼 없음. 확인해야함"); return; }
        
        _button.onClick.AddListener(OnClicked);
    }
    private void OnDestroy()
    {
        if (_button == null) return;
        _button.onClick.RemoveListener(OnClicked);
    }
    private void OnClicked()
    {
        Manager.Audio.PlaySFX("Button");
    }
}
