using System;
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

        public frmProdutoDetalhe(int id, bool op) : this()
        {
            Id = id;
            Op = op;

            btnAcao.Text = "Atualizar";

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                List<Categoria> categorias = new List<Categoria>();
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM Categoria";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Categoria categoria = new Categoria
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = Convert.ToString(reader["nome"])
                                };

                                categorias.Add(categoria);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao consultar produtos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                comboCat.DataSource = categorias;
                comboCat.DisplayMember = "Nome"; // Define qual propriedade será exibida como texto no ComboBox
                comboCat.ValueMember = "Id";
            }
            CarregarProduto(id, op);

        }

        public frmProdutoDetalhe(bool op) : this()
        {
            Op = op;

            btnAcao.Text = "Cadastrar";
            cbDispo.Checked = true;
            cbDispo.Visible = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                List<Categoria> categorias = new List<Categoria>();
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM Categoria";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Categoria categoria = new Categoria
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = Convert.ToString(reader["nome"])
                                };

                                categorias.Add(categoria);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao consultar produtos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                comboCat.DataSource = categorias;
                comboCat.DisplayMember = "Nome"; // Define qual propriedade será exibida como texto no ComboBox
                comboCat.ValueMember = "Id";
            }
        }

        public int Id { get; private set; }
        public bool Op { get; private set; }

        private void CarregarProduto(int id, bool op)
        {
            if (op) // If operation is update
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "SELECT Id, Nome, Descricao, Preco, Disponivel, fkCat FROM Produtos WHERE Id = @id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);

                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    txtId.Text = Convert.ToString(reader["Id"]);
                                    txtNome.Text = reader["Nome"].ToString();
                                    txtDesc.Text = reader["Descricao"].ToString();
                                    txtPreco.Text = Convert.ToString(reader["Preco"]);
                                    cbDispo.Checked = Convert.ToBoolean(reader["Disponivel"]);
                                    comboCat.SelectedIndex = Convert.ToInt32(reader["fkCat"])-1;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao consultar produtos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAcao_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    if (Op) // Update operation
                    {
                        string query = "UPDATE Produtos SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco, Disponivel = @Disp, fkCat = @Cat WHERE Id = @Id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Nome", txtNome.Text);
                            command.Parameters.AddWithValue("@Descricao", txtDesc.Text);
                            command.Parameters.AddWithValue("@Preco", Convert.ToDecimal(txtPreco.Text)); // Assuming Preco is numeric
                            command.Parameters.AddWithValue("@Id", Id);
                            command.Parameters.AddWithValue("@Disp", cbDispo.Checked);
                            command.Parameters.AddWithValue("@Cat", comboCat.SelectedIndex + 1);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Produto atualizado com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Nenhum produto foi atualizado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else // Insert operation
                    {
                        string query = "INSERT INTO Produtos (Nome, Descricao, Preco) VALUES (@Nome, @Descricao, @Preco)";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Nome", txtNome.Text);
                            command.Parameters.AddWithValue("@Descricao", txtDesc.Text);
                            command.Parameters.AddWithValue("@Preco", Convert.ToDecimal(txtPreco.Text)); // Assuming Preco is numeric

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Produto cadastrado com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Nenhum produto foi cadastrado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
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
