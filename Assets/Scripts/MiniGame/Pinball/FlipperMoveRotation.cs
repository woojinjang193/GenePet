using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperMoveRotation : MonoBehaviour
{
    [Header("각도(도)")]
    [SerializeField] private float _downAngle = -45f; // 손 뗐을 때
    [SerializeField] private float _upAngle = 45f;    // 누를 때

    [Header("속도(도/초)")]
    [SerializeField] private float _upSpeed = 1200f;
    [SerializeField] private float _downSpeed = 900f;

    private Rigidbody2D _rb;
    private bool _pressed;

    public void SetPressed(bool pressed) => _pressed = pressed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        float target = _pressed ? _upAngle : _downAngle;
        float speed = _pressed ? _upSpeed : _downSpeed;

        float cur = _rb.rotation;
        float next = Mathf.MoveTowardsAngle(cur, target, speed * Time.fixedDeltaTime);

        _rb.MoveRotation(next);
    }
}
