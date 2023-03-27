using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject controller;

    GameObject refPiece = null;

    int board_x,board_y;

    public bool attack = false; // just move to empty space

    public void Start()
    {
        if(attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        board bc = controller.GetComponent<board>();

        NetMakeMove mm = new NetMakeMove();
        mm.originalX = refPiece.GetComponent<chessman>().GetBoardX();
        mm.originalY = refPiece.GetComponent<chessman>().GetBoardY();
        mm.destinationX = board_x; 
        mm.destinationY = board_y;
        client.Instance.SendToServer(mm);
        mm.teamID = (refPiece.GetComponent<chessman>().playerTurn=="white") ? 0 : 1;

        if (attack)
        {
            GameObject capturedPiece = controller.GetComponent<board>().GetPosition(board_x, board_y);

            if (capturedPiece.name == "white_king") 
                controller.GetComponent<board>().Winner("black");
            else if 
                (capturedPiece.name == "black_king") controller.GetComponent<board>().Winner("white");

            Destroy(capturedPiece);
        }
        if (refPiece.name == "white_pawn" && board_y == 7)
        {
            Destroy(refPiece);
            bc.Create("white_queen", board_x, board_y);
        }
        else
            if (refPiece.name == "black_pawn" && board_x == 0)
        {
            Destroy(refPiece);
            bc.Create("black_queen", board_x, board_y);
        }
        else
        {
            controller.GetComponent<board>().SetPositionEmpty(refPiece.GetComponent<chessman>().GetBoardX(), refPiece.GetComponent<chessman>().GetBoardY());

            refPiece.GetComponent<chessman>().SetBoardX(board_x);
            refPiece.GetComponent<chessman>().SetBoardY(board_y);
            refPiece.GetComponent<chessman>().SetCoords();

            controller.GetComponent<board>().SetPosition(refPiece);
        }

        refPiece.GetComponent<chessman>().DestroyMovePlates();
        controller.GetComponent<board>().NextTurn();
    }

    public void SetCoords(int x,int y)
    {
        board_x = x;
        board_y = y;
    }

    public void SetReference(GameObject obj)
    {
        refPiece = obj;
    }

    public GameObject GetReference()
    {
        return refPiece;
    }

}
