using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfirmRequester
{
    void Confirmed();
    void Canceled();
}
