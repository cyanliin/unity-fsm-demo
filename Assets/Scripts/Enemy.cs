using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStatus status; // !!!!!! <------- 
    public Transform target;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject healPool;

    public float maxHp = 100;
    public float hp = 100;

    private NavMeshAgent agent;
    private IEnumerator fireCoroutine;
    private bool isAttacking = false; // 記錄是否正在攻擊

    public int healCount = 2;


    // 行為列舉
    public enum EnemyStatus
    {
        Idle, Chase, Attack, Escape, Heal, GetHeals
    }

    void Start()
    {
        // 目前狀態
        status = EnemyStatus.Idle;

        // 導航 AI
        agent = GetComponent<NavMeshAgent>();

        // 定義開火coroutine 供後續啟動、停止
        fireCoroutine = FireBullet();
    }

    void Update()
    {
        // 狀態機

        // 算距離
        float d = Vector3.Distance(transform.position, target.transform.position);
        // Debug.Log("距離:" + d);

        // Idle (閒置) 
        if (status == EnemyStatus.Idle)
        {
            // 往 Chase 判斷
            if (d < 10)
            {
                status = EnemyStatus.Chase;
                return;
            }

            // 往 Heal 判斷
            if (hp < 50)
            {
                status = EnemyStatus.Heal;
                return;
            }

            // 行為
            Idle();
        }

        // Chase (追逐)
        if (status == EnemyStatus.Chase)
        {
            // 往 Idle 的判斷
            if (d > 12)
            {
                status = EnemyStatus.Idle;
                return;
            }

            // 往 Heal 的判斷
            if (hp < 50)
            {
                status = EnemyStatus.Heal;
                return;
            }

            // 往 Attack 判斷
            if (d < 5)
            {
                status = EnemyStatus.Attack;
                return;
            }

            // 行為
            Chase();
        }

        // Attack (攻擊)
        if (status == EnemyStatus.Attack)
        {
            // 往 Chase 的判斷
            if (d > 5)
            {
                status = EnemyStatus.Chase;
                StopCoroutine(fireCoroutine);
                isAttacking = false;
                return;
            }

            // 往 Heal 的判斷
            if (hp < 50)
            {
                status = EnemyStatus.Heal;
                StopCoroutine(fireCoroutine);
                isAttacking = false;
                return;
            }

            // 行為
            Attack();
        }

        // Escape (逃跑)
        if (status == EnemyStatus.Escape)
        {
            Escape();
        }

        // Heal (治療)
        if (status == EnemyStatus.Heal)
        {
            // 往 Idle 判斷
            if (hp >= 100)
            {
                status = EnemyStatus.Idle;
                return;
            }

            // 往 GetHeals 判斷
            if (healCount == 0) 
            {
                status = EnemyStatus.GetHeals;
                return;
            }

            // 往 Chase 判斷
            Heal();
        }

        // GetHeal (取得治療道具)
        if (status == EnemyStatus.GetHeals)
        {
            // 往 Idle 判斷
            if (healCount >= 2)
            {
                status = EnemyStatus.Idle;
                return;
            }

            // 行為
            GetHeals();
        }
    }

    // 狀態行為：閒置
    private void Idle()
    {
        agent.isStopped = true;
    }

    // 狀態行為：追逐
    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    // 狀態行為：攻擊
    private void Attack()
    {
        agent.isStopped = true;

        // 看向目標
        transform.LookAt(target);
        
        // 只執行一次，開始重複射擊
        if (!isAttacking) 
        {
            StartCoroutine(fireCoroutine);
            isAttacking = true;
        }
    }

    IEnumerator FireBullet()
    {
        // 無窮迴圈
        while (true)
        {
            // // 產生出子彈
            Instantiate(bulletPrefab, firePoint.transform.position, transform.rotation);

            // 停兩秒
            yield return new WaitForSeconds(1);
        }
    }

    // 狀態行為：逃離
    private void Escape()
    {

    }

    // 狀態行為：取的補血道具
    private void GetHeals()
    {
        agent.isStopped = false;
        agent.SetDestination(healPool.transform.position);
    }

    // 狀態行為：補血
    private void Heal()
    {
        if (healCount > 0)
        {
            hp = 100;
            healCount = healCount - 1;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 當被子彈攻擊
        if (other.tag == "Bullet")
        {
            hp = hp - 10;

            if (hp < 0)
            {
                Destroy(gameObject);
            }
        }

        // 當被治療池 
        if (other.tag == "HealPool")
        {
            healCount = 2;
        }
    }




}
