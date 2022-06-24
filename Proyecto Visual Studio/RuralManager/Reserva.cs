using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RuralManager
{
    public class Reserva
    {
        private int Identificador;
        private string Nombre;
        private string Apellidos;
        private string Telefono;
        private int CodigoPostal;
        private string Email;
        private int Apartamento;
        private int Personas;
        private DateTime Checkin;
        private DateTime Checkout;
        private string Notas; 
        private float Importe;
        private string numTarjeta;
        private string FechaCadTarjeta;
        private bool Pagado;
        private int FacturaAsociada;

        public Reserva(int id, string nombre, string apellidos, string telefono, int codigopostal, string email, int apartamento, int personas, DateTime checkin, 
            DateTime checkout, string Notas, float importe, string numTarjeta, string FechaCadTarjeta, bool Pagado, int factura)
        {
            this.Identificador = id;
            this.Nombre = nombre;
            this.Apellidos = apellidos;
            this.Telefono = telefono;
            this.CodigoPostal = codigopostal;
            this.Email = email;
            this.Apartamento = apartamento;
            this.Personas = personas;
            this.Checkin = checkin;
            this.Checkout = checkout;
            this.Notas = Notas;
            this.Importe = importe;
            this.numTarjeta = numTarjeta;
            this.FechaCadTarjeta = FechaCadTarjeta;
            this.Pagado = Pagado;
            this.FacturaAsociada = factura;
        }

        public DateTime GetSetCheckin { get => Checkin; set => Checkin = value; }
        public DateTime GetSetCheckout { get => Checkout; set => Checkout = value; }
        public string GetSetNombre { get => Nombre; set => Nombre = value; }
        public int GetId { get => Identificador; }
        public int GetSetApartamento { get => Apartamento; set => Apartamento = value; }
        public string GetSetApellidos { get => Apellidos; set => Apellidos = value; }
        public bool GetPagado { get => Pagado; }
        public string GetNotas { get => Notas; }
        public int GetSetPersonas { get => Personas; set => Personas = value; }
        public int GetCodigoPostal { get => CodigoPostal; }
        public float GetSetImporte { get => Importe; set => Importe = value; }
        public int GetFactura { get => FacturaAsociada;  }
        public string GetSetEmail { get => Email; set => Email = value; }

        public string[] getDatosReserva()
        {
            string[] datosReserva = { Nombre, Apellidos, Telefono, CodigoPostal.ToString(), Email, Apartamento.ToString(), Personas.ToString(), Checkin.ToString("yyyy-MM-dd"), Checkout.ToString("yyyy-MM-dd"), Importe.ToString(), numTarjeta, FechaCadTarjeta, Pagado.ToString(), Notas};

            return datosReserva;
        }
        public DateTime[] getCheckInAndOut()
        {
            DateTime[] checks = {Checkin, Checkout};

            return checks;
        }

    }
}
