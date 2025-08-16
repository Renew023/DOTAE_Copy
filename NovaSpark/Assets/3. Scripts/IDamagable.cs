using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // 데미지를 받았을 때 호출
    /// <param name="amount">입힐 데미지 양</param>
    void TakeDamage(float amount, CharacterObject characterObject);//공격자 정보도 필요함.

    void CallTakeDamage(float amount, CharacterObject characterObject); //네트워크 전달용.

    float GetHp();

    float GetHpPercent();
}
