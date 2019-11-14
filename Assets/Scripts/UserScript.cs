using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserScript : MonoBehaviour
{
    void Start()
    {
        this.currentML = 0;
        this.transform.position = calculatePPfromML(0);
        this.Money = 400;
        GetStats().UpdateText();
    }

    public int currentML;
    public int playerNumber;
    public int Money;
    public int Votes;
    public int BaseVotes;

    bool isMoving = false;

    Vector2 targetPosition;
    Vector2 velocity;

    float smoothTime = 0.25f;
    float maxSpeed = 10f;
    float smoothDistance = 0.01f;

    void Update()
    {
        if (isMoving)
        {
            if (Vector2.Distance(new Vector2(this.transform.position.x, targetPosition.y), targetPosition) < smoothDistance)
            {
                DoneMoving();
            }
            this.transform.position = Vector2.SmoothDamp(
                this.transform.position,
                targetPosition,
                ref velocity,
                smoothTime,
                maxSpeed,
                Time.deltaTime);
        }
    }

    Vector2 calculatePPfromML(int ML)
    {
        float angle = 2f * Mathf.PI / 24f * (float)ML;
        float radius = 0.44f * (9 - playerNumber);
        Vector2 center = new Vector2(0, 0);
        Vector2 position = new Vector2(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius) + center;
        return position;
    }

    public MapLocationScript getLocationAt(int ML)
    {
        // Get All Locations
        MapLocationScript[] AllLocations = GameObject.FindObjectsOfType<MapLocationScript>();

        // Get Current Location
        MapLocationScript currentLocation = AllLocations[0];

        for (int i = 0; i < 24; i++)
        {
            if (AllLocations[i].MapLocationNo == ML)
            {
                currentLocation = AllLocations[i];
            }
        }
        return currentLocation;
    }


    public void MoveTo(int location, bool abs = false, bool gomoney = true)
    {
        int newPosition = 0;
        if (abs)
        {
            newPosition = location;
        }
        else
        {
            newPosition = (this.currentML + location) % 24;
        }
        if (this.currentML > newPosition && gomoney)
        {
            // Pass Go!
            PassGoCollectMoney();
        }
        this.currentML = newPosition;
        this.targetPosition = calculatePPfromML(this.currentML);
        isMoving = true;
    }

    void DoneMoving()
    {
        // Stop Any Flash Messages
        FlashStop();

        // We are done moving to the new location
        isMoving = false;

        MapLocationScript currentLocation = getLocationAt(this.currentML);
        GetStats().updateInfoBox(currentLocation);

        if (currentLocation.isSpecial)
        {
            switch (currentLocation.type)
            {
                case "CV":
                    {
                        getNewVerdict();
                        break;
                    }
                case "GJ": { GoToJail(); break; }
                case "M": { Media(); break; }

            }
        }
        // TODO: If current place is a special place
        // Execute Special commands here!

        GetStats().UpdateText();

        if (!isMoving)
        {
            StateManager theStateManager = GameObject.FindObjectOfType<StateManager>();
            theStateManager.autoMoveDone(this.currentML);
        }
    }

    void getNewVerdict()
    {
        int verdict = Random.Range(0, 12);

        switch (verdict)
        {
            case 0:
                {
                    FlashStart(" Caught in Land Based Property Scam, go to Jail ");
                    GoToJail();
                    break;
                }

            case 1:
                {
                    FlashStart(" The region needs further repairs, pay 100K ");
                    this.changeMoney(-100);
                    break;
                }

            case 3:
                {
                    FlashStart(" Defamatory article published about you, lose 10K votes ");
                    this.changeVotes(-10);
                    break;
                }

            case 4:
                {
                    FlashStart(" Started a NGO, pay 50K, gain 30K votes  ");
                    this.changeMoney(-50);
                    this.changeVotes(30);
                    break;
                }

            case 5:
                {
                    FlashStart(" Win defamation case against newspaper, receive 50K ");
                    this.changeMoney(50);
                    break;
                }

            case 6:
                {
                    FlashStart(" Natural calamity occurred, pay 100K ");
                    this.changeMoney(-100);
                    break;
                }

            case 7:
                {
                    FlashStart(" Your relative became the high court chief justice, get out of jail free card ");
                    GOOJ++;
                    break;
                }

            case 8:
                {
                    FlashStart(" Receive donations collect 150K ");
                    this.changeMoney(150);
                    break;
                }

            case 9:
                {
                    FlashStart(" Local Activist gains popularity lose 50K votes ");
                    this.changeVotes(50);
                    break;
                }

            case 10:
                {
                    FlashStart(" Conduct special rally pay 200K, gain 70K votes ");
                    this.changeMoney(-200);
                    this.changeVotes(70);
                    break;
                }
            case 11:
                {
                    FlashStart(" Court Order: MANDIR WAHI BANEGA! Fund 100k ");
                    this.changeMoney(-100);
                    this.changeVotes(10);
                    break;
                }
        }
        this.GetStats().UpdateText();
    }

    int GOOJ = 0;

    public PlayerStatScript GetStats()
    {
        StateManager theStateManager = GameObject.FindObjectOfType<StateManager>();
        theStateManager.recalculateVotes();
        return GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>();
    }
    void GoToJail(int JAIL_COUNT = 1)
    {
        if (GOOJ > 0)
        {
            GOOJ--;
            FlashStart(" You used a 'Get out of Jail Free' Card ");
            return;
        }

        jailCount = JAIL_COUNT;
        MoveTo(12, true, false);
    }

    public int jailCount = 0;
    public bool JailCounter()
    {
        if (jailCount == 0)
        {
            return false;
        }
        jailCount--;
        string[] names = { "AAP", "BJP", "Congress" };
        FlashStart("Player " + names[playerNumber] + " is in Jail, skipping turn!");
        return true;
    }

    public bool hasMedia = false;
    void Media()
    {
        FlashStart("You landed on the Media Card, you are immune to financial losses till you reach go");
        hasMedia = true;
        GetStats().UpdateText();
    }

    public void FlashStart(string newText)
    {
        GameObject.Find("FlashText").GetComponent<Text>().text = newText;
    }

    public void FlashStop()
    {
        GameObject.Find("FlashText").GetComponent<Text>().text = "";
    }

    public void PassGoCollectMoney()
    {
        this.hasMedia = false;
        this.Money += 100;
        this.GetStats().UpdateText();
    }

    public void changeMoney(int change)
    {
        if (change < 0 && this.hasMedia)
        {
            return;
        }
        this.Money += change;
        if (this.Money < 0)
        {
            this.Money = 0;
        }
    }

    public void changeVotes(int change)
    {
        this.BaseVotes += change;
    }
}
