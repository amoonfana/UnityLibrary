using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {
    private PathController pc;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Ground").GetComponent<PathController>();
    }

	void OnMouseUpAsButton()
    {
        pc.map.setWalkGrid(transform.position.x, transform.position.z);
        Destroy(gameObject);
    }
}
