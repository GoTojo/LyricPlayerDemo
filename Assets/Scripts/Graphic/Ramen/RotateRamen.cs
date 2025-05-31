using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRamen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.Rotate(new Vector3(0, 300f, 0) * Time.deltaTime);        
    }
}
