using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        updateValueImage(0);
    }

    public int DiceValue;
    int spinningCount = 3;
    public bool isRolling = false;

    public Sprite[] DiceImages;


    // Update is called once per frame
    void Update()
    {
        if (this.isRolling)
        {
            if (spinningCount > 0)
            {
                // Spin
                // Rotate with ?
                spinningCount--;
            }
            else
            {
                stopRolling();
            }
        }
    }

    public void updateValueImage(int val = -1)
    {
        if (val == -1) { val = this.DiceValue; }
        GetComponent<Image>().sprite = this.DiceImages[val];
    }
    public void startRolling()
    {
        this.spinningCount = 10;
        DiceValue = 0;
        updateValueImage();
        this.isRolling = true;
    }

    public void stopRolling()
    {
        this.isRolling = false;
        DiceValue = Random.Range(1, 7);
        updateValueImage();
        StateManager theStateManager = GameObject.FindObjectOfType<StateManager>();
        theStateManager.rollDone(this.DiceValue);
    }

}