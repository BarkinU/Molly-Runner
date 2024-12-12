using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;
    private PlayerController playerController;
    public Button deathProtectionButton;
    public Button powerUpAgainstButton;
    public Button instanX2Button;
    public Button increaseSpeedButton;
    public Button reduceMollySizeButton;
    public Button jumpButton;
    public float jumpTimer = 10f;

    [Space]
    [Space]
    [Space]
    [Header("                                   Visual Effects")]
    [SerializeField] private ParticleSystem deathProtectionVFX;
    [SerializeField] private ParticleSystem powerUpAgainstBossVFX;
    [SerializeField] private ParticleSystem instantX2MollyVFX;
    [SerializeField] private ParticleSystem increaseSpeedVFX;
    [SerializeField] private ParticleSystem reduceMollySizeVFX;
    [SerializeField] private ParticleSystem jumpVFX;

    [Header("                                   Have Amounts")]
    [SerializeField] private TextMeshProUGUI deathProtectionAmounttext;
    [SerializeField] private TextMeshProUGUI powerUpAmountText;
    [SerializeField] private TextMeshProUGUI instantX2AmountText;
    [SerializeField] private TextMeshProUGUI increaseSpeedAmountText;
    [SerializeField] private TextMeshProUGUI reduceMollySizeAmountText;
    [SerializeField] private TextMeshProUGUI jumpAmountText;

    void Awake()
    {
        instance = this;
        TextAdjustment();
        StartInteractables();
    }

    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void StartInteractables()
    {
        if (SaveManager.instance.jumpAmount > 0)
        {
            jumpButton.interactable = true;
        }
        else
            jumpButton.interactable = false;

        if (SaveManager.instance.instantX2Amount > 0)
        {
            instanX2Button.interactable = true;
        }
        else
            instanX2Button.interactable = false;

        if (SaveManager.instance.increaseSpeedAmount > 0)
        {
            increaseSpeedButton.interactable = true;
        }
        else
            increaseSpeedButton.interactable = false;

        if (SaveManager.instance.deathProtectionAmount > 0)
        {
            deathProtectionButton.interactable = true;
        }
        else
            deathProtectionButton.interactable = false;

        if (SaveManager.instance.reduceMollySizeAmount > 0)
        {
            reduceMollySizeButton.interactable = true;
        }
        else
            reduceMollySizeButton.interactable = false;

        if (SaveManager.instance.powerUpAgainstBossAmount > 0)
        {
            powerUpAgainstButton.interactable = true;
        }
        else
            powerUpAgainstButton.interactable = false;
    }

    public void DeathProtection()
    {
        if (SaveManager.instance.deathProtectionAmount > 0)
        {
            InteractableAdjustment();

            deathProtectionVFX.gameObject.SetActive(true);
            deathProtectionVFX.Play();

            StartCoroutine(SkillTimer(5f, deathProtectionVFX.gameObject));

            SaveManager.instance.deathProtectionAmount--;
            playerController.deathProtectionBool = true;
        }
        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();
    }

    public void PowerUpAgainstBoss()
    {
        if (SaveManager.instance.powerUpAgainstBossAmount > 0)
        {
            InteractableAdjustment();

            powerUpAgainstBossVFX.gameObject.SetActive(true);
            powerUpAgainstBossVFX.Play();

            SaveManager.instance.powerUpAgainstBossAmount--;
            GameManager.instance.playerAttackValue += 6;
            GameManager.instance.AttackAdjustment();

        }
        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();

    }

    public void InstanX2Molly()
    {
        if (SaveManager.instance.instantX2Amount > 0)
        {
            InteractableAdjustment();
            instantX2MollyVFX.transform.parent.gameObject.SetActive(true);
            instantX2MollyVFX.Play();

            StartCoroutine(SkillTimer(instantX2MollyVFX.main.duration, instantX2MollyVFX.gameObject));

            SaveManager.instance.instantX2Amount--;
            Detection.instance.spawnVFX.Play();

            Detection.instance.AddRunners(GameManager.instance.totalBabyList.Count);


        }
        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();
    }


    public void IncreaseSpeed()
    {
        if (SaveManager.instance.increaseSpeedAmount > 0)
        {
            InteractableAdjustment();

            increaseSpeedVFX.gameObject.SetActive(true);
            increaseSpeedVFX.Play();

            StartCoroutine(SkillTimer(instantX2MollyVFX.main.duration, instantX2MollyVFX.gameObject));

            SaveManager.instance.increaseSpeedAmount--;
            playerController.forwardSpeed += 3f;

        }
        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();
    }

    public void ReduceMollySize()
    {
        if (SaveManager.instance.reduceMollySizeAmount > 0)
        {
            InteractableAdjustment();

            reduceMollySizeVFX.gameObject.SetActive(true);
            reduceMollySizeVFX.Play();

            StartCoroutine(SkillTimer(reduceMollySizeVFX.main.duration, reduceMollySizeVFX.gameObject));

            SaveManager.instance.reduceMollySizeAmount--;
            foreach (GameObject item in GameManager.instance.totalBabyList)
            {
                item.transform.localScale = item.transform.localScale / 2;
            }
        }

        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();
    }
    public void Jump()
    {
        if (SaveManager.instance.jumpAmount > 0 && playerController.currentPhase != PlayerController.Phases.fight && playerController.currentPhase != PlayerController.Phases.stack)
        {
            InteractableAdjustment();
            jumping = true;
            jumpVFX.gameObject.SetActive(true);
            jumpVFX.Play();

            StartCoroutine(SkillTimer(jumpVFX.main.duration, jumpVFX.gameObject));

            SaveManager.instance.jumpAmount--;

        }

        else
        {
            NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You cant use it now"));
        }
        TextAdjustment();

        SaveManager.instance.Save();
    }

    public void FixedUpdate()
    {
        if (jumping)
            JumpEvent();
    }

    void InteractableAdjustment()
    {
        instanX2Button.interactable = false;
        increaseSpeedButton.interactable = false;
        reduceMollySizeButton.interactable = false;
        jumpButton.interactable = false;
        powerUpAgainstButton.interactable = false;
        deathProtectionButton.interactable = false;
    }

    private void JumpEvent()
    {
        if (jumpTimer > 0)
        {
            playerController.transform.position = Vector3.Lerp(playerController.transform.position,
            new Vector3(playerController.transform.position.x,
            6f, playerController.transform.position.z),
            Time.deltaTime);

            jumpTimer -= Time.deltaTime;

            if (Mathf.Abs(playerController.transform.position.z - playerController.finishObject.transform.position.z) < 2f)
            {
                playerController.transform.position = new Vector3(playerController.transform.position.x, 0.095f, playerController.transform.position.z);

                playerController.babiesParent.transform.position = new Vector3(playerController.babiesParent.transform.position.x, 0.095f, playerController.babiesParent.transform.position.z);
            }

        }
        else
        {
            playerController.transform.position = Vector3.Lerp(playerController.transform.position,
            new Vector3(playerController.transform.position.x,
            0.095f, playerController.transform.position.z),
            Time.deltaTime);
            if (playerController.transform.position.y <= 0.1f)
            {
                CancelInvoke("JumpEvent");
            }
        }
    }
    public bool jumping = false;

    public void CancelJumpEvent()
    {
        jumping = false;
        playerController.babiesParent.transform.position = new Vector3(playerController.transform.position.x,
            0.095f, playerController.transform.position.z);
        playerController.transform.position = new Vector3(playerController.transform.position.x,
            0.095f, playerController.transform.position.z);
    }

    public void TextAdjustment()
    {
        jumpAmountText.text = SaveManager.instance.jumpAmount.ToString();
        reduceMollySizeAmountText.text = SaveManager.instance.reduceMollySizeAmount.ToString();
        increaseSpeedAmountText.text = SaveManager.instance.increaseSpeedAmount.ToString();
        instantX2AmountText.text = SaveManager.instance.instantX2Amount.ToString();
        deathProtectionAmounttext.text = SaveManager.instance.deathProtectionAmount.ToString();
        powerUpAmountText.text = SaveManager.instance.powerUpAgainstBossAmount.ToString();

    }

    private IEnumerator SkillTimer(float timer, GameObject go)
    {
        yield return new WaitForSeconds(timer);
        if (go == instantX2MollyVFX.gameObject)
            instantX2MollyVFX.transform.parent.gameObject.SetActive(false);
        else
        {
            playerController.deathProtectionBool = false;
            go.SetActive(false);
        }

    }

}
