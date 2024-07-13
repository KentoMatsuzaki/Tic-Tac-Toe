using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TicTacToe : MonoBehaviour
{
    // キャンバス
    private Canvas canvas;

    // マス目のサイズ
    private const int Size = 3;

    // セルの配列
    private Image[,] cells;

    // セルの色（非選択時）
    private Color defaultColor = Color.white;

    // セルの色（選択時）
    private Color selectedColor = Color.yellow;

    // 選択中の行数
    private int selectedRow;

    // 選択中の列数
    private int selectedColumn;

    // 〇のスプライト
    [SerializeField] private Sprite circle;

    // ✕のスプライト
    [SerializeField] private Sprite cross;

    private void Start()
    {
        // キャンバスを作成
        SetUpCanvas();

        // セルの配列を初期化
        cells = new Image[Size, Size];

        for(int r = 0; r < Size; r++)
        {
            for(int c = 0; c < Size; c++)
            {
                // セルを作成して配列に登録する
                var cellObj = new GameObject();
                cellObj.transform.parent = canvas.transform;
                var cell = cellObj.AddComponent<Image>();
                cells[r, c] = cell;
            }
        }

        StartCoroutine(GameFlow());
    }

    void Update()
    {
        // 入力に応じて選択中の行数・列数を変更
        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedColumn--;
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedColumn++;
        if (Input.GetKeyDown(KeyCode.UpArrow)) selectedRow--;
        if (Input.GetKeyDown(KeyCode.DownArrow)) selectedRow++;

        // 配列の範囲に収める
        if (selectedColumn < 0) selectedColumn = 0;
        if (selectedColumn >= Size) selectedColumn = Size - 1;
        if (selectedRow < 0) selectedRow = 0;
        if (selectedRow >= Size) selectedRow = Size - 1;

        // セルの色を変更する
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                var cell = cells[r, c];
                cell.color = (r == selectedRow && c == selectedColumn) ? selectedColor : defaultColor;
            }
        }        
    }

    // キャンバスのセットアップ
    private void SetUpCanvas()
    {
        // Canvasのオブジェクトを作成
        GameObject canvasObj = new GameObject("Canvas");

        // Canvasコンポーネントを追加して設定する
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // CanvasScalerコンポーネントを追加して設定する
        var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        // GridLayoutGroupコンポーネントを追加して設定する
        var gridLayoutGroup = canvasObj.AddComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(250, 250);
        gridLayoutGroup.spacing = new Vector2(25, 25);
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayoutGroup.constraintCount = Size;
    }

    // 選択しているセルのスプライトを変更する
    private void ChangeSelectedCellSprite()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var cell = cells[selectedRow, selectedColumn];
            cell.sprite = circle;
        }
    }

    // ランダムなセルのスプライトを変更する
    private void ChangeRandomCellSprite()
    {
        Image randomCell = GetRandomCell();

        while(randomCell.sprite != null)
        {
            randomCell = GetRandomCell();
        }

        randomCell.sprite = cross;
    }

    private int GetRandomRow() => Random.Range(0, Size);

    private int GetRandomCol() => Random.Range(0, Size);

    private Image GetRandomCell() => cells[GetRandomRow(), GetRandomCol()];

    // ゲームオーバーの判定を行う
    private bool IsGameOver()
    {
        return false;
    }

    // 自分のターン
    private IEnumerator PlayerTurn()
    {
        bool isTurnEnded = false;

        while (!isTurnEnded)
        {
            ChangeSelectedCellSprite();
            isTurnEnded = true;
        }

        yield return null;
    }

    // 相手のターン
    private IEnumerator EnemyTurn()
    {
        bool isTurnEnded = false;

        while (!isTurnEnded)
        {
            ChangeRandomCellSprite();
            isTurnEnded = true;
        }

        yield return null;
    }

    // リザルト表示
    //private IEnumerator ShowResult()
    //{

    //}

    //　ゲームをリセット
    private void ResetGame()
    {

    }

    // ゲームフロー
    private IEnumerator GameFlow()
    {
        while (true)
        {
            while (IsGameOver())
            {
                yield return PlayerTurn();

                if (IsGameOver()) break;

                yield return EnemyTurn();
            }
            //yield return ShowResult();

            ResetGame();
        }
    }
}
