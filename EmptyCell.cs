using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyCell : MonoBehaviour
{
    public int id;
    public GameObject Camera;


    private void OnMouseDown()
    {
        Camera.GetComponent<Game>().Create(this.gameObject, id);
    }
}
