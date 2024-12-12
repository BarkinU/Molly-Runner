using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Detection : MonoBehaviour
{
    public static Detection instance;
    public ObjectPooler objectPooler;
    public ParticleSystem spawnVFX;


    void Awake()
    {
        instance = this;
        objectPooler = GameObject.FindGameObjectWithTag("ObjectPooler").GetComponent<ObjectPooler>();
    }

    Options collidedDoor;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 9)
        {
            spawnVFX.Play();
            collidedDoor = other.GetComponentInParent<Options>();
            Collider collidedDoorCollider = other;
            int runnersAmountToAdd = collidedDoor.GetRunnersAmountToAdd(collidedDoorCollider, GameManager.instance.totalBabyList.Count);
            AddRunners(runnersAmountToAdd);
            other.gameObject.SetActive(false);
        }
        GameManager.instance.UpdateList();

    }

    public void AddRunners(int amount)
    {
        if (collidedDoor.bonus.GetBonusType() == BonusUtils.BonusType.Add || collidedDoor.bonus.GetBonusType() == BonusUtils.BonusType.Multiply)
        {
            if (amount + GameManager.instance.totalBabyList.Count >= 600)
            {
                amount = Mathf.Abs(600 - GameManager.instance.totalBabyList.Count);
            }
            if (GameManager.instance.totalBabyList.Count < 600)
            {
                for (int i = 0; i < amount; i++)
                {
                    float radiusX = Random.insideUnitCircle.x;
                    float radiusZ = Random.insideUnitCircle.y;
                    GameObject runnerInstance = objectPooler.SpawnFromPool("Baby", transform.position + new Vector3(radiusX, transform.position.y, radiusZ) - Vector3.forward * 1.8f, Quaternion.identity);
                    runnerInstance.transform.parent = PlayerController.instance.babiesParent;
                    GameManager.instance.totalBabyList.Add(runnerInstance);

                }
            }
        }
        else
        {
            if (amount > GameManager.instance.totalBabyList.Count)
            {
                int j = GameManager.instance.totalBabyList.Count;
                if (j > 0)
                    for (int i = 0; i < j; i++)
                    {
                        GameManager.instance.totalBabyList[0].SetActive(false);
                        GameManager.instance.totalBabyList.RemoveAt(0);
                    }
            }
            else
            {
                //subs and division
                for (int j = 0; j < amount; j++)
                {
                    if (GameManager.instance.totalBabyList[0] != null)
                    {
                        GameManager.instance.totalBabyList[0].SetActive(false);
                        GameManager.instance.totalBabyList.RemoveAt(0);
                    }
                }
            }
        }
        GameManager.instance.UpdateList();
    }
}


