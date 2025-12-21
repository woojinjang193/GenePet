using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TextCSVtoSO
{
    private static string csvPath = "Assets/CSV/Text.csv"; // CSV 경로
    private static string textSoDir = "Assets/ScriptableObjects/TextSO"; // SO 저장 경로
    private static int startRow = 1; // 데이터 시작 행
    private static int columnCount = 6; //열 개수

    [MenuItem("CSV TO SO/Import Text")]
    public static void StartImportingText()
    {
        ImportTextCSV();
    }
    private static void ImportTextCSV()
    {
        if (!File.Exists(csvPath)) // CSV 파일 없으면
        {
            Debug.LogError("Text CSV 파일 없음: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length < startRow) // 데이터가 시작되는 행보다 줄이 적으면
        {
            Debug.LogError("Text CSV에 데이터 없음");
            return;
        }

        if (!Directory.Exists(textSoDir))  //폴더 없으면
        {
            Directory.CreateDirectory(textSoDir); // 폴더 생성
        }

        for (int i = startRow; i < lines.Length; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line)) continue; //빈줄이면 넘어감

            var splitData = Regex.Matches(
                line,
                "\"([^\"]*(\"\"[^\"]*)*)\"|([^,]+)" //쉼표 기준으로 분리하지만 ""안에있는 쉼표는 무시
            );

            if (splitData.Count < columnCount) //열개수 부족시
            {
                Debug.LogWarning($"열 개수 부족 (행 {i}): {line}");
                continue;
            }

            string id = Clean(GetString(splitData, 0));
            string kor = Clean(GetString(splitData, 1));
            string eng = Clean(GetString(splitData, 2));
            string de = Clean(GetString(splitData, 3));
            string jp = Clean(GetString(splitData, 4));
            string ch = Clean(GetString(splitData, 5));

            string soPath = $"{textSoDir}/{id}.asset"; //SO 경로

            TextSO so = AssetDatabase.LoadAssetAtPath<TextSO>(soPath); //기존 SO 불러오기

            if (so == null) //SO 가 없으면
            {
                so = ScriptableObject.CreateInstance<TextSO>(); 
                AssetDatabase.CreateAsset(so, soPath);//새로 만듬
                Debug.Log($"새 TextSO 생성: {id}");
            }

            so.KOR = kor;
            so.ENG = eng;
            so.DE = de;
            so.JP = jp;
            so.CH = ch;

            EditorUtility.SetDirty(so);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("== Text CSV → SO 임포트 완료 ==");
    }

    private static string GetString(MatchCollection mc, int index)
    {
        return mc[index].Groups[1].Success? mc[index].Groups[1].Value : mc[index].Groups[3].Value;
    }

    private static string Clean(string text) //문자열 세탁기
    {
        return text
            .Replace("\"\"", "\"")   // "" > " 로 변경
            .Trim()                             //문자열 앞뒤 공백 제거
            .Trim('\"', '“', '”', '「', '」'); //텍스트를 감싸는 따옴표 제거
    }
}