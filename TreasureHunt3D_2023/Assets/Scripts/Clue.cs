using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue
{
    private int index;
    private string message;
    private bool isActive = false;
    private bool isResolved = false;
    private Vector2 location = new Vector2();

    public Clue(int _index, string _msg, float longitude, float latitude)
    {
        index = _index;
        message = _msg;
        location.x = longitude;
        location.y = latitude;
    }


    public bool IsActive {
        get { return isActive; }
        set { isActive = value; }
    }

    public bool IsResolved {
        get { return isResolved; }
        set { isResolved = value; }
    }

    public Vector2 Location
    {
        get { return location; }
    }

    /**
     * @Description Message of the Clue
     */
    public string Message
    {
        get { return message; }
    }

}
