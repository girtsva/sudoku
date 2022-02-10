using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // adding rows first in order to create the data grid and be able to make design changes
            dataGridView1.Rows.Add(9);

            // invoking function to color the alternating Sudoku squares
            ColorAlternateSquares();

            // setting restrictions on data grid
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AllowUserToResizeColumns = false;
            
            // setting difficulty level to "Easy" by default
            comboBox1.SelectedItem = "Easy";

            StartGame();
        }

        // triggering event to draw distinct 3x3 square lines
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.Black, 2), dataGridView1.Height / 3, 0, dataGridView1.Height / 3, dataGridView1.Height);
            e.Graphics.DrawLine(new Pen(Color.Black, 2), (int)(dataGridView1.Height / 1.5), 0, (int)(dataGridView1.Height / 1.5), dataGridView1.Height);
            e.Graphics.DrawLine(new Pen(Color.Black, 2), 0, dataGridView1.Height / 3, dataGridView1.Height, dataGridView1.Height / 3);
            e.Graphics.DrawLine(new Pen(Color.Black, 2), 0, (int)(dataGridView1.Height / 1.5), dataGridView1.Height, (int)(dataGridView1.Height / 1.5));
        }

        // function to set color of the alternating Sudoku squares to darker tone
        private void ColorAlternateSquares()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = ((i / 3) + (j / 3)) % 2 == 0 ? SystemColors.Control : Color.LightGray;
                }
            }
        }

        private void StartGame()
        {
            ResetFontColorForAllCells();
            ResetFontStyleForAllCells();
            LoadValues();
            CopyGrid();
            HideCells();
        }

        private void ResetFontColorForAllCells()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = Color.Black;
                }
            }
        }

        private void ResetFontStyleForAllCells()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.Font =
                        new Font("Modern No. 20", (float)20.25, FontStyle.Bold);
                }
            }
        }

        // restrict input to digits 1-9 and backspace; MaxLInputLength set to 1 in Design mode for each column
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != '\b' || e.KeyChar == '0';
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl tb)
            {
                tb.KeyPress -= dataGridView1_KeyPress;
                tb.KeyPress += dataGridView1_KeyPress;
            }
        }

        private void LoadValues()
        {
            // setting all cell values to null and readonly
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                { 
                    cell.Value = null;
                    cell.ReadOnly = true;
                }
            }

            // calling recursive method to find suitable value for each cell
            FindCellValue(0, -1);
        }

        Random random = new Random();

        private bool FindCellValue(int i, int j)
        {
            // increase j (column) value to move in right direction to the next cell
            // when columns end, reset column to zero
            if (++j > 8)
            {
                j = 0;

                // increase i (row) value to move down to the next row
                // exit when rows end
                if (++i > 8)
                {
                    return true;
                }
            }

            var numbersLeft = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int value;

            // assign to cell a random number from numbersLeft list, check if it is not duplicate
            // go to the next cell and check if it can be assigned with next random non-duplicate number
            do
            {
                // if there is no numbers left in the list, return to the previous cell
                // and assign a different number to it
                if (numbersLeft.Count < 1)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    return false;
                }

                // assign to cell a random number from the numbers left in the list
                value = numbersLeft[random.Next(0, numbersLeft.Count)];
                dataGridView1.Rows[i].Cells[j].Value = value;

                // remove the assigned to cell value from the list of numbers left
                numbersLeft.Remove(value);
            }
            while (!IsUniqueNumber(value, i, j) || !FindCellValue(i, j));

            return true;
        }

        private bool IsUniqueNumber(int value, int x, int y)
        {
            for (int i = 0; i < 9; i++)
            {
                // check all the cells in row
                if (i != y && Convert.ToInt32(dataGridView1.Rows[x].Cells[i].Value) == value)
                {
                    return false;
                }
                
                // check all the cells in column
                if (i != x && Convert.ToInt32(dataGridView1.Rows[i].Cells[y].Value) == value)
                {
                    return false;
                }
            }

            // check all the cells in the given square
            for (int i = x - (x % 3); i < x - (x % 3) + 3; i++)
            {
                for (int j = y - (y % 3); j < y - (y % 3) + 3; j++)
                {
                    if (i != x && j != y && Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value) == value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // below code necessary for hiding random cell values according to
        // user's selected game difficulty level
        private int _cellValuesToHideCount;
        private void HideCells()
        {
            if ((string)comboBox1.SelectedItem == "Easy")
            {
                _cellValuesToHideCount = 20;
            }

            else if ((string)comboBox1.SelectedItem == "Medium")
            {
                _cellValuesToHideCount = 33;
            }

            else if ((string)comboBox1.SelectedItem == "Hard")
            {
                _cellValuesToHideCount = 45;
            }

            List<string> rowColCombo = new List<string>();

            for (int i = 0; i < _cellValuesToHideCount; i++)
            {
                int randomRow;
                int randomCol;
                string rowCol;

                do
                {
                    randomRow = random.Next(0, 9);
                    randomCol = random.Next(0, 9);
                    rowCol = randomRow.ToString() + randomCol.ToString();

                } while (rowColCombo.Contains(rowCol));

                rowColCombo.Add(rowCol);

                // setting properties for cells with removed values
                dataGridView1.Rows[randomRow].Cells[randomCol].Value = null;
                dataGridView1.Rows[randomRow].Cells[randomCol].Style.ForeColor = Color.Red;
                dataGridView1.Rows[randomRow].Cells[randomCol].ReadOnly = false;
            }
        }
        
        // code below necessary for copying datagridview to grid[][] and returning when Solution pressed
        private readonly int[][] _grid = new int[9][];

        private void InitializeGrid()
        {
            _grid[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[3] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[4] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[5] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[6] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[7] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _grid[8] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        private void CopyGrid()
        {
            InitializeGrid();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _grid[i][j] = Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value);
                }
            }
        }

        private void RestoreGrid()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = _grid[i][j];
                }
            }
        }

        // check solution for empty cells and non-duplicate cells
        private void VerifySolution()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (dataGridView1.Rows[i].Cells[j].Value == null)
                    {
                        MessageBox.Show(@"In order to check Your solution, You have to fill in all cells first!");
                        return;
                    }
                }
            }

            bool duplicateFound = false;

            ResetFontStyleForAllCells();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!IsUniqueNumber(Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value), i, j))
                    {
                        //MessageBox.Show($"Duplicate found! Value {dataGridView1.Rows[i].Cells[j].Value} in cell {i}-{j}");
                        dataGridView1.Rows[i].Cells[j].Style.Font =
                            new Font("Modern No. 20", (float)26.25, FontStyle.Bold | FontStyle.Underline);
                        duplicateFound = true;
                        //return;
                    }
                }
            }

            if (duplicateFound)
            {
                MessageBox.Show(@"Duplicates found!");
            }
            else
            {
                ResetFontStyleForAllCells();
                MessageBox.Show(@"Congratulations! Your solution seems fine!");
            }
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void buttonSolution_Click(object sender, EventArgs e)
        {
            RestoreGrid();
        }
        
        private void buttonVerify_Click(object sender, EventArgs e)
        {
           VerifySolution();
        }
    }
}
