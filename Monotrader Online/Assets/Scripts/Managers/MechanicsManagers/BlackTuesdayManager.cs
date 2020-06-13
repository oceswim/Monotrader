using Photon.Realtime;
using UnityEngine;
using TMPro;

//all players
public class BlackTuesdayManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private float DELTA,increase;
    private Room myRoom;
    private float euros, dollars, pounds, yens;
    public TMP_Text blackTuesdayText;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        SetDelta();
        euros = (float)myRoom.CustomProperties[EUROS_PRICE];
        dollars = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        pounds = (float)myRoom.CustomProperties[POUNDS_PRICE];
        yens = (float)myRoom.CustomProperties[YEN_PRICE];
    }

    // Update is called once per frame
    private void SetDelta()
    {
        increase = Random.Range(.05f, .21f);
        DELTA = 1 + increase;
    }
    private void WorldVariation()
    {
        euros *= DELTA;
        dollars *= DELTA;
        pounds *= DELTA;
        yens *= DELTA;
        SetNewPrices(dollars, euros, pounds, yens);
        blackTuesdayText.text = $"Something horrible happened. Every currencies increased of {increase}%";
    }
    private void SetNewPrices(float d, float e, float p, float y)
    {
        SetRoomProperty(DOLLARS_PRICE, d);
        SetRoomProperty(EUROS_PRICE, e);
        SetRoomProperty(POUNDS_PRICE, p);
        SetRoomProperty(YEN_PRICE, y);

    }
    private void SetRoomProperty(string hashKey, float value)
    {
        if (myRoom.CustomProperties[hashKey] == null)
        {
            myRoom.CustomProperties.Add(hashKey, value);
        }
        else
        {

            myRoom.CustomProperties[hashKey] = value;

        }
        myRoom.SetCustomProperties(myRoom.CustomProperties);

    }
}
