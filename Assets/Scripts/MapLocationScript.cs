using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapLocationScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        this.transform.position = calculatePfromML(this.MapLocationNo);

        Sprite mySprite = this.GetComponent<SpriteRenderer>().sprite;

        if (this.type == "C")
        {
            if (this.MapLocationNo == 7)
            {
                mySprite = INCSprite;
            }
            if (this.MapLocationNo == 13)
            {
                mySprite = AAPSprite;
            }
            if (this.MapLocationNo == 19)
            {
                mySprite = BJPSprite;
            }
        }

        if (this.type == "M")
        {
            mySprite = MediaSprite;
        }
        if (this.type == "J")
        {
            mySprite = JailSprite;
        }
        if (this.type == "GJ")
        {
            mySprite = GoJailSprite;
        }
        if (this.type == "G")
        {
            mySprite = GoSprite;
        }
		if (this.type == "CV" && this.MapLocationNo % 6 == 3)
        {
            mySprite = CourtSprite;
        }
		if (this.type == "CV" && this.MapLocationNo % 4 == 1)
        {
            mySprite = ECSprite;
        }
		
        this.GetComponent<SpriteRenderer>().sprite = mySprite;
    }

    // Update is called once per frame
    void Update() { }
    public Sprite CourtSprite;
    public Sprite ECSprite;
    public Sprite JailSprite;
    public Sprite GoJailSprite;
    public Sprite GoSprite;
    public Sprite BJPSprite;
    public Sprite INCSprite;
    public Sprite AAPSprite;
    public Sprite MediaSprite;


    public int MapLocationNo;
    public string type;
    public bool isDisplayingInfo;
    public string CName;
    public int Demands = 0;
    public int Population = 0;
    public int RallyMoney = 0;
    public int Status1 = 0;
    public int Status2 = 0;
    public int Status3 = 0;

    public bool isSpecial = false;


    public Vector2 calculatePfromML(int ML, float radius = 4.4f)
    {
        float angle = 2f * Mathf.PI / 24f * (float)ML;
        Vector2 center = new Vector2(0, 0);
        Vector2 position = new Vector2(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius) + center;
        return position;
    }

    MapLocationScript Old;

    private void OnMouseEnter()
    {
        Old = GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>().updateInfoBox(this);
    }
    private void OnMouseExit()
    {
		GameObject.Find("PlayerStatsPanel").GetComponent<PlayerStatScript>().updateInfoBox(Old);
    }
}