using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public float closeRange;
    public float sightRange;
    public float patrolRange;
    public Node start;
    public float damage;
    public float cooldown;

    public Ability firstAbility;

    private GameObject _playerTarget;
    private Vector3 _patrolCenter;
    private SpriteRenderer _spriteRenderer;
    private bool _isChasing = false;

    [SerializeField] private Sprite _spriteFront;
    [SerializeField] private Sprite _spriteSide;
    [SerializeField] private Sprite _spriteBack;

    void Start()
    {
        _patrolCenter = transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        GetAbilityValues();

        StartCoroutine(UpdateClosestPlayer());
    }

    private void GetAbilityValues()
    {
        firstAbility = GetComponentInChildren<PlayerInfos>().characterClass.abilities[1];
        cooldown = firstAbility.cooldown;
        closeRange = firstAbility.range;
        damage = firstAbility.damages;
    }

    void Update()
    {
        if (start != null)
        {
            start.Evaluate();
        }
    }

    private IEnumerator UpdateClosestPlayer()
    {
        while (true)
        {
            FindClosestPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        GameObject closestPlayer = null;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = p;
            }
        }

        if (closestPlayer != _playerTarget)
        {
            _playerTarget = closestPlayer;
            _isChasing = (_playerTarget != null);
            UpdateBehaviorTree();
        }
    }

    private void UpdateBehaviorTree()
    {
        Patrol patrol = new Patrol(agent, _patrolCenter, patrolRange);
        SeePlayer seePlayer = new SeePlayer(agent, _playerTarget, sightRange);
        ChasePlayer chasePlayer = new ChasePlayer(agent, _playerTarget);
        CloseEnough closeEnough = new CloseEnough(agent, _playerTarget, closeRange);
        Attack attack = new Attack(_playerTarget, damage, cooldown, gameObject, firstAbility);

        Sequence sequence1 = new Sequence(new List<Node> { closeEnough, attack });
        Selector selector1 = new Selector(new List<Node> { sequence1, chasePlayer });
        Sequence sequence2 = new Sequence(new List<Node> { seePlayer, selector1 });
        Selector selector2 = new Selector(new List<Node> { sequence2, patrol });

        start = selector2;
    }
}
