using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private bool fDestroy = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fDestroy)
        {
            Destroy(this.gameObject);
        }
    }

	public void Delete()
	{
		fDestroy = true;
	}
}
