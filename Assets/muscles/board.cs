using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class board : MonoBehaviour
{
    public GameObject piece;
    
    private GameObject[,] positions =new GameObject[8,8];
    private GameObject[] black = new GameObject[16];
    private GameObject[] white = new GameObject[16];

    private string currentPlayer = "white";

    private int playerCount = -1;
    public int currentTeam = -1;

    private bool gameOver = false;

    private bool localGame = true;


 //   private void Awake()
  //  {
    //    RegisterEvents();
   // }

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;

        GameUI.instance.SetLocalGame += OnSetLocalGame;
    }

    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;
        GameObject piece = positions[mm.originalX, mm.originalY];
        Debug.Log($"mm: {mm.teamID} {piece.name}: {mm.originalX} {mm.originalY} -> {mm.destinationX} {mm.destinationY}");
        if(mm.teamID != currentTeam)
        {
            GameObject controller = GameObject.FindGameObjectWithTag("GameController");
            
            controller.GetComponent<board>().SetPositionEmpty(mm.originalX, mm.originalY);
            piece.GetComponent<chessman>().SetBoardX(mm.destinationX);
            piece.GetComponent<chessman>().SetBoardY(mm.destinationY);
            piece.GetComponent<chessman>().SetCoords();
            controller.GetComponent<board>().SetPosition(piece);
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeMove mm = msg as NetMakeMove;

        server.Instance.Broadcast(mm);
    }

    private void OnSetLocalGame(bool obj)
    {
        localGame = obj;
    }

    private void OnStartGameClient(NetMessage msg)
    {
        Debug.Log("yo");
        GameUI.instance.ChangeCamera((currentTeam == 0) ? CameraAngle.team_white : CameraAngle.team_black);
    }

    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;

        GameUI.instance.SetLocalGame -= OnSetLocalGame;
    }

    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome nw = msg as NetWelcome;

        nw.AssignedTeam = ++playerCount;

        server.Instance.SendToClient(cnn,nw);

        if(playerCount==1)
        {
            server.Instance.Broadcast(new NetStartGame());
        }
    }

    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;

        currentTeam = nw.AssignedTeam;

        Debug.Log($"My assigned team is {nw.AssignedTeam}");

        if(localGame && currentTeam==0)
        {
            server.Instance.Broadcast(new NetStartGame());
        }
        
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //    Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        RegisterEvents();
        white = new GameObject[]
        {
            Create("white_rook",0,0), Create("white_rook",7,0),
            Create("white_knight",1,0), Create("white_knight",6,0),
            Create("white_bishop",2,0), Create("white_bishop",5,0),
            Create("white_king",4,0), Create("white_queen",3,0),
            Create("white_pawn",0,1), Create("white_pawn",1,1), Create("white_pawn",2,1), Create("white_pawn",3,1),
            Create("white_pawn",4,1), Create("white_pawn",5,1), Create("white_pawn",6,1), Create("white_pawn",7,1),
        };

        black = new GameObject[]
        {
                Create("black_rook",7,7), Create("black_rook",0,7),
                Create("black_knight",1,7), Create("black_knight",6,7),
                Create("black_bishop",2,7), Create("black_bishop",5,7),
                Create("black_king",4,7), Create("black_queen",3,7),
                Create("black_pawn",0,6), Create("black_pawn",1,6), Create("black_pawn",2,6), Create("black_pawn",3,6),
                Create("black_pawn",4,6), Create("black_pawn",5,6), Create("black_pawn",6,6), Create("black_pawn",7,6),
        };

        for(int i=0;i<white.Length;++i)
        {
            SetPosition(white[i]);
            SetPosition(black[i]);
        }
        
    }
    
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        chessman cm = obj.GetComponent<chessman>();
        cm.name= name;
        cm.SetBoardX(x);
        cm.SetBoardY(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        chessman cm = obj.GetComponent<chessman>();
        positions[cm.GetBoardX(), cm.GetBoardY()] = obj;
    }
    
    public void SetPositionEmpty(int x,int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x,int y)
    {
        return positions[x, y];
    }

    public bool PositionIsInBoard(int x,int y)
    {
        return (x >= 0 && y >= 0 && x < positions.GetLength(0) && y < positions.GetLength(0));
    }

    public bool GetCurrentPlayer()
    {
        if (currentPlayer == "white" && currentTeam == 0)
            return true;
        return (currentPlayer == "black" && currentTeam == 1);
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
            currentPlayer = "black";
        else
            currentPlayer = "white";
    }

    public void Update()
    {
        if (IsGameOver() && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;
        GameObject.FindGameObjectWithTag("BravoPlayer").GetComponent<Text>().enabled = true;
    }

}
