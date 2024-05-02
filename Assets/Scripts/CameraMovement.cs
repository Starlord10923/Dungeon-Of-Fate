using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public float distance;
    public float height;
    public float rotationSpeed = 6f;
    private Vector3 offset;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        offset = new Vector3(0,4f,-4f);
        distance = offset.magnitude;
        height = offset.y;
    }

    void LateUpdate()
    {
        Quaternion playerRotation = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 desiredPosition = player.position + playerRotation * offset;
        desiredPosition.y = player.position.y + height;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, rotationSpeed * Time.deltaTime);

        transform.LookAt(player.position+player.forward*distance+player.up*height/2);
    }

}
