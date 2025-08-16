using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillPool : MonoBehaviour
{

    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private List<SkillActionBase> _colliders;

    [SerializeField] public Skill skill { get; private set; }
    //[SerializeField] protected Transform target;

    [SerializeField] protected float damage;
    [SerializeField] protected float distance;
    [SerializeField] protected float time;
    [SerializeField] protected Vector2 mouseDelta;
    [SerializeField] protected Vector2 direction;
    [SerializeField] protected Coroutine coroutine;

    public void Initialize(Vector2 mouseDelta, float damage, float distance, float time)
    {
        skill = GetComponentInParent<Skill>(true);
        _colliders = GetComponentsInChildren<SkillActionBase>(true).ToList();
        _particle = GetComponentInChildren<ParticleSystem>(true);

        this.mouseDelta = mouseDelta;
        this.damage = damage;
        this.distance = distance;
        this.time = time;
        this.direction = (mouseDelta - (Vector2)skill.owner.transform.position).normalized;
        for (int i = 0; i < _colliders.Count; i++)
        {
            _colliders[i].Initialize(mouseDelta, damage, distance, time);
        }

    }// Start is called before the first frame update

    private void OnEnable()
    {
        foreach (var collider in _colliders)
        {
            collider.gameObject.SetActive(true);
        }
        StartCoroutine(skill.StartDeleteDelay(time));
    }

    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        if (skill.ownerRB != null)
        {
            skill.ownerRB.velocity = Vector2.zero;
            skill.skillRB.velocity = Vector2.zero;
        }
        //gameObject.transform.position = Vector2.zero;
        //_attackedTarget.Clear();
    }
}
