using TMPro;
using UnityEngine;

public class ButtonCell : MonoBehaviour
{
    public int row;
    public int column;

    public TicTacToeManager ticTacToeManager;

    private void OnMouseDown()
    {
        ticTacToeManager.HandlePlayerMove(row, column);
    }

    public void SetSymbol()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = symbol;
    }
}
