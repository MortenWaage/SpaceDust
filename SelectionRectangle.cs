using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRectangle : MonoBehaviour
{
    Camera cam;
    [SerializeField] RectTransform rectSelectSpriteGreen;
    [SerializeField] RectTransform rectSelectSpriteRed;

    Vector3 dragCheck;
    Vector3 rectStart;
    Vector3 rectEnd;

    bool isDragging = false;
    bool enemyDrag = false;

    int startDragDistance = 3;

    void Start()
    {
        rectSelectSpriteGreen.gameObject.SetActive(false);
        rectSelectSpriteRed.gameObject.SetActive(false);
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetButton("SelectEnemy")) enemyDrag = true;
        else enemyDrag = false;
        

        switch (enemyDrag)
        {
            case false:

                GreenSelectionRectangleLoop();

                break;

            case true:

                RedSelectionRectangleLoop();

                break;

        }      
    }


    void GreenSelectionRectangleLoop()
    {
        switch (isDragging)
        {
            case true:

                if (!Input.GetMouseButton(0)) ChangeGreenState(false);


                DrawGreenRectangle();


                if (Input.GetMouseButtonUp(0))
                {
                    SetSelectionScreenRect();
                    ChangeGreenState(false);
                    ChangeRedState(false);
                    enemyDrag = false;
                }

                break;

            case false:

                if (Input.GetMouseButtonDown(0))
                {
                    dragCheck = Input.mousePosition;
                    if (dragCheck == Vector3.zero) break;

                    ChangeGreenState(true);
                    SetRectStartPosition();
                }

                break;
        }
    }
    void RedSelectionRectangleLoop()
    {
        switch (isDragging)
        {
            case true:

                if (!Input.GetMouseButton(0)) ChangeRedState(false);

                                
                DrawRedRectangle();


                if (Input.GetMouseButtonUp(0))
                {
                    SetSelectionScreenRect();                    
                    ChangeGreenState(false);
                    ChangeRedState(false);
                    enemyDrag = false;
                }

                break;

            case false:

                if (Input.GetMouseButtonDown(0))
                {
                    dragCheck = Input.mousePosition;
                    if (dragCheck == Vector3.zero) break;

                    ChangeRedState(true);
                    SetRectStartPosition();
                }

                break;
        }
    }
    void DrawGreenRectangle()
    {
        if (!rectSelectSpriteGreen.gameObject.activeInHierarchy)
            rectSelectSpriteGreen.gameObject.SetActive(true);

        rectEnd = Input.mousePosition;
        Vector3 center = (rectStart + rectEnd) * 0.5f;
        rectSelectSpriteGreen.position = center;

        float sizeX = Mathf.Abs(rectStart.x - rectEnd.x);
        float sizeY = Mathf.Abs(rectStart.y - rectEnd.y);

        rectSelectSpriteGreen.sizeDelta = new Vector2(sizeX, sizeY);
    }
    void DrawRedRectangle()
    {
        if (!rectSelectSpriteRed.gameObject.activeInHierarchy)
            rectSelectSpriteRed.gameObject.SetActive(true);

        rectEnd = Input.mousePosition;
        Vector3 center = (rectStart + rectEnd) * 0.5f;
        rectSelectSpriteRed.position = center;

        float sizeX = Mathf.Abs(rectStart.x - rectEnd.x);
        float sizeY = Mathf.Abs(rectStart.y - rectEnd.y);

        rectSelectSpriteRed.sizeDelta = new Vector2(sizeX, sizeY);
    }
    void SetSelectionScreenRect()
    {

        Vector3 center = (rectStart + rectEnd) * 0.5f;
        float rectHalfWidth = Mathf.Abs(rectStart.x - rectEnd.x) * 0.5f;
        float rectHalfHeight = Mathf.Abs(rectStart.y - rectEnd.y) * 0.5f;

        Rect rect = new Rect(center.x - rectHalfWidth, center.y - rectHalfHeight, (rectHalfWidth * 2), (rectHalfHeight * 2));

        if (!enemyDrag) CheckFriendlyUnitsInSelection(rect);
        else CheckEnemyUnitsInSelection(rect);

    }
    void CheckFriendlyUnitsInSelection(Rect rect)
    {
        var playerShips = MainController.Instance.unitManager.PlayerShips;

        foreach (var unit in playerShips)
        {
            ShipCore ship = unit.GetComponent<ShipCore>();
            Vector3 shipScreenPosition = ship.IsInView();

            if (shipScreenPosition == Vector3.zero || ship.IsDead()) continue;

            if (rect.Contains(new Vector2(shipScreenPosition.x, shipScreenPosition.y)))
                ship.AppendToSelectedList();
   
        }
    }
    void CheckEnemyUnitsInSelection(Rect rect)
    {      
        List<GameObject> listOfEnemies = GetEnemiesInSelection(rect);

        var selectedShips = MainController.Instance.unitController.SelectedShips;
        if (selectedShips == null) return;

        foreach (GameObject ship in selectedShips)
        {
            if (ship == null) continue;

            var core = ship.GetComponent<ShipCore>();
            core.ClearAllTargets();
            core.NavClearDestinationList();

            core.SetNewTargetList(listOfEnemies);

            core.NavSetAutomaticControl();
            core.NavSetCombatStateAttack();
            core.SetCombatStatePursue();
            
        }
    }
    List<GameObject> GetEnemiesInSelection(Rect rect)
    {
        var enemyShips = MainController.Instance.unitManager.EnemyShips;
        List<GameObject> listOfTargets = new List<GameObject>();

        foreach (var unit in enemyShips)
        {
            ShipCore ship = unit.GetComponent<ShipCore>();
            Vector3 shipScreenPosition = ship.IsInView();

            if (shipScreenPosition == Vector3.zero || ship.IsDead()) continue;

            if (rect.Contains(new Vector2(shipScreenPosition.x, shipScreenPosition.y)))
            {
                listOfTargets.Add(unit);
            }
        }

        return listOfTargets;
    }

    void SetRectStartPosition()
    {
        if (Vector3.Distance(dragCheck, Input.mousePosition) > startDragDistance)
        {
            rectStart = Input.mousePosition;
            rectEnd = rectStart;
        }
    }
    void ChangeGreenState(bool newState)
    {
        isDragging = newState;
        rectSelectSpriteGreen.gameObject.SetActive(newState);
        dragCheck = Vector3.zero;
    }
    void ChangeRedState(bool newState)
    {
        isDragging = newState;
        rectSelectSpriteRed.gameObject.SetActive(newState);
        dragCheck = Vector3.zero;
    }
}
