using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECommerce.WindowsForms
{
    public partial class Form2 : Form
    {
        private string _connectionString = "User ID=postgres;Password=fatec;Host=localhost;Port=5432;Database=dbDatabase;";

        public Form2()
        {
            InitializeComponent();
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text) || string.IsNullOrEmpty(txtPreco.Text) || string.IsNullOrEmpty(txtDesc.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "insert into Produtos(nome, descricao, preco) values('@nome', '@descricao', @preco)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@nome", $"%{txtNome.Text}%"));
                        command.Parameters.Add(new NpgsqlParameter("@descricao", $"%{txtDesc.Text}%"));
                        command.Parameters.Add(new NpgsqlParameter("@preco", $"%{txtPreco.Text}%"));
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao consultar produtos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }



            }
        }
    }
}
