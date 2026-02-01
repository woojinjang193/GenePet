using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PinballVisualManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Camera _worldCamera; // 메인 카메라

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

    public event Action<BrickColor, Sprite, int> OnItemFlown;
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
        GameObject go = Manager.Pool.Get(particle, worldPos, transform);

        StartCoroutine(ParticleRoutine(go));
    }
    private IEnumerator ParticleRoutine(GameObject go) //파티클 키고끄는 코루틴
    {
        go.SetActive(true);
        yield return new WaitForSeconds(_particleDuration);
        go.SetActive(false);

        Manager.Pool.Release(go);
        //Destroy(go); //TODO: 풀로 변경
    }
    // =========== 아이템 획득시 ====================
    private void OnGivenItem(BrickColor color, LevelReward reward, Vector3 worldPos)
    {
        if (_uiCanvas == null) return;
        if (color == BrickColor.None && reward.RewardType == RewardType.None) return;

        // UI 아이콘 생성(캔버스 아래)
        RectTransform canvasRect = _uiCanvas.transform as RectTransform;

        GameObject go = Manager.Pool.Get(_itemIcon, Vector3.zero, _uiCanvas.transform); //UI는 position 0으로 둠
        RectTransform iconRect = go.GetComponent<RectTransform>();

        Image iconImg = go.GetComponent<Image>();
        if (iconImg != null) iconImg.color = Color.white; //알파 초기화용

        //  reward에 맞는 스프라이트로 세팅
        if (Manager.Item != null)
        {
            iconImg.sprite = Manager.Item.ItemImages.GetItemSprite(reward.RewardType);
        }

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
            Manager.Pool.Release(go);
            return;
        }

        Vector2 targetLocal = WorldToCanvasLocal(canvasRect, target.position); // 목표도 동일 변환 경로 사용

        int rewardAmount = reward.Amount;
        StartCoroutine(ItemFlyRoutine(go, iconRect, targetLocal, color, iconImg.sprite, rewardAmount));
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

        Manager.Pool.Release(go);
    }
    private IEnumerator ItemFlyRoutine(GameObject go, RectTransform iconRect, Vector2 targetLocal, BrickColor color, Sprite icon, int rewardAmount)
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
            Manager.Pool.Release(go);
            //Destroy(go);
        }

        // 도착 후 UI 갱신 트리거
        OnItemFlown?.Invoke(color, icon, rewardAmount);
    }
    // 월드 포지션(또는 UI 월드 포지션)을 캔버스 로컬(anchoredPosition)로 변환
    private Vector2 WorldToCanvasLocal(RectTransform canvasRect, Vector3 worldPos)
    {
        // 1) 월드 -> 스크린(월드 카메라로)
        Camera wc = _worldCamera != null ? _worldCamera : Camera.main;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(wc, worldPos);

        // 2) 스크린 -> 캔버스 로컬(UI 카메라/Overlay 설정에 맞게)
        Camera uiCam = _uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _uiCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, uiCam, out Vector2 localPos
        );

        return localPos;
    }

}
