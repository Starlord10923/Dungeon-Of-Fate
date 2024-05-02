using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionFlickerEffect : MonoBehaviour
{

    public Material EmissionMaterial;
    [Range(0.0f, 100.0f)]
    public float minIntensity = 0.01f;
    [Range(0.0f, 100.0f)]
    public float maxIntensity = 0.01f;
    [Range(0.0f, 100.0f)]
    public float pulsateSpeed = 0.01f;
    [Range(0.0f, 100.0f)]
    public float pulsateMaxDistance = 0.01f;
    public Color EmissionColor;


    // Start is called before the first frame update
    void Start()
    {

        EmissionMaterial = gameObject.transform.GetComponent<Renderer>().material;
        EmissionMaterial.EnableKeyword("_EMISSION");

    }


    

    // Update is called once per frame
    void Update()
    {
           

        float emission = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * pulsateSpeed, pulsateMaxDistance));
        //Color baseColor = ;
        Color finalColor = EmissionColor * Mathf.LinearToGammaSpace(emission);
        EmissionMaterial.SetColor("_EmissionColor", finalColor);

    }
}
