using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeFoliage : MonoBehaviour
{
    private int[] rarityIndices;
    public GameObject[] spawnableObjs; // must be the same size as the rarity list in generateFoliage!!!
    public float seperationAmount; // How much space should be between each tile
    public float minNoiseAmount; // how much space at minimum must the columns or rows be offset by
    public float maxNoiseAmount; // how much space at maximum must the columns or rows be offset by
    private int size;
    public bool varyY = false; //whether or not the y value can be random
    public float yMin; //max variation in the negative direction
    public float yMax; //max variation in the positive direction
    public bool spawnAsGrid; //spawn the objects in a grid pattern
    public bool alignWithNormal = false; //should the objects in the grid spawn facing the plane's normal

    //grabs the rarity indices from the generateFoliage script
    private void getRarityIndices()
    {
        generateFoliage generator = this.gameObject.GetComponent<generateFoliage>();
        generator.startGeneration();
        size = generator.pub_size;
        rarityIndices = generator.outputList;
    }

    //given a spawn position and a rarityIndice, place a tile and randomly rotate it.
    private void spawnTile(Vector3 spawnPosition, int rarityIndice)
    {
        float rotation;
        if (spawnAsGrid)
        {
            float[] rotations = { 0f, 90f, 180f, 270f };
            System.Random temp = new System.Random();
            int pick = temp.Next(4);
            rotation = rotations[pick];
        }
        else
        {
            System.Random temp = new System.Random();
            int pick = temp.Next(360);
            rotation = pick;
        }
        planePointCalculator plane = this.gameObject.GetComponent<planePointCalculator>();
        if (plane.isOnPlane(spawnPosition))
        {
            if (varyY)
            {
                float offsetAmount = Random.Range(yMin, yMax);
                spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + offsetAmount, spawnPosition.z);
            }
            GameObject tile = Instantiate(spawnableObjs[rarityIndice], spawnPosition, Quaternion.Euler(0f, rotation, 0f));
            //transform.rotation = Quaternion.LookRotation(directionVector);
            if (alignWithNormal)
            {
                Vector3 directionVector = plane.getNormal();
                tile.transform.rotation = Quaternion.LookRotation(directionVector);
                tile.transform.localEulerAngles = new Vector3(tile.transform.localEulerAngles.x + 90, tile.transform.localEulerAngles.y, tile.transform.transform.localEulerAngles.z);
            }
        }
    }

    //iterates over the size of the patch and spawns foliage
    //TODO: Need to create actual models, start with 10 test models (DONE) first before comitting to making a ton of foliage 
    //TODO: make a visual representation of a four corner plane that can then trim the objects that are out of the boundaries (DONE) Hard :(
    //also make it so that the foliage can follow angle requirements... This will be a pain in the ass :(
    private void spawnGrid(Vector3 topRightCorner, int size, float seperationCof, bool square = true)
    {
        planePointCalculator plane = this.gameObject.GetComponent<planePointCalculator>();
        int count = 0; // count keeps track of which tile is being spawned
        System.Random temp = new System.Random();
        bool offsetColumn = System.Convert.ToBoolean(temp.Next(2)); //randomly pick whether to offset rows or columns
        float offsetAmount = Random.Range(minNoiseAmount, maxNoiseAmount); //using unity random to get a float (Thanks Unity!!)
        Debug.Log(offsetAmount);
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float xSpawn = this.gameObject.transform.position.x + (x * seperationCof);
                float zSpawn = this.gameObject.transform.position.z + (z * seperationCof);
                if (offsetColumn && z%2 == 0)
                {
                    //Debug.Log("This is distorted on the Y axis");
                    xSpawn += offsetAmount;
                }
                else if (x%2 == 0  && !offsetColumn)
                {
                    //Debug.Log("This is distorted on the Z axis");
                    zSpawn += offsetAmount;
                }
                //call to getPlaneCoordFrom2D() goes here using zSpawn and xSpawn as parameters
                Vector3 spawnSpot = plane.getPlaneCoordFrom2D(xSpawn, zSpawn);                
                spawnTile(spawnSpot, rarityIndices[count]);
                count++;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        getRarityIndices();
        Vector3 topRight = this.transform.position;
        spawnGrid(topRight, size, seperationAmount);
    }
}
