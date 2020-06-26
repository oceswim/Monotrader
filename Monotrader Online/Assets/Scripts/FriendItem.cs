using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
/// <summary>
/// Friend UI item used to represent the friend status as well as message. 
/// It aims at showing how to share health for a friend that plays on a different room than you for example.
/// But of course the message can be anything and a lot more complex.
/// </summary>
public class FriendItem : MonoBehaviour {

	
	[HideInInspector]
	public string FriendId
	{
		set{
			NameLabel.text = value;
		}
		get{
			return NameLabel.text;
		}
	}


	public TMP_Text NameLabel;
	public TMP_Text FortuneLabel;
	public static FriendItem instance = null;
	public const string REDPREF = "myRedVal";
	public const string GREENPREF = "myGreenVal";
	public const string BLUEPREF = "myBlueVal";
	private int r, g, b;
	private Button myButton;
	public void Awake()
	{
		myButton = GetComponent<Button>();
		NameLabel.text = string.Empty;
		FortuneLabel.text = string.Empty;
		SetColor();
		
	}
	private void SetColor()
	{
		r = PlayerPrefs.GetInt(REDPREF);
		g = PlayerPrefs.GetInt(GREENPREF);
		b = PlayerPrefs.GetInt(BLUEPREF);
		Debug.Log(r + " " + g + " " + b);
		int transparancy=0;
		if(r>200 && b>200 && g>200)
		{
			transparancy = 100;
		}
		else
		{
			transparancy = 255;
		}
		Color32 color = new Color32((byte)r, (byte)g, (byte)b, (byte)transparancy);
		var newColorBlock = myButton.colors;
		newColorBlock.disabledColor = color;
		myButton.colors = newColorBlock;
	}
	public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
	{
		string _status;

		switch(status)
		{
		case 1:
			_status = "Invisible";
			break;
		case 2:
			_status = "Online";
			break;
		case 3:
			_status = "Away";
			break;
		case 4:
			_status = "Do not disturb";
			break;
		case 5:
			_status = "Looking For Game/Group";
			break;
		case 6:
			_status = "Playing";
			break;	
		default:
			_status = "Offline";
			break;
		}

		//StatusLabel.text = _status;

		if (gotMessage)
		{
			string _health = string.Empty;
			if (message!=null)
			{
				string[] _messages = message as string[];
				if (_messages!=null && _messages.Length>=2)
				{
					_health = (string)_messages[1] + "%";
				}
				
			}
			
			//Health.text = _health;
		}
	}
	
}
