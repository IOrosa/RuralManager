using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuralManager
{
    
    public class Tarifa
    {
        private DateTime Dia;
        private int Apartamento;
        private float Precio;

        public Tarifa(DateTime Dia, int Apartamento, float Precio)
        {            
            this.Dia = Dia;
            this.Apartamento = Apartamento;
            this.Precio = Precio;
        }

        public DateTime GetSetDia { get => Dia; set => Dia = value; }
        public int GetSetApartamento { get => Apartamento; set => Apartamento = value; }
        public float GetSetPrecio { get => Precio; set => Precio = value; }
    }
}
