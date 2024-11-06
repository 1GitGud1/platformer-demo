using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotScript : MonoBehaviour
{
    public GameObject equipMarker;

    public void RemoveItem(){
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }

    public void PlaceEquipMarker(){
        Instantiate(equipMarker, transform, false);
    }

    public void RemoveEquipMarker(){
        GameObject.Destroy(GameObject.Find("equipMarker(Clone)"));
    }
}
