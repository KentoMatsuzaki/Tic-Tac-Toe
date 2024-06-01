using UnityEngine;
using UnityEngine.UI;

/// <summary>�Z�~�Q�[��</summary>
public class TicTacToe : MonoBehaviour
{
    /// <summary>�Ֆʂ̃T�C�Y</summary>
    private const int Size = 3;

    /// <summary>�Z���̔z��</summary>
    private Image[,] _cells;

    void Start()
    {
        _cells = new Image[Size, Size];

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                // �Q�[���I�u�W�F�N�g���쐬
                var obj = new GameObject($"Cell : [{r},{c}]");

                // �p�l���̎q�I�u�W�F�N�g�ɐݒ�
                obj.transform.parent = transform;

                // �Z����z��Ɋi�[
                var cell = obj.AddComponent<Image>();
                _cells[r, c] = cell;
            }
        }
    }

    void Update()
    {
        
    }
}
