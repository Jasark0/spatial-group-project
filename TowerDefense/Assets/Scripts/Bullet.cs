using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifeTime = 0, lifeTime = 0;
    public Vector3 moveDirection;
    bool shot = false;
    public string bulletOwner = "";
    public void Init(Vector3 direction, float shotPower, string owner)
    {
        moveDirection = direction.normalized; // Normalize to ensure consistent movement speed
        GetComponent<Rigidbody>().AddForce(direction * shotPower);
        bulletOwner = owner;
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
        if (bulletOwner == "Turret" && other.CompareTag("Turret"))
            return;

        if (bulletOwner == "Enemy" && other.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Enemy") && bulletOwner == "Turret")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Turret") && bulletOwner == "Enemy")
        {
            Turret turret = other.GetComponent<Turret>();
            if (turret != null)
            {
                turret.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
