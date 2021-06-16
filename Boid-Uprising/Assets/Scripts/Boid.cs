using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    private Rigidbody rb;

    // The minimum distance that another object should be from the boid
    public static float minDistance;

    private Vector3 velocityVector;

    public float velocityX, velocityY, velocityZ;
    public float directionX, directionY, directionZ;

    public List<GameObject> BoidNeighbours = new List<GameObject>();

    private GameObject closestBoid;

    private Camera camera;

    private bool isWrappingX = true;
    private bool isWrappingY = true;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        velocityVector = new Vector3(velocityX, velocityY, velocityZ);

        rb = GetComponent<Rigidbody>();

        GenNewVelocity(GenRandomDirection());

        //Vector3 testA = new Vector3(2, 3, 4);
        //Vector3 testB = new Vector3(1, 2, 3);

        //Debug.Log(Vector3.Angle(testA, testB));

        //Debug.Log(AngleBetweenVectors(testA, testB));
    }

    private void GenNewVelocity(Vector3 desiredDirection)
    {
        Vector3 newVelocity = desiredDirection.normalized * velocityVector.magnitude;

        rb.velocity = newVelocity;

        // Boid looks in the direction of travel
        transform.rotation = Quaternion.LookRotation(desiredDirection);
    }

    private Vector3 GenRandomDirection()
    {
        float directionX = Random.Range(-10, 10);
        float directionY = Random.Range(-10, 10);
        float directionZ = Random.Range(0, 0);

        Vector3 newDirection = new Vector3(
            directionX,
            directionY,
            directionZ
            );

        return newDirection;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = new Color(0, 0, 0, .5f);
    //    Gizmos.DrawSphere(transform.position, maxDistance);
    //}

    // Update is called once per frame
    void Update()
    {
        // Wraps the boid around the view port
        // So that it does not stray away from the camera
        WrapObject();

        closestBoid = FindClosestBoid();

        // Checking position vector distance is too close (over reaching the minDistance value)
        if (DistanceBetweenBoids(transform.position, closestBoid.transform.position) < 0)
        {
            CheckTrajectory();
        }
    }

    private void CheckTrajectory()
    {
        // Less than ~45 degrees
        if (AngleBetweenVectors(gameObject.transform.position, closestBoid.transform.position) < 0.785398)
        {
            // Increase trajectory angle
            do
            {
                directionX -= 0.1f;
                directionY += 0.1f;
            } while (AngleBetweenVectors(gameObject.transform.position, closestBoid.transform.position) >= 0.785398);
        }
        // Greater than ~315 degrees
        else if (AngleBetweenVectors(gameObject.transform.position, closestBoid.transform.position) > 5.49779)
        {
            // Decrease trajectory angle
            do
            {
                directionX += 0.1f;
                directionY -= 0.1f;
            } while (AngleBetweenVectors(gameObject.transform.position, closestBoid.transform.position) >= 0.785398);
        }

        GenNewVelocity(new Vector3(directionX, directionY, directionZ));
    }

    private GameObject FindClosestBoid()
    {
        GameObject nearestBoid = BoidNeighbours[0];

        for (int i = 0; i < BoidNeighbours.Count; i++)
        {
            if (DistanceBetweenBoids(gameObject.transform.position, nearestBoid.transform.position) 
                > DistanceBetweenBoids(gameObject.transform.position, BoidNeighbours[i].transform.position))
            {
                nearestBoid = BoidNeighbours[i];
            }
        }

        return nearestBoid;
    }

    private float DistanceBetweenBoids(Vector3 posA, Vector3 posB)
    {
        Vector3 netVector = posB - posA;

        float distance = netVector.magnitude - (2 * minDistance);

        return distance;
    }

    // Using dot products to calculate an angle between vectors
    private double AngleBetweenVectors(Vector3 vA, Vector3 vB)
    {
        float angle;

        float dotProduct = CalculateDotProduct(vA.x, vB.x) + CalculateDotProduct(vA.y, vB.y) + CalculateDotProduct(vA.z, vB.z);

        //Debug.Log("CalculateDotProduct(vA.x, vB.x) = " + CalculateDotProduct(vA.x, vB.x));
        //Debug.Log("CalculateDotProduct(vA.y, vB.y) = " + CalculateDotProduct(vA.y, vB.y));
        //Debug.Log("CalculateDotProduct(vA.z, vB.z) = " + CalculateDotProduct(vA.z, vB.z));
        //Debug.Log("(vA.magnitude = " + vA.magnitude);
        //Debug.Log("(vB.magnitude = " + vB.magnitude);
        //Debug.Log("(vA.magnitude * vB.magnitude) = " + (vA.magnitude * vB.magnitude));

        //Debug.Log("dotProduct / (vA.magnitude * vB.magnitude) =" + dotProduct / (vA.magnitude * vB.magnitude));

        angle = Mathf.Acos(dotProduct / (vA.magnitude * vB.magnitude));

        return angle;
    }

    // Calculates the dot product between two vectors on a specific dimension
    // e.g. x1 dot x2, y1 dot y2, etc
    private float CalculateDotProduct(float a, float b)
    {
        return a * b;
    }

    // Credit: https://gamedevelopment.tutsplus.com/articles/create-an-asteroids-like-screen-wrapping-effect-with-unity--gamedev-15055
    private void WrapObject()
    {
        bool isVisible = CheckRenderers();

        //if (!isVisible)
        //{
        //    Debug.Log(isVisible);
        //}

        if (isVisible)
        {
            isWrappingX = false;
            isWrappingY = false;
            return;
        }

        if (isWrappingX && isWrappingY)
        {
            return;
        }

        Vector3 viewportPosition = camera.WorldToViewportPoint(transform.position);
        Vector3 newPosition = transform.position;

        if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
        {
            newPosition.x = -newPosition.x;

            isWrappingX = true;
        }

        if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0))
        {
            newPosition.y = -newPosition.y;

            isWrappingY = true;
        }

        transform.position = newPosition;
    }

    bool CheckRenderers()
    {
        if (GetComponent<Renderer>().isVisible)
        {
            return true;
        }

        // Otherwise, the object is invisible
        return false;
    }
}
