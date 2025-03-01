using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject[] wayPoints;
    private int nextWayPointIndex = 0;
    public int health = 1;
    public int speed = 1;

    private Material m_Material;
    private GameManager gameManager;

    void Start()
    {
        wayPoints = GameObject.FindGameObjectsWithTag("Waypoints");
        wayPoints = wayPoints.OrderBy(wp => int.Parse(wp.name)).ToArray();
        m_Material = GetComponent<Renderer>().material;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        MoveBalloon();
        if (health == 3)
        {
            m_Material.color = Color.red;
        }
        else if (health == 2)
        {
            m_Material.color = Color.blue;
        }
        else if (health == 1)
        {
            m_Material.color = Color.green;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Dart") 
        {
            health--;
            if (health == 2)
            {
                m_Material.color = Color.blue;
            }
            else if (health == 1)
            {
                m_Material.color = Color.green;
            }
            else if (health <= 0)
            {
                if (m_Material.color == Color.red)
                {
                    gameManager.UpdateScore(3);
                }
                else if (m_Material.color == Color.blue)
                {
                    gameManager.UpdateScore(2);
                }
                else if (m_Material.color == Color.green)
                {
                    gameManager.UpdateScore(1);
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
            Destroy(this.gameObject);
        }
    }
}

