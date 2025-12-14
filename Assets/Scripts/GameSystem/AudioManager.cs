using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public bool IsReady { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        IsReady = true; //테스트용, 지워야함
        //어드레서블 세팅
    }
    public void PlayBGM(string bgm)
    {

    }
    public void PlaySFX(string SFX)
    {

    }
}
