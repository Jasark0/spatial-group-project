using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This script handles the logic for shooting balloons,
// including projectile instantiation, aiming, and shooting mechanics.
public class BalloonGun : Balloon
{
    public float attackRange = 10f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    private float lastFireTime;
    private Turret currentTarget;
    public float shotPower = 1000f;
    protected override void Start()
    {
        base.Start();
        lastFireTime = Time.time;
    }

    protected override void Update()
    {
        if (!IsShooting())
        {
            base.Update();
        }
        FindAndAttackTurret();
    }

    void FindAndAttackTurret()
    {
        if (currentTarget == null || !IsTargetInRange(currentTarget))
        {
            currentTarget = FindClosestTurret();
        }

        if (currentTarget)
        {
            RotateTowardsTarget(currentTarget);
            Shoot(currentTarget);
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
            if (distance < minDistance && distance <= attackRange)
            {
                minDistance = distance;
                closest = turret;
            }
        }

        return closest ? closest.GetComponent<Turret>() : null;
    }

    bool IsTargetInRange(Turret target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    void RotateTowardsTarget(Turret target)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void Shoot(Turret target)
    {
        if (Time.time - lastFireTime > fireRate && bulletPrefab && firePoints.Length > 0)
        {
            if (animator)
            {
                animator.SetTrigger("Shoot");
            }

            foreach (Transform firePoint in firePoints)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                if (bullet.TryGetComponent<Bullet>(out var bulletScript))
                {
                    Vector3 direction = target.transform.position - firePoint.position;
                    bulletScript.InitWithPower(direction, shotPower, "Enemy");
                }
            }
            lastFireTime = Time.time;
        }
    }

    bool IsShooting()
    {
        return Time.time - lastFireTime < fireRate;
    }
}
