using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public PlayerInfos playerInfos;
    public Ability firstAbility;

    public GameObject _playerTarget;
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

        playerInfos = GetComponent<PlayerInfos>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        GetAbilityValues();

        StartCoroutine(UpdateClosestPlayer());
    }

    private void GetAbilityValues()
    {
        firstAbility = playerInfos.characterClass.abilities[1];
        cooldown = firstAbility.cooldown;
        closeRange = firstAbility.range;
        damage = firstAbility.damages;
        agent.speed = playerInfos.characterClass.speed;
    }

    void Update()
    {
        if (start != null)
        {
            start.Evaluate();
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private IEnumerator UpdateClosestPlayer()
    {
        while (true)
        {
            FindClosestTarget();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

        List<GameObject> targets = new List<GameObject>();

        foreach (GameObject p in players)
        {
            if (p != gameObject)
                targets.Add(p);
        }
        foreach (GameObject b in bots)
        {
            if (b != gameObject)
                targets.Add(b);
        }

        float minDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (GameObject t in targets)
        {
            float distance = Vector3.Distance(transform.position, t.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = t;
            }
        }

        if (closestTarget != _playerTarget)
        {
            _playerTarget = closestTarget;
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
