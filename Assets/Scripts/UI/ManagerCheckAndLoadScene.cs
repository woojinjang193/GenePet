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
        while (Manager.Gene == null || !Manager.Gene.IsReady)
        {
            _loadingText.text = "Gene data Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForSave()
    {
        while (Manager.Save == null || !Manager.Save.IsReady)
        {
            _loadingText.text = "Save data Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForLang()
    {
        while (Manager.Lang == null || !Manager.Lang.IsReady)
        {
            _loadingText.text = "Language data Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForAudio()
    {
        while (Manager.Audio == null || !Manager.Audio.IsReady)
        {
            _loadingText.text = "Audio data Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForGame()
    {
        while (Manager.Game == null || !Manager.Game.IsReady)
        {
            _loadingText.text = "Game Manager Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForItem()
    {
        while (Manager.Item == null || !Manager.Item.IsReady)
        {
            _loadingText.text = "Item Manager Loading..";
            yield return null;
        }
    }
    private IEnumerator WaitForFire()
    {
        while (Manager.Fire == null || !Manager.Fire.IsReady)
        {
            _loadingText.text = "Firebase Manager Loading..";
            yield return null;
        }
    }
}

