using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject[] wayPoints;
    private int nextWayPointIndex = 0;
    public int health = 10;
    public int speed = 1;
    private GameManager gameManager;

    protected virtual void Start()
    {
        wayPoints = GameObject.FindGameObjectsWithTag("Waypoints");
        wayPoints = wayPoints.OrderBy(wp => int.Parse(wp.name)).ToArray();
        gameManager = FindObjectOfType<GameManager>();
    }

    protected virtual void Update()
    {
        MoveBalloon();
    }

private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Dart"))
    {
        health--;

        if (health <= 0)
        {
            if (other.gameObject.GetComponent<BalloonGun>() != null)
            {
                gameManager.UpdateScore(5);
            }
            else
            {
                gameManager.UpdateScore(3);
            }

            Destroy(this.gameObject);
        }
    }
}

private void MoveBalloon()
{
    var lastWayPointIndex = wayPoints.Length - 1;
    Vector3 lastWayPoint = wayPoints[lastWayPointIndex].transform.position + new Vector3(0, 2, 0);
    Vector3 nextWayPoint = wayPoints[nextWayPointIndex].transform.position + new Vector3(0, 2, 0);
    Vector3 direction = nextWayPoint - transform.position;

    // If enemy is more than 0.1 meters from the last waypoint
    if (Vector3.Distance(transform.position, lastWayPoint) > 0.1f)
    {
        // Rotate the balloon to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime * 100);

        // Keep moving towards the next waypoint
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    // Increase index so if enemy reaches one waypoint
    if (Vector3.Distance(transform.position, nextWayPoint) < 0.5f && nextWayPointIndex < lastWayPointIndex)
    {
        nextWayPointIndex++;
    }

    // Balloon at Finish
    if (nextWayPointIndex == lastWayPointIndex && Vector3.Distance(transform.position, lastWayPoint) < 0.5f)
    {
        speed = 0;
    }
}
}

