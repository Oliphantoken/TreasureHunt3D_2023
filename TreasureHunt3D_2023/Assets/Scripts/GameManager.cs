using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Clue[] clueList = new Clue[3];
    public string msg = "You have been chosen to participate in an epic, once in a life time Treasure Hunt.\nPress Start to begin.";
    public Text msgUI;
    public int currentClueIndex;
    private const int MAX_CLUES = 3;
    private float stopwatch = 0;
    private Vector2 userLocation;
    private int userScore = 0;
    private WaitForSeconds wait = new WaitForSeconds(1f);
    private bool done = false;


    void Start()
    {
        currentClueIndex = -1;
        //Create clues (any amount you want) and populate their states  
        clueList[0] = new Clue(0, "I remember the smell... a stink that irritates the nose!", 51.2372f, 0.5763f);
        clueList[1] = new Clue(1, "The floor, was it furry?", 0, 0);
        clueList[2] = new Clue(2, "Ah it was furry sandals! And blue walls! That where I found the treasure!", 0, 0);

        //Start game
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        //yield return new WaitForSeconds(1f);
        //show intro message
        GUI.Instance.ShowMessage(msg);
        
        //Wait for some time plus for the GPS info to be ready
        yield return new WaitForSeconds(4f);
        while(!GPSInformation.Instance.IsReady)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Wait");
        }

        //Activate the first clue
        ActivateNewClue();

        //Check distance between you and next clue
        StartCoroutine(CheckDistance());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("!!!!!");
            Debug.Log(GPSInformation.Instance.Location);
            Debug.Log(clueList[currentClueIndex].Location);
        }
    }


    public void ActivateNewClue()
    { 
        //Activate new clue
        currentClueIndex++;

        if(currentClueIndex < MAX_CLUES)
        {
            clueList[currentClueIndex].IsActive = true;
            GUI.Instance.ShowClue(currentClueIndex, clueList[currentClueIndex].Message);
        }    
    }  
 
    public IEnumerator CheckDistance()
    {
        while (!done)
        {
            if (clueList[currentClueIndex].IsActive)
            {
                double dist = GPSInformation.Instance.DistInMetersBetweenCoords(GPSInformation.Instance.Location, clueList[currentClueIndex].Location);
                //double dist = GPSInformation.distance(GPSInformation.Instance.Location.y, clueList[currentClueIndex].Location.y,
                  //                                      GPSInformation.Instance.Location.x, clueList[currentClueIndex].Location.x
                    //                                    );

                Debug.Log("My Loc: " + GPSInformation.Instance.Location.x + ", " + GPSInformation.Instance.Location.y);
                Debug.Log("Clue Loc: " + clueList[currentClueIndex].Location.x + ", " + clueList[currentClueIndex].Location.y);
                Debug.Log("Distance: " + dist + "m");
                if (dist < 20)
                {
                    ResolveClue();
                    yield return new WaitForSeconds(4f);
                    if (!done) ActivateNewClue();
                }
            }

            yield return wait;
        }
    }

    public void ResolveClue()
    {
        //Resolve current clue
        if (currentClueIndex > 0)
        {
            clueList[currentClueIndex].IsResolved = true;
            clueList[currentClueIndex].IsActive = false;
        }

        if (currentClueIndex + 1 < MAX_CLUES)
        {
            GUI.Instance.ShowHeader("Good!");
            GUI.Instance.ShowMessage("You are one step closer to the treasure!\nDon't give up!");
        }
        else
        {
            done = true;
            GUI.Instance.ShowHeader("Congratulations!");
            GUI.Instance.ShowMessage("You have found the treasure!");
        }
    }

}
