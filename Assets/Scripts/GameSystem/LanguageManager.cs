using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LanguageManager : Singleton<LanguageManager>
{
    [SerializeField] private LanguageFontTableSO _fontTable; //언어별 폰트 테이블
    public TMP_FontAsset CurFont { get; private set; } //  현재 적용 폰트

    public Language CurLanguage {  get; private set; }

    public bool IsReady { get; private set; }

    private Dictionary<string, TextSO> _textDic = new();
    public Action<TMP_FontAsset> OnLanguageChanged; // 언어 + 최종 폰트 같이 전달

    private int _curLoadCount = 0;
    private int _totalLoadCount = 3; 

    protected override void Awake()
    {
        base.Awake();
        IsReady = false;

        var handle = Addressables.LoadAssetsAsync<TextSO>("TextSO", null);
        handle.Completed += OnTextSOLoaded;

        var handle_font = Addressables.LoadAssetAsync<LanguageFontTableSO>("LanguageFontTableSO");
        handle_font.Completed += OnFontLoaded;

        StartCoroutine(LoadCurLanguageRoutine());
    }
    private void OnFontLoaded(AsyncOperationHandle<LanguageFontTableSO> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"LanguageFontTableSO 로드 실패: {handle.OperationException}");
            return;
        }

        _fontTable = handle.Result;
        CheckIsReady();
        Debug.Log($"LanguageFontTableSO 로드 완료");
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

    private IEnumerator LoadCurLanguageRoutine() // 언어설정 위해 SaveManager가 준비될 때까지 대기하는 코루틴
    {
        // SaveManager가 준비될 때까지 대기
        while (Manager.Save == null || Manager.Save.IsReady == false)
        {
            Debug.Log("세이브매니저 기다리는중");
            yield return null;
        }

        // 준비 완료되면 세이브 데이터에서 언어 불러오기
        CurLanguage = Manager.Save.CurrentData.UserData.CurLanguage;

        if(CurLanguage == Language.None)//첫 접속일때
        {
            var systemLang = Application.systemLanguage;
            CurLanguage = FirstLangageSetting(systemLang);
            Manager.Save.CurrentData.UserData.CurLanguage = CurLanguage;
        }

        while (_fontTable == null) yield return null; // 폰트 테이블 Addressables 로드 완료까지 대기

        Debug.Log($"현재 언어 설정됨: {CurLanguage}");
       
        RefreshCurFont(); //현재 언어 폰트 갱신
        OnLanguageChanged?.Invoke(CurFont);
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
                case Language.SP: return text = _textDic[textID].SP;
                case Language.JP:  return text = _textDic[textID].JP;
                case Language.CHS:  return text = _textDic[textID].CHS;
                case Language.CHT:  return text = _textDic[textID].CHT;
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

        RefreshCurFont(); // 현재 언어 폰트 갱신
        OnLanguageChanged?.Invoke(CurFont);
    }
    //==================CurLanguage 기준으로 CurFont 갱신========================
    private void RefreshCurFont() 
    {
        if (_fontTable == null) { CurFont = null; return; } // 로드 전 호출 대비
        CurFont = _fontTable.GetFont(CurLanguage);
        Debug.Log($"[LanguageManager] 현재 폰트 : {CurFont.name}");
    }

    //===================시스템 언어로 첫실행 언어 결정================
    private Language FirstLangageSetting(SystemLanguage systemLanguage)
    {
        switch(systemLanguage)
        {
            case SystemLanguage.Korean: return Language.KR;
            case SystemLanguage.English: return Language.EN;
            case SystemLanguage.German: return Language.DE;
            case SystemLanguage.Spanish: return Language.SP;
            case SystemLanguage.Japanese: return Language.JP;
            case SystemLanguage.ChineseSimplified: return Language.CHS;
            case SystemLanguage.ChineseTraditional: return Language.CHT;
        }

        return Language.EN; //그 외 언어면 영어로 설정
    }

}
