using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PinballVisualManager : MonoBehaviour
{
    [Header("파괴 파티클")]
    [SerializeField] private GameObject _color0DestroyParticle;
    [SerializeField] private GameObject _color1DestroyParticle;
    [SerializeField] private GameObject _color2DestroyParticle;
    [SerializeField] private GameObject _color3DestroyParticle;

    [Header("아이템 아이콘")]
    [SerializeField] private GameObject _itemIcon; // 반드시 UI 프리팹

    [Header("아이템 아이콘을 띄울 Canvas")]
    [SerializeField] private Canvas _uiCanvas; // 슬롯이 붙어있는 Canvas

    [Header("슬롯 트렌스폼")]
    [SerializeField] private RectTransform _slot1Transform;
    [SerializeField] private RectTransform _slot2Transform;
    [SerializeField] private RectTransform _slot3Transform;

    [Header("파티클 유지시간")]
    [SerializeField] private float _particleDuration = 2f;

    [Header("아이템 날아가는 속도")]
    [SerializeField] private float _itemFlySpeed = 800f;

    [Header("일반 아이템 팝업 시간")]
    [SerializeField] private float _popupDuration = 0.4f;

    public event Action<BrickColor, LevelReward> OnItemFlown;
    // ================= 이벤트 등록/해제======================
    public void RegisterBrick(PinballBrick brick)
    {
        brick.OnBroken += OnBrickBroken;
        brick.OnGiveItem += OnGivenItem;
    }
    public void UnregisterBrick(PinballBrick brick)
    {
        brick.OnBroken -= OnBrickBroken;
        brick.OnGiveItem -= OnGivenItem;
    }
    // =========== 블록 파괴시 ====================
    private void OnBrickBroken(BrickColor color, Vector3 worldPos) //블록 파괴시
    {
        GameObject particle;

        switch (color) //파티클 할당
        {
            case BrickColor.None: particle = _color0DestroyParticle; break;
            case BrickColor.one: particle = _color1DestroyParticle; break;
            case BrickColor.two: particle = _color2DestroyParticle; break;
            case BrickColor.three: particle = _color3DestroyParticle; break;
            default: particle = _color0DestroyParticle; break; //없으면 기본 파티클
        }

        GameObject go = Instantiate(particle, worldPos, Quaternion.identity, transform); //TODO: 풀로 변경

        StartCoroutine(ParticleRoutine(go));
    }
    private IEnumerator ParticleRoutine(GameObject go) //파티클 키고끄는 코루틴
    {
        go.SetActive(true);
        yield return new WaitForSeconds(_particleDuration);
        go.SetActive(false);

        Destroy(go); //TODO: 풀로 변경
    }
    // =========== 아이템 획득시 ====================
    private void OnGivenItem(BrickColor color, LevelReward reward, Vector3 worldPos)
    {
        if (_uiCanvas == null) return;

        // UI 아이콘 생성(캔버스 아래)
        RectTransform canvasRect = _uiCanvas.transform as RectTransform;
        GameObject go = Instantiate(_itemIcon, _uiCanvas.transform);  //TODO: 풀로 교체
        RectTransform iconRect = go.GetComponent<RectTransform>();
        Image iconImg = go.GetComponent<Image>();

        // TODO: 여기서 reward에 맞는 스프라이트로 iconImg.sprite 세팅

        // 시작점: 월드 > 캔버스 로컬
        iconRect.anchoredPosition = WorldToCanvasLocal(canvasRect, worldPos);

        go.SetActive(true);

        if (color == BrickColor.None)
        {
            // 일반 아이템: 제자리 팝업
            StartCoroutine(PopupFadeRoutine(go, iconImg));
            return;
        }

        // 색 아이템: 슬롯으로 날아감
        RectTransform target = color switch
        {
            BrickColor.one => _slot1Transform,
            BrickColor.two => _slot2Transform,
            BrickColor.three => _slot3Transform,
            _ => null
        };

        if (target == null)
        {
            Destroy(go);  //TODO: 풀로 교체
            return;
        }

        Vector2 targetLocal = WorldToCanvasLocal(canvasRect, target.position);
        StartCoroutine(ItemFlyRoutine(go, iconRect, targetLocal, color, reward));
    }

    private IEnumerator PopupFadeRoutine(GameObject go, Image img)
    {
        // 간단 페이드아웃
        float t = 0f;
        Color color = img != null ? img.color : Color.white;

        while (t < _popupDuration)
        {
            t += Time.deltaTime;
            float a = 1f - (t / _popupDuration);

            if (img != null)
            {
                color.a = a;
                img.color = color;
            }

            yield return null;
        }

        Destroy(go); //TODO: 풀로 교체
    }
    private IEnumerator ItemFlyRoutine(GameObject go, RectTransform iconRect, Vector2 targetLocal, BrickColor color, LevelReward reward)
    {
        const float arriveDist = 10f; // 픽셀

        while (go != null && Vector2.Distance(iconRect.anchoredPosition, targetLocal) > arriveDist)
        {
            iconRect.anchoredPosition = Vector2.MoveTowards(
                iconRect.anchoredPosition,
                targetLocal,
                _itemFlySpeed * Time.deltaTime
            );
            yield return null;
        }

        if (go != null)
        {
            iconRect.anchoredPosition = targetLocal;
            Destroy(go);
        }

        // 도착 후 UI 갱신 트리거
        OnItemFlown?.Invoke(color, reward);
    }
    // 월드 포지션(또는 UI 월드 포지션)을 캔버스 로컬(anchoredPosition)로 변환
    private Vector2 WorldToCanvasLocal(RectTransform canvasRect, Vector3 worldPos)
    {
        Camera cam = _uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _uiCanvas.worldCamera;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam,
            out Vector2 localPos
        );

        return localPos;
    }
}
