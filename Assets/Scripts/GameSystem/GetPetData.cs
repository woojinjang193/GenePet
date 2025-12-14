using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetPetData
{
    public static PetSaveData GetPetDataByID(string id)
    {
        var list = Manager.Save.CurrentData.UserData.HavePetList;

        for (int i = 0; i < list.Count; i++)
        {
            var pet = list[i];

            if (pet.ID == id)
            {
                return pet;
            }
        }
        return null;
    }
    public static PetSaveData GetHadPetDataByID(string id)
    {
        var list = Manager.Save.CurrentData.UserData.HadPetList;
    
        for (int i = 0; i < list.Count; i++)
        {
            var pet = list[i];
    
            if (pet.ID == id)
            {
                return pet;
            }
        }
        return null;
    }
    public static PetSaveData GetIslandPetDataByID(string id)
    {
        var list = Manager.Save.CurrentData.UserData.IslandPetList;
    
        for (int i = 0; i < list.Count; i++)
        {
            var pet = list[i];
    
            if (pet.ID == id)
            {
                return pet;
            }
        }
        return null;
    }
}
