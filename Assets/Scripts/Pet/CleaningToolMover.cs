using UnityEngine;

public class CleaningToolMover : MonoBehaviour
{
    private Vector3 _lastPos; //이전 프레임 위치
    private float _movedDist; //이동거리 누적
    private PetController _target; //닿아있는 펫

    private void OnEnable()
    {
        _target = null;  //펫 참조 초기화
    }

    private void Update()
    {
        Vector3 cur = transform.position; //현재 위치
        float dist = (cur - _lastPos).magnitude;  //프레임 이동거리
        _movedDist += dist; //총 이동거리 누적
        _lastPos = cur; //다음 프레임 비교를 위해 저장

        if (_target != null)  //펫과 닿아있으면
        {
            _target.AddCleaningProgress(dist); //펫에게 이동량 전달
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Pet"))  //Pet 태그와 닿음
        {
            _target = col.GetComponent<PetController>();   //펫 컨트롤러 가져오기
            _lastPos = transform.position; //초기 위치 저장
            _movedDist = 0f;  //누적 이동거리 초기화
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Pet"))  //벗어났으면
        {
            _target = null;  //연결 해제
        }
    }
}
