using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobberVisual : MonoBehaviour
{
    [Header("이동 거리")]
    [SerializeField] private float _downDistance = 0.2f; // 찌가 아래로 내려가는 최대 거리

    [Header("비트 대비 시간 비율")]
    [SerializeField, Range(0.05f, 0.9f)] private float _downRatio = 0.25f;
    // 1박(beatDuration) 중 '내려가는 구간'이 차지하는 비율
    // 예) beatDuration=0.5초, downRatio=0.25 => 내려가는데 0.125초, 올라오는데 0.375초

    [Header("최소 시간(너무 빠른 BPM 안전장치)")]
    [SerializeField] private float _minDownTime = 0.02f; // BPM이 너무 빠를 때 downTime이 0에 가까워지는 걸 방지(최소 내려가는 시간)
    [SerializeField] private float _minUpTime = 0.02f;     // BPM이 너무 빠를 때 upTime이 0에 가까워지는 걸 방지(최소 올라오는 시간)

    //[Header("찌 올리는 높이, 스피드")]
    //[SerializeField] private float _upDistance = 1f;
    //[SerializeField] private float _moveSpeed = 10f;

    private Vector3 _baseLocalPos;   // 찌의 "원래 위치"

    private bool _isPlaying;  // 현재 펄스(내려갔다 올라오는 연출)가 진행 중인지
    private float _t;   // 펄스가 시작된 후 경과 시간(Time.deltaTime 누적)

    // 내려가는 시간 / 올라오는 시간(beatDuration 기반으로 매번 계산)
    private float _downTime;
    private float _upTime;

    private void Awake()
    {
        _baseLocalPos = transform.localPosition;
    }

    public void PulseDown(float beatDuration)
    {
        // 리듬이 빨라지면 beatDuration이 짧아짐
        // => downTime/upTime도 같이 짧아져서 찌 움직임도 자동으로 빨라짐
        _downTime = Mathf.Max(_minDownTime, beatDuration * _downRatio);
        _upTime = Mathf.Max(_minUpTime, beatDuration * (1f - _downRatio));

        // 새 펄스 시작: 시간 초기화
        _t = 0f;
        _isPlaying = true;
    }
    //public void BobberUp()
    //{
    //    StartCoroutine(BobberUpRoutine());
    //}
    //private IEnumerator BobberUpRoutine()
    //{
    //    float startY = transform.position.y;
    //    float targetY = transform.position.y + _upDistance;
    //
    //    while (transform.position.y < targetY)
    //    {
    //        Vector3 pos = transform.position;
    //        pos.y = Mathf.MoveTowards(pos.y, targetY, _moveSpeed * Time.deltaTime);
    //        transform.position = pos;
    //
    //        yield return null;
    //    }
    //}

    private void Update()
    {
        if (!_isPlaying) return;

        _t += Time.deltaTime;    // 펄스 진행 시간 누적

        float yOffset;        // 현재 프레임에서 적용할 Y 오프셋(아래로 내려가면 음수)

        if (_t <= _downTime)         // 1) 내려가는 구간: 0 ~ _downTime
        {
            float a = _t / _downTime; // 0~1 진행률
            a = EaseOut(a); // 자연스럽게(처음 빠르고 끝 느리게) 보이도록 보정

            yOffset = -Mathf.Lerp(0f, _downDistance, a);      // 0 -> -_downDistance 로 이동
        }
        else if (_t <= _downTime + _upTime)  // 2) 올라오는 구간: _downTime ~ (_downTime + _upTime)
        {
            float b = (_t - _downTime) / _upTime; // 0~1 진행률
            b = EaseOut(b);
            yOffset = -Mathf.Lerp(_downDistance, 0f, b);    // -_downDistance -> 0 으로 복귀
        }
        else   // 3) 종료: 기준 위치로 스냅하고 종료
        {
            yOffset = 0f;
            _isPlaying = false;
        }
        // 최종 위치 적용: (기준 위치 + 오프셋)
        transform.localPosition = _baseLocalPos + new Vector3(0f, yOffset, 0f);
    }

    private float EaseOut(float x) // 간단한 ease-out 곡선(원하면 애니메이션 커브로 바꿔도 됨)
    {
        // 0~1 입력 -> 0~1 출력
        // (1 - (1-x)^3) : 초반 빠르고 후반 부드럽게 멈춤
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    // 옵션: 씬에서 찌 위치를 수동으로 옮긴 다음,
    // 그 위치를 "기준 위치"로 다시 잡고 싶을 때 호출
    public void RebindBasePos()
    {
        _baseLocalPos = transform.localPosition;
    }
}
