using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject FreshCube;
        
    public void SpawnMe()
    {
        Instantiate(FreshCube);        
        
        FindObjectOfType<ManipulateAugmentations>().CubeModel = FreshCube;

        //delete old cubemodel.
    }
}
