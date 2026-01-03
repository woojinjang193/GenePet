using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        Manager.Audio.PlaySFX("Button");
    }
}
