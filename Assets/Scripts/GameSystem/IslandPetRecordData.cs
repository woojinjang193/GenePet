using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPetRecordData
{
    public GenesContainer Genes;

    public IslandPetRecordData()
    {
        Genes = new GenesContainer();
    }

    public IslandPetRecordData(PetSaveData source)
    {
        Genes = source.Genes;
    }
}
