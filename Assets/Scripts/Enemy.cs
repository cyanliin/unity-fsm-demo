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

    public float maxHp = 100;
    public float hp = 100;

    private NavMeshAgent agent;
    private IEnumerator fireCoroutine;
    private bool isAttacking = false; // 記錄是否正在攻擊

    // 行為列舉
    public enum EnemyStatus
    {
        Idle, Chase, Attack, Escape, Heal
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
        if (status == EnemyStatus.Idle)
        {
            Idle();
        }
        if (status == EnemyStatus.Chase)
        {
            Chase();
        }
        if (status == EnemyStatus.Attack)
        {
            Attack();
        }
        if (status == EnemyStatus.Escape)
        {
            Escape();
        }

        if (status == EnemyStatus.Heal)
        {
            Heal();
        }
    }

    // 狀態行為：閒置
    private void Idle()
    {
        // ...
    }

    // 狀態行為：追逐
    private void Chase()
    {
        agent.SetDestination(target.position);
    }

    // 狀態行為：攻擊
    private void Attack()
    {
        // 看向目標
        transform.LookAt(target);
        
        // 只執行一次，開始重複射擊
        if (!isAttacking) 
        {
            StartCoroutine(fireCoroutine);
            isAttacking = true;
        }
        //if (???)
        //{
        //    StopCoroutine(fireCoroutine);
        //}
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

    // 狀態行為：補血
    private void Heal()
    {

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
    }




}
