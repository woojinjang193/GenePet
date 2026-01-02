using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LanguageManager : Singleton<LanguageManager>
{
    public Language CurLanguage {  get; private set; }

    public bool IsReady { get; private set; }

    private Dictionary<string, TextSO> _textDic = new();
    public Action _OnLanguageChanged;

    private int _curLoadCount = 0;
    private int _totalLoadCount = 2; 

    protected override void Awake()
    {
        base.Awake();
        IsReady = false;

        var handle = Addressables.LoadAssetsAsync<TextSO>("TextSO", null);
        handle.Completed += OnTextSOLoaded;

        StartCoroutine(LoadCurLanguageRoutine());
    }

    private void OnTextSOLoaded(AsyncOperationHandle<IList<TextSO>> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"TextSO 로드 실패: {handle.OperationException}");
            return;
        }

        _textDic.Clear();

        foreach (var so in handle.Result)
        {
            if (so == null) continue;

            string key = so.name; //SO 이름을 키로 지정

            if (_textDic.ContainsKey(key))
            {
                Debug.LogWarning($"중복 TextSO key 감지됨: {key}");
                continue;
            }

            _textDic.Add(key, so);
        }

        CheckIsReady();
        Debug.Log($"TextSO 로드 완료: {_textDic.Count}개");
    }

    private IEnumerator LoadCurLanguageRoutine() // SaveManager가 준비될 때까지 대기하는 코루틴
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
        CheckIsReady();
    }

    private void CheckIsReady()
    {
        _curLoadCount++;
        if (_curLoadCount >= _totalLoadCount)
        {
            IsReady = true;
            Debug.Log($"LanguageManager Ready");
        }
    }

    public string GetText(string textID) //로컬라이즈된 텍스트 반환
    {
        string text = "";

        if (_textDic.ContainsKey(textID))
        {
            switch (CurLanguage)
            {
                case Language.KR: return text = _textDic[textID].KOR;
                case Language.EN: return text = _textDic[textID].ENG;
                case Language.DE:  return text = _textDic[textID].DE;
                case Language.JP:  return text = _textDic[textID].JP;
                case Language.CH:  return text = _textDic[textID].CH;
            }
        }
        else
        {
            Debug.LogWarning("잘못된 텍스트 키값");
        }
        return text;
    }

    public void ChangeLanguage(Language language) //언어 변경시 호출
    {
        if (CurLanguage == language) return;
        CurLanguage = language;
        Debug.Log($"언어 변경됨: {CurLanguage}");

        _OnLanguageChanged?.Invoke();
    }
}
