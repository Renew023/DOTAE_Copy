using System;
using System.Collections.Generic;
[Serializable]
public class BloodData {
    public DesignEnums.BloodType BloodName;
    public List<DesignEnums.Ability> bloodAbility;
    public List<int> abilityInitLevel;
    public List<float> abilityValue;
    public List<int> abilityMaxLevel;
}
