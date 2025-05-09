using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationAmmount;
    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random();

        rotationAmmount = new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), Random.Range(-15, 15));
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(rotationAmmount * Time.deltaTime);
    }
}
