using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRecordData
{
    public string PetId;
    public string DisplayName;
    public GenesContainer Genes;

    //public PetRecordData()
    //{
    //    PetId = "";
    //    DisplayName = "";
    //    Genes = new GenesContainer();
    //}

    public PetRecordData(PetSaveData source)
    {
        PetId = source.ID;
        DisplayName = source.DisplayName;
        Genes = source.Genes;
    }
}
