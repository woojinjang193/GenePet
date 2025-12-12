using UnityEngine;

public class CleaningToolMover : MonoBehaviour
{
    private Vector3 _lastPos; //이전 프레임 위치
    private float _movedDist; //이동거리 누적
    private PetController _target; //닿아있는 펫

    [SerializeField] private ParticleSystem _bubble;
    [SerializeField] private ParticleSystem _foam;

    private bool _isBubbling = false;
    private float _notMovingTimer = 0f;

    private void OnEnable()
    {
        _target = null;  //펫 참조 초기화
        _isBubbling = false;
        _notMovingTimer = 0f;
    }
    private void Update()
    {
        Vector3 cur = transform.position; //현재 위치
        float dist = (cur - _lastPos).magnitude;  //프레임 이동거리
        _movedDist += dist; //총 이동거리 누적
        _lastPos = cur; //다음 프레임 비교를 위해 저장

        bool touching = (_target != null);
        bool moving = (dist > 0.02f);

        if (touching)  //펫과 닿아있으면
        {
            if (moving)
            {
                //파티클 샤워도구 따라다니기
                _bubble.transform.position = transform.position;
                _foam.transform.position = transform.position;

                _notMovingTimer = 0f;

                if (!_isBubbling)
                {
                    _isBubbling = true;
                    _bubble.Play();
                    _foam.Play();
                    Debug.Log("버블 플레이");
                }
            }
            else
            {
                _notMovingTimer += Time.deltaTime;

                // 0.3초 이상 멈춘 경우만 Stop
                if (_notMovingTimer >= 0.3f && _isBubbling)
                {
                    _isBubbling = false;
                    _bubble.Stop();
                    _foam.Stop();
                    Debug.Log("버블 멈춤 (0.3초 지속)");
                }
            }
            _target.Clean(dist); //펫에게 이동량 전달
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Pet"))  //Pet 태그와 닿음
        {
            _target = col.GetComponent<PetController>();   //펫 컨트롤러 가져오기
            _lastPos = transform.position; //초기 위치 저장
            _movedDist = 0f;  //누적 이동거리 초기화
            _notMovingTimer = 0f; // 파티클 스탑 타이머 초기화
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Pet"))  //벗어났으면
        {
            _bubble.Stop();
            _foam.Stop();
            _isBubbling = false;
            _notMovingTimer = 0f; // 파티클 스탑 타이머 초기화
            
            _target = null;  //연결 해제
            Debug.Log("버블 스탑");
        }
    }
}
