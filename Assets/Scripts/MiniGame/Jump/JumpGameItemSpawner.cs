using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameItemSpawner : MonoBehaviour
{
    [Header("청크당 아이템수")]
    [SerializeField] private int _minItemCount;
    [SerializeField] private int _maxItemCount;
}
