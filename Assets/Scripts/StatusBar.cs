using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Enemy enemy;
    public Image hpBar;
    public Vector3 offset;

    void Start()
    {
        
    }

    void Update()
    {
        // position
        transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position) + offset;

        // hp
        float sx = enemy.hp / enemy.maxHp;
        if (sx > 1)
            sx = 1;
        if (sx < 0)
            sx = 0;
        hpBar.rectTransform.localScale = new Vector3(sx, 1, 1);
    }
}
