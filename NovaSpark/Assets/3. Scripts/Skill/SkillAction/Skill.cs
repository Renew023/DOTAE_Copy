using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill : MonoBehaviour
{
	[Header("외부에서 받아와야하는 Data")]
	[SerializeField] private ParticleSystem _particle;
    [SerializeField] private List<SkillPool> _colliders;
    [SerializeField] public CharacterObject owner;
    [SerializeField] public Rigidbody2D ownerRB;
    [SerializeField] public Rigidbody2D skillRB;

    [SerializeField] public Transform target;
	[SerializeField] private bool isTargeting;

    [SerializeField] private List<float> _damage = new();
    [SerializeField] private List<float> _distance = new();
    [SerializeField] private List<float> _time = new();

    [SerializeField] private int _curStack;

	public void Initialize(CharacterObject owner, Transform target, Vector2 mouseDelta, int key)
    {
		if (key == 0) return;
		_colliders = GetComponentsInChildren<SkillPool>(true).ToList();
		skillRB = GetComponent<Rigidbody2D>();
		_particle = GetComponentInChildren<ParticleSystem>();
		this.owner = owner;
		this.target = target;

		_distance.Clear();
		_distance.AddRange(DataManager.Instance.SkillDataByID[key].distance);
		_time.Clear();
		_time.AddRange(DataManager.Instance.SkillDataByID[key].time);
		//gameObject.transform.parent = owner.gameObject.transform;
		gameObject.transform.localPosition = owner.gameObject.transform.localPosition;
        //gameObject.transform.position = this.owner.transform.position;
		for(int i=0; i<_distance.Count; i++)
		{
			_colliders[i].Initialize(mouseDelta, owner.characterRuntimeData.damage.Max, _distance[i], _time[i]);
        }
    }

    private void Awake()
	{
		gameObject.SetActive(false);
	}

    private void OnEnable()
	{
		owner.isSkill = true;
        ownerRB = owner.GetComponent<Rigidbody2D>();
		
		foreach (var collider in _colliders)
		{
			collider.gameObject.SetActive(false);
		}
		_curStack = 0;
		_colliders[_curStack].gameObject.SetActive(true);
		//시작할 때가 아닌 삭제되기 전에 딜레이  
	}

	private void OnDisable()
	{
		if(owner != null)
		owner.isSkill = false;
	}

	public IEnumerator StartDeleteDelay(float time)
	{
		yield return new WaitForSeconds(time);
		NextSkill();
	}

	public void NextSkill()
	{
        _colliders[_curStack].gameObject.SetActive(false);
        if (_colliders.Count > ++_curStack)
        {
			//_attackedTarget.Clear(); // 다음 스킬에서는 공격을 또 할 수 있도록
            _colliders[_curStack].gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}