using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetarioMaui.Models
{
    public class RecetaModel
    {
        private string? imagen;

        public string Nombre { get; set; } = "";
        public string Ingredientes { get; set; } = "";
        public string Procedimineto { get; set; } = "";
        public DateTime UltimaElaboracion { get; set; } = new DateTime(1492, 10, 12);
        public string? Imagen
        {
            get
            {
                if (imagen == null)
                {
                    return "imagenpordefecto.png";
                }
                return imagen;
            }
            set
            {
                imagen = value;
            }
        }
        public byte TiempoPreparacion { get; set; }
    }
}