using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRarityUI : MonoBehaviour
{
    [SerializeField] private Image[] _images;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _filledStar;
    public void ShowRarity(RarityType rarity)
    {
        switch (rarity)
        {
            case RarityType.Common:    StarsOn(1); break;
            case RarityType.Rare:      StarsOn(2); break;
            case RarityType.Epic:      StarsOn(3); break;
            case RarityType.Legendary: StarsOn(4); break;

        }
    }

    private void StarsOn(int num)
    {
        if (num > _images.Length) return;

        for (int i = 0; i < _images.Length; i++)
        {
            if(i < num)
            {
                _images[i].sprite = _filledStar;
            }
            else
            {
                _images[i].sprite = _emptyStar;
            }
        }
        
    }
}
