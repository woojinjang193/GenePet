using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissingPosterAmount : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _text.text = Manager.Save.CurrentData.UserData.Items.MissingPoster.ToString();
    }
}
