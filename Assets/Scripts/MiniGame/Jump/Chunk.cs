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

        // 첫 청크만 Start, 나머지는 전부 Loop
        Sprite background = isFirstChunk ? preset.BackgroundStart : preset.BackgroundLoop;
        _spriteRenderer.sprite = background;
        transform.position = Vector3.up * startY;
    }
}
