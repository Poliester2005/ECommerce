using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ECommerce.WindowsForms.Models;
using Npgsql;

namespace ECommerce.WindowsForms
{
    public partial class frmProdutoDetalhe : Form
    {
        private string _connectionString = "User ID=postgres;Password=fatec;Host=localhost;Port=5432;Database=dbDatabase;";

        public frmProdutoDetalhe()
        {
            InitializeComponent();
        }

        public frmProdutoDetalhe(int id) : this()
        {
            Id = id;
            CarregarProduto(id);
        }

        public int Id { get; }

        private void CarregarProduto(int id)
        {
            List<Produto> produtos = new List<Produto>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Id, Nome, Descricao, Preco FROM Produtos WHERE Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nome = reader["Nome"].ToString(),
                                    Descricao = reader["Descricao"].ToString(),
                                    Preco = Convert.ToDecimal(reader["Preco"])
                                };
                                produtos.Add(produto);
                            }
                        }
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

            if (produtos.Count > 0)
            {
                txtId.Text = produtos[0].Id.ToString();
                txtNome.Text = produtos[0].Nome;
                txtDesc.Text = produtos[0].Descricao;
                txtPreco.Text = produtos[0].Preco.ToString();
            }
            else
            {
                MessageBox.Show("Nenhum produto encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
