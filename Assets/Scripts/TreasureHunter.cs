using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum AttachmentRule{KeepRelative,KeepWorld,SnapToTarget};

public class TreasureHunter : MonoBehaviour
{

    OVRCameraRig oVRCameraRig;

OVRManager oVRManager;

OVRHeadsetEmulator oVRHeadsetEmulator;
public Camera viewpointCamera;

    public collectible[] collectiblesInScene;
    public TreasureHunterInventory inventory;

    float currentTotalScore;

    bool treasure1Collected = false;
    float spheresCollected =0;
    bool treasure2Collected = false;
    float cylindersCollected = 0;

    bool treasure3Collected = false;
    float cubesCollected = 0;
    float capsulesCollected = 0;
    
    float totalCollected = 0;

    public TextMesh outputText;
    public TextMesh outputText2;
    public TextMesh outputText3;
    public TextMesh wealthDisplay;
    public TextMesh win;
    public TextMesh score;
    float counter = 0;
    string treasureKind;
    int objectCount = 0;
    Vector3 previousPointerPos;

    public GameObject rightPointerObject;
    public GameObject leftPointerObject;
    public LayerMask collectiblesMask;
    //public GameObject CenterEyeAnchor;

    collectible thingIGrabbed;
    GameObject thingOnGun;

    public GameObject upperPlane;
          bool loseGame = false;

    public TextMesh skimbleLyrics;

    

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,0.01f,collectiblesMask);
        if(Input.GetKeyDown("space")) {

            RaycastHit point;
            Debug.Log("Space worked");

            if(Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.forward, out point, 100.0f)){

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

        //float thisScore = calculateScore();

        if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)){
            Debug.Log("Inventory");

            //int amount = inventory.collectibleAmount.Sum(amount)

            score.text = "LJ Enloe\n Partner: Bea Manaligod\n Number of objects: " + objectCount + "\nWealth: " + 10;

        }
        
        else if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){
            outputText3.text="Grip";
            //In Unity, I can't directly get the overlapping actors of a component. I need to query it manually with Physics.OverlapSphere or OnTriggerEnter
            //I overlap with 1 cm radius to try to get only things near hand
            //this will also return collider for the hand mesh if there is one. I disabled it but keep it in mind. You need to make sure hand is on a different layer
            //collectiblesMask is defined at the top right of the Inspector where it says Layer. The layer controls which things to hit (there is no "class filter" like in UE4)
            Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,0.01f,collectiblesMask);
            if (overlappingThings.Length>0){
                attachGameObjectToAChildGameObject(overlappingThings[0].gameObject,rightPointerObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed=overlappingThings[0].gameObject.GetComponent<collectible>();
            }
        }else if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) || OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.B) || OVRInput.GetUp(OVRInput.RawButton.RThumbstick)){
            letGo();

        //since you can't merge paths the way I did in BP, I need to create a function that does the force grab thing or else I would duplicate code
        //equivalent to ShootAndGrabNoSnap in UE4 version (A)
        }else if (OVRInput.GetDown(OVRInput.RawButton.A)){
            outputText3.text="Force Grab at Distance";
            forceGrab(true);

        //equivalent to ShootAndGrabSnap in UE4 version (B)
        }else if (OVRInput.GetDown(OVRInput.RawButton.B)){
            outputText3.text="Force Grab Snap";
            forceGrab(false);

        //equivalent to MagneticGrip in UE4 version (RS/R3)
        }else if (OVRInput.GetDown(OVRInput.RawButton.RThumbstick)){
            outputText3.text="Magnetic Grip";
            Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,1,collectiblesMask);
            if (overlappingThings.Length>0){
                collectible nearestCollectible=getClosestHitObject(overlappingThings);
                attachGameObjectToAChildGameObject(nearestCollectible.gameObject,rightPointerObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed=nearestCollectible.gameObject.GetComponent<collectible>();
            }
        }

        else if(thingIGrabbed!=null) {
          //  if(thingIGrabbed.transform.position.y<=-.55) {
            if (rightPointerObject.gameObject.transform.position.x < viewpointCamera.transform.position.x + .2 && 
            rightPointerObject.gameObject.transform.position.x > viewpointCamera.transform.position.x - .2 &&
            rightPointerObject.gameObject.transform.position.y < viewpointCamera.transform.position.y - .3 &&
            rightPointerObject.gameObject.transform.position.y > viewpointCamera.transform.position.y - .6 &&
            rightPointerObject.gameObject.transform.position.z < viewpointCamera.transform.position.z + .2 &&
            rightPointerObject.gameObject.transform.position.z > viewpointCamera.transform.position.z - .2
            ) {
            if(thingIGrabbed.treasureName=="Sphere") {
                spheresCollected++;
            }
            else if(thingIGrabbed.treasureName=="Cylinder") {
                cylindersCollected++;
            }
            else if(thingIGrabbed.treasureName=="Capsule") {
                capsulesCollected++;
                Destroy(upperPlane.gameObject);
                loseGame=true;
                
            }
            else {
                cubesCollected++;
            }
            Destroy(thingIGrabbed.gameObject);
            thingIGrabbed=null;
            totalCollected++;
            wealthDisplay.text = "LJ Enloe\n Bea Manaligod" + totalCollected + " Total\n" + spheresCollected + " Spheres ($1)\n" + cylindersCollected + " Cylinders ($10)\n" + cubesCollected + " Cubes ($5)\n" + capsulesCollected + " Capsules ($100)\n" + "Wealth: $" + (spheresCollected + 10*cylindersCollected + 5*cubesCollected + 100*capsulesCollected);
            if(loseGame==true) {
                wealthDisplay.text="YOU LOSE";
                // skimbleLyrics.text="Skimbleshanks, the Railway Cat \nThe Cat of the Railway Train \nThere's a whisper down the line \nAt eleven thirty-nine\nWhen the Night Mails ready to depart\nSaying, Skimble, where is Skimble?\nHas he gone to hunt the thimble?\nWe must find him or the train cant start!\nAll the guards and all the porters\nAnd the stationmaster's daughters\nWould be searching high and low\nSaying Skimble, where is Skimble?\nFor unless he's very nimble\nThen the Night Mail just can't go.\nAt eleven forty-two\nWith the signal overdue\nAnd the passengers all frantic to a man\nThat's when I would appear\nAnd I'd saunter to the rear\nI'd been busy in the luggage van!\nThen he gives one flash\nOf his glass-green eyes\nAnd the signal goes All Clear!\nAnd we're off at last\nFor the northern part\nOf the Northern Hemisphere!\nSkimbleshanks, the Railway Cat\nSkimbleshanks, the Railway Cat\nSkimbleshanks\nSkimbleshanks\nSkimbleshanks, the Railway Cat\nThe Cat of the Railway Train\nYou could say that by and large\nIt was me who was in charge\nOf the Sleeping Car Express\nFrom the driver and the guards\nTo the bagmen playing cards\nI would supervise them all, more or less\nI will watch without winking\nAnd I'll see what you are thinking\nAnd it's certain that I wouldn't approve\nOf hilarity and riot\nSo the folk are very quiet\nWhen Skimble is about and on the move\nYou can play no pranks with Skimbleshanks\nI'm a Cat that cannot be ignored!\nSo nothing goes wrong\nOn the Northern Mail\nWhen Skimbleshanks is aboard\nOh, it's very pleasant\nWhen you've found your little den\nWith your name written up on the door (Woo! Woo!)\nAnd the berth is very neat\nWith a newly folded sheet\nAnd there's not a speck of dust on the floor\nThen the guard looks in politely\nAnd will ask you very brightly\nDo you like your morning tea?\n(Weak or strong)\nBut I was just behind him\nAnd was ready to remind him\nFor Skimble won't let anything go wrong\nWhen you creep into your cozy berths and pull up the counterpane\nYou ought to reflect that it's very nice\nTo know that you won't be bothered by mice\nYou can leave all that to the Railway Cat\nThe Cat of the Railway Train\nSkimbleshanks, the Railway Cat (Skimbleshanks)\nThe Cat of the Railway Train\nAnd he gives you a wave\nOf his long brown tail\nWhich says, I'll see you again!\nYou will meet without fail\nOn the Midnight Mail\nThe Cat of the Railway Train!";
            }
        }
        }
        if(capsulesCollected+cylindersCollected+cubesCollected+spheresCollected>=5) {
            wealthDisplay.text = "YOU WON!";
        }
        previousPointerPos=rightPointerObject.gameObject.transform.position;
    }

    collectible getClosestHitObject(Collider[] hits){
        float closestDistance=10000.0f;
        collectible closestObjectSoFar=null;
        foreach (Collider hit in hits){
            collectible c=hit.gameObject.GetComponent<collectible>();
            if (c){
                float distanceBetweenHandAndObject=(c.gameObject.transform.position-rightPointerObject.gameObject.transform.position).magnitude;
                if (distanceBetweenHandAndObject<closestDistance){
                    closestDistance=distanceBetweenHandAndObject;
                    closestObjectSoFar=c;
                }
            }
        }
        return closestObjectSoFar;
    }
    //could have more easily just passed in attachment rule.... but I wanted to keep the code similar to the BP example
    void forceGrab(bool pressedA){
        RaycastHit outHit;
        //notice I'm using the layer mask again
        if (Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.up, out outHit, 100.0f,collectiblesMask))
        {
            AttachmentRule howToAttach=pressedA?AttachmentRule.KeepWorld:AttachmentRule.SnapToTarget;
            attachGameObjectToAChildGameObject(outHit.collider.gameObject,rightPointerObject.gameObject,howToAttach,howToAttach,AttachmentRule.KeepWorld,true);
            thingIGrabbed=outHit.collider.gameObject.GetComponent<collectible>();
        }
    }

    void letGo(){
        if (thingIGrabbed){
            Collider[] overlappingThingsWithLeftHand=Physics.OverlapSphere(leftPointerObject.transform.position,0.01f,collectiblesMask);
            if (overlappingThingsWithLeftHand.Length>0){
                if (thingOnGun){
                    detachGameObject(thingOnGun,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                    simulatePhysics(thingOnGun,Vector3.zero,true);
                }
                attachGameObjectToAChildGameObject(overlappingThingsWithLeftHand[0].gameObject,leftPointerObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
                thingOnGun=overlappingThingsWithLeftHand[0].gameObject;
                thingIGrabbed=null;
            }else{
                detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                simulatePhysics(thingIGrabbed.gameObject,(rightPointerObject.gameObject.transform.position-previousPointerPos)/Time.deltaTime,true);
                thingIGrabbed=null;
            }
        }
    }
    
    //since Unity doesn't have sceneComponents like UE4, we can only attach GOs to other GOs which are children of another GO
    //e.g. attach collectible to controller GO, which is a child of VRRoot GO
    //imagine if scenecomponents in UE4 were all split into distinct GOs in Unity
    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld){
        GOToAttach.transform.parent=newParent.transform;
        handleAttachmentRules(GOToAttach,locationRule,rotationRule,scaleRule);
        if (weld){
            simulatePhysics(GOToAttach,Vector3.zero,false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        GOToHandle.transform.localPosition=
        (locationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.position:
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition:
        new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles=
        (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles:
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
        new Vector3(0,0,0);

        GOToHandle.transform.localScale=
        (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale:
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
        new Vector3(1,1,1);
    }

    public void simulatePhysics(GameObject target,Vector3 oldParentVelocity,bool simulate){
        Rigidbody rb=target.GetComponent<Rigidbody>();
        if (rb){
            if (!simulate){
                Destroy(rb);
            } 
        } else{
            if (simulate){
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB=target.AddComponent<Rigidbody>();
                newRB.velocity=oldParentVelocity;
            }
        }
    }
}
