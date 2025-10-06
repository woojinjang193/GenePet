using UnityEngine;

public static class Manager
{
    public static IAPManager IAP => IAPManager.GetInstance();
    public static RandomGeneManager RandomPart => RandomGeneManager.GetInstance();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        IAPManager.CreateManager();
        RandomGeneManager.CreateManager();
    }
}
