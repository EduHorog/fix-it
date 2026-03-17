using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;  // Для корректной диагональной скорости
    public float runSpeed = 20.0f;     // Настраиваемая скорость

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Чтение ввода (WASD/стрелки)
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Ограничение скорости на диагонали (для клавиатуры)
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        // Применение скорости через velocity
        body.linearVelocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
