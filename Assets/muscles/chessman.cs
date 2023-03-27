using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;


    private int board_x = -1;
    private int board_y = -1;

    public string playerTurn;

    public Sprite white_pawn, white_rook, white_knight, white_bishop, white_queen, white_king;
    public Sprite black_pawn, black_rook, black_knight, black_bishop, black_queen, black_king;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch(this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; playerTurn = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; playerTurn = "black";  break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; playerTurn = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; playerTurn = "black";  break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; playerTurn = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; playerTurn = "black";  break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; playerTurn = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; playerTurn = "white";  break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; playerTurn = "white";  break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; playerTurn = "white";  break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; playerTurn = "white";  break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; playerTurn = "white";  break;
        }
    }
    
    public void SetCoords()
    {
        float x = board_x;
        float y = board_y;

       
        x -= 3.5f;
        y -= 3.5f;
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetBoardX()
    {
        return board_x; 
    }

    public int GetBoardY()
    {
        return board_y;
    }

    public void SetBoardX(int x)
    {
        board_x = x;
    }

    public void SetBoardY(int y)
    {
        board_y = y;
    }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<board>().IsGameOver() && controller.GetComponent<board>().GetCurrentPlayer())
            {
                DestroyMovePlates();
                InitiateMovePlates();
            }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i=0;i<movePlates.Length;i++)
            Destroy(movePlates[i]);
    }

    public void InitiateMovePlates()
    {
        switch(this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1,0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(-1, -1);
                LineMovePlate(0, -1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                break;

            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;

            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1,1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,-1);
                break;

            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;

            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(0, -1);
                LineMovePlate(-1, 0);
                break;

            case "black_pawn":
                if (board_y == 6)
                    PawnMovePlate(board_x, board_y - 2);
                PawnMovePlate(board_x, board_y - 1);
                break;
            case "white_pawn":
                if (board_y == 1)
                    PawnMovePlate(board_x, board_y + 2);
                PawnMovePlate(board_x, board_y + 1);
                break;
        }   
    }

    public void LineMovePlate(int xIncr,int yIncr)
    {
        board bc = controller.GetComponent<board>();
        int x = board_x + xIncr;
        int y = board_y + yIncr;
        while(bc.PositionIsInBoard(x, y) && bc.GetPosition(x,y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncr;
            y += yIncr;
        }

        if(bc.PositionIsInBoard(x, y) && bc.GetPosition(x, y).GetComponent<chessman>().playerTurn!=playerTurn)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(board_x + 1, board_y + 2);
        PointMovePlate(board_x - 1, board_y + 2);
        PointMovePlate(board_x + 1, board_y - 2);
        PointMovePlate(board_x - 1, board_y - 2);
        PointMovePlate(board_x + 2, board_y + 1);
        PointMovePlate(board_x + 2, board_y - 1);
        PointMovePlate(board_x - 2, board_y + 1);
        PointMovePlate(board_x - 2, board_y - 1);
       
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(board_x, board_y + 1);
        PointMovePlate(board_x, board_y - 1);
        PointMovePlate(board_x + 1, board_y);
        PointMovePlate(board_x - 1, board_y);
        PointMovePlate(board_x + 1, board_y + 1);
        PointMovePlate(board_x - 1, board_y - 1);
        PointMovePlate(board_x - 1, board_y + 1);
        PointMovePlate(board_x + 1, board_y - 1);
    }

    public void PointMovePlate(int x,int y)
    {
        board bc = controller.GetComponent<board>();
        if (bc.PositionIsInBoard(x, y))
        {
            GameObject cp = bc.GetPosition(x, y);
            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if(cp.GetComponent<chessman>().playerTurn!=playerTurn)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x,int y)
    {
        board bc = controller.GetComponent<board>();
        if (bc.PositionIsInBoard(x, y))
        {
            

            if (bc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                
            }
            if (bc.PositionIsInBoard(x + 1, y) && bc.GetPosition(x + 1, y) != null && bc.GetPosition(x + 1, y).GetComponent<chessman>().playerTurn != playerTurn)
                if ((playerTurn == "white" && board_y - y == -1) || (playerTurn == "black" && board_y - y == 1))
                {
                    MovePlateAttackSpawn(x + 1, y);
                    
                }

            if (bc.PositionIsInBoard(x - 1, y) && bc.GetPosition(x - 1, y) != null && bc.GetPosition(x - 1, y).GetComponent<chessman>().playerTurn != playerTurn)
                if ((playerTurn == "white" && board_y - y == -1) || (playerTurn == "black" && board_y - y == 1))
                {
                    MovePlateAttackSpawn(x - 1, y);
                    
                }

        }
    }

    public void MovePlateSpawn(int mat_x, int mat_y)
    {
        float x = mat_x;
        float y = mat_y;
        x -= 3.5f;
        y -= 3.5f;
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        NewBehaviourScript mpScript = mp.GetComponent<NewBehaviourScript>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(mat_x, mat_y);

    }

    public void MovePlateAttackSpawn(int mat_x, int mat_y)
    {
        float x = mat_x;
        float y = mat_y;
        x -= 3.5f;
        y -= 3.5f;
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        NewBehaviourScript mpScript = mp.GetComponent<NewBehaviourScript>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(mat_x, mat_y);

    }

}
