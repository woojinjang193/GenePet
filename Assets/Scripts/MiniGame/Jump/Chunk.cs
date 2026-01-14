using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public float StartY { get; private set; }
    public float EndY { get; private set; }

    private void Awake()
    {
        if(_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    public void Init(float startY, float endY, JumpGameDifficultyPreset preset, bool isFirstChunk, bool isLastChunk)
    {
        StartY = startY;
        EndY = endY;

        Sprite background;
        if (isFirstChunk)
        {
            background = preset.BackgroundStart;
        }
        else if (isLastChunk)
        {
            background = preset.BackgroundEnd;
        }
        else
        {
            background = preset.BackgroundLoop;
        }
        _spriteRenderer.sprite = background;
        transform.position = Vector3.up * startY;
    }
}
