using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public float health = 0;
    public Animator enemyAnimator;
    [SerializeField] public Slider healthBar;
    public bool isDead;
    private AudioSource mySource;
    [SerializeField] private ParticleSystem[] myDeathVFX;
    [HideInInspector] public TextMeshProUGUI enemyHealthBarText;

    void Awake()
    {
        health = 100;
        enemyAnimator = GetComponent<Animator>();
        mySource = GetComponent<AudioSource>();
        myDeathVFX = GetComponentsInChildren<ParticleSystem>();
        enemyHealthBarText = transform.GetChild(0).Find("HealthText").GetComponent<TextMeshProUGUI>();

    }

    void Update()
    {
        if (PlayerController.instance.currentPhase == PlayerController.Phases.fight)
        {
            if (health <= 0 && isDead == false)
            {
                Death();
            }
            else
                healthBar.value = health;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAnimator.SetBool("isAttacking", true);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.currentPhase == PlayerController.Phases.dead)
            {
                enemyAnimator.SetBool("isWin", true);
            }
        }
    }

    void Death()
    {
        health = Mathf.Clamp(health, 0, 2000);
        healthBar.value = health;
        enemyAnimator.SetBool("isDead", true);
        PlayerController.instance.myAnim.SetBool("isRunning", true);
        AudioManager.instance.Play("VictoryEnemyDead");
        isDead = true;

        for (int i = 0; i < myDeathVFX.Length; i++)
        {
            myDeathVFX[i].Play();
        }

    }
    void EnemyAttack()
    {
        var player = PlayerController.instance;

        mySource.clip = Array.Find(AudioManager.instance.clips, x => x.name == "enemyhit");
        mySource.Play();

        if (enemyAnimator.GetBool("isDead") == false)
        {
            player.myHealthBar.value -= ((15 + ((int)(SaveManager.instance.currentLevel / 5))) - GameManager.instance.playerDefenseValue);
            player.healthBarText.text = player.myHealthBar.value.ToString();

            if (player.myHealthBar.value <= 0)
                player.currentPhase = PlayerController.Phases.dead;
        }

    }

}
