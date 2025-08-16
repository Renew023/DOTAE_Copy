using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestPlayer : MonoBehaviour
{
    public SkillTreeManager skillTreeManager { get; private set; }

    void Awake()
    {
        var state = new PlayerSkillState();
        skillTreeManager = new SkillTreeManager(state, this, 10, 3, 10);
    }
}

