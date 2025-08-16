using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDatable
{ 
    int GetID();
    int GetLevel();
    string GetName();

    Sprite GetIcon();
}
