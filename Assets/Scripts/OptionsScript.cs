using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
    }
    public StateManager theStateManager;
    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonHandler(int Votes, string name, int Cost, int TxCost)
    {
        cl = currentPlayerObj.getLocationAt(currentPlayerObj.currentML);

        // Debug.Log("BUTTON CALLED!");
        currentPlayerObj.Money -= (TxCost + Cost);

        for (int i = 0; i < 3; i++)
        {
            if (GameObject.FindObjectsOfType<UserScript>()[i].playerNumber == 0)
            {
                GameObject.FindObjectsOfType<UserScript>()[i].Money += TxCost * cl.Status1 / 100;
            }
            if (GameObject.FindObjectsOfType<UserScript>()[i].playerNumber == 1)
            {
                GameObject.FindObjectsOfType<UserScript>()[i].Money += TxCost * cl.Status2 / 100;
            }
            if (GameObject.FindObjectsOfType<UserScript>()[i].playerNumber == 2)
            {
                GameObject.FindObjectsOfType<UserScript>()[i].Money += TxCost * cl.Status3 / 100;
            }
        }

        switch (currentPlayerObj.playerNumber + 1)
        {
            case 1: { cl.Status1 += 25; break; }
            case 2: { cl.Status2 += 25; break; }
            case 3: { cl.Status3 += 25; break; }
        }

        switch (name)
        {
            case "AAP": { cl.Status1 -= 25; break; }
            case "BJP": { cl.Status2 -= 25; break; }
            case "INC": { cl.Status3 -= 25; break; }
        }
        Debug.Log(name);

        theStateManager.recalculateVotes();
        currentPlayerObj.GetStats().UpdateText();
        currentPlayerObj.GetStats().updateInfoBox(this.cl);

        GameObject.Find("PayAAPButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("PayBJPButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("PayINCButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("BuyButton").GetComponent<Button>().onClick.RemoveAllListeners();
        this.populateOptions(this.currentPlayerObj);
    }

    public void BackButton()
    {
        theStateManager.ChangeCamera();
        GameObject.Find("PayAAPButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("PayBJPButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("PayINCButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("BuyButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }
    MapLocationScript cl;
    UserScript currentPlayerObj;
    public void populateOptions(UserScript currentPlayer)
    {
        currentPlayerObj = currentPlayer;
        cl = currentPlayer.getLocationAt(currentPlayer.currentML);

        int me = (currentPlayer.playerNumber + 1);

        int Votes = (cl.Population / 4);
        int Cost = (cl.Demands / 4);

        int CalledFor = (cl.Status1 + cl.Status2 + cl.Status3);
        int Occupied = CalledFor;
        int TxCost = 0;

        switch (me)
        {
            case 1: { CalledFor -= cl.Status1; break; }
            case 2: { CalledFor -= cl.Status2; break; }
            case 3: { CalledFor -= cl.Status3; break; }
        }

        if (CalledFor > 0)
        {
            TxCost = cl.RallyMoney;
        }

        string name = "";
        if (cl.Status1 > 0 && me != 1 && (Cost + TxCost <= currentPlayer.Money))
        {
            name = "AAP";
            string toPay = "To buy 25% [" + Votes + "K Votes] from " + name + " you need to pay " + Cost + "K INR with Rally Cost: " + TxCost + "K INR";
            GameObject.Find("PayAAPButton").GetComponent<Button>().interactable = true;
            GameObject.Find("PayAAPButton").GetComponent<Button>().onClick.AddListener(() => ButtonHandler(Votes, name, Cost, TxCost));
            GameObject.Find("PayAAPButton").GetComponentInChildren<Text>().text = toPay;
            // enable button to pay to Aap
        }
        else { GameObject.Find("PayAAPButton").GetComponent<Button>().interactable = false; }

        if (cl.Status2 > 0 && me != 2 && (Cost + TxCost <= currentPlayer.Money))
        {
            name = "BJP";
            string toPay = "To buy 25% [" + Votes + "K Votes] from " + name + " you need to pay " + Cost + "K INR with Rally Cost: " + TxCost + "K INR";
            GameObject.Find("PayBJPButton").GetComponent<Button>().interactable = true;
            GameObject.Find("PayBJPButton").GetComponent<Button>().onClick.AddListener(() => ButtonHandler(Votes, name, Cost, TxCost));
            GameObject.Find("PayBJPButton").GetComponentInChildren<Text>().text = toPay;
            // enable button to pay to BJP
        }
        else { GameObject.Find("PayBJPButton").GetComponent<Button>().interactable = false; }

        if (cl.Status3 > 0 && me != 3 && (Cost + TxCost <= currentPlayer.Money))
        {
            name = "INC";
            string toPay = "To buy 25% [" + Votes + "K Votes] from " + name + " you need to pay " + Cost + "K INR with Rally Cost: " + TxCost + "K INR";
            GameObject.Find("PayINCButton").GetComponent<Button>().interactable = true;

            GameObject.Find("PayINCButton").GetComponent<Button>().onClick.AddListener(() => ButtonHandler(Votes, name, Cost, TxCost));

            GameObject.Find("PayINCButton").GetComponentInChildren<Text>().text = toPay;
            // enable button to pay to INC
        }
        else { GameObject.Find("PayINCButton").GetComponent<Button>().interactable = false; }

        if (CalledFor < 100 && Occupied != 100 && (Cost + TxCost <= currentPlayer.Money))
        {
            name = "Independent";
            string toPay = "To buy 25% [" + Votes + "K Votes] from " + name + " you need to pay " + Cost + "K INR with Rally Cost: " + TxCost + "K INR";
            GameObject.Find("BuyButton").GetComponent<Button>().interactable = true;
            GameObject.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => ButtonHandler(Votes, name, Cost, TxCost));

            GameObject.Find("BuyButton").GetComponentInChildren<Text>().text = toPay;
            // enable button to pay to Independent
        }
        else { GameObject.Find("BuyButton").GetComponent<Button>().interactable = false; }

    }
}
