using UnityEngine;
using UnityEngine.UI;

/// <summary>〇×ゲーム</summary>
public class TicTacToe : MonoBehaviour
{
    /// <summary>盤面のサイズ</summary>
    private const int Size = 3;

    /// <summary>セルの配列</summary>
    private Image[,] _cells;

    void Start()
    {
        _cells = new Image[Size, Size];

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                // ゲームオブジェクトを作成
                var obj = new GameObject($"Cell : [{r},{c}]");

                // パネルの子オブジェクトに設定
                obj.transform.parent = transform;

                // セルを配列に格納
                var cell = obj.AddComponent<Image>();
                _cells[r, c] = cell;
            }
        }
    }

    void Update()
    {
        
    }
}
