using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercontrol : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal!=0)
        {
            transform.position = new Vector3(transform.position.x+ horizontal*0.1f, transform.position.y, transform.position.z);
        }
        if (vertical != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + vertical * 0.1f);
        }
    }
}
