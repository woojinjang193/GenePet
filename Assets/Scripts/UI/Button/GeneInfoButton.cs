using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneInfoButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GeneInfomationUI _geneInfo;
    [SerializeField] private PetManager _petManager;
    private void Awake()
    {
        if(_button == null)
        {
            _button = GetComponent<Button>();
        }
        if(_geneInfo == null)
        {
            _geneInfo = FindObjectOfType<GeneInfomationUI>();
        }
        if(_petManager == null)
        {
            _petManager = FindObjectOfType<PetManager>();
        }
        _button.onClick.AddListener(OpenGeneInfo);
    }
    private void OpenGeneInfo()
    {
        var zoomedPet = _petManager.ZoomedPet;
        if (zoomedPet != null)
        {
            _geneInfo.gameObject.SetActive(true);
            _geneInfo.Init(zoomedPet);
        }
    }
}
