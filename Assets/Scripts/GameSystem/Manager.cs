using UnityEngine;

public static class Manager
{
    public static IAPManager IAP => IAPManager.GetInstance();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        IAPManager.CreateManager();
    }
}
