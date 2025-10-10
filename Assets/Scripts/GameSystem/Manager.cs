using UnityEngine;

public static class Manager
{
    public static IAPManager IAP => IAPManager.GetInstance();
    public static GeneManager Gene => GeneManager.GetInstance();
    public static SaveManager Save => SaveManager.GetInstance();
    public static GameManager Game => GameManager.GetInstance();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        IAPManager.CreateManager();
        GeneManager.CreateManager();
        GameManager.CreateManager();
        SaveManager.CreateManager();
    }
}
