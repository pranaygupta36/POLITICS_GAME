using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        updateCurrentPlayer();
        cam1.enabled = true;
        canvas1.enabled = true;
        cam2.enabled = false;
        canvas2.enabled = false;

        createAndPlaceMapLocations();
        updateState(GameState.readyToRoll);
        Dice = GameObject.FindObjectOfType<DiceScript>();
        currentRolled = -1;
    }

    public int TotalRounds = 20;
    public int currentRound = 1;
    public GameObject MapLocationPrefab;
    public int currentPlayerNo;
    public int currentRolled;
    DiceScript Dice;
    public enum GameState { readyToRoll, isDiceRolling, autoMoving, onGoingTurn, waitingForInput };
    public GameState currentState;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public Camera cam1;
    public Camera cam2;
    public Canvas canvas1;
    public Canvas canvas2;
    public int CurrentCamera = 1;

    public void ChangeCamera()
    {
        if (this.currentState == GameState.onGoingTurn)
        {
            CurrentCamera = (CurrentCamera + 1) % 2;
            cam1.enabled = !cam1.enabled;
            canvas1.enabled = !canvas1.enabled;
            cam2.enabled = !cam2.enabled;
            canvas2.enabled = !canvas2.enabled;

            GameObject.FindObjectOfType<OptionsScript>().populateOptions(currentPlayer());
            this.updateState(GameState.waitingForInput);
            return;
        }

        if (this.currentState == GameState.waitingForInput)
        {
            CurrentCamera = (CurrentCamera + 1) % 2;
            cam1.enabled = !cam1.enabled;
            canvas1.enabled = !canvas1.enabled;
            cam2.enabled = !cam2.enabled;
            canvas2.enabled = !canvas2.enabled;

            this.updateState(GameState.onGoingTurn);
            return;
        }

    }
    void createAndPlaceMapLocations()
    {
        string[] TypeArray = new string[24];
        string[] CNameArray = new string[] { "GO", "Seemapuri", "Mustafabad", "Dilli High Court", "Shahdara", "Tilak Nagar", "Media", "Dwarka", "Rohini", "Election Commition", "Janakpuri", "VIkaspuri", "Tihar Jail", "Gandhi Nagar", "Trilokpuri", "Dilli High Court", "Chandni Chowk", "Karol Bagh", "Go To Jail", "Rajouri Garden", "New Delhi", "Election Commition", "Kalkaji", "Greater Kailash" };
        string[] DemandsArray = new string[] { "100", "110", "140", "130", "150", "190", "160", "170", "150", "180", "200", "210", "150", "230", "240", "250" };
        string[] PopulationArray = new string[] { "50", "55", "70", "65", "75", "95", "80", "85", "75", "90", "100", "105", "75", "115", "120", "125" };
        string[] RallyMoneyArray = new string[] { "10", "11", "14", "13", "15", "19", "16", "17", "15", "18", "20", "21", "15", "23", "24", "25" };

        // Create MapLocations
        for (int j = 0; j < 24; j++)
        {
            string x = "C";
            if (j % 3 == 0)
            {
                x = "CV";
            }
            TypeArray[j] = x;
        }

        TypeArray[0] = "G";
        TypeArray[6] = "M";
        TypeArray[12] = "J";
        TypeArray[18] = "GJ";

        int K = 0;
        for (int i = 0; i < 24; i++)
        {
            // Instantiate a new copy of the stone prefab
            GameObject newML = Instantiate(MapLocationPrefab);
            MapLocationScript newMLScript = newML.GetComponent<MapLocationScript>();


            newMLScript.MapLocationNo = i;
            newMLScript.type = TypeArray[i];
            newMLScript.CName = CNameArray[i];

            newMLScript.Status1 = 0;
            newMLScript.Status2 = 0;
            newMLScript.Status3 = 0;

            if (newMLScript.type == "C")
            {
                newMLScript.Demands = int.Parse(DemandsArray[K]);
                newMLScript.Population = int.Parse(PopulationArray[K]);
                newMLScript.RallyMoney = int.Parse(RallyMoneyArray[K]);
                K += 1;
            }

            else
            {
                newMLScript.isSpecial = true;
            }

            if (i == 7)
            {
                newMLScript.Status3 = 50;
            }
            if (i == 13)
            {
                newMLScript.Status1 = 50;
            }
            if (i == 19)
            {
                newMLScript.Status2 = 50;
            }

            if (i == 0)
            {
                GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>().updateInfoBox(newMLScript);
            }

        }
        recalculateVotes();
    }

    void startNewTurn()
    {
        currentPlayer().FlashStop();
        GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>().UpdateText();

        Dice.updateValueImage(0);
        currentPlayerNo = (currentPlayerNo + 1) % 3;
        if (currentPlayerNo == 0)
        {
            currentRound++;
        }

        if (currentRound == TotalRounds)
        {
            EndGame();
        }
        bool skipTurn = currentPlayer().JailCounter();

        if (skipTurn)
        {
            Invoke("startNewTurn", 1f);
        }
        else
        {
            updateState(GameState.readyToRoll);
        }
        updateCurrentPlayer();
		Dice.startRolling();
    }

    UserScript currentPlayer()
    {
        UserScript[] allPlayers = GameObject.FindObjectsOfType<UserScript>();
        return allPlayers[currentPlayerNo]; ;
    }

    public void rollDone(int outcome)
    {
        currentRolled = outcome;
        updateState(GameState.autoMoving);
        currentPlayer().MoveTo(outcome);
    }

    public void autoMoveDone(int location_of_player)
    {
        updateState(GameState.onGoingTurn);
    }

    public void ButtonClick()
    {
        switch (this.currentState)
        {
            case GameState.readyToRoll:
                {
                    Dice.startRolling();
                    updateState(GameState.isDiceRolling);
                    break;
                }
            case GameState.onGoingTurn:
                {
                    this.startNewTurn();
                    break;
                }
        }
    }

    void updateTextBT(string newText)
    {
        GameObject.Find("GameButton").GetComponentInChildren<Text>().text = newText;
    }

    void updateState(GameState newState)
    {
        this.currentState = newState;
        switch (this.currentState)
        {
            case GameState.readyToRoll: { updateTextBT("Ready to Role!"); break; }
            case GameState.isDiceRolling: { updateTextBT("Rolling!"); break; }
            case GameState.autoMoving: { updateTextBT("Moving!"); break; }
            case GameState.onGoingTurn: { updateTextBT("End Turn!"); break; }
        }
    }

    void updateCurrentPlayer()
    {
        string[] names = { "AAP", "BJP", "Congress" };

        string newText = "Current Rounds Remaining: " + (20 - currentRound);
        newText += "\nCurrent Player = " + names[this.currentPlayerNo];

        this.GetComponentInParent<Text>().text = newText;
    }

    public void recalculateVotes()
    {
        int p1 = GameObject.Find("User-0").GetComponent<UserScript>().BaseVotes;
        int p2 = GameObject.Find("User-1").GetComponent<UserScript>().BaseVotes;
        int p3 = GameObject.Find("User-2").GetComponent<UserScript>().BaseVotes;

        MapLocationScript[] AllLocations = GameObject.FindObjectsOfType<MapLocationScript>();
        for (int j = 0; j < 24; j++)
        {
            MapLocationScript L = AllLocations[j];
            if (L.type == "C")
            {
                p1 += L.Status1 * L.Population / 100;
                p2 += L.Status2 * L.Population / 100;
                p3 += L.Status3 * L.Population / 100;
            }
        }
        if (p1 < 0)
        {
            p1 = 0;
            GameObject.Find("User-0").GetComponent<UserScript>().BaseVotes = 0;
        }
        if (p2 < 0)
        {
            p2 = 0;
            GameObject.Find("User-1").GetComponent<UserScript>().BaseVotes = 0;
        }
        if (p3 < 0)
        {
            p3 = 0;
            GameObject.Find("User-2").GetComponent<UserScript>().BaseVotes = 0;
        }
        GameObject.Find("User-0").GetComponent<UserScript>().Votes = p1;
        GameObject.Find("User-1").GetComponent<UserScript>().Votes = p2;
        GameObject.Find("User-2").GetComponent<UserScript>().Votes = p3;
    }
    public void EndGame()
    {
        this.updateState(GameState.onGoingTurn);


        CurrentCamera = (CurrentCamera + 1) % 2;
        cam1.enabled = !cam1.enabled;
        canvas1.enabled = !canvas1.enabled;
        cam2.enabled = !cam2.enabled;
        canvas2.enabled = !canvas2.enabled;


        GameObject.Find("ConstituencyPanel2").SetActive(false);
        GameObject.Find("OptionsPanel").SetActive(false);

        // GameObject.Find("PlayerStatsPanel2").SetActive(false);
        GameObject.Find("DoneButton").SetActive(false);
        GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>().UpdateText();

        UserScript[] players = GameObject.FindObjectsOfType<UserScript>();
        float max = Mathf.Max(players[1].Votes, players[0].Votes, players[2].Votes);

        GameObject.Find("PlayerStatsPanel2").GetComponent<Text>().text = GameObject.Find("PlayerStatsPanel2").GetComponent<Text>().text + "\n\n\n Winner is with Votes: " + max;

        // GameObject.Find("DoneButton").GetComponentInChildren<RectTransform>().transform.position = new Vector2(124, 40);

        // Vector2 center = new Vector2(0, 0);
        // GameObject.Find("DoneButton").transform.position = new Vector2(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius) + center;

        // GameObject.Find("DoneButton").GetComponentInChildren<Text>().text = "Start Again";
        // GameObject.Find("DoneButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));

    }
}

