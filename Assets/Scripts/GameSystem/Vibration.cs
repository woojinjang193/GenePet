using UnityEngine;

public static class Vibration
{
    public static void Vibrate(float seconds, int amplitude)
    {
        if (Application.platform != RuntimePlatform.Android) return; // 에디터/비안드로이드면 무시
        if (seconds <= 0) return;

        long milliseconds = Mathf.Max(1, Mathf.RoundToInt(seconds * 1000f));
        // activity 획득
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        //Vibrator 서비스 획득
        using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
        using var vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");

        // 진동기 없으면 종료
        if (vibrator == null) return;
        if (!vibrator.Call<bool>("hasVibrator")) return;

        int sdkInt;
        // SDK 버전 체크
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            sdkInt = version.GetStatic<int>("SDK_INT");

        if (sdkInt >= 26)
        {
            // amplitude 범위 클램프(1~255)
            amplitude = Mathf.Clamp(amplitude, 1, 255);

            // VibrationEffect로 약한 진동 + 길이 지정
            using var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            using var effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createOneShot",
                milliseconds,
                amplitude
            );

            vibrator.Call("vibrate", effect);
        }
        else
        {
            // 구버전은 세기 제어 불가(길이만)
            vibrator.Call("vibrate", milliseconds);
        }
    }
}
