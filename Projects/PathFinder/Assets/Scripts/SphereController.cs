using UnityEngine;
using System.Collections.Generic;

public class SphereController : MonoBehaviour
{
    public Stack<Vector2> path;

    PathController PC;
    Rigidbody sphere;
    Vector3 toPosition;
    float height;
    float threshold;

    void Awake()
    {
        PC = GameObject.FindGameObjectWithTag("Ground").GetComponent<PathController>();
        sphere = GetComponent<Rigidbody>();
        toPosition = transform.position;
        height = transform.localScale.x * 0.5f;
        threshold = transform.localScale.x * 0.1f;
        sphere.freezeRotation = true;
    }

    void FixedUpdate()
    {
        //if (Vector3.Distance(toPosition, sphere.transform.position) < threshold)
        //{
        //    if (path.Count != 0)
        //    {
        //        Vector2 t = path.Pop();
        //        toPosition = new Vector3(t.x, height, t.y);
        //        sphere.velocity = (toPosition - sphere.transform.position) * 10;
        //    }
        //    else
        //    {
        //        sphere.velocity = Vector3.zero;
        //    }
        //}
        if(path != null)
        {
            if (transform.position == toPosition && path.Count != 0)
            {
                Vector2 t = path.Pop();
                toPosition.Set(t.x, height, t.y);
            }

            transform.position = Vector3.MoveTowards(transform.position, toPosition, 10 * Time.deltaTime);
        }
    }

    public void findPath(float x, float y)
    {
        PC.map.setStartGrid(transform.position.x, transform.position.z);
        PC.map.setEndGrid(x, y);
        path = PC.map.getPath();

        if (path.Count != 0)
        {
            Vector2 t = path.Pop();
            toPosition.Set(t.x, height, t.y);
        }
    }
}
