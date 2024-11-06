using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTypeScript : MonoBehaviour
{
    public int type;

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}
