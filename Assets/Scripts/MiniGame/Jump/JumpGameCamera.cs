using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class JumpGameCamera : MonoBehaviour
{
    [SerializeField] private JumpPlayerController _player;

    [Header("전에 움직였던 y보다 _gap 보다 높을떄 이동")]
    [SerializeField] private float _gap;
    [Header("플레이어보다 얼마나 위로 갈지")]
    [SerializeField] private float _distanceFromPlayer;
    [Header("카메라 이동 스피드 ")]
    [SerializeField] private float _cameraMoveSpeed;

    private float _lastMovedY;

    private Coroutine _moveRoutine;

    public Action<bool> OnCameraMoving;
    
    private void Awake()
    {
        _player.OnPlayerGrounded += OnPlayerGrounded;
        _lastMovedY = _player.transform.position.y;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();

        if (_player != null)
            _player.OnPlayerGrounded -= OnPlayerGrounded;
    }
    private void OnPlayerGrounded(float curY)
    {
        if (curY - _lastMovedY < _gap) return;

        float targetY = curY + _distanceFromPlayer;

        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
        }

        _moveRoutine = StartCoroutine(MoveTo(targetY));
        _lastMovedY = curY;
    }

    private IEnumerator MoveTo(float targetY) //게임 종료시 코루틴 스탑 시켜야함
    {
        OnCameraMoving?.Invoke(true); //카메라 이동시작 (입력 제한)

        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, _cameraMoveSpeed * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            yield return null;
        }

        OnCameraMoving?.Invoke(false);
        _moveRoutine = null;
    }
}
