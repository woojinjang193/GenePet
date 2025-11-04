using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRecordData
{
    public string PetId;
    public string DisplayName;
    public GenesContainer Genes;
    public string Remark;

    public PetRecordData()
    {
        PetId = "";
        DisplayName = "";
        Genes = new GenesContainer();
        Remark = "Raising";
    }

    public PetRecordData(PetSaveData source)
    {
        PetId = source.ID;
        DisplayName = source.DisplayName;
        Genes = source.Genes;
        Remark = "Raising";
    }
}
