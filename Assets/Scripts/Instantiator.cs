using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObject manager = new GameObject("Manager");

        sphere.transform.position = new Vector3(3, 2, 0);
        cube.transform.position = new Vector3(-3, 2, 0);
        capsule.transform.position = new Vector3(0, 2, -3);

        sphere.AddComponent<SphereControl>();
        cube.AddComponent<CubeControl>();
        capsule.AddComponent<CapsuleControl>();
        ground.AddComponent<Renderer>();
        manager.AddComponent<TouchManager>();

        ground.name = "Ground";
        rend = ground.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
