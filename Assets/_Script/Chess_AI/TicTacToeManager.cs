using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{
    public GameObject cellPrefab; //Prefab nút bấm
    public Transform boardParent; //Panel
    public int boardSize; //Kích thước bàn cờ 10x10

    private string[,] board; //Mảng 2 chiều để lưu trạng thái của bàn cờ
    public List<ButtonCell> cells = new List<ButtonCell>(); //Danh sách các ô trong bàn cờ

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
        for(int i = 0; i < boardSize; i++) //row
        {
            for(int j = 0; j < boardSize; j++) //col
            {
                var go = Instantiate(cellPrefab, new Vector3(j, -i, 0), Quaternion.identity, boardParent);
                var cell = go.GetComponent<ButtonCell>();
                cell.row = i;
                cell.column = j;
                cell.ticTacToeManager = this;
                int row = i;
                int column = j;
                //Đăng ký sự kiện
                cell.GetComponent<Button>().onClick.AddListener(() => HandlePlayerMove(row, column));
                cells.Add(cell);
            }
        }
    }

    //Hàm xử lý khi người chơi nhấn và ô
    public void HandlePlayerMove(int row, int column)
    {
        if(!isPlayerTurn || board[row, column] != null) return;
        //Cập nhật trạng thái bàn cờ
        board[row,column] = "O";
        UpdateCellUI(row, column, "O");
        //Kiểm tra xem người chơi có chiến thắng không
    }

    void UpdateCellUI (int row, int column, string symbol)
    {
        //Cập nhật giao diện cho ô
        var cell = cells.First(x => x.row == row && x.column == column);
        cell.SetSymbol(symbol);

        //Tô màu cho ô
        var image = cell.GetComponent<Image>();
        if(symbol == "X")
        {
            image.color = Color.red;
        }
        else if(symbol == "O")
        {
            image.color = Color.blue;
        }
    }
}
