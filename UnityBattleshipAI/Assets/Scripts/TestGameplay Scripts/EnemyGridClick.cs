using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGridClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 100.0f))
        {
            if(hit.transform)
            {
                Debug.Log(ray);
            }
        }
    }
}
