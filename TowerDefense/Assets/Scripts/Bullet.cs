using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifeTime = 0, lifeTime = 0;
    public Vector3 moveDirection;
    bool shot = false;
    public void Init(Vector3 direction)
    {
        moveDirection = direction.normalized; // Normalize to ensure consistent movement speed
        shot = true;
    }

    void Update()
    {
        if (shot)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, moveDirection); // Align the top (Y-axis) with moveDirection
            transform.Translate(speed * Time.deltaTime * Vector3.up, Space.Self); // Move in the direction of move direction
            lifeTime += Time.deltaTime;
            if (lifeTime >= maxLifeTime)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
