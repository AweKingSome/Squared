using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TranspositionTable 
{
    public Game game;
    static Game.Seed[] zeroBoard = new Game.Seed[25];

    private void Awake()
    {
        for (int j = 0; j < 25; j++)
        {
            zeroBoard[j] = Game.Seed.EMPTY;
        }
    }
     
public struct Entry
    {
        public Game.Seed[] table;
        public int moveNum;
        public int score; 
        public Game.Seed playerTurn;
        public Game.Seed[] nextMove;
    };

     Entry[] T = new Entry[8388593];

     int Index(Game.Seed[] board)  {
        String IndexS = "";
        int indexA = 0;
        String indexTotal = "";
        int index = 0;

        for (int i = 0;i < 25; i++)
        {
            
            if (board[i] == Game.Seed.EMPTY)
                index = 0;
            else if (board[i] == Game.Seed.X)
                index = 1;
            else if (board[i] == Game.Seed.O)
                index = 2;
            IndexS += "" + (index);
            if (i % 2 == 1 )
            {
                int IndexInt = int.Parse(IndexS);
                if (IndexInt == 0)
                    indexA = 0;
                if (IndexInt == 1)
                    indexA = 1;
                if (IndexInt == 2)
                    indexA = 2;
                if (IndexInt == 10)
                    indexA = 3;
                if (IndexInt == 11)
                    indexA = 4;
                if (IndexInt == 12)
                    indexA = 5;
                if (IndexInt == 20)
                    indexA = 6;
                if (IndexInt == 21)
                    indexA = 7;
                if (IndexInt == 22)
                    indexA = 8;
                IndexS = "";
                indexTotal += "" + indexA;

            }
            if (i == 24)
            {
                int IndexInt = int.Parse(IndexS);
                if (IndexInt == 0)
                    indexA = 0;
                if (IndexInt == 1)
                    indexA = 1;
                if (IndexInt == 2)
                    indexA = 2;
                IndexS = "";
                indexTotal += "" + indexA;
            }

        }
        return (int)((Int64.Parse(indexTotal)) % (T.Length));
    }


    /*
     * Empty the Transition Table.
     */
    public void Reset()
    { // fill everything with 0, because 0 value means missing data

        for (int i= 0; i< T.Length; i++)
        {
            T[i].moveNum = -1;
            T[i].playerTurn = Game.Seed.EMPTY;
            T[i].table = null;
            T[i].nextMove = null;

        }
    }


    /**
     * Store a value for a given key
     * @param key: 56-bit key
     * @param value: non-null 8-bit value. null (0) value are used to encode missing data.
     */
    public void Put(Game.Seed[] table, int moveNum, int score, Game.Seed playerTurn)
    {

        int i = Index(table); // comPute the Index position
        T[i].moveNum = moveNum;
        T[i].table = table;
        T[i].playerTurn = playerTurn;
        T[i].score = score;
    }


    /** 
     * Get the value of a key
     * @param key
     * @return 8-bit value associated with the key if present, 0 otherwise.
     */
    public int  Get(Game.Seed[] table, int moveNum, Game.Seed playerTurn) 
    {
        int i = Index(table); // comPute the Index position
        if (T[i].table == table && T[i].moveNum == moveNum && T[i].playerTurn == playerTurn)
            return T[i].score;            // and return value if key matches
        else
            return 1000;                   // or 1000 if missing entry
    }
}
