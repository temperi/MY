using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace post
{
    public partial class Form1 : Form
    {
        private int lastInsertedId = -1;
        private SQLiteConnection con;

        public Form1()
        {
            InitializeComponent();

            con = new SQLiteConnection("Data Source=DB.db;Version=3;");
            con.Open();

            LoadDataIntoDataGridView();

        }

        private void LoadDataIntoDataGridView()
        {
            string selectQuery = "SELECT * FROM Equipment";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, con))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    dataGridView1.Columns["Name"].HeaderText = "Название";
                    dataGridView1.Columns["Model"].HeaderText = "Модель";
                    dataGridView1.Columns["Date_work"].HeaderText = "Дата эксплуатации";
                    dataGridView1.Columns["Guarantee"].HeaderText = "Гарантия";
                    dataGridView1.Columns["quantity"].HeaderText = "Кол-во";
                    dataGridView1.Columns["condition"].HeaderText = "Состояние";
                    dataGridView1.Columns["location_id"].HeaderText = "Кабинет";

                    dataGridView1.Columns["id"].Visible = false;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    dataGridView1.ScrollBars = ScrollBars.Both;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string selectQuery = "SELECT * FROM Equipment";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, con))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    dataGridView1.Columns["Name"].HeaderText = "Название";
                    dataGridView1.Columns["Date_work"].HeaderText = "Дата эксплуатации";
                    dataGridView1.Columns["Guarantee"].HeaderText = "Гарантия";
                    dataGridView1.Columns["quantity"].HeaderText = "Кол-во";
                    dataGridView1.Columns["condition"].HeaderText = "Состояние";
                    dataGridView1.Columns["location_id"].HeaderText = "Кабинет";

                    dataGridView1.Columns["id"].Visible = false;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    dataGridView1.ScrollBars = ScrollBars.Both;
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string Model = textBox6.Text.Trim();
            string dateWork = textBox2.Text.Trim();
            string Guarantee = textBox3.Text.Trim();
            string quantity = textBox4.Text.Trim();
            string condition = textBox5.Text.Trim();
            string locationId = comboBox1.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(Model) || string.IsNullOrEmpty(dateWork) || string.IsNullOrEmpty(Guarantee) ||
                string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(condition) || string.IsNullOrEmpty(locationId))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(dateWork, out _))
            {
                MessageBox.Show("Неверный формат даты. Пожалуйста, введите корректную дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(quantity, out _))
            {
                MessageBox.Show("Неверный формат количества. Пожалуйста, введите корректное число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string insertQuery = @"
        INSERT INTO Equipment (Name, Model, Date_work, Guarantee, quantity, condition, location_id)
        VALUES (@Name, @Model, @Date_work, @Guarantee, @quantity, @condition, @location_id);
        SELECT last_insert_rowid();";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, con))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("Model", Model);
                command.Parameters.AddWithValue("@Date_work", dateWork);
                command.Parameters.AddWithValue("@Guarantee", Guarantee);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@condition", condition);
                command.Parameters.AddWithValue("@location_id", locationId);

                lastInsertedId = Convert.ToInt32(command.ExecuteScalar());
            }

            MessageBox.Show("Данные успешно добавлены!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int id = Convert.ToInt32(selectedRow.Cells["id"].Value);

                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DeleteRecordFromDatabase(id);

                    LoadDataIntoDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DeleteRecordFromDatabase(int id)
        {
            string deleteQuery = "DELETE FROM Equipment WHERE id = @id";

                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, con))
                {
                    command.Parameters.AddWithValue("@id", id);

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    command.ExecuteNonQuery();
                }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string input = textBox.Text.Replace(".", "");
            if (input.Length > 8)
            {
                textBox.Text = input.Substring(0, 8);
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }

            string formattedDate = FormatDate(input);
            if (formattedDate != textBox.Text)
            {
                textBox.Text = formattedDate;
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
        private string FormatDate(string input)
        {
            if (input.Length > 2)
            {
                input = input.Insert(2, ".");
            }
            if (input.Length > 5)
            {
                input = input.Insert(5, ".");
            }
            return input;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (lastInsertedId == -1)
            {
                MessageBox.Show("Нет записей для отмены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string deleteQuery = "DELETE FROM Equipment WHERE id = @id;";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, con))
            {
                command.Parameters.AddWithValue("@id", lastInsertedId);
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Последняя запись успешно удалена!");
        }
    }
}
