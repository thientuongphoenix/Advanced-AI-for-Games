using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform boardParent;
    public int boardSize;

    private string[,] board;
    public List<ButtonCell> cells = new List<ButtonCell>();

    //Turn của người chơi
    public bool isPlayerTurn = true;

    //Chiều dài của chuỗi để chiến thắng
    public int winLenght = 5;

    //Các hướng để kiểm tra chiến thắng
    private readonly int[] dx = { 1, 0, 1, 1 };
    private readonly int[] dy = { 0, 1, 1, -1 };

    private void Start()
    {
        CreateBoard();
    }

    void CreateBoard()
    {
        board = new string[boardSize, boardSize];
        for(int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                var go = Instantiate(cellPrefab, new Vector3(j, -i, 0), Quaternion.identity, boardParent);
                var cell = go.GetComponent<ButtonCell>();
                cell.row = i;
                cell.column = j;
                cell.ticTacToeManager = this;
                int row = i;
                int column = j;
                cell.GetComponent<Button>().onClick.AddListener(() => HandlePlayerMove(row, column));
                cells.Add(cell);
            }
        }
    }

    public void HandlePlayerMove(int row, int column)
    {

    }

    //void UpdateCellUI
}
