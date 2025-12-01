using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class FamilyTreeUI : MonoBehaviour
{
    [SerializeField] private PetVisualLoaderForUI[] _blocks;

    public void Init(PetSaveData curPet)
    {
        AllOff();

        PetSaveData now = curPet;

        _blocks[0].gameObject.SetActive(true);
        _blocks[0].SetMyPet(now.Genes); //본인표시

        // 1번 블록부터 부모 표시 시작
        for (int i = 1; i < _blocks.Length; i++)
        {
            string fatherId = now.FatherId;
            string motherId = now.MotherId;

            if (string.IsNullOrWhiteSpace(fatherId) || string.IsNullOrWhiteSpace(motherId))// 둘 중 하나라도 없으면 종료
            {
                Debug.Log($"{i}번째 세대 루프 종료 아빠ID:{fatherId}, 엄마ID:{motherId}");
                break;
            }
                
            var father = GetPetData.GetHadPetDataByID(fatherId);  // 아버지 데이터
            var mother = GetPetData.GetIslandPetDataByID(motherId);  // 어머니 데이터

            if (father == null || mother == null)// 기록 없으면 종료
            {
                Debug.Log($"루프 종료 아빠:{father}, 엄마:{mother}");
                break;
            }
                

            _blocks[i].gameObject.SetActive(true); //블록 on
            _blocks[i].SetParents(father.Genes, mother.Genes); // 부모 렌더링

            // 다음 루프는 아버지를 기준으로 위로 올라감
            now = father;
        }
    }
    private void AllOff()
    {
        for (int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i].gameObject.SetActive(false);
        }
    }

}
