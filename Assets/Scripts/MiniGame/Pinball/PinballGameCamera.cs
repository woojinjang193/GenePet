using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballGameCamera : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PinballGameManager _manager;
    [SerializeField] private PinballRouletteZone _rouletteZone;

    [Header("카메라 이동 설정")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed;
    

    private Vector3 _startPos;
    private Coroutine _moveCo;
    private void Awake()
    {
        _startPos = transform.position;
        _manager.OnGameStart += CameraReset;
        _rouletteZone.OnRouletteStart += CameraMoveDown;
    }
    private void OnDestroy()
    {
        _manager.OnGameStart -= CameraReset;
        _rouletteZone.OnRouletteStart -= CameraMoveDown;
    }
    public void CameraMoveDown()
    {
        if (_target == null) return;

        if (_moveCo != null) StopCoroutine(_moveCo);
        _moveCo = StartCoroutine(CameraMoveRoutine());
    }
    private IEnumerator CameraMoveRoutine()
    {
        float targetY = _target.position.y;

        while (transform.position.y > targetY)
        {
            Vector3 pos = transform.position;

            float nextY = Mathf.MoveTowards(pos.y, targetY, _moveSpeed * Time.deltaTime);
            transform.position = new Vector3(pos.x, nextY, pos.z);

            yield return null;
        }

        _moveCo = null;
    }

    public void CameraReset()
    {
        if (_moveCo != null) StopCoroutine(_moveCo);
        transform.position = _startPos;
        _moveCo = null;

        _rouletteZone.ResetFlag();
    }
}
