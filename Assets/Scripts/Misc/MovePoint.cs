using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    public bool mouseOverMovePoint = false;
    public GameObject centerPoint;

    //private void OnMouseDown()
    private void OnMouseUp()
    {
        if (GameManager.instance.isPaused == false)
        {
            if (Input.mousePosition.y > Screen.height * 0.135f) //Solamente se podrá hacer clic en un MovePoint si el mouse sobrepasa el 10% del alto de la pantalla. Esto permitira presionar menus sin presionar un movepoint.
            {
                // Se hizo clic en un Tile.
                GameManager.instance.activePlayer.MoveToPoint(centerPoint.transform.position); //Se usa la instancia singleton de GameManager. Solo se puede usar cuando hay 1 solo GameManager.
                MoveGrid.instance.HideMovePoints();// Despues de hacer clic, se ocultan los move points para no hacer multiples clics.
                PlayerInputMenu.instance.HideMenus();// Se oculta el menu cuando se esta moviendo un jugador.
            }
        }
    }

    private void OnMouseEnter()
    {
        if (GameManager.instance.isPaused == false)
        {
            mouseOverMovePoint = true;
        }
    }

    private void OnMouseExit()
    {
        mouseOverMovePoint = false;

    }

    public bool ReturnMouseOver()
    {
        return mouseOverMovePoint;
    }
}
