using System.Diagnostics;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : LivingEntity, IDamageable
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }
    public float damage = 10f;
    public float traceDistance = 10f;
    public float AttackDistance = 1.5f;
    public float AttackInterber = 1f;
    private float lastAttackTime;
    public Transform target;
    public LayerMask targetLayer;
    public ParticleSystem hitEffect;
    public Collider monsterCollider;
    public Renderer monsterRanderer;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public MonsterData monsterData;
    private NavMeshAgent agent;
    private Animator monsterAnimator;
    private AudioSource monsterAudioSource;
    private Status currentStatus;
    public static readonly int IdHasTarget = Animator.StringToHash("HasTarget");
    public static readonly int IdDie = Animator.StringToHash("Die");

    public Status CurrentStatus
    {
        get => currentStatus;
        set
        {
            currentStatus = value;
            switch (currentStatus)
            {
                case Status.Idle:
                    monsterAnimator.SetBool(IdHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    monsterAnimator.SetBool(IdHasTarget, true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    monsterAnimator.SetBool(IdHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Die:
                    agent.isStopped = true;
                    monsterAnimator.SetTrigger(IdDie);
                    monsterCollider.enabled = false;
                    monsterAudioSource.PlayOneShot(deathClip);
                    break;
            }
        }
    }

    public void Setup (MonsterData  data)
    {
        startingHealth = data.maxHp;
        damage = data.damage;
        agent.speed = data.speed;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterAnimator = GetComponent<Animator>();
        monsterAudioSource = GetComponent<AudioSource>();
        if (monsterData != null)
        {
            Setup(monsterData);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        monsterCollider.enabled = true;

        CurrentStatus = Status.Idle;
    }

    private void OnDisable()
    // 몬스터가 Nav위에 없을때 경로를 리셋해줌.
    {
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case Status.Idle:
            UpdateIdle();
            break;
            case Status.Trace:
            UpdateTrace();
            break;
            case Status.Attack:
            UpdateAttack();
            break;
            case Status.Die:
            UpdateDie();
            break;
        }
    }

    private void UpdateDie() { }

    private void UpdateAttack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > AttackDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        if (Time.time >= lastAttackTime + AttackInterber)
        {
            lastAttackTime = Time.time;
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                var hitNormal = (transform.position - target.position).normalized;
                damageable.OnDamage(damage, target.position, hitNormal);
            }
        }
    }

    private void UpdateTrace()
    {
        if(target != null && Vector3.Distance(transform.position, target.position) < AttackDistance)
        {
            CurrentStatus = Status.Attack;
            return;
        }

        if(target == null && Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            target = null;
            CurrentStatus = Status.Idle;
            return;
        }

        agent.SetDestination(target.position);
    }

    private void UpdateIdle()
    {
        if(target != null && Vector3.Distance(transform.position, target.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }
        target = FindTarget(traceDistance);
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        Transform closet = null;
        float closetDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < closetDistance)
            {
                closetDistance = distance;
                closet = colliders[i].transform;
            }
        }
        return closet;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDeath)
        {
            return;
        }
        base.OnDamage(damage, hitPoint, hitNormal);

        monsterAudioSource.PlayOneShot(hitClip);
        hitEffect.transform.position = hitPoint;
        hitEffect.transform.forward = hitNormal;
        hitEffect.Play();
    }

    public override void Die()
    {
        base.Die();
        CurrentStatus = Status.Die;
    }
}
