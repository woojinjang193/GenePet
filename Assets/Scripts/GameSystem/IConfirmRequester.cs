using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfirmRequester 
{
    void Confirmed(int requestNum);
    void Canceled(int requestNum);
}
