using UnityEngine;
using System.Collections.Generic;
using PathFinder;

public class PathController : MonoBehaviour
{
    public GameObject cube;
    public GameObject sphere;
    public GameObject camera1;
    public Rigidbody playerRB;
    public SphereController playerSC;
    public Graph map;
    public float speed;

    private float height;
    private int xo;
    private int yo;
    private int xs;
    private int ys;
    private int n;
    private int m;
    private Vector3 target;
    private Vector3 movement;

	// Use this for initialization
    void Awake()
    {
        height = 0.25f;
        xo = -20;
        yo = -10;
        xs = 1;
        ys = 1;
        n = 40;
        m = 20;
        map = new Graph(xo, yo, xs, ys, n, m);
	}

    void Update()
    {
        int floorMask = LayerMask.GetMask("Floor"); // A layer mask so that a ray can be cast just at game objects on the floor layer.
        float camRayLength = 100f;  // The length of the ray from the camera into the scene.
        RaycastHit floorHit;    // Create a RaycastHit variable to store information about what was hit by the ray.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse cursor on screen in the direction of the camera.

        // Perform the ray cast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                Vector3 position3 = new Vector3(floorHit.point.x, height, floorHit.point.z);
                GameObject playerGO = Instantiate(sphere, position3, Quaternion.identity) as GameObject;
                playerRB  = playerGO.GetComponent<Rigidbody>();
                playerSC = playerGO.GetComponent<SphereController>();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                playerSC.findPath(floorHit.point.x, floorHit.point.z);

            }
        }
    }

    void OnMouseDown()
    {
        int floorMask = LayerMask.GetMask("Floor"); // A layer mask so that a ray can be cast just at game objects on the floor layer.
        float camRayLength = 100f;  // The length of the ray from the camera into the scene.
        RaycastHit floorHit;    // Create a RaycastHit variable to store information about what was hit by the ray.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse cursor on screen in the direction of the camera.

        // Perform the ray cast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 position2 = map.setWallGrid(floorHit.point.x, floorHit.point.z);
                Vector3 position3 = new Vector3(position2.x, height, position2.y);
                Instantiate(cube, position3, Quaternion.identity);
            }
        }
    }
}
