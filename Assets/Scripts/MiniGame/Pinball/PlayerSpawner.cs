using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform[] _doors;
    [SerializeField] private float _doorMoveDistance = 1.2f;
    [SerializeField] private float _doorOpenSpeed = 1.2f;
    [SerializeField] private float _playerSpawnDelay = 1f;

    private Coroutine _openRoutine; // 진행중 코루틴 저장
    private bool _isOpening;        // 중복 호출 방지
    public void OpenDoor()
    {
        if (_isOpening) return;     // [추가] 이미 열고 있으면 무시
        _isOpening = true;          // [추가] 열기 시작 플래그

        int rand = Random.Range(0, _doors.Length);
        _player.transform.position = _doors[rand].position;

        if (_openRoutine != null) StopCoroutine(_openRoutine); // [추가] 혹시 남아있으면 중지

        _openRoutine = StartCoroutine(OpenDoorRoutine(_doors[rand]));
        Debug.Log("문열기 시작");
    }
    public void CloseDoor() //문위치 초기화
    {
        for(int i = 0;  i < _doors.Length; i++) 
        {
            _doors[i].localPosition = Vector3.zero;
        }
        _player.SetActive(false);

        
    }
    private IEnumerator OpenDoorRoutine(Transform door)
    {
        float startY = door.position.y;  // 시작 Y
        float targetY = startY + _doorMoveDistance; // 목표 Y
        const float EPS = 0.001f;   // 비교 오차 여유

        Debug.Log("scale:" + Time.timeScale + " dt:" + Time.deltaTime + " udt:" + Time.unscaledDeltaTime);
        Debug.Log("문열기 코루틴 시작");

        while (door.position.y < targetY - EPS)
        {
            float step = _doorOpenSpeed * Time.deltaTime;
            float nextY = Mathf.MoveTowards(door.position.y, targetY, step); 
            door.position = new Vector3(door.position.x, nextY, door.position.z);
            yield return null;
        }

        door.position = new Vector3(door.position.x, targetY, door.position.z); // 정확히 타깃에 스냅
        yield return new WaitForSeconds(_playerSpawnDelay);

        _player.SetActive(true);

        _isOpening = false;
        _openRoutine = null;
    }

}
