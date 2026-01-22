using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Progress;

public class PinballVisualManager : MonoBehaviour
{
    [Header("파괴 파티클")]
    [SerializeField] private GameObject _color0DestroyParticle;
    [SerializeField] private GameObject _color1DestroyParticle;
    [SerializeField] private GameObject _color2DestroyParticle;
    [SerializeField] private GameObject _color3DestroyParticle;

    [Header("파괴 파티클")]
    [SerializeField] private GameObject _itemIcon;

    [Header("슬롯 트렌스폼")]
    [SerializeField] private RectTransform _slot1Transform;
    [SerializeField] private RectTransform _slot2Transform;
    [SerializeField] private RectTransform _slot3Transform;

    [Header("파티클 유지시간")]
    [SerializeField] private float _particleDuration = 2f;

    [Header("아이템 날아가는 속도")]
    [SerializeField] private float _itemFlySpeed = 3f;

    public event Action<BrickColor, LevelReward> OnItemFlown;
    public void RegisterBrick(PinballBrick brick) //이벤트 해제 해줘야함
    {
        brick.OnBroken += OnBrickBroken;
        brick.OnGiveItem += OnGivenItem;
    }
    public void UnregisterBrick(PinballBrick brick)
    {
        brick.OnBroken -= OnBrickBroken;
        brick.OnGiveItem -= OnGivenItem;
    }
    private void OnBrickBroken(BrickColor color, Vector3 worldPos) //블록 파괴시
    {
        GameObject particle;

        switch (color)
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
    private IEnumerator ParticleRoutine(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(_particleDuration);
        go.SetActive(false);

        Destroy(go); //TODO: 풀로 변경
    }
    private void OnGivenItem(BrickColor color, LevelReward reward, Vector3 worldPos)
    {
        GameObject go = Instantiate(_itemIcon, worldPos, Quaternion.identity, transform); //TODO: 풀로 변경
        go.SetActive(true);

        if (color == BrickColor.None) //일반 블록 연출
        {
            //사라지는 연출
        }
        else if((color == BrickColor.one))
        {
            StartCoroutine(ItemFlyRoutine(go, _slot1Transform, color, reward));
        }
        else if((color == BrickColor.two))
        {
            StartCoroutine(ItemFlyRoutine(go, _slot2Transform, color, reward));
        }
        else if((color == BrickColor.three))
        {
            StartCoroutine(ItemFlyRoutine(go, _slot3Transform, color, reward));
        }
    }

    private IEnumerator ItemFlyRoutine(GameObject item, RectTransform targetRect,BrickColor color, LevelReward reward)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        // 목표 UI의 스크린 좌표(픽셀)
        Vector3 targetScreen = RectTransformUtility.WorldToScreenPoint(cam, targetRect.position);

        // 스크린 -> 월드로 변환 (item이 있는 Z(카메라 거리)에 맞춤)
        float zDist = Mathf.Abs(cam.transform.position.z - item.transform.position.z);
        Vector3 targetWorld = cam.ScreenToWorldPoint(new Vector3(targetScreen.x, targetScreen.y, zDist));

        // 도착 판정 거리
        const float arriveDist = 0.05f;

        // 이동
        while (item != null && Vector3.Distance(item.transform.position, targetWorld) > arriveDist)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                targetWorld,
                _itemFlySpeed * Time.deltaTime
            );

            yield return null;
        }

        // 도착 처리
        if (item != null)
        {
            item.transform.position = targetWorld;
            Destroy(item); // TODO: 풀로 변경
        }

        OnItemFlown?.Invoke(color, reward);
    }
}
