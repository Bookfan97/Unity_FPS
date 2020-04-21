using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop = 2f;
    private Vector3 targetPoint, startPoint;
    public NavMeshAgent agent;
    public float keepChasingTime;
    private float chaseCounter, fireCount, shotWaitCounter, shootTimeCounter;
    public GameObject bullet;
    public Transform firePoint;
    public float fireRate, waitBetweenShots=2f, timeToShoot = 1f;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        shootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;
        if(!chasing)
        {
            if(Vector3.Distance(transform.position, targetPoint)<distanceToChase)
            {
                chasing = true;
                fireCount = 1f;
            }
            if(chaseCounter>0)
            {
                chaseCounter -= Time.deltaTime;
            }
            if(chaseCounter<=0)
            {
                agent.destination = startPoint;
            }
            if (agent.remainingDistance < .25f)
            {
                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            //transform.LookAt(targetPoint);
            //theRB.velocity = transform.forward * moveSpeed;
            if (PlayerController.instance.gameObject.activeInHierarchy)
            {
                if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
                {
                    agent.destination = targetPoint;
                }
                else
                {
                    agent.destination = transform.position;
                }
                if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
                {
                    chasing = false;
                    chaseCounter = keepChasingTime;
                    shotWaitCounter = waitBetweenShots;
                    //agent.destination=startPoint;
                }
                if (shotWaitCounter > 0)
                {
                    shotWaitCounter -= Time.deltaTime;
                    if (shotWaitCounter <= 0)
                    {
                        shootTimeCounter = timeToShoot;
                    }
                    anim.SetBool("isMoving", true);
                }
                else
                {
                    shootTimeCounter -= Time.deltaTime;
                    if (shootTimeCounter > 0)
                    {
                        fireCount -= Time.deltaTime;
                        if (fireCount <= 0)
                        {
                            fireCount = fireRate;
                            firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1.5f, 0f));
                            Vector3 targetDir = PlayerController.instance.transform.position - transform.position;
                            float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
                            if (Mathf.Abs(angle) < 30f)
                            {
                                Instantiate(bullet, firePoint.position, firePoint.rotation);
                                anim.SetTrigger("fireShot");
                            }
                            else
                            {
                                shotWaitCounter = waitBetweenShots;
                            }

                        }
                        agent.destination = transform.position;
                    }
                    else
                    {
                        shotWaitCounter = waitBetweenShots;
                    }
                    anim.SetBool("isMoving", false);
                }
            }
        }
    }
}
