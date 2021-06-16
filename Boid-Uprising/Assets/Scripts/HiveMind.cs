using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HiveMind : MonoBehaviour
{
    public int NumberOfBoids = 1;
    public GameObject BoidPrefab;


    // + or - the maximum co-ordinates a boid can spawn from origin
    public float XCoordLimit = 50f;
    public float YCoordLimit = 50f;
    public float ZCoordLimit = 0f;

    public float InitialXVelocity = 1f;
    public float InitialYVelocity = 1f;
    public float InitialZVelocity = 0f;

    public float MaxDirectionX = 10f;
    public float MaxDirectionY = 10f;
    public float MaxDirectionZ = 0f;

    public float MinDistanceBetweenBoids = 5;

    private List<GameObject> boidsList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateBoids();
    }

    private void CreateBoids()
    {
        Boid.minDistance = MinDistanceBetweenBoids;

        GameObject newBoid;
        for (int i = 0; i < NumberOfBoids; i++)
        {
            newBoid = Instantiate(BoidPrefab);
            boidsList.Add(newBoid);
            newBoid.transform.position = GenSpawnCoords();
            GenVelocityVector(newBoid);
            GenDirectionVector(newBoid);
            for (int j = 0; j < boidsList.Count; j++)
            {
                if (boidsList[j] != newBoid)
                {
                    boidsList[j].GetComponent<Boid>().BoidNeighbours.Add(newBoid);
                    newBoid.GetComponent<Boid>().BoidNeighbours.Add(boidsList[j]);
                }
            }
        }
    }

    private Vector3 GenSpawnCoords()
    {
        Vector3 spawnCoords = new Vector3(
            Random.Range(-XCoordLimit, XCoordLimit),
            Random.Range(-YCoordLimit, YCoordLimit),
            Random.Range(-ZCoordLimit, ZCoordLimit)
            );

        return spawnCoords;
    }

    private void GenVelocityVector(GameObject newBoid)
    {
        Boid newBoidObject = newBoid.GetComponent<Boid>();
        newBoidObject.velocityX = InitialXVelocity;
        newBoidObject.velocityY = InitialYVelocity;
        newBoidObject.velocityZ = InitialZVelocity;
    }

    private void GenDirectionVector(GameObject newBoid)
    {
        Boid newBoidObject = newBoid.GetComponent<Boid>();
        newBoidObject.directionX = Random.Range(-MaxDirectionX, MaxDirectionX);
        newBoidObject.directionY = Random.Range(-MaxDirectionY, MaxDirectionY);
        newBoidObject.directionZ = Random.Range(-MaxDirectionZ, MaxDirectionZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
