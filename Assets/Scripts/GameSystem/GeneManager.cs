using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GeneManager : Singleton<GeneManager>
{
    [SerializeField] private float _legendaryPerc = 0.0005f;   //0 하나씩 지워야함
    [SerializeField] private float _epicPerc = 0.001f;         //0 하나씩 지워야함
    [SerializeField] private float _rarePerc = 0.005f;         //0 하나씩 지워야함

    private Dictionary<PartType, List<PartBaseSO>> _parts = new Dictionary<PartType, List<PartBaseSO>>();
    private List<PartBaseSO> _options = new List<PartBaseSO>();

    private int _loadedTypeCount = 0;
    private int _totalTypes = 10;
    private bool _ready = false;

    public bool IsReady { get { return _ready; } }

    protected override void Awake()
    {
        base.Awake();
        CreateBuckets();
        LoadAllGenes();
    }
    private void CreateBuckets()
    {
        if (_parts.ContainsKey(PartType.Body) == false) _parts[PartType.Body] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Color) == false) _parts[PartType.Color] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Ear) == false) _parts[PartType.Ear] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Eye) == false) _parts[PartType.Eye] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Horn) == false) _parts[PartType.Horn] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Mouth) == false) _parts[PartType.Mouth] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Pattern) == false) _parts[PartType.Pattern] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Personality) == false) _parts[PartType.Personality] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Tail) == false) _parts[PartType.Tail] = new List<PartBaseSO>();
        if (_parts.ContainsKey(PartType.Wing) == false) _parts[PartType.Wing] = new List<PartBaseSO>();
    }

    private void LoadAllGenes()
    {
        AsyncOperationHandle<IList<BodySO>> handle_Body = Addressables.LoadAssetsAsync<BodySO>("BodySO", null);
        handle_Body.Completed += OnBodiesLoaded;

        AsyncOperationHandle<IList<ColorSO>> handle_color = Addressables.LoadAssetsAsync<ColorSO>("ColorSO", null);
        handle_color.Completed += OnColorsLoaded;

        AsyncOperationHandle<IList<EarSO>> handle_Ear = Addressables.LoadAssetsAsync<EarSO>("EarSO", null);
        handle_Ear.Completed += OnEarsLoaded;

        AsyncOperationHandle<IList<EyeSO>> handle_Eye = Addressables.LoadAssetsAsync<EyeSO>("EyeSO", null);
        handle_Eye.Completed += OnEyesLoaded;

        AsyncOperationHandle<IList<HornSO>> handle_Horn = Addressables.LoadAssetsAsync<HornSO>("HornSO", null);
        handle_Horn.Completed += OnHornsLoaded;

        AsyncOperationHandle<IList<MouthSO>> handle_Mouth = Addressables.LoadAssetsAsync<MouthSO>("MouthSO", null);
        handle_Mouth.Completed += OnMouthsLoaded;

        AsyncOperationHandle<IList<PatternSO>> handle_Pattern = Addressables.LoadAssetsAsync<PatternSO>("PatternSO", null);
        handle_Pattern.Completed += OnPatternsLoaded;

        AsyncOperationHandle<IList<PersonalitySO>> handle_Personality = Addressables.LoadAssetsAsync<PersonalitySO>("PersonalitySO", null);
        handle_Personality.Completed += OnPersonalitiesLoaded;

        AsyncOperationHandle<IList<TailSO>> handle_Tail = Addressables.LoadAssetsAsync<TailSO>("TailSO", null);
        handle_Tail.Completed += OnTailsLoaded;

        AsyncOperationHandle<IList<WingSO>> handle_Wing = Addressables.LoadAssetsAsync<WingSO>("WingSO", null);
        handle_Wing.Completed += OnWingsLoaded;

    }

    private void OnBodiesLoaded(AsyncOperationHandle<IList<BodySO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Body].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Body].Add(handle.Result[i]);
            }
            Debug.Log($"BodySO 로드: {_parts[PartType.Body].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"BodySO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnColorsLoaded(AsyncOperationHandle<IList<ColorSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Color].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Color].Add(handle.Result[i]);
            }
            Debug.Log($"ColorSO 로드: {_parts[PartType.Color].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"ColorSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnEarsLoaded(AsyncOperationHandle<IList<EarSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Ear].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Ear].Add(handle.Result[i]);
            }
            Debug.Log($"EarSO 로드: {_parts[PartType.Ear].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"EarSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnEyesLoaded(AsyncOperationHandle<IList<EyeSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Eye].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Eye].Add(handle.Result[i]);
            }
            Debug.Log($"EyeSO 로드: {_parts[PartType.Eye].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"EyeSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnHornsLoaded(AsyncOperationHandle<IList<HornSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Horn].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Horn].Add(handle.Result[i]);
            }
            Debug.Log($"HornSO 로드: {_parts[PartType.Horn].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"HornSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnMouthsLoaded(AsyncOperationHandle<IList<MouthSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Mouth].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Mouth].Add(handle.Result[i]);
            }
            Debug.Log($"MouthSO 로드: {_parts[PartType.Mouth].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"MouthSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnPatternsLoaded(AsyncOperationHandle<IList<PatternSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Pattern].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Pattern].Add(handle.Result[i]);
            }
            Debug.Log($"PatternSO 로드: {_parts[PartType.Pattern].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"PatternSO 로드 실패: {handle.OperationException}");
        }
    }
    private void OnPersonalitiesLoaded(AsyncOperationHandle<IList<PersonalitySO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Personality].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Personality].Add(handle.Result[i]);
            }
            Debug.Log($"PersonalitySO 로드: {_parts[PartType.Personality].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"PersonalitySO 로드 실패: {handle.OperationException}");
        }

    }
    private void OnTailsLoaded(AsyncOperationHandle<IList<TailSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Tail].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Tail].Add(handle.Result[i]);
            }
            Debug.Log($"TailSO 로드: {_parts[PartType.Tail].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"TailSO 로드 실패: {handle.OperationException}");
        }

    }
    private void OnWingsLoaded(AsyncOperationHandle<IList<WingSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _parts[PartType.Wing].Clear();

            for (int i = 0; i < handle.Result.Count; i++)
            {
                _parts[PartType.Wing].Add(handle.Result[i]);
            }
            Debug.Log($"WingSO 로드: {_parts[PartType.Wing].Count}개");
            CheckIsReady();
        }
        else
        {
            Debug.LogError($"WingSO 로드 실패: {handle.OperationException}");
        }

    }

    private RarityType RollRarity()
    {
        float randV = UnityEngine.Random.value;

        float legendaryCut = _legendaryPerc;
        float epicCut = _legendaryPerc + _epicPerc;
        float rareCut = _legendaryPerc + _epicPerc + _rarePerc;

        if (randV < legendaryCut)
        {
            return RarityType.Legendary;
        }
        else if (randV < epicCut)
        {
            return RarityType.Epic;
        }
        else if (randV < rareCut)
        {
            return RarityType.Rare;
        }
        else
        {
            return RarityType.Common;
        }
    }

    private PartBaseSO TryPickByRarity(List<PartBaseSO> source, RarityType want, PartType partType) //레어도로 파츠 뽑기
    {
        _options.Clear();

        if (source == null || source.Count == 0)
        {
            Debug.LogError($"{partType}리스트가 빔");
            return null;
        }
        //뽑고싶은 레어도의 파츠를 리스트에 넣음
        for (int i = 0; i < source.Count; i++)
        {
            PartBaseSO cur = source[i];
            if (cur != null && cur.Rarity == want)
            {
                _options.Add(cur);
            }
        }
        //옵션이 없으면 null 리턴
        if (_options.Count == 0)
        {
            Debug.LogError($"{want} 등급의 {partType} 파츠가 없음");
            return null; 
        }

        int rand = UnityEngine.Random.Range(0, _options.Count);
        return _options[rand];
    }                  

    private PartBaseSO PickOneByRarity(List<PartBaseSO> source, PartType partType)
    {
        if (source == null || source.Count == 0)
        {
            Debug.Log($"리스트가 빔: {partType}");
            return null;
        }
        RarityType target = RollRarity();
        PartBaseSO pick = TryPickByRarity(source, target, partType);
        if (pick == null)
        {
            return null;
        }
        return pick;
    }

    private T GetRandomPart<T>(PartType type) where T : PartBaseSO
    {
        List<PartBaseSO> list;

        if (_parts.TryGetValue(type, out list) == false) //딕셔너리에 리스트 없으면
        {
            list = new List<PartBaseSO>();
            _parts[type] = list;
            Debug.Log($"리스트 만듬: {type} out {list}");
        }
        PartBaseSO pick = PickOneByRarity(list, type);
        if (pick == null)
        {
            return null;
        }
        return pick as T;
    }

    public BodySO GetRandomBodySO() { return GetRandomPart<BodySO>(PartType.Body); }
    public EyeSO GetRandomEyeSO() { return GetRandomPart<EyeSO>(PartType.Eye); }
    public EarSO GetRandomEarSO() { return GetRandomPart<EarSO>(PartType.Ear); }
    public MouthSO GetRandomMouthSO() { return GetRandomPart<MouthSO>(PartType.Mouth); }
    public TailSO GetRandomTailSO() { return GetRandomPart<TailSO>(PartType.Tail); }
    public WingSO GetRandomWingSO() { return GetRandomPart<WingSO>(PartType.Wing); }
    public HornSO GetRandomHornSO() { return GetRandomPart<HornSO>(PartType.Horn); }
    public PatternSO GetRandomPatternSO() { return GetRandomPart<PatternSO>(PartType.Pattern); }
    public ColorSO GetRandomColorSO() { return GetRandomPart<ColorSO>(PartType.Color); }
    public PersonalitySO GetRandomPersonalitySO() { return GetRandomPart<PersonalitySO>(PartType.Personality); }

    public T GetPartSOByID<T>(PartType part, string id) where T : PartBaseSO
    {
        List<PartBaseSO> list;

        if (_parts.TryGetValue(part, out list) == false)
        {
            Debug.LogError($"{part}리스트 없음");
            return null;
        }

        if (list == null || list.Count == 0)
        {
            Debug.LogError($"{part} 리스트가 비었음");
            return null;
        }
            
        for (int i = 0; i < list.Count; i++)
        {
            PartBaseSO cur = list[i];
            if(cur == null)
            {
                continue;
            }

            if (id == cur.ID)
            {
                T typed = cur as T;
                if (typed == null)
                {
                    Debug.LogError($"GetPartSOByID 타입 불일치: 요청 타입 {typeof(T).Name}, 실제 타입 {cur.GetType().Name}, id={id}");
                    return null;
                }
                return typed;
            }
        }

        Debug.LogWarning($"GetPartSOByID: {part} 에서 id '{id}' 찾지 못함");
        return null;
    }

    private void CheckIsReady()
    {
        _loadedTypeCount++;
        if(_loadedTypeCount >= _totalTypes)
        {
            _ready = true;
        }
    }
}




