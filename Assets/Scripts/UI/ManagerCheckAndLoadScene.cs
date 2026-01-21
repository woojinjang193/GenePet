using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerCheckAndLoadScene : MonoBehaviour
{
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _touchToStart;
    private bool _isAllReady = false;
    private bool _isLoading = false;

    private void Awake()
    {
        _button.onClick.AddListener(OnClicked);

        ShopManager.CreateManager();
        GeneManager.CreateManager();
        GameManager.CreateManager();
        SaveManager.CreateManager();
        LanguageManager.CreateManager();
        //AudioManager.CreateManager();
        ItemManager.CreateManager();
        ServerSaveManager.CreateManager();
        //FirebaseAuthManager.CreateManager();
        //MiniGameManager.CreateManager();
        PoolManager.CreateManager();
    }

    private void Start()
    {

        StartCoroutine(LoadRoutine());
    }

    private void OnClicked()
    {
        if(_isLoading || !_isAllReady)
        {
            return;
        }

        Manager.Audio.PlaySFX("Button");
        SceneManager.LoadScene("InGameScene");
    }

    private IEnumerator LoadRoutine()
    {
        _isLoading = true;

        List<IEnumerator> loadSteps = new List<IEnumerator>()
        {
            WaitForGene(),
            WaitForSave(),
            WaitForLang(),
            WaitForAudio(),
            WaitForGame(),
            WaitForItem(),
            WaitForFire(),
            WaitForServerSave(),
        };

        int totalSteps = loadSteps.Count;
        float stepProgress = 1f / totalSteps;

        for (int i = 0; i < totalSteps; i++)
        {
            yield return StartCoroutine(loadSteps[i]);
            _loadingBar.value = Mathf.Clamp01(_loadingBar.value + stepProgress);
        }

        _isAllReady = true;
        _isLoading = false;
        _loadingText.text = "Ready to start!!";

        _touchToStart.SetActive(true);
    }

    private IEnumerator WaitForGene()
    {
        _loadingText.text = "Gene data Loading..";

        while (Manager.Gene == null || !Manager.Gene.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForSave()
    {
        _loadingText.text = "Save data Loading..";

        while (Manager.Save == null || !Manager.Save.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForLang()
    {
        _loadingText.text = "Language data Loading..";

        while (Manager.Lang == null || !Manager.Lang.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForAudio()
    {
        _loadingText.text = "Audio data Loading..";

        while (Manager.Audio == null || !Manager.Audio.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForGame()
    {
        _loadingText.text = "Game Manager Loading..";

        while (Manager.Game == null || !Manager.Game.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForItem()
    {
        _loadingText.text = "Item Manager Loading..";

        while (Manager.Item == null || !Manager.Item.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForFire()
    {
        _loadingText.text = "Firebase Manager Loading..";

        while (Manager.Fire == null || !Manager.Fire.IsReady)
        {
            yield return null;
        }
    }
    private IEnumerator WaitForServerSave()
    {
        _loadingText.text = "Syncing Save Data..";

        Manager.Server.DownloadIfNewer();

        while (!Manager.Server.IsReady)
        {
            yield return null;
        }
    }
}

