using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RuralManager
{
    public class ConexionBD
    {
        private string connString = "Server=127.0.0.1;port=3306;database=ruraldb;uid=root;pwd=orosa";

        public ConexionBD()
        {
            
        }

        //
        // Reservas
        //
        public List<Apartamento> obtenerApartamentos()
        {
            string query = "SELECT * FROM APARTAMENTO";
            List<Apartamento> resultadoQuery = new List<Apartamento>();
            Apartamento filaApartamento;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        filaApartamento = new Apartamento((string)reader["Nombre"], Int32.Parse(reader["CapacidadMax"].ToString()), Int32.Parse(reader["CapacidadBase"].ToString()),
                            float.Parse(SafeGetString(reader, 4)));
                        resultadoQuery.Add(filaApartamento);
                    }
                }
                else
                {
                    MessageBox.Show("Error al cargar apartamentos");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return resultadoQuery;
        }

        public List<Reserva> obtenerReservas()
        {
            string query = "SELECT * FROM RESERVA";            
            int i = 0;

            // Lista de array de reservas
            List<Reserva> reservas = new List<Reserva>();
            Reserva filaReserva;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);
            
            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                            filaReserva = new Reserva(Int32.Parse(SafeGetString(reader,0)), SafeGetString(reader, 1), SafeGetString(reader, 2), SafeGetString(reader, 3),
                                Int32.Parse(SafeGetString(reader, 4)), SafeGetString(reader, 5), Int32.Parse(SafeGetString(reader, 6)),
                                Int32.Parse(SafeGetString(reader, 7)), (DateTime)reader["Checkin"], (DateTime)reader["Checkout"], SafeGetString(reader, 10), 
                                float.Parse(SafeGetString(reader, 11)),  SafeGetString(reader, 12), SafeGetString(reader, 13), bool.Parse(SafeGetString(reader, 14)), 
                                Int32.Parse(SafeGetString(reader, 15)));

                        reservas.Add(filaReserva);
                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("Error al cargar reservas");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return reservas;
        }

        public string SafeGetString(MySqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        public void crearReserva(string[] datosReserva)
        {
            for(int i = 0; i < 15; i++)
            {
                if(datosReserva[i] == null)
                {
                    datosReserva[i] = "0";
                }                
            }

            string query = "INSERT INTO RESERVA (Nombre, Apellidos, Telefono, CodigoPostal, Email, Apartamento, Personas, Checkin, Checkout, Importe, NumeroTarjeta," +
                "FechaCaducidadTarjeta, Pagado, Notas, FacturaAsociada) VALUES ('" + datosReserva[0] + "', '" + datosReserva[1] + "', '" + datosReserva[2] + "', '" + datosReserva[3] + "', '" +
                datosReserva[4] + "', '" + datosReserva[5] + "', '" + datosReserva[6] + "', '" + datosReserva[7] + "', '" + datosReserva[8] + "', '" +
                datosReserva[9] + "', '" + datosReserva[10] + "', '" + datosReserva[11] + "', '" + datosReserva[12] + "', '" + datosReserva[13] + "', '-1');" ;

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteReader();
                conn.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine();
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void modificarReserva(String[] datosReserva, int idReserva)
        {
            string query = "UPDATE RESERVA " + "SET Nombre='" + datosReserva[0] + "', Apellidos='" + datosReserva[1] + "', Telefono='" + datosReserva[2] + "', " +
                "CodigoPostal='" + datosReserva[3] + "', Email='" + datosReserva[4] + "', Apartamento='" + datosReserva[5] + "', Personas='" + 
                datosReserva[6] + "', Checkin='" + datosReserva[7] + "', Checkout='" + datosReserva[8] + "', Importe='" + datosReserva[9] + "', NumeroTarjeta='" + datosReserva[10] + 
                "', FechaCaducidadTarjeta='" + datosReserva[11] + "', Pagado='" + datosReserva[12] + "'" + ", Notas='" + datosReserva[13] +
                "' WHERE Identificador = '" + idReserva + "';";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteReader();                
                conn.Close();
                //MessageBox.Show("Se ha modificado la reserva correctamente", "Aviso");
            }
            catch (Exception ex)
            {
                //Console.WriteLine();
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void eliminarReserva(int idReserva)
        {
            string query = "DELETE FROM RESERVA WHERE Identificador ='" + idReserva + "';";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteReader();
                conn.Close();
                MessageBox.Show("La reserva ha sido borrada correctamente","Aviso");
            }
            catch (Exception ex)
            {
                //Console.WriteLine();
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public float calcularImporteReserva(int indiceApartamento, DateTime Checkin, DateTime Checkout, float precioBase)
        {
            if(Checkin >= Checkout)
            {
                MessageBox.Show("No se puede calcular el importe ya que el checkin es igual o mayor que el checkout.");
                return 0;
            }

            float importe = 0;

            int contadorDiasTotales = 1;
            int contadorDiasConTarifa = 0;
            
            string query = "SELECT Dia, Precio " +
                "FROM TARIFA " +
                "WHERE Apartamento='" + indiceApartamento + "' AND (Dia='" + Checkin.ToString("yyyy-MM-dd") + "'";

            while (Checkin != Checkout.AddDays(-1))
            {
                Checkin = Checkin.AddDays(1);
                query += " OR Dia='" + Checkin.ToString("yyyy-MM-dd") + "'";

                contadorDiasTotales++;
            }
            query += ");";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        importe += float.Parse(SafeGetString(reader, 1));
                        contadorDiasConTarifa++;
                    }
                }

                if((contadorDiasTotales - contadorDiasConTarifa) > 0)
                {
                    // Si quedn días sin contar que no tenñian tarifa declarada
                    importe += (contadorDiasTotales - contadorDiasConTarifa) * precioBase;
                }
                
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return importe;
        }

        //
        // Tarifas
        //

        public List<Tarifa> obtenerTarifas()
        {
            List<Tarifa> resultadoQuery = new List<Tarifa>();
            Tarifa filaTarifa;

            string query = "SELECT Dia, Apartamento, Precio FROM TARIFA";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        filaTarifa = new Tarifa((DateTime)reader["Dia"], Int32.Parse(SafeGetString(reader, 1)), float.Parse(SafeGetString(reader, 2)));
                        resultadoQuery.Add(filaTarifa);
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return resultadoQuery;
        }

        public List<Tarifa> obtenerTarifasDeDia(DateTime dia)
        {
            // devuelve las tarifas encontradas para ese dia

            List<Tarifa> tarifas = new List<Tarifa>();

            string query = "SELECT Dia, Apartamento, Precio FROM TARIFA WHERE Dia='" + dia.ToString("yyyy-MM-dd") + "';";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tarifas.Add(new Tarifa((DateTime)reader["Dia"], Int32.Parse(SafeGetString(reader, 1)), float.Parse(SafeGetString(reader, 2))));
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return tarifas;
        }

        public void editarTarifaDia(List<DateTime> dias, float[] precios, int[] apartamento)
        {
            string query = "DELETE FROM Tarifa WHERE Dia = '" + dias.ElementAt(0).ToString("yyyy-MM-dd") + "' ";

            for(int i = 1; i < dias.Count(); i++)
            {
                query += " OR Dia='" + dias.ElementAt(i).ToString("yyyy-MM-dd") + "'";
            }

            query += ";";
            query += "INSERT INTO TARIFA (Dia, Apartamento, Precio) VALUES";

            for (int k = 0; k < dias.Count(); k++)
            {
                for (int i = 0; i < precios.Count(); i++)
                {
                    if (precios[i] != 0)
                    {
                        query += " ('" + dias.ElementAt(k).ToString("yyyy-MM-dd") + "','" + apartamento[i] + "','" + precios[i] + "'),";
                    }
                }
            }
            query = query.Remove(query.Length - 1) + ";";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                conn.Close();
                if(dias.Count() == 1)
                    MessageBox.Show("Se han actualizado las tarifas para el dia " + dias.ElementAt(0).ToString("yyyy-MM-dd") + " correctamente.");
                else
                    MessageBox.Show("Se han actualizado las tarifas para los días seleccionados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        public void actualizarPreciosBaseTarifa(float[] precios, int[] apartamento)
        {
            string query = "";                

            for(int i = 0; i < apartamento.Count(); i++)
            {
                query += "UPDATE APARTAMENTO SET PrecioBase = " + precios[i] + " WHERE Identificador='" + (apartamento[i]+1) + "';";
            }

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                conn.Close();
                MessageBox.Show("Se han actualizado las tarifas base correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        public void eliminarTarifas(List<DateTime> dias)
        {
            // HACER UN UPDATE DE APARTAMENTOS
            string query = "DELETE FROM TARIFA WHERE Dia='" + dias.ElementAt(0).ToString("yyyy-MM-dd") + "' ";

            for (int i = 1; i < dias.Count(); i++)
            {
                query += " OR Dia='" + dias.ElementAt(i).ToString("yyyy-MM-dd") + "'";
            }
            query += ";";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                conn.Close();
                MessageBox.Show("Se han eliminado las tarifas personalizadas para los dias seleccionados.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //
        // Facturas
        //

        public int comprobarNFacturas()
        {
            int nfacturas = 0;
            string query = "SELECT * FROM FACTURA";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        nfacturas++;
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "La query ha tenido un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return nfacturas;
        }

        public int generarNuevaFactura(string[] datosFactura)
        {
            int numFactura = 0;

            for (int i = 0; i < datosFactura.Length; i++)
            {
                if (datosFactura[i] == null)
                {
                    datosFactura[i] = "0";
                }
            }
            // 0: NombreCompleto, 1: DNI, 2: Direccion, 3: Codigo Postal, 4: fechaActual, 5: checkin, 6: checkout, 7: apartamento, 8: personas, 9: precio
            // 10: observaciones, 11: ReservaAsociada
            
            string query = "INSERT INTO FACTURA (NombreyApellidos, DNI, Calle, CP, Fecha, Observaciones) VALUES ('" + datosFactura[0] + "', '" + datosFactura[1] + 
                "', '" + datosFactura[2] + "', '" + datosFactura[3] + "', '" + datosFactura[4] + "', '" + datosFactura[10] + "'); " +

                //Se asigna la factura a la reserva en la tabla de reservas como clave foránea
                "UPDATE RESERVA SET FacturaAsociada=(SELECT LAST_INSERT_ID()) WHERE Identificador=" + datosFactura[11] + ";" +
                // Se recoge el último valor de factura
                "SELECT LAST_INSERT_ID();";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        numFactura = Int32.Parse(SafeGetString(reader, 0));

                    }                      
                }

                MessageBox.Show("Se ha generado la factura correctamente.");

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return numFactura;
        }              

        public List<Factura> obtenerFacturas()
        {
            string query = "SELECT f.NumeroFactura, f.NombreyApellidos, f.DNI, f.Calle, f.CP, f.Fecha, f. Observaciones, r.Identificador " +
                "FROM FACTURA f, RESERVA r " +
                "WHERE f.NumeroFactura = r.FacturaAsociada " +
                "ORDER BY NumeroFactura Asc; ";

            // Lista de array de reservas
            List<Factura> facturas = new List<Factura>();
            Factura filaReserva;
            int i = 0, ultimoNumeroFactura = 0;
            
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Si es el mismo numero, añadir el apartamento como apartamento asociado al array
                        if(Int32.Parse(SafeGetString(reader, 0)) == ultimoNumeroFactura)
                        {                            
                            i++;
                            facturas.Last().añadirApartamentoAsociado(Int32.Parse(SafeGetString(reader, 7)));
                        }
                        else
                        {                            
                            i = 0;                            
                            ultimoNumeroFactura = Int32.Parse(SafeGetString(reader, 0));
                            
                            filaReserva = new Factura(Int32.Parse(SafeGetString(reader, 0)), SafeGetString(reader, 1), SafeGetString(reader, 2), SafeGetString(reader, 3),
                            Int32.Parse(SafeGetString(reader, 4)), SafeGetString(reader, 5), SafeGetString(reader, 6), Int32.Parse(SafeGetString(reader, 7)));

                            facturas.Add(filaReserva);
                        }                     
                        
                    }
                }
                else
                {
                    Console.WriteLine("Sin filas");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return facturas;
        }

        public void actualizarFactura(string[] datosFactura)
        {
            string query = "UPDATE FACTURA " + "SET NombreyApellidos='" + datosFactura[0] + "', DNI='" + datosFactura[1] + "', Calle='" + datosFactura[2] + 
                "', CP='" + datosFactura[3] + "', Fecha='" + datosFactura[4] + "', Observaciones='" + datosFactura[10] + "' " +
                " WHERE NumeroFactura = '" + datosFactura[12] + "';";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteReader();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void asignarNuevasFacturas(List<Reserva> Reservas, int numFactura)
        {
            // se acutalizan las reservas añadiendoles el número de factura asociado

            string query = "UPDATE RESERVA " + "SET FacturaAsociada='" + numFactura + "' " +
                " WHERE Identificador = '" + Reservas.ElementAt(0).GetId + "'";

            for(int i = 1; i < Reservas.Count(); i++)
            {
                query += " OR Identificador = '" + Reservas.ElementAt(i).GetId + "' "; 
            }
            query += ";";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand command = new MySqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteReader();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
