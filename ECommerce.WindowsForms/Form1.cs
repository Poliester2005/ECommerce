using ECommerce.WindowsForms.Models;
using Npgsql;

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

                    string query = "SELECT Id, Nome, Descricao, Preco FROM Produtos WHERE Deletado = false AND Nome ILIKE @NomePesquisa";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@NomePesquisa", $"%{txtPesquisa.Text}%"));

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

            // Mostrar os produtos no DataGridView se houver algum resultado
            if (produtos.Count > 0)
            {
                dgvProdutos.DataSource = produtos;
                dgvProdutos.Columns["Id"].Visible = false;
                dgvProdutos.Columns["Nome"].HeaderText = "Nome";
                dgvProdutos.Columns["Descricao"].HeaderText = "Descrição";
                dgvProdutos.Columns["Preco"].HeaderText = "Preço";
            }
            else
            {
                dgvProdutos.DataSource = null; // Limpar o DataGridView
                MessageBox.Show("Nenhum produto encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCad_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show(); // Show Form2
        }
    }
}