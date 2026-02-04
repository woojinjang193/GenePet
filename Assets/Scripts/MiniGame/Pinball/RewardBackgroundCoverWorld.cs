using UnityEngine;

public class RewardBackgroundCoverWorld : MonoBehaviour
{
    [Header("필수")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _topPanel;
    [SerializeField] private Transform _ceilingCoverPoint;

    [Header("여유(픽셀)")]
    [SerializeField] private float _padding = 0f;

    private RectTransform _bg;
    private RectTransform _parent;
    private Camera _cam;

    private bool _dirty; // 재적용 필요 플래그

    private void Awake()
    {
        _bg = GetComponent<RectTransform>();
        _parent = _bg.parent as RectTransform;
        _cam = _canvas != null ? _canvas.worldCamera : null;
    }
    private void OnEnable()
    {
        Canvas.willRenderCanvases += OnWillRenderCanvases; //  UI 렌더 직전에 적용
    }
    private void OnDisable()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases; // 구독 해제
    }
    public void RequestApply() //CameraResolution에서 이거만 호출
    {
        _dirty = true; // 다음 UI 렌더 타이밍에 1회 적용
    }
    private void OnWillRenderCanvases() //“제대로 반영된 뒤” 타이밍
    {
        if (!_dirty) return; // 요청 없으면 아무것도 안 함
        _dirty = false;      // 1회만 적용

        Canvas.ForceUpdateCanvases(); // 레이아웃/캔버스 최신화(타이밍 안정화)

        ApplyOnce(); // 실제 적용
    }
    private void ApplyOnce()
    {
        if (_canvas == null || _topPanel == null || _ceilingCoverPoint == null) return;
        if (_parent == null) return;
        if (_cam == null) _cam = _canvas.worldCamera;

        Vector3 sp = _cam.WorldToScreenPoint(_ceilingCoverPoint.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, sp, _cam, out Vector2 lp);

        float yFromBottom = lp.y - _parent.rect.yMin;
        float topOffset = -_topPanel.rect.height;
        float bottomOffset = yFromBottom - _padding;

        _bg.anchorMin = new Vector2(0f, 0f);
        _bg.anchorMax = new Vector2(1f, 1f);
        _bg.offsetMax = new Vector2(0f, topOffset);
        _bg.offsetMin = new Vector2(0f, bottomOffset);
    }
}
