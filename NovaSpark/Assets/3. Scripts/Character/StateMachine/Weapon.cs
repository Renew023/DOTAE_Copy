using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private CharacterObject character;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider;
    [SerializeField] private Quaternion rotation;
    [SerializeField] public Animator animator;
    [SerializeField] public bool curAttackState = false;
    [SerializeField] private string componentTag;

    [Header("런타임")]
    [SerializeField] private ParticleInstance attackParticle;
    [SerializeField] private HashSet<int> oneAttackCollider = new();

    #region Photon 잔재
    //[SerializeField] private PhotonView photonView;

    //private void WeaponEnable()
    //{
    //    character.WeaponEnable();
    //}

    //private void WeaponEnd()
    //{
    //    character.AttackEnd();
    //}
    #endregion

    private void Awake()
    {
        //gameObject.AddPhoton();
        //photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if(character == null)
        Init();
        character.Weapon = this;
    }

    private void OnDisable()
    {
        //WeaponEnd();
        //Attack(false);
        collider.enabled = false;
        if (attackParticle != null)
        {
            ParticleManager.Instance.RemoveEffect(ParticleParameterHash.PlayerAttack_Particle, attackParticle);
            attackParticle = null;
        }
        curAttackState = false;
    }

    IEnumerator ReEnableCollider(bool isAttack)
    {
        collider.enabled = !isAttack;
        yield return null; // FixedUpdate 틱 한 번 대기
        collider.enabled = isAttack;
    }

    public void Attack(bool isAttack)
    {
        if (curAttackState == isAttack) return;
        curAttackState = isAttack;

        if (!gameObject.activeInHierarchy) 
        {
            //Debug.LogWarning("Weapon이 비활성화 상태입니다. 공격을 수행할 수 없습니다.");
            return;
        }

        StartCoroutine(ReEnableCollider(isAttack));
        if (character == null)
        {
            character = gameObject.GetComponentInParent<CharacterObject>();
        }
        if (isAttack)
        {
            oneAttackCollider.Clear();
            damage = character.characterRuntimeData.damage.Current;
            if (attackParticle == null)
            {
                attackParticle = ParticleManager.Instance.CreateEffect(ParticleParameterHash.PlayerAttack_Particle, this.gameObject);
            }
            //Debug.Log("파티클 생성");
        }
        else
        {
            if (attackParticle != null)
            {
                ParticleManager.Instance.RemoveEffect(ParticleParameterHash.PlayerAttack_Particle, attackParticle);
                attackParticle = null;
                //Debug.Log("파티클 제거");
            }
        }
    }

    public void ChangePlace(bool isPlace)
    {
        spriteRenderer.enabled = isPlace;
    }

    public void OnTriggerEnter2D(Collider2D collision)
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
            if(componentTag == "NPC" && collision.GetComponent<NPC>() != null) 
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

            if (character.characterRuntimeData.criticalPercent.Current * RoomSettingData.Instance.criticalBalance > Random.Range(0f, 100f))
            {
                resultDamage = damage * Random.Range(1.6f, 2.0f);
                //UserHelpManager.Instance.CreateText("크리티컬 적중");
            }

            taker.CallTakeDamage(resultDamage, this.character);
            //UserHelpManager.Instance.CreateText($"데미지 {damage}를 입혔습니다.");
        }
    }

    private void Init()
    {
        character = gameObject.GetComponentInParent<CharacterObject>(true);
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

        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        collider = gameObject.GetComponent<Collider2D>();
        animator = gameObject.GetComponent<Animator>();
        collider.enabled = false;
        rotation = transform.rotation;
    }
}
