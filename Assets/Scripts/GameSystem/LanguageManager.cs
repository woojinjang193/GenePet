using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    public Language CurLanguage {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(LoadCurLanguageRoutine());
    }

    private IEnumerator LoadCurLanguageRoutine()
    {
        // SaveManager가 준비될 때까지 대기
        while (Manager.Save == null || Manager.Save.IsReady == false)
        {
            Debug.Log("세이브매니저 기다리는중");
            yield return null;
        }

        // 준비 완료되면 세이브 데이터에서 언어 불러오기
        CurLanguage = Manager.Save.CurrentData.UserData.CurLanguage;

        Debug.Log($"현재 언어 설정됨: {CurLanguage}");
    }

    public void ChangeLanguage(Language language)
    {
        CurLanguage = language;
        Debug.Log($"언어 변경됨: {CurLanguage}");
    }
}
