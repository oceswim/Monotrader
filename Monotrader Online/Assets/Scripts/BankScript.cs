using UnityEngine;

public class BankScript : MonoBehaviour
{
    public static BankScript instance = null;
    public int amount { get; set; }
    public void UpdateAmount(int val){
        amount += val;
    }



}
