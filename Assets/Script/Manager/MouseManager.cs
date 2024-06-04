using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;


    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;




    public event Action<Vector3> OnMouseClicked;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
    }
    private void Update()
    {
        SetCurrentSorTexture();
        MouseControl();
    }


    void SetCurrentSorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hitInfo))
        {
            //todo «–ªª Û±ÍÃ˘Õº
            switch (hitInfo.collider.gameObject.tag)
            {
                case "ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto); break;
            }
        }

    }


    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if(hitInfo.collider.gameObject.CompareTag("ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}
