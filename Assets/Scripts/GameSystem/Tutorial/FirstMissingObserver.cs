using UnityEngine;

public class FirstMissingObserver : MonoBehaviour
{
    [SerializeField] private TutorialController _contoller;

    private SaveManager _save;
    private bool _isTutorialCompleted = false;
    private void Awake()
    {
        if (Manager.Save != null) _save = Manager.Save;
        if (TutorialCompleted())
        {
            _isTutorialCompleted = true;
        }
    }
    private void OnEnable()
    {
        if (_isTutorialCompleted) return;
        if (TutorialCompleted()) return; //플래그 false면 켤때마다 검사

        _contoller.TryStartTutorial(TutorialTriggerKey.FirstMissing);
    }
    private bool TutorialCompleted()
    {
        bool isFirstEditConpleted = _save.CurrentData.UserData.tutorialFlags.FirstMissing;

        return isFirstEditConpleted;
    }
}
