using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBall : MonoBehaviour
{
    private float damage;
    private CharacterObject character;
    private Rigidbody2D rigidBody2D;
    private HashSet<int> oneAttackCollider = new();
    private string componentTag;

    public void Initialize(Vector2 pos, CharacterObject owner)
    {
        oneAttackCollider.Clear();
        rigidBody2D = GetComponent<Rigidbody2D>();
        rigidBody2D.velocity = pos * owner.characterRuntimeData.attackSpeed.Current * 5f;

        damage = owner.characterRuntimeData.damage.Current;
        character = owner;

        if (character.TryGetComponent<NPC>(out NPC a))
        {
            componentTag = "NPC";
        }
        if (character.TryGetComponent<Enemy>(out Enemy b))
        {
            componentTag = "Enemy";
        }
        if (character.TryGetComponent<Player>(out Player c))
        {
            componentTag = "Player";
        }

        StartCoroutine(SpawnOut());
        StartCoroutine(RotateBall());
    }

    public IEnumerator SpawnOut()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    public IEnumerator RotateBall()
    {
        while (true)
        {
            transform.Rotate(0, 0, Time.deltaTime * 3f);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (oneAttackCollider.Contains(collision.GetInstanceID())) return;
        oneAttackCollider.Add(collision.GetInstanceID());
        //if (!character.photonView.IsMine)
        //{
        //    UtilityCode.Logger("공격자가 맞나요");
        //    return; 
        //}
        if (!RoomSettingData.Instance.isBattleBetweenNPCs)
        {
            if (componentTag == "NPC" && collision.GetComponent<NPC>() != null)
                return;
        }

        if (!RoomSettingData.Instance.isBattleBetweenEnemys)
        {
            if (componentTag == "Enemy" && collision.GetComponent<Enemy>() != null)
                return;
        }

        if (!RoomSettingData.Instance.isNPCAttackEnable)
        {
            if (componentTag == "Player" && collision.GetComponent<NPC>() != null)
                return;
        }

        //if (character.GetComponent<NPC>() != null && collision.GetComponent<NPC>() != null)
        //{
        //    return;
        //}

        if (collision.gameObject == this.character.gameObject) return; //네트워크에서는 IsMine 을 쓸 수도 있음.

        if (collision.TryGetComponent<IDamageable>(out IDamageable taker))
        {
            float resultDamage = damage;

            if (character.characterRuntimeData.criticalPercent.Current * RoomSettingData.Instance.criticalBalance > UnityEngine.Random.Range(0f, 100f))
            {
                resultDamage = damage * UnityEngine.Random.Range(1.6f, 2.0f);
                //UserHelpManager.Instance.CreateText("크리티컬 적중");
            }

            taker.CallTakeDamage(resultDamage, this.character);
            //UserHelpManager.Instance.CreateText($"데미지 {damage}를 입혔습니다.");
        }
    }
}