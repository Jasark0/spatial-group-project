using UnityEngine;

public class TurretCannon : Turret
{
    protected override Balloon FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest != null && minDistance <= range ? closest.GetComponent<Balloon>() : null;
    }
}

