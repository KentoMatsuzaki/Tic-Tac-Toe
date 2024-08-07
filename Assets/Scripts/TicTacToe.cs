﻿using UnityEngine;
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

    // ゲーム情報を表示するテキスト
    [SerializeField] private Text turnText;

    [SerializeField] private Text resultText;

    // ターン情報
    private string turnPlayer;

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

        // ゲームフローを開始する
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
        gridLayoutGroup.cellSize = new Vector2(150, 150);
        gridLayoutGroup.spacing = new Vector2(25, 25);
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayoutGroup.constraintCount = Size;
    }

    // 選択しているセルのスプライトを変更する
    private bool ChangeSelectedCellSprite()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var cell = cells[selectedRow, selectedColumn];

            if (cell.sprite == null)
            {
                cell.sprite = circle;
                return true;
            }
        }
        return false;
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

    // 最善手のセルのスプライトを変更する
    private void ChangeBestCellSprite()
    {
        int bestScore = int.MinValue;
        int row = -1;
        int column = -1;

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                if (cells[r, c].sprite == null)
                {
                    cells[r, c].sprite = cross;
                    int score = Minimax(0, false);
                    cells[r, c].sprite = null;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        row = r;
                        column = c;
                    }
                }
            }
        }

        if (row != -1 && column != -1 && cells[row, column].sprite == null)
        {
            cells[row, column].sprite = cross;
        }
    }

    // 再帰的に現在の一手を評価する
    int Minimax(int depth, bool isMaximizing)
    {
        if (IsWin())
        {
            return isMaximizing ? -1 : 1;
        }
        if (IsDraw())
        {
            return 0;
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (cells[r, c].sprite == null)
                    {
                        cells[r, c].sprite = cross;

                        int score = Minimax(depth + 1, false);

                        cells[r, c].sprite = null;

                        bestScore = Mathf.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }

        // 相手が自分のスコアを最小化するように行動すると仮定
        else
        {
            int bestScore = int.MaxValue;

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (cells[r, c].sprite == null)
                    {
                        cells[r, c].sprite = circle;

                        int score = Minimax(depth + 1, true);

                        cells[r, c].sprite = null;

                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }

    private int GetRandomRow() => Random.Range(0, Size);

    private int GetRandomCol() => Random.Range(0, Size);

    private Image GetRandomCell() => cells[GetRandomRow(), GetRandomCol()];

    // ゲームの終了判定を行う
    private bool IsGameOver()
    {
        if (IsWin() || IsDraw())
        {

            return true;
        }
        return false;
    }

    // 引き分け判定
    private bool IsDraw()
    {
        if(IsWin()) return false;

        foreach (var cell in cells) if (cell.sprite == null) return false;

        //resultText.text = "Draw";

        return true;
    }

    // 勝利判定
    private bool IsWin()
    {
        if (CheckRowsForWinner() || CheckColumnsForWinner() || CheckDiagonalsForWinner())
        {
            //resultText.text = $"勝者：{turnPlayer}";
            return true;
        }
        else return false;
    }

    // 行の判定
    private bool CheckRowsForWinner()
    {
        for(int r = 0; r < Size; r++)
        {
            if(cells[r, 0].sprite != null && cells[r, 0].sprite == cells[r, 1].sprite 
                && cells[r, 0].sprite == cells[r, 2].sprite) return true;
        }
        return false;
    }

    // 列の判定
    private bool CheckColumnsForWinner()
    {
        for(int c = 0; c < Size; c++)
        {
            if (cells[0, c].sprite != null && cells[0, c].sprite == cells[1, c].sprite 
                && cells[0, c].sprite == cells[2, c].sprite) return true;
        }
        return false;
    }

    // 斜めの判定
    private bool CheckDiagonalsForWinner()
    {
        if (cells[0, 0].sprite != null && cells[0, 0].sprite ==
            cells[1, 1].sprite && cells[0, 0].sprite == cells[2, 2].sprite) return true;

        if (cells[0, 2].sprite != null && cells[0, 2].sprite ==
            cells[1, 1].sprite && cells[0, 2].sprite == cells[2, 0].sprite) return true;

        return false;
    }

    // 自分のターン
    private IEnumerator PlayerTurn()
    {
        turnText.text = "あなたのターンです";
        bool isTurnEnded = false;
        turnPlayer = "Player";

        while (!isTurnEnded)
        {
            if (ChangeSelectedCellSprite())
            {
                isTurnEnded = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1);
    }

    // 相手のターン
    private IEnumerator EnemyTurn()
    {
        turnText.text = "相手のターンです";
        bool isTurnEnded = false;
        turnPlayer = "AI";

        while (!isTurnEnded)
        {
            //ChangeRandomCellSprite();
            ChangeBestCellSprite();
            isTurnEnded = true;

            yield return null;
        }

        yield return new WaitForSeconds(1);
    }

    //　ゲームをリセット
    private IEnumerator ResetGame()
    {
        foreach (var cell in cells) cell.sprite = null;
        yield return new WaitForSeconds(1);
    }

    // ゲームフロー
    private IEnumerator GameFlow()
    {
        while (true)
        {
            while (!IsGameOver())
            {
                yield return PlayerTurn();

                if (IsGameOver()) break;

                yield return EnemyTurn();
            }

            yield return ResetGame();
            resultText.text = string.Empty;
        }
    }
}
