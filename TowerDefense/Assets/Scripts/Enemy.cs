using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10, turnSpeed = 5;

    public float health = 100f;
    public float attackRange = 5;
    public float damagePerSecond = 1;
    Turret currentTarget = null;

    float timeOfLastAttack;
    void Start()
    {
        timeOfLastAttack = Time.time;
    }
    void Update()
    {
        if (currentTarget)
        {
            MoveTowards(currentTarget.transform.position);
            if (Vector3.Distance(currentTarget.transform.position, transform.position) < attackRange)
                Attack();
        }
        else
            currentTarget = FindClosestTurret();
    }

    void Attack()
    {
        if (Time.time - timeOfLastAttack > 1)
        {
            currentTarget.TakeDamage(damagePerSecond);
            timeOfLastAttack = Time.time;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            AudioManager.Instance.PlayExplosion();
            Destroy(gameObject);
        }
    }

    Turret FindClosestTurret()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject turret in turrets)
        {
            float distance = Vector3.Distance(currentPosition, turret.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = turret;
            }
        }

        return closest != null ? closest.GetComponent<Turret>() : null;
    }

    public bool MoveTowards(Vector3 target)
    {
        target.y = transform.position.y;
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        var dPos = speed * Time.deltaTime;

        var prevPos = transform.position;
        transform.Translate(Vector3.forward * dPos);
        var curPos = transform.position;
        var closestPoint = GetClosestPointToLine(prevPos, (curPos - prevPos).normalized, prevPos - target);

        transform.position = Clamp(closestPoint, prevPos, curPos);

        return Vector3.Distance(transform.position, target) <= dPos;
    }
    Vector3 GetClosestPointToLine(Vector3 origin, Vector3 direction, Vector3 point2origin) =>
        origin - Vector3.Dot(point2origin, direction) * direction;

    Vector3 Clamp(Vector3 point, Vector3 start, Vector3 end)
    {
        var start2end = (end - start).normalized;
        var start2point = (point - start).normalized;
        if (start2point != start2end)
            return start;
        var end2point = (point - start).normalized;
        if (end2point == start2end)
            return end;
        return point;
    }
}