using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GeneticInfoSO", menuName = "SO/GeneticInfoSO")]
public class GeneticInfoSO : ScriptableObject
{
    public BodySO bodyDominant;
    public BodySO bodyRecessive;

    public PatternSO patternDominant;
    public PatternSO patternRecessive;

    public EyeSO eyeDominant;
    public EyeSO eyeRecessive;

    public MouthSO mouthDominant;
    public MouthSO mouthRecessive;

    public EarSO EarDominant;
    public EarSO EarRecessive;

    public HornSO hornDominant;
    public HornSO hornRecessive;

    public TailSO tailDominant;
    public TailSO tailRecessive;

    public WingSO wingDominant;
    public WingSO wingRecessive;

    public ColorSO colorDominant;
    public ColorSO colorRecessive;

    public PersonalitySO personalityDominant;
    public PersonalitySO personalityRecessive;
    
}
