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
    public void OpenDoor()
    {
        int rand = Random.Range(0, _doors.Length);
        _player.transform.position = _doors[rand].position;

        StartCoroutine(OpenDoorRoutine(_doors[rand]));
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
        float targetY = door.position.y + _doorMoveDistance;

        while (door.position.y < targetY)
        {
            float nextY = Mathf.MoveTowards(door.position.y, targetY, _doorOpenSpeed * Time.deltaTime);
            door.position = new Vector3(door.position.x, nextY, door.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(_playerSpawnDelay);

        _player.SetActive(true);
    }
}
