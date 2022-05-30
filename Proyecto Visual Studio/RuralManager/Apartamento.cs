using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuralManager
{
    public class Apartamento
    {
        private string Nombre = "";
        private int capacidadMax;
        private int capacidadMin;
        private float precioBase;

        public string GetNombre { get => Nombre; set => Nombre = value; }
        public int GetSetCapacidadMax { get => capacidadMax; set => capacidadMax = value; }
        public int GetSetCapacidadMin { get => capacidadMin; set => capacidadMin = value; }
        public float GetSetPrecioBase { get => precioBase; set => precioBase = value; }

        public Apartamento(String Nombre, int capacidadMax, int capacidadMin, float precioBase)
        {
            this.Nombre = Nombre;
            this.capacidadMax = capacidadMax;
            this.capacidadMin = capacidadMin;
            this.precioBase = precioBase;
        }
    }

    
}
