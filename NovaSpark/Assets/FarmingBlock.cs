using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FarmingBlock : MonoBehaviour, IDamageable, IDatable
{
    private int maxHP;
    public FarmingRuntimeData data;
    private NavMeshObstacle obstacle;
    private SpriteRenderer sprite;

    public void Initialize(int key)
    {
        data = new FarmingRuntimeData(key);
        obstacle = GetComponent<NavMeshObstacle>();
        sprite = GetComponent<SpriteRenderer>();
        maxHP = data.blockHp;
    }

    public void CallTakeDamage(float amount, CharacterObject attacker)
    {
        TakeDamage(amount, attacker);
    }

    public float GetHp()
    {
        return data.blockHp;
    }

    public float GetHpPercent()
    {
        return (float)data.blockHp / (float)maxHP;
    }

    public void TakeDamage(float amount, CharacterObject attacker)
    {
        if (data.blockHp <= 0) return;

        if (!attacker.TryGetComponent<Player>(out var player))
        {
            return;
        }

        int damage = RoomSettingData.Instance.farmingBlockAttackDamage;
        //int damage = (int)amount - data.defence;
        data.blockHp -= damage;
        int count = RoomSettingData.Instance.farmingBlockAttackCountItem;
        //int count = damage / (maxHP > 0? maxHP : 1 / data.dropCount > 0 ? data.dropCount : 1);

        //Debug.Log("블럭 채취 개수" + count);

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < data.getItemIdList.Count; j++)
            {
                if (!(data.itemDropValue[j] * RoomSettingData.Instance.farmingBlockItemDropPercent > UnityEngine.Random.Range(0, 100))) continue;
                int itemId = data.getItemIdList[j];

                //Debug.Log("드랍된 아이템" + itemId);
                int valueRandom = data.getItemMaxDropValue[j];

                int itemValue = UnityEngine.Random.Range(0, valueRandom + 1);
                Item item = DataManager.Instance.GetItemByID(itemId);
                //Debug.Log("드랍된 아이템" + item.name_kr);

                if (RoomSettingData.Instance.isGetMaterial)
                {
                    int exp = (int)(itemValue * RoomSettingData.Instance.isGetMaterialExpPercent);
                    if(exp != 0)
                    player.AddExp(exp);

                    if (attacker.TryGetComponent<PlayerInventory>(out var inventory))
                    {
                        inventory.AddItem(item, itemValue);
                    }
                }
                //PhotonView.Find(attackerViewID).GetComponent<PlayerInventory>().AddItem(item, valueRandom);
            }
        }

        Color baseColor = Color.white;
        Color hitColor = Color.red; //Color.Lerp(baseColor, Color.red, 0.2f);

        StartCoroutine(WhileBack(
            0.5f,
            (timer) =>
            {
                sprite.color = Color.Lerp(hitColor, baseColor, timer / 0.2f);
                //spriteRenderer.color = Color.Lerp(hitColor, baseColor, timer / 0.2f);
            }));

        if (data.blockHp <= 0)
        {
            obstacle.enabled = false;
            gameObject.SetActive(false);
            RoomSettingData.Instance.blockCount--;
        }
    }

    private IEnumerator WhileBack(float time, Action<float> action)
    {
        float timer = 0f;

        while (time > timer)
        {
            timer += Time.deltaTime;
            action(timer);
            yield return null;
        }
    }

    public int GetID()
    {
        return data.FarmingBlockID;
    }

    public int GetLevel()
    {
        return 0;
    }

    public string GetName()
    {
        return data.name_kr;
    }

    public Sprite GetIcon()
    {
        return sprite.sprite;
    }
}
