using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.WindowsForms.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public bool Deletado { get; set; }
        public bool Disponivel { get; internal set; }
        public string? Categoria { get; internal set; }
    }
}
