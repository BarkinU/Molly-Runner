using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleEnum
    {
        Hammer, Maze, MovingCylinder, ObstanceWall, ObstanceX, Saw, Turret, DroppingBalls, ParkingDoor, MovingHead, WindRose, Empty
    }
    public ObstacleEnum obstacleType;

    [Header("                                Wind Rose")]
    public float windRoseRotnSpeed = 5f;

    [Header("                                Moving Cylinder")]
    [SerializeField] private bool state;
    private Vector3 velocity = Vector3.zero;
    [Header("                                 Moving Head")]
    [SerializeField] private float movingHeadRotateSpeed = 250f;

    #region Pendulum
    float timer = 0f;
    float speed = 100; // hammer speed
    int phase = 0;
    bool processDone;
    #endregion

    [Header("                                 Turret")]
    [SerializeField] private GameObject turretBall;
    private bool ballFired;
    Transform turretBallBasePosn;
    [SerializeField] private bool flatFire = false;
    public float ballTimer = 0;
    private float ballSpeed = 11;
    private PlayerController playerController;


    [Header("                                 DroppingBalls")]
    public List<GameObject> balls;


    private void OnEnable()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (obstacleType == ObstacleEnum.Turret)
        {
            turretBallBasePosn = transform.GetChild(1);
        }

    }

    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        if (playerController.currentPhase != PlayerController.Phases.inGame)
        {
            return;
        }
        switch (obstacleType)
        {

            case ObstacleEnum.Hammer:

                timer += Time.deltaTime;
                if (timer > 1f)
                {
                    phase++;
                    phase %= 4;            //Keep the phase between 0 to 3.
                    timer = 0f;
                }

                //speedlerin başındaki hard 
                switch (phase)
                {

                    case 0:
                        transform.Rotate(0f, 0f, speed * (1 - timer) * Time.deltaTime);  //Speed, from maximum to zero.
                        break;
                    case 1:
                        transform.Rotate(0f, 0f, -speed * timer * Time.deltaTime);       //Speed, from zero to maximum.
                        break;
                    case 2:
                        transform.Rotate(0f, 0f, -speed * (1 - timer) * Time.deltaTime); //Speed, from maximum to zero.
                        break;
                    case 3:
                        transform.Rotate(0f, 0f, speed * timer * Time.deltaTime);        //Speed, from zero to maximum.
                        break;
                }
                break;
        }
    }

    void Update()
    {
        ObstacleType();
    }

    void ObstacleType()
    {
        switch (obstacleType)
        {


            case ObstacleEnum.Turret:

                if (playerController && Vector3.Distance(transform.position, playerController.transform.position) < 22f)
                {
                    if (flatFire == false)
                    {

                        turretBall.transform.Translate(0, 0, ballSpeed * Time.deltaTime);
                        if (ballFired == false)
                        {
                            transform.LookAt(playerController.transform);
                            ballFired = true;
                        }

                        ballTimer += Time.deltaTime;
                        if (ballTimer > 1.5f)
                        {
                            turretBall.transform.position = turretBallBasePosn.position;
                            ballTimer = 0;
                            ballFired = false;
                        }
                    }
                    else
                    {
                        turretBall.transform.Translate(0, 0, ballSpeed * Time.deltaTime);
                        ballTimer += Time.deltaTime;
                        if (ballTimer > 1)
                        {
                            turretBall.transform.position = turretBallBasePosn.position;
                            ballTimer = 0;
                            ballFired = false;
                        }
                    }
                }

                break;

            case ObstacleEnum.Saw:
                transform.Rotate(0, -250f * Time.deltaTime, 0);
                break;
            case ObstacleEnum.Maze:
                transform.Rotate(0, 125f * Time.deltaTime, 0, Space.World);
                transform.GetChild(1).transform.Rotate(0, 500 * Time.deltaTime, 0);

                break;
            case ObstacleEnum.MovingCylinder:

                if (transform.localPosition.x > 1.4f)
                    state = false;
                else if (transform.localPosition.x < -1.4f)
                    state = true;

                if (state == false)
                {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(-2.1f, transform.localPosition.y, transform.localPosition.z), ref velocity, 1f);
                    transform.Rotate(350 * Time.deltaTime, 0, 0);
                }
                else
                {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(2.1f, transform.localPosition.y, transform.localPosition.z), ref velocity, 1f);
                    transform.Rotate(-350 * Time.deltaTime, 0, 0);
                }
                break;

            case ObstacleEnum.ObstanceX:
                if (processDone)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, .35f, transform.position.z), ref velocity, .2f);
                    if (transform.position.y < .4f)
                        processDone = false;
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, 3.4f, transform.position.z), ref velocity, .9f);
                    if (transform.position.y > 3f)
                        processDone = true;
                }
                break;

            case ObstacleEnum.ObstanceWall:
                if (processDone)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, .5f, transform.position.z), ref velocity, .25f);
                    if (transform.position.y < 1f)
                        processDone = false;
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, 4f, transform.position.z), ref velocity, .9f);
                    if (transform.position.y > 3.5f)
                        processDone = true;
                }
                break;
            case ObstacleEnum.WindRose:
                transform.Rotate(0, 0, windRoseRotnSpeed * Time.deltaTime, Space.Self);
                break;
            case ObstacleEnum.ParkingDoor:
                transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, new Quaternion(0, 30, 0, 0), Time.deltaTime);
                transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, new Quaternion(0, 30, 0, 0), Time.deltaTime);
                break;
            case ObstacleEnum.MovingHead:
                transform.Rotate(0, movingHeadRotateSpeed * Time.deltaTime, 0, Space.World);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && obstacleType == ObstacleEnum.DroppingBalls)
        {
            foreach (GameObject go in balls)
            {
                go.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}
