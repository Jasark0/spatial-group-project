using UnityEngine;

public class TurretCannon : Turret
{
    [SerializeField] private float launchAngle = 45f; // The fixed angle in degrees to fire at
    [SerializeField] private float minTargetDistance = 5f; // Minimum distance to consider a target valid

    protected override Balloon FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);

            // Skip enemies that are too close
            if (distance < minTargetDistance) continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest != null && minDistance <= range ? closest.GetComponent<Balloon>() : null;
    }

    protected override void Shoot(Balloon target)
    {
        if (Time.time - timeOfLastAttack > fireRate && bulletPrefab && firePoints.Length > 0)
        {
            foreach (Transform firePoint in firePoints)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                if (bullet.TryGetComponent<Bullet>(out var bulletScript))
                {
                    // Calculate direction to target
                    Vector3 targetPosition = target.transform.position;
                    Vector3 startPosition = firePoint.position;
                    Vector3 direction = targetPosition - startPosition;
                    float heightDifference = targetPosition.y - startPosition.y;
                    float targetDistance = new Vector3(direction.x, 0, direction.z).magnitude;
                    Vector3 horizontalDir = new Vector3(direction.x, 0, direction.z).normalized;

                    float projectileVelocity = CalculateVelocityForFixedAngle(targetDistance, heightDifference, launchAngle);

                    // Apply the launch angle to the direction
                    Vector3 velocityVector = Quaternion.AngleAxis(-launchAngle, Vector3.Cross(Vector3.up, horizontalDir)) * horizontalDir;

                    bulletScript.InitWithVelocity(velocityVector, projectileVelocity, "Turret");
                }
            }
            timeOfLastAttack = Time.time;
        }
    }

    private float CalculateVelocityForFixedAngle(float distance, float heightDifference, float angle)
    {
        // Convert angle from degrees to radians
        float angleRad = angle * Mathf.Deg2Rad;

        // Get gravity magnitude
        float gravity = Mathf.Abs(Physics.gravity.y);

        // v = sqrt((g * d) / sin(2θ))
        float velocity = Mathf.Sqrt(gravity * distance / Mathf.Sin(2 * angleRad));

        return velocity;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw minimum range circle
        Gizmos.DrawWireSphere(transform.position, minTargetDistance);

        DrawAngleIndicator(transform.position, launchAngle, range);
    }

    private void DrawAngleIndicator(Vector3 position, float angle, float length)
    {
        // Draw a series of sample projectile paths at the specified angle
        for (int i = 0; i < 4; i++)
        {
            float direction = i * 90f; // Draw in four cardinal directions
            DrawProjectilePath(position, direction, angle, length);
        }
    }

    private void DrawProjectilePath(Vector3 startPos, float direction, float angle, float maxDistance)
    {
        int steps = 25;
        Vector3[] points = new Vector3[steps];

        // Get direction vector based on angle in degrees
        Vector3 forward = Quaternion.Euler(0, direction, 0) * Vector3.forward;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        // Calculate initial velocity
        float angleRad = angle * Mathf.Deg2Rad;
        float velocity = CalculateVelocityForFixedAngle(maxDistance / 2, 0f, angle); // Half max distance for visualization

        // Initial velocity components
        Vector3 initialVelocity = forward * velocity * Mathf.Cos(angleRad) + Vector3.up * velocity * Mathf.Sin(angleRad);

        // Plot the trajectory
        float timeStep = 0.1f;
        float gravity = Mathf.Abs(Physics.gravity.y);

        for (int i = 0; i < steps; i++)
        {
            float time = timeStep * i;

            // Position using projectile motion formula
            Vector3 pos = startPos +
                initialVelocity * time +
                0.5f * Physics.gravity * time * time;

            points[i] = pos;

            // Draw a point
            if (i > 0)
                Gizmos.DrawLine(points[i - 1], points[i]);

            // Stop if we've hit the ground or gone too far
            if (pos.y < startPos.y || Vector3.Distance(startPos, pos) > maxDistance)
                break;
        }
    }
}

