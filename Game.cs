using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
    public string gameMode;
    public GameObject X, O;
    public GameObject[] Bar = new GameObject[8];

    public Text Instructions;
    public Text Victory;
    //public Text nameOf2ndPlayer;
    public enum Seed { EMPTY, X, O };

    public Seed Turn;
    //keep track of X,O, and empty cells
    public GameObject[] allCreates = new GameObject[25];
    Seed[] player = new Seed[25];
    Seed[] test = new Seed[25];

    public TranspositionTable table = new TranspositionTable();
    //TranspositionTable.Entry[] 
    private int numOfMoves = 0;

    private void Awake()
    {
        //GameObject persistentObj = GameObject.FindGameObjectWithTag("PersistentObj") as GameObject;
        //gameMode = persistentObj.GetComponent<PersistenceScript>().gameMode;
        //Destroy(persistentObj);
        gameMode = "vscpu";
        //set Turn to 1st player, cross
        Turn = Seed.X;
        //  table.Reset();
        Instructions.text = "1st Player's Turn";
        
        for (int i = 0; i < 25; i++)
        {
            test[i] = Seed.EMPTY;
            player[i] = Seed.EMPTY;
        }
        test[0] = Seed.O;
        test[1] = Seed.O;
        test[5] = Seed.O;
        test[8] = Seed.O;
        test[24] = Seed.X;
        RotatingCCW(test);
    }
 
    public void Create(GameObject emptycell, int id)
    {
        //conditions to create X or O 
        if (Turn == Seed.X)
        {
            allCreates[id] = Instantiate(X, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;
            numOfMoves++;
            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                //change the instructions
                Instructions.text = "1st Player WON!!!";
                Victory.text = "1st Player WON!!!";
                //BarImage();


            }
            else
            {
                Turn = Seed.O;
                Instructions.text = "2nd Player's Turn";

            }
        }
        else if (Turn == Seed.O && gameMode == "2player")
        {
            allCreates[id] = Instantiate(O, emptycell.transform.position, Quaternion.identity);
            player[id] = Turn;
            numOfMoves++;
            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                //change the instructions
                Instructions.text = "2nd Player WON!!!";
                Victory.text = "2nd Player WON!!!";
                //BarImage();
            }
            else
            {
                Turn = Seed.X;
                Instructions.text = "1st Player's Turn";

            }
        }
        
        if (Turn == Seed.O && gameMode == "vscpu")
        {
            int bestScore = -1000, bestPos = -1, score;
            numOfMoves++;
            if (player[12] == Seed.EMPTY)
                bestPos = 12;
            else
            {
                for (int i = 0; i < 25; i++)
                {
                    if (player[i] == Seed.EMPTY)
                    {
                        float ran = Random.Range(0f, 1f);
                        player[i] = Seed.O;
                        numOfMoves++;



                        score = Solve(player, false);
                        player[i] = Seed.EMPTY;
                        numOfMoves--;

                        print("Index: " + i  + " Score: " + score);

                        if (bestScore < score)
                        {
                            bestScore = score;
                            bestPos = i;
                        }
                        if (ran > 0.75f && bestScore == score)
                        {
                            bestScore = score;
                            bestPos = i; 
                        }
                    }
                }
            }
            print("Best Score: " + bestScore);

            if (bestPos > -1)
            {
                Destroy(allCreates[bestPos]);

                allCreates[bestPos] = Instantiate(O, allCreates[bestPos].transform.position, Quaternion.identity);
                player[bestPos] = Turn;
                
            }
        
            if (Won(Turn))
            {
                // change the turn
                Turn = Seed.EMPTY;

                //change the instructions
                Instructions.text = "2nd Player WON!!!";
                Victory.text = "2nd Player WON!!!";
                //BarImage();

            }
            else
            {
                Turn = Seed.X;
                Instructions.text = "1st Player's Turn";
            }
        }
        
        Destroy(emptycell);

        if (IsDraw())
        {
            Instructions.text = "ITS A TIE!!!";
            Victory.text = "ITS A TIE!!!";
        }
    }
    bool IsAnyEmpty()
    {
        for (int i = 0; i < 25; i++)
        {
            if (player[i] == Seed.EMPTY)
            {
                return true;
            }
        }
        return false;
    }

    bool Won(Seed currplayer)
    {
        bool hasWon = false;
        int[,] allConditions = new int[30, 4] { { 0,1,5,6},{ 1,2,6,7},{ 2,3,7,8},{ 3,4,8,9},{ 5,6,10,11},{ 6,7,11,12},{ 7,8,12,13},{ 8,9,13,14},
                                                { 10,11,15,16},{ 11,12,16,17},{ 12,13,17,18},{ 13,14,18,19},{ 15,16,20,21},{ 16,17,21,22},{ 17,18,22,23},{ 18,19,23,24},
                                                { 0,2,10,12},{ 1,3,11,13},{ 2,4,12,14},{ 5,7,15,17},{ 6,8,16,18},{ 7,9,17,19},{ 10,12,20,22},{ 11,13,21,23},{ 12,14,22,24},
                                                { 0,3,15,18},{ 1,4,16,19},{ 5,8,20,23},{ 6,9,21,24},
                                                { 0,4,20,24} };
        //check conditions
        for (int i = 0; i < 30; i++)
        {
            if (player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == currplayer)
            {
                hasWon = true;
                break;
            }
        }
        return hasWon;
    }
    bool AlmostWon(Seed currplayer)
    {
        bool hasAlmostWon = false;
        int[,] allConditions = new int[30, 4] { { 0,1,5,6},{ 1,2,6,7},{ 2,3,7,8},{ 3,4,8,9},{ 5,6,10,11},{ 6,7,11,12},{ 7,8,12,13},{ 8,9,13,14},
                                                { 10,11,15,16},{ 11,12,16,17},{ 12,13,17,18},{ 13,14,18,19},{ 15,16,20,21},{ 16,17,21,22},{ 17,18,22,23},{ 18,19,23,24},
                                                { 0,2,10,12},{ 1,3,11,13},{ 2,4,12,14},{ 5,7,15,17},{ 6,8,16,18},{ 7,9,17,19},{ 10,12,20,22},{ 11,13,21,23},{ 12,14,22,24},
                                                { 0,3,15,18},{ 1,4,16,19},{ 5,8,20,23},{ 6,9,21,24},
                                                { 0,4,20,24} };
        //check conditions
        for (int i = 0; i < 30; i++)
        {
            if ((player[allConditions[i, 0]] == Seed.EMPTY &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == currplayer) ||(player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == Seed.EMPTY &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == currplayer) || (player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == Seed.EMPTY &&
                player[allConditions[i, 3]] == currplayer) || (player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == Seed.EMPTY))
            {
                hasAlmostWon = true;
                break;
            }
        }
        return hasAlmostWon;
    }
    bool HalfWon(Seed currplayer)
    {
        bool hasHalfWon = false;
        int[,] allConditions = new int[30, 4] { { 0,1,5,6},{ 1,2,6,7},{ 2,3,7,8},{ 3,4,8,9},{ 5,6,10,11},{ 6,7,11,12},{ 7,8,12,13},{ 8,9,13,14},
                                                { 10,11,15,16},{ 11,12,16,17},{ 12,13,17,18},{ 13,14,18,19},{ 15,16,20,21},{ 16,17,21,22},{ 17,18,22,23},{ 18,19,23,24},
                                                { 0,2,10,12},{ 1,3,11,13},{ 2,4,12,14},{ 5,7,15,17},{ 6,8,16,18},{ 7,9,17,19},{ 10,12,20,22},{ 11,13,21,23},{ 12,14,22,24},
                                                { 0,3,15,18},{ 1,4,16,19},{ 5,8,20,23},{ 6,9,21,24},
                                                { 0,4,20,24} };
        //check conditions
        for (int i = 0; i < 30; i++)
        {
            if ((player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == Seed.EMPTY &&
                player[allConditions[i, 3]] == Seed.EMPTY) || (player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == Seed.EMPTY &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == Seed.EMPTY) || (player[allConditions[i, 0]] == currplayer &&
                player[allConditions[i, 1]] == Seed.EMPTY &&
                player[allConditions[i, 2]] == Seed.EMPTY &&
                player[allConditions[i, 3]] == currplayer) || 
                (player[allConditions[i, 0]] == Seed.EMPTY &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == Seed.EMPTY)  || 
                (player[allConditions[i, 0]] == Seed.EMPTY &&
                player[allConditions[i, 1]] == currplayer &&
                player[allConditions[i, 2]] == Seed.EMPTY &&
                player[allConditions[i, 3]] == currplayer) || 
                (player[allConditions[i, 0]] == Seed.EMPTY &&
                player[allConditions[i, 1]] == Seed.EMPTY &&
                player[allConditions[i, 2]] == currplayer &&
                player[allConditions[i, 3]] == currplayer))
            {
                hasHalfWon = true;
                break;
            }
        }
        return hasHalfWon;
    }
    void BarImage()
    {
        int[,] allConditions = new int[8, 3] { { 0,1,2}, { 3,4,5}, { 6,7,8},
                                                {0,3,6 },{1,4,7 },{2,5,8 },
                                                    {0,4,8 },{2,4,6 } };
        //check conditions
        for (int i = 0; i < 8; i++)
        {
            if (player[allConditions[i, 0]] == Seed.O &&
                player[allConditions[i, 1]] == Seed.O &&
                player[allConditions[i, 2]] == Seed.O)
            {
                Bar[i].SetActive(true);
                break;
            }
            else if (player[allConditions[i, 0]] == Seed.X &&
                player[allConditions[i, 1]] == Seed.X &&
                player[allConditions[i, 2]] == Seed.X)
            {
                Bar[i].SetActive(true);
                break;
            }
        }
    }
    bool IsDraw()
    {
        bool p1Won, p2Won, anyEmpty, isDraw = false;
        p1Won = Won(Seed.X);
        p2Won = Won(Seed.O);
        anyEmpty = IsAnyEmpty();

        if (!p1Won && !p2Won && !anyEmpty)
        {
            isDraw = true;
        }
        return isDraw;
    }
    
    struct Point
    {
        public int x;
        public int y;
        public Seed value;
    };
    Seed[] RotatingHalf(Seed[] board)
    {
        int[,] coordinates = new int[25, 2]{ { -2,2},{ -1,2},{ 0,2},{ 1,2},{2,2},{-2,1},{ -1,1},{0,1},
                                                {1,1},{ 2,1},{ -2,0},{ -1,0},{0,0},{ 1,0},{ 2,0},{ -2,-1},
                                                { -1,-1},{ 0,-1},{ 1,-1},{ 2,-1},{-2,-2},{ -1,-2},{ 0,-2},{1,-2},{ 2,-2} };
        Point[] boardWCoor = new Point[25];
        Seed[] returnValue = new Seed[25];
        for (int i = 0; i < 25; i++)
        {
            boardWCoor[i].x = coordinates[i, 0];
            boardWCoor[i].y = coordinates[i, 1];
            boardWCoor[i].value = board[i];
            int temp = boardWCoor[i].x;
            boardWCoor[i].x = boardWCoor[i].y;
            boardWCoor[i].y = -temp;
        }
        int returnValIndex = 0;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                for (int k = 0; k < 25; k++)
                {
                    if (boardWCoor[k].x == j && boardWCoor[k].y == i)
                    {
                        returnValue[returnValIndex] = boardWCoor[k].value;
                        returnValIndex++;
                    }

                }

            }
        }

        return returnValue;
    }
    Seed[] RotatingCW(Seed[] board)
    {
        int[,] coordinates = new int[25, 2]{ { -2,2},{ -1,2},{ 0,2},{ 1,2},{2,2},{-2,1},{ -1,1},{0,1},
                                                {1,1},{ 2,1},{ -2,0},{ -1,0},{0,0},{ 1,0},{ 2,0},{ -2,-1},
                                                { -1,-1},{ 0,-1},{ 1,-1},{ 2,-1},{-2,-2},{ -1,-2},{ 0,-2},{1,-2},{ 2,-2} };
        Point[] boardWCoor = new Point[25];
        Seed[] returnValue = new Seed[25];
        for (int i = 0; i < 25; i++)
        {
            boardWCoor[i].x = coordinates[i, 0];
            boardWCoor[i].y = coordinates[i, 1];
            boardWCoor[i].value = board[i];
            int temp = boardWCoor[i].x;
            boardWCoor[i].x = boardWCoor[i].y;
            boardWCoor[i].y = temp;
        }
        int returnValIndex = 0;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                for (int k = 0; k < 25; k++)
                {
                    if (boardWCoor[k].x == j && boardWCoor[k].y == i)
                    {
                        returnValue[returnValIndex] = boardWCoor[k].value;
                        returnValIndex++;
                    }

                }

            }
        }

        return returnValue;
    }
    Seed[] RotatingCCW(Seed[] board)
    {
        int[,] coordinates = new int[25, 2]{ { -2,2},{ -1,2},{ 0,2},{ 1,2},{2,2},{-2,1},{ -1,1},{0,1},
                                                {1,1},{ 2,1},{ -2,0},{ -1,0},{0,0},{ 1,0},{ 2,0},{ -2,-1},
                                                { -1,-1},{ 0,-1},{ 1,-1},{ 2,-1},{-2,-2},{ -1,-2},{ 0,-2},{1,-2},{ 2,-2} };
        Point[] boardWCoor = new Point[25];
        Seed[] returnValue = new Seed[25];
        for (int i = 0; i < 25; i++)
        {
            boardWCoor[i].x = coordinates[i, 0];
            boardWCoor[i].y = coordinates[i, 1];
            boardWCoor[i].value = board[i];
            int temp = boardWCoor[i].x;
            boardWCoor[i].x = -boardWCoor[i].y;
            boardWCoor[i].y = -temp;
        }
        int returnValIndex = 0;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                for (int k = 0; k < 25; k++)
                {
                    if (boardWCoor[k].x == j && boardWCoor[k].y == i)
                    {
                        returnValue[returnValIndex] = boardWCoor[k].value;
                        returnValIndex++;
                    }

                }

            }
        }

        return returnValue;
    }
    int Minimax(Seed currPlayer, Seed[] board, int alpha, int beta,int depth)
    {
        /*
        if (!PossibleNonlosingMoves(currPlayer, board))
            return -((25 - numOfMoves) / 2);
        */
        if (IsDraw())
            return 0;
        
        int min = -(25 - numOfMoves) / 2;
        if (currPlayer == Seed.O)
            min = -(26 - numOfMoves) / 2;

        if (alpha < min)
        {
            alpha = min;
            if (alpha >= beta)
            {
                print("Alpha: " + alpha);
                return alpha;
            }
        }
        
        if (Won(Seed.O))
            return (27 - numOfMoves)/2;
        if (Won(Seed.X))
            return -((26 - numOfMoves)/2);
        if (depth == 0)
            return 0;
        int max = (26 - numOfMoves) / 2;
        if (currPlayer == Seed.O)
            max = (27 - numOfMoves) / 2;
        
        
        int val = table.Get(board, numOfMoves, currPlayer);
        if (val != 1000)
        {
            max = val + -(9 + 1); // -9 is min score
            print("Max: " + max);
        }
        
        if (beta > max)
        {
            beta = max;
            if (alpha >= beta)
            {
                print("Beta: " + beta);
                return beta;
            }
        }
        int score;

        if (currPlayer == Seed.O)
        {
            for (int i = 0; i < 25; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.O;
                    numOfMoves++;
                    
                    int tableScore = table.Get(board, numOfMoves, Seed.X);
                    if (tableScore != 1000)
                    {
                        score = tableScore;
                    }
                    else
                    {
                        score = Minimax(Seed.X, board, alpha, beta, depth - 1);
                        table.Put(board, numOfMoves, score, Seed.X);
                        table.Put(RotatingCW(board), numOfMoves, score, Seed.X);
                        table.Put(RotatingCCW(board), numOfMoves, score, Seed.X);
                        table.Put(RotatingHalf(board), numOfMoves, score, Seed.X);
                    }

                    board[i] = Seed.EMPTY;
                    numOfMoves--;

                    if (score > alpha)
                        alpha = score;
                    if (alpha > beta)
                        break;
                }
            }

            return alpha;
        }
        else
        {
            for (int i = 0; i < 25; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.X;
                    numOfMoves++;

                    int tableScore = table.Get(player, numOfMoves, Seed.O);

                    if (tableScore != 1000)
                    {
                        score = tableScore;
                    }
                    else
                    {
                        score = Minimax(Seed.O, board, alpha, beta, depth - 1);
                        table.Put(board, numOfMoves, score, Seed.O);
                        table.Put(RotatingCW(board), numOfMoves, score, Seed.O);
                        table.Put(RotatingCCW(board), numOfMoves, score, Seed.O);
                        table.Put(RotatingHalf(board), numOfMoves, score, Seed.O); 
                    }
                    board[i] = Seed.EMPTY;
                    numOfMoves--;
                    if (score < beta)
                        beta = score;
                    if (alpha > beta)
                        break;
                }
            }

            return beta;
        }
    }
    int MinimaxCheckForPossibleNonlosingMoves(Seed currPlayer, Seed[] board, int alpha, int beta, int depth)
    {
            if (IsDraw())
                return 0;
            else if (Won(Seed.O))
                return (27 - numOfMoves) / 2;
            else if (Won(Seed.X))
                return -((26 - numOfMoves) / 2);
            else if (depth == 0)
                return 0;
        int max = (24 - numOfMoves) / 2;
        if (currPlayer == Seed.O)
            max = (25 - numOfMoves) / 2;
        int val = table.Get(board, numOfMoves, currPlayer);
        if (val != 1000)
        {
            max = val + -(11 + 1); // -11 is min score
            print("Max: " + max);
        }

        if (beta > max)
        {
            beta = max;
            if (alpha >= beta) return beta;
        }
        int score;

        if (currPlayer == Seed.O)
        {
            for (int i = 0; i < 25; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.O;
                    numOfMoves++;

                    int tableScore = table.Get(board, numOfMoves, Seed.X);
                    if (tableScore != 1000)
                    {
                        score = tableScore;
                    }
                    else
                    {
                        score = Minimax(Seed.X, board, alpha, beta, depth - 1);
                        table.Put(board, numOfMoves, score, Seed.X);
                        table.Put(RotatingCW(board), numOfMoves, score, Seed.X);
                        table.Put(RotatingCCW(board), numOfMoves, score, Seed.X);
                        table.Put(RotatingHalf(board), numOfMoves, score, Seed.X);
                    }

                    board[i] = Seed.EMPTY;
                    numOfMoves--;

                    if (score > alpha)
                        alpha = score;
                    if (alpha > beta)
                        break;
                }
            }

            return alpha;
        }
        else
        {
            for (int i = 0; i < 25; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.X;
                    numOfMoves++;

                    int tableScore = table.Get(player, numOfMoves, Seed.O);

                    if (tableScore != 1000)
                    {
                        score = tableScore;
                    }
                    else
                    {
                        score = Minimax(Seed.O, board, alpha, beta, depth - 1);
                        table.Put(board, numOfMoves, score, Seed.O);
                        table.Put(RotatingCW(board), numOfMoves, score, Seed.O);
                        table.Put(RotatingCCW(board), numOfMoves, score, Seed.O);
                        table.Put(RotatingHalf(board), numOfMoves, score, Seed.O);
                    }
                    board[i] = Seed.EMPTY;
                    numOfMoves--;
                    if (score < beta)
                        beta = score;
                    if (alpha > beta)
                        break;
                }
            }

            return beta;
        }
    }
    int Solve(Seed[] board, bool weak = false)
    {

        int min = -(25 - numOfMoves) / 2;
        int max = ((25 + 1) - numOfMoves) / 2;
        if (weak)
        {
            min = -1;
            max = 1;
        }

        while (min < max)
        {                    // iteratively narrow the min-max exploration window
            int med = min + (max - min) / 2;
            if (med <= 0 && min / 2 < med) med = min / 2;
            else if (med >= 0 && max / 2 > med) med = max / 2;
            int r = Minimax(Seed.X,board, med, med + 1,4);   // use a null depth window to know if the actual score is greater or smaller than med
            if (r <= med) max = r;
            else min = r;
        }
        return min;
    }
    bool PossibleNonlosingMoves(Seed currPlayer, Seed[] board)
    {
        if (MinimaxCheckForPossibleNonlosingMoves(currPlayer, board, -1000, 1000, 1) < 0)
            return false;
        return true;
    }
}

