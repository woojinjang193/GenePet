using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Toggle _bgmToggle;
    [SerializeField] private Toggle _sfxToggle;
    private void OnEnable()
    {
        _bgmToggle.SetIsOnWithoutNotify(Manager.Audio.GetBGMMuted()); // isOn=true가 음소거
        _sfxToggle.SetIsOnWithoutNotify(Manager.Audio.GetSFXMuted());

        _bgmToggle.onValueChanged.AddListener(MuteBGM); // 이벤트 연결
        _sfxToggle.onValueChanged.AddListener(MuteSFX);
    }

    private void OnDisable()
    {
        _bgmToggle.onValueChanged.RemoveListener(MuteBGM);
        _sfxToggle.onValueChanged.RemoveListener(MuteSFX);
    }

    private void MuteBGM(bool isMuted)
    {
        Manager.Audio.SetBGMMute(isMuted);
    }
    private void MuteSFX(bool isMuted)
    {
        Manager.Audio.SetSFXMute(isMuted);
    }
}
