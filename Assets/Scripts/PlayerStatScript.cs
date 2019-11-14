using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatScript : MonoBehaviour {
	void Start () {
	}
	void Update () {}

	string [] names =  { "AAP", "BJP", "Congress" };

	public void UpdateText(){		

		UserScript[] players = GameObject.FindObjectsOfType<UserScript>();
		string newText = "\n Player Stats \n";
		
		for (int i=0; i<3; i++) {
			UserScript p = players[i];

			newText += "\n" + names[p.playerNumber] + "\n";
			
			if (p.jailCount > 0){
				newText += "JAILED!\n";
				newText += "Skips " + p.jailCount + " turns\n\n";
			}
			if (p.hasMedia) {
				newText += "Media Card!\n Immune to financial losses\n";
				// newText += "Skips " + p.jailCount + " turns\n\n";
			}
			newText += "Money: " + p.Money + "K\n";
			newText += "Votes: " + p.Votes + "K\n";

		}

		GetComponent<Text>().text = newText;
		GameObject.Find("PlayerStatsPanel2").GetComponent<Text>().text = newText;
	}

	MapLocationScript CurrentLocation;

    public MapLocationScript updateInfoBox(MapLocationScript Location)
    {		
		Text InfoBoxText = GameObject.Find("ConstituencyPanel").GetComponentInChildren<Text>();
		

        if (Location.type == "C")
        {
            InfoBoxText.text = string.Format(@"
{0}
Demands: {1}K
Population: {2}K
Rally Money: {3}K
Status:
1 : {4}%
2 : {5}%
3 : {6}% ", Location.CName, Location.Demands, Location.Population, Location.RallyMoney, Location.Status1, Location.Status2, Location.Status3);
        }

        else {
            InfoBoxText.text = string.Format("\n{0} ", Location.CName);
        }

		Text InfoBoxText2 = GameObject.Find("ConstituencyPanel2").GetComponentInChildren<Text>();
		InfoBoxText2.text = InfoBoxText.text;

		MapLocationScript temp = CurrentLocation;
		CurrentLocation = Location;
		return temp;
    }
}
