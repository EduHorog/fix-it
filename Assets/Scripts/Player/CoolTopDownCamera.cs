using UnityEngine;

public class CoolTopDownCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;          // Игрок

    [Header("Dead Zone (по миру)")]
    [SerializeField] private Vector2 deadZoneSize = new Vector2(2f, 1.5f);

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float maxSpeed = 25f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 1.0f;
    [SerializeField] private float lookAheadSmooth = 0.2f;

    private Vector3 camVelocity = Vector3.zero;
    private Vector3 lookAheadOffset = Vector3.zero;
    private Vector3 lookAheadVelocity = Vector3.zero;

    private Vector3 lastTargetPos;

    void Start()
    {
        if (target != null)
        {
            lastTargetPos = target.position;
            transform.position = target.position + new Vector3(0, 0, -10);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;
        Vector3 camPos = transform.position;

        // 1) Dead zone в мировых координатах
        Vector3 delta = targetPos - camPos;

        Vector3 desiredPos = camPos;

        if (Mathf.Abs(delta.x) > deadZoneSize.x / 2f)
        {
            desiredPos.x = targetPos.x - Mathf.Sign(delta.x) * deadZoneSize.x / 2f;
        }

        if (Mathf.Abs(delta.y) > deadZoneSize.y / 2f)
        {
            desiredPos.y = targetPos.y - Mathf.Sign(delta.y) * deadZoneSize.y / 2f;
        }

        desiredPos.z = -10f;

        // 2) Look-ahead по направлению движения игрока
        Vector3 targetDelta = (targetPos - lastTargetPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        Vector2 horizontalVelocity = new Vector2(targetDelta.x, targetDelta.y);

        Vector3 targetLookAhead = Vector3.zero;
        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = horizontalVelocity.normalized;
            targetLookAhead = new Vector3(dir.x, dir.y, 0) * lookAheadDistance;
        }

        lookAheadOffset = Vector3.SmoothDamp(
            lookAheadOffset,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmooth
        );

        Vector3 finalTargetPos = desiredPos + lookAheadOffset;

        // 3) Плавное движение камеры
        transform.position = Vector3.SmoothDamp(
            camPos,
            finalTargetPos,
            ref camVelocity,
            smoothTime,
            maxSpeed
        );

        lastTargetPos = targetPos;
    }
}

