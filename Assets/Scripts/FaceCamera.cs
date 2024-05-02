using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    RectTransform canvasRect;
    RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform of the canvas
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Get the RectTransform of the UI element
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (canvasRect != null && rectTransform != null)
        {
            // Calculate the direction from UI element to camera
            Vector3 directionToCamera = Camera.main.transform.position - rectTransform.position;

            // Calculate the rotation to look at the camera
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

            // Rotate the UI element to face the camera
            rectTransform.rotation = targetRotation * Quaternion.Euler(0, 180f, 0);
        }
    }
}
