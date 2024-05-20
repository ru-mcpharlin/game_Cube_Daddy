using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashCubesScript : MonoBehaviour
{
    //**********************************************************************************************************//
    #region Variables
    [Space]
    [SerializeField] float DISTANCE_THRESHOLD;
    [SerializeField] ParticleSystem destroyCubeVFX;
    [Space]
    [Header("Components and Scripts")]
    [SerializeField] public PlayerController player;

    [SerializeField] public GameObject[] allLayers;
    [Space]
    [SerializeField] public List<List<GameObject>> listOfLayers = new List<List<GameObject>>();
    [Space]
    [SerializeField] public List<GameObject> layer1;
    [Space]
    [SerializeField] public List<GameObject> layer2;
    [Space]
    [SerializeField] public List<GameObject> layer4;
    [Space]
    [SerializeField] public List<GameObject> layer8;
    [Space]
    [SerializeField] public List<GameObject> layer16;
    [Space]
    [SerializeField] public List<GameObject> layer32;
    [Space]
    [SerializeField] public List<GameObject> layer64;
    [Space]
    [SerializeField] public List<GameObject> layer128;
    [Space]
    [SerializeField] public List<GameObject> layer256;
    [Space]
    [SerializeField] public List<GameObject> layer512;
    [Space]
    [SerializeField] public List<GameObject> layer1024;
    [Space]

    [Space]
    [Space]
    [SerializeField] int[] layerIndexs;


    #endregion



    //**********************************************************************************************************//
    // Start is called before the first frame update
    void Awake()
    {
        //get scripts
        player = FindObjectOfType<PlayerController>();



        allLayers = FindObjectsOfType<GameObject>();

        foreach(GameObject gameObject in allLayers)
        {
            if(gameObject.layer == layerIndexs[0])
            {
                layer1.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[1])
            {
                layer2.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[2])
            {
                layer4.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[3])
            {
                layer8.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[4])
            {
                layer16.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[5])
            {
                layer32.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[6])
            {
                layer64.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[7])
            {
                layer128.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[8])
            {
                layer256.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[9])
            {
                layer512.Add(gameObject);
            }

            else if (gameObject.layer == layerIndexs[10])
            {
                layer1024.Add(gameObject);
            }
        }
        
        listOfLayers.Add(layer1);
        listOfLayers.Add(layer2);
        listOfLayers.Add(layer4);
        listOfLayers.Add(layer8);
        listOfLayers.Add(layer16);
        listOfLayers.Add(layer32);
        listOfLayers.Add(layer64);
        listOfLayers.Add(layer128);
        listOfLayers.Add(layer256);
        listOfLayers.Add(layer512);
        listOfLayers.Add(layer1024);

        allLayers = null;


    }

    public void MakeCubesSquashable(int index)
    {
        foreach(GameObject gameObject in listOfLayers[index])
        {
            if (gameObject != null)
            {
                if (gameObject.CompareTag(player.tag_Environment) || gameObject.CompareTag(player.tag_MagneticEnvironment))
                {
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
            }
        }
    }

    public void CheckCube(Collider collider)
    {
        int currentLayerIndex = layerIndexs[player.cubes_index];
        int colliderLayer = collider.gameObject.layer;
        float distance = Vector3.Distance(player.cubeTransform.position, collider.transform.position);

        if (currentLayerIndex > colliderLayer && 
            colliderLayer >= layerIndexs[0] && 
            colliderLayer <= layerIndexs[layerIndexs.Length-1] &&
            distance <= DISTANCE_THRESHOLD * player.scale)
        {
            Destroy(collider.gameObject);
            Instantiate(destroyCubeVFX, collider.transform.position, Quaternion.identity, null);
        }
    }        
}
