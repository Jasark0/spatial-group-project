using UnityEngine;

public class SpaceshipMover : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private float lifeTime;

    public void Init(Vector3 direction, float speed, float duration)
    {
        this.moveDirection = direction;
        this.speed = speed;
        this.lifeTime = duration;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion offset = Quaternion.Euler(-90f, 0f, 0f);
            transform.rotation = lookRotation * offset;
        }

        SetupTrail();

        Destroy(gameObject, lifeTime);
    }

    private void SetupTrail()
{
    TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();

    trail.time = 1.5f;
    trail.startWidth = 0.2f;
    trail.endWidth = 0f;

    trail.material = new Material(Shader.Find("Sprites/Default"));
    trail.startColor = Color.white;
    trail.endColor = new Color(1, 1, 1, 0);
}

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
