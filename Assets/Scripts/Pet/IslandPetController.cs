using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandPetController : MonoBehaviour
{
    [SerializeField] private PetUnit _pet;

    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _pattern;
    [SerializeField] private SpriteRenderer _ear;
    [SerializeField] private SpriteRenderer _tail;
    [SerializeField] private SpriteRenderer _wing;
    [SerializeField] private SpriteRenderer _eye;
    [SerializeField] private SpriteRenderer _mouth;
    [SerializeField] private SpriteRenderer _horn;

    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    [SerializeField] private Button _button;

    private void Awake()
    {
        
    }
}
