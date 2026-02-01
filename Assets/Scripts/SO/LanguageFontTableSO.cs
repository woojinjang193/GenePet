using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageFontTable", menuName = "GameSO/LanguageFontTableSO")]
public class LanguageFontTableSO : ScriptableObject
{
    [Header("언어별 폰트")]
    public TMP_FontAsset KR; //한국어 폰트
    public TMP_FontAsset EN; //영어 폰트
    public TMP_FontAsset DE; //독일어 폰트
    public TMP_FontAsset SP; //스페인어 폰트
    public TMP_FontAsset JP; //일본어 폰트
    public TMP_FontAsset CHS; //중국어 간체 폰트
    public TMP_FontAsset CHT; //중국어 번체 폰트

    public TMP_FontAsset GetFont(Language lang) //언어에 맞는 폰트 반환
    {
        TMP_FontAsset font = lang switch
        {
            Language.KR => KR,
            Language.EN => EN,
            Language.DE => DE,
            Language.SP => SP,
            Language.JP => JP,
            Language.CHS => CHS,
            Language.CHT => CHT,
            _ => null,        
        };

        return font != null ? font : EN; // 없으면 EN 사용
    }
}
