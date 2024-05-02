using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Transform mainCamera;
    public List<Transform> diceFaces;
    public float RotationSpeed = 500f; // Adjust as needed
    public bool isRotating = false;
    public int currentTop = 0;
    public AudioSource audioSource;

    void Start()
    {
        GameManager.Instance.dice = this;

        mainCamera = Camera.main.transform;

        foreach (Transform childTransform in transform)
        {
            if (childTransform.name.StartsWith("Position"))
            {
                diceFaces.Add(childTransform);
            }
        }
        gameObject.SetActive(false);
        audioSource.volume = GameManager.Instance.volume;
    }

    public void RollDice()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateDiceRandomly());
        }
    }

    IEnumerator RotateDiceRandomly()
    {
        isRotating = true;
        int rotationsCount = 30;
        float rotationTime = 0.12f;

        for (int i = 0; i < rotationsCount; i++)
        {
            audioSource.Play();

            Vector3 rotationAxis = Random.onUnitSphere;
            float rotationAmount = RotationSpeed * rotationTime;

            // Rotate the dice around a random axis
            Quaternion targetRotation = Quaternion.AngleAxis(rotationAmount, rotationAxis) * transform.rotation;

            float elapsedTime = 0f;
            while (elapsedTime < rotationTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / rotationTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
                yield return null;
            }
        }

        LogDiceRoll();
        isRotating = false;
    }

    void LogDiceRoll()
    {
        float minDist = Mathf.Infinity;
        int minDistIndex = -1;
        for (int index = 0; index < diceFaces.Count; index++)
        {
            var distance = Vector3.Distance(mainCamera.position, diceFaces[index].position);

            if (distance < minDist)
            {
                minDist = distance;
                minDistIndex = index;
            }
        }
        currentTop = minDistIndex + 1;
    }
}
