using ECommerce.WindowsForms.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ECommerce.WindowsForms
{
    public partial class Form1 : Form
    {
        private string _connectionString = "User ID=postgres;Password=fatec;Host=localhost;Port=5432;Database=dbDatabase;";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Produto> produtos = new List<Produto>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT p.Id, p.Nome AS NomeProduto, p.Descricao, p.Preco, p.Disponivel, c.nome AS NomeCategoria
                        FROM Produtos p
                        JOIN Categoria c ON p.fkCat = c.id
                        WHERE p.Deletado = false AND p.nome ILIKE @NomePesquisa";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NomePesquisa", $"%{txtPesquisa.Text}%");

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nome = Convert.ToString(reader["NomeProduto"]),
                                    Descricao = Convert.ToString(reader["Descricao"]),
                                    Preco = Convert.ToDecimal(reader["Preco"]),
                                    Disponivel = Convert.ToBoolean(reader["Disponivel"]),
                                    Categoria = Convert.ToString(reader["NomeCategoria"]) // Assuming Categoria is of type string in Produto
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
            }

            // Mostrar os produtos no DataGridView se houver algum resultado
            if (produtos.Count > 0)
            {
                dgvProdutos.DataSource = produtos;
                dgvProdutos.Columns["Id"].Visible = false;
                dgvProdutos.Columns["Deletado"].Visible = false;
                dgvProdutos.Columns["Nome"].HeaderText = "Nome";
                dgvProdutos.Columns["Descricao"].HeaderText = "Descrição";
                dgvProdutos.Columns["Preco"].HeaderText = "Preço";
                dgvProdutos.Columns["Disponivel"].HeaderText = "Disponível";
                dgvProdutos.Columns["Categoria"].HeaderText = "Categoria";
            }
            else
            {
                dgvProdutos.DataSource = null; // Limpar o DataGridView
                MessageBox.Show("Nenhum produto encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDet_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvProdutos.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                var produtoDetalhe = new frmProdutoDetalhe(id, true);
                produtoDetalhe.ShowDialog();
            }
        }

        private void btnCad_Click(object sender, EventArgs e)
        {
            var produtoDetalhe = new frmProdutoDetalhe(false);
            produtoDetalhe.ShowDialog();
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvProdutos.SelectedRows[0].Cells["Id"].Value);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "UPDATE Produtos SET Deletado = true WHERE id = @Id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Produto Deletado com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // Atualizar a exibição dos produtos após a deleção
                                button1_Click(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Nenhum produto foi deletado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao realizar operação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
