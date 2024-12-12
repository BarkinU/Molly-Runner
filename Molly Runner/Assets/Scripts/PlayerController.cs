using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    #region Mains

    public static PlayerController instance;

    public Transform target;

    public GameObject playerPrefab;
    [SerializeField] private float sideSpeed;
    public float forwardSpeed;
    public float xPosn;
    public Animator myAnim;
    public Animation obstacleHitAnim;
    [SerializeField] private GameObject skillBar;
    public AudioSource crowdSound;
    public List<ParticleSystem> babyDeathParticles;
    [SerializeField] private List<ParticleSystem> mySkills;


    #endregion

    public Phases currentPhase;
    public enum Phases
    {
        mainScreen,
        inGame,
        dead,
        finish,
        stack,
        fight,
        cave

    }

    #region Stats
    [SerializeField] private float enemyHealth;

    private Quaternion oldRotation;
    #endregion


    #region Babies

    public Transform babiesParent;
    [HideInInspector] public Transform objectPoolerParent;
    private float stacklessFinishTimer = .8f;

    #endregion

    #region bools

    private bool isDeadPanelActive = false;
    private bool attackCompleted = false;
    private bool finishCompleted = false;
    [HideInInspector] public bool deathProtectionBool = false;
    public GameObject finishObject;

    #endregion

    #region MobileTouch

    private Touch theTouch;

    #endregion

    #region PlayerHealthSystem

    public Slider myHealthBar;
    public TextMeshProUGUI healthBarText;

    #endregion

    #region Audio

    [SerializeField] private AudioClip[] myClips;
    public AudioSource mySource;

    #endregion
    private float sortTimer = 0;
    public int columns = 3;
    public float space = .12f;
    private void Awake()
    {

        if (instance == null && instance != this)
            instance = this;
        else if (instance != null)
        {

            Destroy(this);

        }
    }

    private void Start()
    {
        currentPhase = Phases.mainScreen;

        //Character anim adjustments
        myAnim.SetBool("isRunning", false);
        myAnim.SetBool("isDied", false);
        myAnim.SetBool("isAttacking", false);

        myHealthBar.maxValue = 100;
        myHealthBar.value = 100;
        healthBarText.text = myHealthBar.maxValue.ToString();

        objectPoolerParent = GameObject.Find("ObjectPoolerParent").transform;
    }

    private void Update()
    {
        switch (currentPhase)
        {
            case Phases.mainScreen:

                // just for wait
                break;

            case Phases.inGame:
                TouchControl();
                break;
            case Phases.dead:
                if (isDeadPanelActive == false)
                {
                    myAnim.SetBool("isRunning", false);
                    if (target != null)
                        target.GetComponent<Enemy>().enemyAnimator.SetBool("isAttacking", false);
                    myAnim.SetBool("isDied", true);

                    UIManager.instance.FailedScreen();
                    crowdSound.Stop();
                    AudioManager.instance.Play("PlayerDeath2");
                    isDeadPanelActive = true;
                }
                break;
            case Phases.stack:

                BabyStackOperation();
                if (GameManager.instance.gameObject != null)
                {
                    if (GameManager.instance.totalBabyList.Count <= 0)
                    {
                        currentPhase = Phases.fight;
                    }

                }
                this.transform.position = new Vector3(transform.position.x, 0.095f, transform.position.z);
                VFXManager.instance.CancelInvoke();
                break;
            case Phases.fight:
                if (babiesParent.GetChild(0).transform.GetChild(0).transform.gameObject.activeSelf)
                {
                    babiesParent.SetParent(this.transform);
                    babiesParent.GetChild(0).transform.GetChild(0).transform.gameObject.SetActive(false);
                }
                if (Vector3.Distance(transform.position, target.transform.position) > 1.2f)
                {
                    myAnim.SetBool("isRunning", true);
                    transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime);
                }
                else
                {
                    transform.LookAt(target);
                    if (attackCompleted == false && UIManager.instance.finishPanel.activeSelf == false)
                    {
                        enemyHealth = target.GetComponent<Enemy>().health;
                        oldRotation = transform.rotation;
                        myAnim.SetBool("isAttacking", true);
                        myAnim.SetBool("isRunning", false);
                        CameraScript.instance.myState = CameraScript.MyState.Running;

                        GameManager.instance.increaseFightSpeedButton.gameObject.SetActive(true);
                        attackCompleted = true;
                    }
                }
                this.transform.position = new Vector3(transform.position.x, 0.095f, transform.position.z);
                VFXManager.instance.CancelInvoke();

                break;
            case Phases.finish:
                Time.timeScale = 1;
                stacklessFinishTimer -= Time.deltaTime;
                if (finishCompleted == false && stacklessFinishTimer <= 0)
                {
                    CameraScript.instance.myState = CameraScript.MyState.Finished;
                    if (target != null)
                    {
                        GameManager.instance.increaseFightSpeedButton.gameObject.SetActive(false);
                        Time.timeScale = 1;
                    }
                    finishCompleted = true;
                }
                transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed, Space.World);
                babiesParent.transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed, Space.World);
                transform.position = Vector3.Lerp(transform.position, new Vector3(0, transform.position.y, transform.position.z), Time.deltaTime * 2f);
                babiesParent.position = Vector3.Lerp(babiesParent.position, new Vector3(0, babiesParent.position.y, babiesParent.position.z), Time.deltaTime * 2f);
                break;

        }
    }


    private void BabySorting()
    {
        for (int i = 0; i < GameManager.instance.totalBabyList.Count; i++)
        {
            Vector3 pos = CalcPosition(i);
            if (GameManager.instance.totalBabyList[i].activeSelf == true && GameManager.instance.totalBabyList[i] != null)
                GameManager.instance.totalBabyList[i].transform.position = Vector3.Lerp(GameManager.instance.totalBabyList[i].transform.position, new Vector3(transform.position.x - .2f + pos.x, pos.y, transform.position.z - 1.2f - pos.z), Time.deltaTime * 4f);
        }
    }

    private void BabyStackOperation()
    {
        sortTimer += Time.deltaTime;
        if (sortTimer < .3f)
            babiesParent.GetChild(0).gameObject.SetActive(false);
        if (sortTimer < 2f)
            BabySorting();
        else
            babiesParent.transform.Translate(0, 0, Time.deltaTime * 5f);

    }

    Vector3 CalcPosition(int index) // call this func for all your objects
    {
        float posX = (index % columns) * space;
        float posz = (index / columns) * space;
        return new Vector3(posX, transform.position.y, posz);
    }

    void MollyFistAttack()
    {
        if (UIManager.instance.finishPanel.activeSelf == false && target.GetComponent<Enemy>().isDead == false)
        {
            if (myHealthBar.value > 0 && myAnim.GetBool("isDied") == false)
            {
                enemyHealth -= (10 + GameManager.instance.playerAttackValue);
                enemyHealth = Mathf.Clamp(enemyHealth, 0, 20000);

                target.GetComponent<Enemy>().health = enemyHealth;
                target.GetComponent<Enemy>().enemyHealthBarText.text = enemyHealth.ToString();

                mySource.clip = myClips[0];
                mySource.Play();
            }
        }
    }

    void MollyKickAttack()
    {
        if (UIManager.instance.finishPanel.activeSelf == false && target.GetComponent<Enemy>().isDead == false)
        {
            if (myHealthBar.value > 0 && myAnim.GetBool("isDied") == false)
            {
                enemyHealth -= (10 + GameManager.instance.playerAttackValue);
                enemyHealth = Mathf.Clamp(enemyHealth, 0, 20000);

                target.GetComponent<Enemy>().health = enemyHealth;
                target.GetComponent<Enemy>().enemyHealthBarText.text = enemyHealth.ToString();

                mySource.clip = myClips[1];
                mySource.Play();
            }
        }
    }

    IEnumerator EnemyDeadTimer(Animator anim)
    {
        yield return new WaitForSeconds(1f);
        transform.rotation = oldRotation;
        myAnim.SetBool("isAttacking", false);
        myAnim.SetBool("isRunning", true);
        CameraScript.instance.myState = CameraScript.MyState.Finished;
        currentPhase = Phases.finish;
    }

    private void TouchControl()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed, Space.World);
        babiesParent.transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed, Space.World);

        babiesParent.transform.position = Vector3.Lerp(babiesParent.transform.position, new Vector3(xPosn, transform.position.y, babiesParent.transform.position.z), 7f * Time.deltaTime);

        //template oluştur touchlar için mekaniğe göre her projece bas geç
        if (Input.GetMouseButton(0) && myAnim.GetBool("isRunning") == true)
        {
            xPosn += Input.GetAxis("Touch") * sideSpeed * Time.deltaTime;
            xPosn = Mathf.Clamp(xPosn, -1.4f, 1.4f);
            transform.position = new Vector3(xPosn, transform.position.y, transform.position.z);    //lerp çevir
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OpenGateButton"))
        {
            other.gameObject.transform.parent.GetChild(0).GetComponent<Animation>().Play();
        }

        //babylayer
        if (other.transform.gameObject.layer == 8 && currentPhase == Phases.stack)
        {
            //this
            transform.localScale += Vector3.one * .002f;

            if (myHealthBar.value >= myHealthBar.maxValue)
                myHealthBar.maxValue += 1;

            myHealthBar.value += 1;
            healthBarText.text = myHealthBar.value.ToString();

            // baby
            GameManager.instance.totalBabyList.Remove(other.gameObject);
            other.gameObject.SetActive(false);

        }

        //cave
        if (other.transform.gameObject.layer == 11)
        {
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false;
            babiesParent.gameObject.SetActive(false);
            objectPoolerParent.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }

        //obstacles layer
        if (other.transform.gameObject.layer == 6 && deathProtectionBool == false)
        {
            myHealthBar.value -= 20;
            healthBarText.text = myHealthBar.value.ToString();
            if (myHealthBar.value <= 0)
                currentPhase = Phases.dead;

            obstacleHitAnim.Play();
            AudioManager.instance.Play("obstacle");
            GameManager.instance.InvokeRepeating("UpdateList", 0, .1f);


        }

        //finishobjectlayer
        if (other.transform.gameObject.layer == 10)
        {
            if (this.gameObject.transform.position.y > 1f)
                VFXManager.instance.CancelJumpEvent();
            skillBar.SetActive(false);
            if (target != null)
            {
                myAnim.SetBool("isRunning", false);
                transform.GetChild(0).LookAt(target);
                currentPhase = Phases.stack;

                foreach (ParticleSystem item in mySkills)
                {
                    item.Stop();
                }
                crowdSound.Stop();

            }
            else
            {
                currentPhase = Phases.finish;
            }
            other.enabled = false;

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (target.GetComponent<Enemy>().isDead == true)
            {
                StartCoroutine(EnemyDeadTimer(target.GetComponent<Animator>()));
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.gameObject.layer == 6 && deathProtectionBool == false)
        {
            if (other.gameObject.name == "ball")
            {
                myHealthBar.value -= 20;
                healthBarText.text = myHealthBar.value.ToString();

                if (myHealthBar.value <= 0)
                    currentPhase = Phases.dead;

                obstacleHitAnim.Play();
                AudioManager.instance.Play("obstacle");
                GameManager.instance.UpdateList();
            }
        }
    }
}
