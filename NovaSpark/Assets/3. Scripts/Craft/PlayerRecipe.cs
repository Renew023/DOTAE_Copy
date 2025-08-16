using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 해금한 레시피를 저장 및 관리
/// </summary>
public class PlayerRecipe : MonoBehaviour
{
   [SerializeField] private List<int> unlockedRecipeIds = new();

    private void Awake()
    {
        // 초기화: 플레이어가 해금한 레시피 ID를 로드
        //LoadUnlockedRecipes();

        unlockedRecipeIds = new List<int>
        {
            1001,1002,1003,1004,1005,
            1006,1007,1008,1009,1011,
            1012,1013,1014,1015,1016,
            1017,1018,
        };
    }

    /// <summary>
    /// 새 레시피 해금
    /// </summary>
    public void Unlock(int id)
    {
        if (!unlockedRecipeIds.Contains(id))
            unlockedRecipeIds.Add(id);
    }

    /// <summary>
    /// 현재 해금된 레시피들의 ID 목록을 반환
    /// </summary>
    public List<int> GetUnlockedRecipeIds()
    {
        return unlockedRecipeIds.ToList();
    }
}
