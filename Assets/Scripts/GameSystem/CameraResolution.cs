using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResolution : MonoBehaviour
{
    private Camera _cam; // [유지] 카메라 캐시

    [Header("Base (9:16)")]
    [SerializeField] private float _baseAspect = 9f / 16f;  // 기준 비율(9:16, width/height)
    [SerializeField] private float _baseOrthoSize = 5f;      // 9:16에서 맞춘 OrthoSize

    [Header("Max Tall (<= 20:9 portrait -> 9:20)")]
    [SerializeField] private float _maxTallAspect = 9f / 20f; // 최대 세로비(20:9의 세로형 = 9:20)

    private void Awake()
    {
        _cam = GetComponent<Camera>(); 
        _cam.clearFlags = CameraClearFlags.SolidColor; // 레터박스 배경색 보이게
        _cam.backgroundColor = Color.black;            // 검정 바
    }

    private void Start()
    {
        Apply();
    }

    private void OnPreCull()
    {
        Apply(); // 해상도/회전 변동 대응
    }

    private void Apply()
    {
        if (_cam == null || !_cam.orthographic) return; //Orthographic만 처리

        float currentAspect = Screen.width / (float)Screen.height; // 현재 화면비(width/height)

        // ===== 1) 월드: 너무 길쭉하면(20:9 초과) 20:9로 "계산값"을 고정 =====
        float aspectForSize = Mathf.Max(currentAspect, _maxTallAspect); // 더 길쭉한 화면은 20:9로 클램프

        // 9:16보다 길쭉하면 OrthoSize 키워서 "가로폭 유지"(좌우 안 잘림)
        if (aspectForSize < _baseAspect) // currentAspect 대신 aspectForSize 사용
            _cam.orthographicSize = _baseOrthoSize * (_baseAspect / aspectForSize); 
        else
            _cam.orthographicSize = _baseOrthoSize; 

        // ===== 2) 화면: 20:9보다 더 길쭉하면 레터박스(위/아래 검정) =====
        if (currentAspect < _maxTallAspect) // 20:9 초과로 길쭉한 경우만
            _cam.rect = GetLetterboxRect(currentAspect, _maxTallAspect); // 20:9 영역만 그리기
        else
            _cam.rect = new Rect(0f, 0f, 1f, 1f); // 정상 범위면 전체 사용
    }

    private Rect GetLetterboxRect(float windowAspect, float targetAspect)
    {
        // windowAspect = Screen.width/Screen.height, targetAspect = 고정할 width/height(여기선 9/20)
        float scaleHeight = windowAspect / targetAspect; // 1보다 작으면 위/아래 바 필요
        if (scaleHeight < 1f)
        {
            float h = scaleHeight;
            float y = (1f - h) * 0.5f;
            return new Rect(0f, y, 1f, h);   // 레터박스(위/아래)
        }
        else
        {
            float scaleWidth = 1f / scaleHeight; 
            float x = (1f - scaleWidth) * 0.5f;    
            return new Rect(x, 0f, scaleWidth, 1f);     //필러박스(좌/우) - 태블릿 등
        }
    }
}
