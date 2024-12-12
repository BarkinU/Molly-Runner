using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public static CameraScript instance;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private Vector3 relativePosn = Vector3.zero;
    public enum MyState
    {
        Running,
        Finished
    }
    public MyState myState;
    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
    }

    void Start()
    {
        target = PlayerController.instance.transform;
        myState = MyState.Running;
    }

    void LateUpdate()
    {
        switch (myState)
        {
            case MyState.Running:
                transform.position = new Vector3(transform.position.x, target.position.y + offset.y, target.position.z + offset.z);
                break;
            case MyState.Finished:
                transform.position = Vector3.Lerp(transform.position, GameManager.instance.finishText.transform.parent.position - Vector3.forward * 3.2f + Vector3.right * 0.3f, Time.deltaTime * 2f);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, new Quaternion(0, 0, 0, 1), Time.deltaTime);
                GameManager.instance.FinishEvent();
                break;
        }
    }

}
