using UnityEngine;

public static class Manager
{
    public static ShopManager Shop => ShopManager.GetInstance();
    public static GeneManager Gene => GeneManager.GetInstance();
    public static SaveManager Save => SaveManager.GetInstance();
    public static GameManager Game => GameManager.GetInstance();
    public static LanguageManager Lang => LanguageManager.GetInstance();
    public static AudioManager Audio => AudioManager.GetInstance();
    public static ItemManager Item => ItemManager.GetInstance();
    public static FirebaseAuthManager Fire => FirebaseAuthManager.GetInstance();
    public static ServerSaveManager Server => ServerSaveManager.GetInstance();
    public static MiniGameManager Mini => MiniGameManager.GetInstance();


    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static void Init()
    //{
    //    ShopManager.CreateManager();
    //    GeneManager.CreateManager();
    //    GameManager.CreateManager();
    //    SaveManager.CreateManager();
    //    LanguageManager.CreateManager();
    //    //AudioManager.CreateManager();
    //    ItemManager.CreateManager();
    //    ServerSaveManager.CreateManager();
    //    //FirebaseAuthManager.CreateManager();
    //}
}
