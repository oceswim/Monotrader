using UnityEngine;

public class BankScript : MonoBehaviour
{
    public static BankScript instance = null;
    public enum type
    {
        gold,
        euros,
        dollars,
        pounds,
        yens
    }
    public int amount { get; set; }
    public void UpdateAmount(int val){
        amount += val;
    }



}
