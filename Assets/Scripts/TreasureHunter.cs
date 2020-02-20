using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreasureHunter : MonoBehaviour
{

    OVRCameraRig oVRCameraRig;

OVRManager oVRManager;

OVRHeadsetEmulator oVRHeadsetEmulator;
Camera viewpointCamera;

    public collectible[] collectiblesInScene;
    public TreasureHunterInventory inventory;

    float currentTotalScore;

    bool treasure1Collected = false;
    bool treasure2Collected = false;

    bool treasure3Collected = false;

    public TextMesh win;
    public TextMesh score;
    float counter = 0;
    string treasureKind;
    int objectCount = 0;

    

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("space")) {

            RaycastHit point;
            Debug.Log("Space worked");

            if(Physics.Raycast(transform.position, transform.forward, out point, 100.0f)){

                //Debug.Log("raycast worked");

                treasureKind = point.transform.GetComponent<collectible>().treasureName;
                //Debug.Log(treasureKind);
                collectible physicalObject = Resources.Load(treasureKind, typeof(collectible)) as collectible;
                
                if(!inventory.collectibleAmount.ContainsKey(physicalObject)) {

                    inventory.collectibleAmount.Add(physicalObject, 1);
                    //Debug.Log("added");
                    objectCount++;

                }
                else {
                    inventory.collectibleAmount[physicalObject]+=1;
                    objectCount++;
                }
                Destroy(point.transform.gameObject);
            }




        }

        float thisScore = calculateScore();

        if(Input.GetKeyDown("i")){
            Debug.Log("Inventory");

            //int amount = inventory.collectibleAmount.Sum(amount)

            score.text = "LJ Enloe\n Partner: Bea Manaligod\n Number of objects: " + objectCount + "\nWealth: " + thisScore;

        }

        
    }


    float calculateScore() {

        

        //List<collectible> collectibleTreasures = new List<collectible>();

        //collectibleTreasures=this.gameObject.GetComponent<TreasureHunterInventory>().collectibleAmount.Keys;
        float totalScore=0;
        foreach(collectible treasure in inventory.collectibleAmount.Keys) {
            totalScore+=inventory.collectibleAmount[treasure]*treasure.treasureValue;
        }

        currentTotalScore=totalScore;
        //Debug.Log(totalScore);
        return totalScore;
        

    }
    
}
