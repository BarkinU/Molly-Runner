using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FSG.MeshAnimator.Snapshot;
using Unity.Jobs;

public class BabyController : MonoBehaviour
{
    public SnapshotMeshAnimator myAnimator;
    private ParticleSystem myParticle;
    private PlayerController PCinstance;

    void Awake()
    {
        myAnimator = GetComponent<SnapshotMeshAnimator>();
        myParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    void Start()
    {
        PCinstance = PlayerController.instance;
    }
    void OnEnable()
    {
        if (GameManager.instance.gameObject != null)
            if (GameManager.instance.isStarted)
                OnPlayClickBaby();
    }
    public void OnPlayClickBaby()
    {
        myAnimator.Play(myAnimator.meshAnimations[1].animationName);
        myAnimator.defaultAnimation = myAnimator.meshAnimations[1];
    }

    IEnumerator ParticleTimer()
    {

        this.transform.parent = PlayerController.instance.objectPoolerParent;

        if (PCinstance.babyDeathParticles.Count < 11)
        {
            PCinstance.babyDeathParticles.Add(myParticle);
            AudioManager.instance.Play("LilPopPop"); //MÜMKÜN OLDUĞUNCA AZALT SES SAYISINI - audio sourceların object poolunu oluşturabilirsen çok iyi olur şirketler çok seviyor
            myParticle.Play();
            yield return new WaitForSeconds(.15f);
            PCinstance.babyDeathParticles.Remove(myParticle);
        }
        GameManager.instance.totalBabyList.Remove(this.gameObject);
        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && PlayerController.instance.currentPhase == PlayerController.Phases.inGame && PlayerController.instance.deathProtectionBool == false)
        {
            StartCoroutine(ParticleTimer());
        }
    }
}
