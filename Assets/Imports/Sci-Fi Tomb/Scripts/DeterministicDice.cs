using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterministicDice : MonoBehaviour
{
    // Start is called before the first frame update
    public bool mode2D;
    public bool diceSleeping;
    public bool deterministicRoll;
    public int diceRoll;
    public int diceRollDeterministic;
    public diceTypeList diceType;
    private int diceSides;
    private float topSide;
    [Range(0, 10000)]
    public float edgelandingforce = 1000f;
    public bool edgelandinginverter = false;
    public Rigidbody myRigidbody;

    public enum diceTypeList
    {
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D16 = 16,
        D20 = 20,
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        //Here you can add Torque and Force do a dice manually if you would like.

    }

    // Update is called once per frame
    void Update()
    {


        //Only execute when the dice has stopped rolling
        if (myRigidbody.IsSleeping())

        {

        
            //Only execute until we got a result
            if (!diceSleeping)
            {

                //Reset the variable that contains the topmost side of the dice

                //Check if we are using 2D mode, if not then use the Y axis for calculations
                if (mode2D == false)
                {
                    topSide = -50000;
                }

                //If we are using 2D mode then use the Z axis
                else
                {
                    topSide = 50000;
                }


                //Loop through the sides of the dice and find which one has the highest transform.Y
                for (int index = 0; index < (int)diceType; index++)
                {
                    var getChild = gameObject.transform.GetChild(index);


                    //Check if we are using 2D mode, if not then use the Y axis for calculations
                    if (mode2D == false)
                    {
                        if (getChild.position.y > topSide)
                        {
                            topSide = getChild.position.y;
                            diceRoll = index + 1;

                            //Debug.Log("Roll:" + diceRoll);
                        }
                    }

                    //If we are using 2D mode then use the Z axis. Keep in mind gravity should be set to Z = 9.81 in the project settings.
                    else
                    {
                        if (getChild.position.z < topSide)
                        {
                            topSide = getChild.position.z;
                            diceRoll = index + 1;

                            //Debug.Log("Roll:" + diceRoll);
                        }
                    }
                }
            }
            diceSleeping = true;

        }

        else
        {
            diceSleeping = false;
        }

    }
}
