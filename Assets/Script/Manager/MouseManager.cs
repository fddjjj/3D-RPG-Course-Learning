using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseManager : SingleTon<MouseManager>
{


    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;




    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
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
                case "enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto); break;
                case "portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto); break;
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
            if (hitInfo.collider.gameObject.CompareTag("enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}
