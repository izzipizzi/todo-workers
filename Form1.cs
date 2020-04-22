using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TODOWorkers
{
    public partial class Form1 : Form
    {

        public bool pass;
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\todosDB.mdf;Integrated Security=True;Connect Timeout=30");

        DataSet data = new DataSet();
        SqlDataAdapter dataAdapter = new SqlDataAdapter();

        private void tabAddWorker_Enter(object sender, EventArgs e)
        {

            //перевірка доступу щоб добавити працівника в список
            // пароль pass тільки тсс
            Check f = new Check();
            f.Owner = this;
            f.ShowDialog();

            if(pass == false)
            {
                tabControl1.SelectTab(tabAddTodo);
                MessageBox.Show("Доступ відхилено системою");
            }
            else
            {
                MessageBox.Show("Успіх");
                pass = false;

            }

        }
        public void Clear(DataGridView dataGridView)
        {
            while (dataGridView.Rows.Count > 1)
                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    dataGridView.Rows.Remove(dataGridView.Rows[i]);
        }

        public void connect()
        {
            try
            {
                connection.Open();

                if (connection.State == ConnectionState.Open)
                {
                    // MessageBox.Show("Зєднання встановленно!");
                }
            }
            catch (Exception)
            {
                if (connection.State != ConnectionState.Open)
                {
                    MessageBox.Show("Немає зєднання з базою");
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loadData();  
        }
        public void loadData()
        {
            Clear(dataGridView1);
            SqlCommand com = new SqlCommand("Select * from todos LEFT JOIN workers on" +
                " todos.worker_id = workers.Id  order by done asc", connection);
            SqlDataAdapter da = new SqlDataAdapter();

            DataSet ds = new DataSet();
            da.SelectCommand = com;
            da.Fill(ds, "todos");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "todos";

            dataGridView1.Columns["todo"].Width = 200;
            dataGridView1.Columns["Id"].Width = 30;

            dataGridView1.Columns["worker_id"].Visible = false;
            dataGridView1.Columns["Id1"].Visible = false;

            dataGridView1.Columns["Id"].HeaderText = "№";
            dataGridView1.Columns["todo"].HeaderText = "Що потрібно зробити";
            dataGridView1.Columns["done"].HeaderText = "Виконано";
            dataGridView1.Columns["name"].HeaderText = "Імя";
            dataGridView1.Columns["date"].HeaderText = "Дата добавлення";
            dataGridView1.Columns["surname"].HeaderText = "Прізвище";
            dataGridView1.Rows[0].Selected = true;
            paint();
            connection.Close();

        }

        public int worker_ID;
        public int getID()
        {
            string id;
            connect();
            try
            {
                SqlCommand com = new SqlCommand("Select MAX(Id) from  workers", connection);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    id = reader[0].ToString();

                    worker_ID = Convert.ToInt32(id);
                    worker_ID++;

                }
                reader.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Сталсь помилка на сервері повторіть будь-ласка");


            }
            connection.Close();
            return worker_ID;

        }public int todo_ID;
        public int getTodoID()
        {
            string id;
            connect();
            try
            {
                SqlCommand com = new SqlCommand("Select MAX(Id) from  todos", connection);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    id = reader[0].ToString();

                    todo_ID = Convert.ToInt32(id);
                    todo_ID++;

                }
                reader.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Сталсь помилка на сервері повторіть будь-ласка");


            }
            connection.Close();
            return todo_ID;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string name = textBox1.Text;
            string surname = textBox2.Text;
            int id = getID();          

            try
            {
                connect();

                SqlCommand commandText = connection.CreateCommand();
                commandText.CommandText = " Insert into workers (Id,name,surname) " +
                    "VALUES (@id, @name, @surname)";
                commandText.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                commandText.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                commandText.Parameters.Add("@surname", SqlDbType.NVarChar).Value = surname;


                commandText.ExecuteNonQuery();
                connection.Close();

                MessageBox.Show("Успішно добавлено");
                tabControl1.SelectTab(tabAddTodo);

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Сталась помилка при додаванні" +ex);
            }
            connection.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text == String.Empty)
            {
                Clear(dataGridView1);
                dataGridView1.Update();
                SqlDataAdapter TodosDA = new SqlDataAdapter("Select * from todos LEFT JOIN workers on" +
                " todos.worker_id = workers.Id order by done asc ", connection);
                TodosDA.TableMappings.Add("Table", "todos");
                TodosDA.Fill(data);
                dataGridView1.DataSource = data;
                dataGridView1.DataMember = "todos";
                dataGridView1.Rows[0].Selected = true;

            }
            else
            {
                Clear(dataGridView1);
                dataGridView1.Update();
                SqlCommand com = new SqlCommand("select * from todos " +
                   "LEFT JOIN workers on todos.worker_id = workers.Id where workers.name like @name order by done asc ", connection);
                com.Parameters.Add("@name", SqlDbType.NVarChar).Value = textBox3.Text + "%";
                SqlDataAdapter TodosDA = new SqlDataAdapter();
                TodosDA.SelectCommand = com;
                TodosDA.TableMappings.Add("Table", "todos");
                TodosDA.Fill(data);
                dataGridView1.DataSource = data;
                dataGridView1.DataMember = "todos";
                dataGridView1.Rows[0].Selected = true;


            }
            paint();
            connection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows[0].Cells[0].Value != null)
            {
                string a = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                if (a == null) { return; }
                else
                {
                    connect();
                    SqlCommand com = new SqlCommand("update todos set done = @yes where Id='" + a + "'", connection);
                    com.Parameters.Add("@yes", SqlDbType.NVarChar).Value = "Так";
                    com.ExecuteNonQuery();
                    loadData();
                    connection.Close();
                }

            }


        }
        public void paint()
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[3].Value.ToString() == "Так")
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.GreenYellow;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255,117,117);

                }



            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            paint();
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            paint();
        }

        private void tabAddTodo_Enter(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("select name,surname from workers ", connection);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();

            comboBox1.DisplayMember = ds.Tables[0].Columns[1].ToString();
            comboBox1.ValueMember = "Виберіть Працівник";
            comboBox1.DataSource = ds.Tables[0];

            comboBox1.Enabled = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Check f = new Check();
            f.Owner = this;
            f.ShowDialog();

            if (pass == false)
            {
                tabControl1.SelectTab(tabAddTodo);
                MessageBox.Show("Доступ відхилено системою");
            }
            else
            {

                string todo = richTextBox1.Text;
                int id = getTodoID();
                /*int worker_id = getWorkerIDbySurname();*/

                try
                {
                    connect();

                    SqlCommand commandText = connection.CreateCommand();
                    commandText.CommandText = " Insert into todos (Id,todo,worker_id,done,date) " +
                        "VALUES (@id, @todo, (Select TOP 1 Id from workers where surname = @surname),@done,@date)";
                    commandText.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    commandText.Parameters.Add("@todo", SqlDbType.NVarChar).Value = todo;
                    commandText.Parameters.Add("@done", SqlDbType.NVarChar).Value = "Ні";
                    commandText.Parameters.Add("@surname", SqlDbType.NVarChar).Value = comboBox1.Text;
                    commandText.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;


                    commandText.ExecuteNonQuery();
                    loadData();

                    MessageBox.Show("Успіх");
                    connection.Close();


                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Сталась помилка при додаванні" + ex);
                }


                pass = false;



            }
        }
    }
}
