using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;


public class Gamecontroller : MonoBehaviour
{

    public GameObject Monkey;
    public GameObject Dice;
    public NavMeshAgent MonkeyAgent;
    public DeterministicDice DeterministicDice;
    public Button Button;
    public TextMeshProUGUI Text;
    public List<GameObject> ChessBoard;
    public int currentPosition;
    public GameObject currentChessBoard;
    public int maxPosition;
    public int newPosition;
    public Vector3 Force;
    public Vector3 Torque;
    public int DiceRoll;
    public bool DiceRolled;
    public int stepsleft;
    public Camera Camera;
    public LayerMask LayerMask;



    // This function executes when the Button is clicked.
    void ButtonClick()
    {
        // Add Random Torque and Force to the Dice.
        Force = new Vector3(Random.Range(-100,100) , Random.Range(100, 200), Random.Range(-100, 100));
        Torque = new Vector3(Random.Range(100, 1000), Random.Range(100, 1000), Random.Range(100, 1000));
        Dice.GetComponent<Rigidbody>().AddForce(Force * Random.Range(5 , 10));
        Dice.GetComponent<Rigidbody>().AddTorque(Torque * Random.Range(5, 100));
        Dice.GetComponent<Rigidbody>().WakeUp();

        DeterministicDice.diceSleeping = false;

        DiceRolled = true;
    }

    //This function handles the position calculation when we loop the lap, so that we can continue walking.
    public void CalculatePosition(int currentPostion)
    {

        newPosition = currentPosition + 1;
        if (newPosition >= maxPosition)
        {
            currentPosition = newPosition - maxPosition;
        }

        else
        {
            currentPosition = newPosition;
        }
 
    }

    // This function moves the Monkey 1 step at a time.
    public void MoveMonkey1Step()
    {
        CalculatePosition(currentPosition);

        currentChessBoard = ChessBoard[currentPosition];

        MonkeyAgent.SetDestination(currentChessBoard.transform.position);

        stepsleft = stepsleft - 1;

    }



    // Start is called before the first frame update
    void Start()
    {
    
        MonkeyAgent = Monkey.GetComponent<NavMeshAgent>();
        DeterministicDice = Dice.GetComponent<DeterministicDice>();
        Button btn = Button.GetComponent<Button>();
        btn.onClick.AddListener(ButtonClick);
        Camera = Camera.main;


    }



    // Update is called once per frame
    void Update()
    {
        // If the dice has landed, we collect the dice roll. Otherwise we do nothing.
        if (DiceRolled == true && DeterministicDice.diceSleeping == true)

        {
            DiceRoll = DeterministicDice.diceRoll;

            Text.SetText("Dice Rolled: " + DiceRoll);

            DiceRolled = false;

            stepsleft = DiceRoll;
        }

        // If we have steps left to walk(We move 1 step at a time), then we continue walking as soon as we reached our destination.
        if (MonkeyAgent.pathPending == false && MonkeyAgent.hasPath == false && stepsleft > 0)
        {

            MoveMonkey1Step();

        }


        // Dice throwing mechanics
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition;
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~LayerMask))
            {
                worldPosition = hitData.point;
                //Debug.Log(hitData.transform);
                Force = ((Dice.transform.position - worldPosition) * -1) * Random.Range(4, 8);
                Torque = new Vector3(Random.Range(100, 1000), Random.Range(100, 1000), Random.Range(100, 1000));

                Dice.GetComponent<Rigidbody>().AddForce(Force * Random.Range(5, 10));
                Dice.GetComponent<Rigidbody>().AddTorque(Torque * Random.Range(5, 100));
                Dice.GetComponent<Rigidbody>().WakeUp();

                //Debug.DrawLine(Dice.transform.position, worldPosition, Color.red, Mathf.Infinity);

                DeterministicDice.diceSleeping = false;

                DiceRolled = true;
            }
                    


        }

    }
}
