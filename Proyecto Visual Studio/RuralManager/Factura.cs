using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace RuralManager
{
    public class Factura
    {
        private string plantilla = @"C:\Users\ivior\Desktop\facturas\plantillaFactura.docx";
        private string plantillaNoModificable = @"C:\Users\ivior\Desktop\facturas\plantillaFacturaNoModificable.docx";
        private string carpetaFacturas = @"C:\Users\ivior\Desktop\facturas\";

        // Datos de factura 
        private int numeroFactura;
        private string nombreCompleto;
        private string DNI;
        private string Direccion;
        private int CodigoPostal;
        private string Fecha;
        private string Observaciones;
        private List<int> reservasAsociadas = new List<int>();

        public int GetSetNumeroFactura { get => numeroFactura; set => numeroFactura = value; }
        public string GetSetNombreCompleto { get => nombreCompleto; set => nombreCompleto = value; }
        public string GetSetDNI { get => DNI; set => DNI = value; }
        public string GetSetDireccion { get => Direccion; set => Direccion = value; }
        public int GetSetCodigoPostal { get => CodigoPostal; set => CodigoPostal = value; }
        public string GetSetFecha { get => Fecha; set => Fecha = value; }
        public string GetSetObservaciones { get => Observaciones; set => Observaciones = value; }
        public List<int> GetSetReservasAsociadas { get => reservasAsociadas; set => reservasAsociadas = value; }

        public Factura(string[] datosFactura)
        {
            // Guardar como factura1, factura2, etc...
            // los números de factura irán ordenados según la tabla de facturas en la base de datos

            ConexionBD conn = new ConexionBD();

            File.Copy(plantillaNoModificable, plantilla);

            string saveAs = carpetaFacturas + "Factura" + conn.generarNuevaFactura(datosFactura);
            CreateWordDocument(plantilla, saveAs, datosFactura, null);
            File.Delete(plantilla);
        }

        public Factura(int numeroFactura, string nombreCompleto, string DNI, string direccion, int codigopostal, string fecha, string observaciones, int reserva)
        {
            this.numeroFactura = numeroFactura;
            this.nombreCompleto = nombreCompleto;
            this.DNI = DNI;
            this.Direccion = direccion;
            this.CodigoPostal = codigopostal;
            this.Fecha = fecha;
            this.Observaciones = observaciones;
            reservasAsociadas.Add(reserva);
        }


        public void añadirApartamentoAsociado(int numReserva)
        {
            reservasAsociadas.Add(numReserva);
        }

        public void modificarDatosFactura(string[] datosFactura, List<Reserva> reservasAsociadas)
        {            
            // Mandar hacer el update de los datos introducidos
            ConexionBD conn = new ConexionBD();
            conn.actualizarFactura(datosFactura);

            string filePath = @"C:\Users\ivior\Desktop\facturas\Factura" + numeroFactura + ".pdf";
            string saveAs = carpetaFacturas + "Factura" + numeroFactura;
            File.Copy(plantillaNoModificable, plantilla);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                CreateWordDocument(plantilla, saveAs, datosFactura, reservasAsociadas);
                MessageBox.Show("Se ha sobreescrito la factura número " + numeroFactura + " satisfactoriamente.", "Aviso");
            }
            else
            {
                CreateWordDocument(plantilla, saveAs, datosFactura, reservasAsociadas);
            }

            File.Delete(plantilla);
        }

        private void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApp, object toFindText, object replaceWithText)
        {
            object matchCase = true;

            object matchwholeWord = true;

            object matchwildCards = false;

            object matchSoundLike = false;

            object nmatchAllforms = false;

            object forward = true;

            object format = false;

            object matchKashida = false;

            object matchDiactitics = false;

            object matchAlefHamza = false;

            object matchControl = false;

            object read_only = false;

            object visible = true;

            object replace = -2;

            object wrap = 1;

            wordApp.Selection.Find.Execute(ref toFindText, ref matchCase,
                                            ref matchwholeWord, ref matchwildCards, ref matchSoundLike,

                                            ref nmatchAllforms, ref forward,

                                            ref wrap, ref format, ref replaceWithText,

                                                ref replace, ref matchKashida,

                                            ref matchDiactitics, ref matchAlefHamza,

                                             ref matchControl);
        }

        private void CreateWordDocument(object filename, object SaveAs, string[] datosFactura, List<Reserva> reservasAsociadas)
        {            
            string[] datosReplace = { "nombreUsuario", "dniUsuario", "calleUsuario", "codpostalUsuario", "fechaFact", "checkin", "checkout",
            "apartamento","personas",null, "observacion", null, "NumFactura"};
            string[] datosReplacePrecios = { "precioBase", "descuen", "10p", "21p", "Suma", "Importe"};


            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            object missing = System.Reflection.Missing.Value;

            Microsoft.Office.Interop.Word.Document myWordDoc = null;


            if (File.Exists((string)filename))
            {
                object readOnly = false;
                object isvisible = false;
                wordApp.Visible = false;
                myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                                    ref missing, ref missing, ref missing,
                                                    ref missing, ref missing, ref missing,
                                                    ref missing, ref missing, ref missing,
                                                     ref missing, ref missing, ref missing, ref missing);
                myWordDoc.Activate();

                // Cambia las palabras por los datos introducidos
                                
                for (int i = 0; i< datosReplace.Length; i++)
                {
                    if(i == 7 && reservasAsociadas != null)
                    {
                        FindAndReplace(wordApp, datosReplace[i], "Múltiples");
                    }
                    else
                    {
                        FindAndReplace(wordApp, datosReplace[i], datosFactura[i]);
                    }                    
                }

                // datos checkin-checkout y cargo por líneas
                if (reservasAsociadas == null)
                {
                    // Si tiene una reserva
                    string Linea = datosFactura[5] + " - " + datosFactura[6] + "\t" + datosFactura[7] + "\t\t\t\t1" + "\t\t0,00€\t" + datosFactura[9] + "\t    21%        ";

                    FindAndReplace(wordApp, "siguienteLinea", Linea);
                }
                else
                {
                    // Multiples reservas asociadas
                    string Linea = "";
                    string[] apartamentos = datosFactura[7].Split(',');

                    // las lineas
                    for (int i = 0; i < reservasAsociadas.Count; i++)
                    {
                        Linea += reservasAsociadas.ElementAt(i).GetSetCheckin.ToString("yyyy-MMM-dd") + " - " + reservasAsociadas.ElementAt(i).GetSetCheckout.ToString("yyyy-MMM-dd") + 
                            "\t" + apartamentos[i] + "\t\t\t\t1" + "\t\t0,00€\t" + reservasAsociadas.ElementAt(i).GetSetImporte.ToString() + "\t    21%        ";                        
                    }
                    FindAndReplace(wordApp, "siguienteLinea", Linea);
                }
                


                // modificar datos precios
                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ",";

                string[] datosAReplazarPrecios = { (float.Parse(datosFactura[9], culture) * 0.79).ToString("0.00"), "", "0,00", 
                    (float.Parse(datosFactura[9], culture) * 0.21).ToString("0.00"), (float.Parse(datosFactura[9], culture) * 0.21).ToString("0.00"), 
                    float.Parse(datosFactura[9], culture).ToString("0.00") };

                for (int i = 0; i < datosReplacePrecios.Length; i++)
                {
                    FindAndReplace(wordApp, datosReplacePrecios[i], datosAReplazarPrecios[i]);
                }


                try
                {
                    // guardarlo como word docx
                    /*myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                                                                ref missing, ref missing, ref missing,
                                                                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                                                                ref missing, ref missing, ref missing);*/

                    // guardarlo como pdf
                    myWordDoc.ExportAsFixedFormat(SaveAs.ToString(), WdExportFormat.wdExportFormatPDF, false, WdExportOptimizeFor.wdExportOptimizeForOnScreen,
                                WdExportRange.wdExportAllDocument, 1, 1, WdExportItem.wdExportDocumentContent, true, true,
                                WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, ref missing);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }               

                myWordDoc.Close();
                wordApp.Quit();
            }
        }
        
        public void asignarNuevasReservas(List<Reserva> reservasAsociadas, string[] datosFactura)
        {
            string filePath = @"C:\Users\ivior\Desktop\facturas\Factura" + numeroFactura + ".pdf";
            string saveAs = carpetaFacturas + "Factura" + numeroFactura;
            File.Copy(plantillaNoModificable, plantilla);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                CreateWordDocument(plantilla, saveAs, datosFactura, reservasAsociadas);
                MessageBox.Show("Se ha sobreescrito la factura número " + numeroFactura + " satisfactoriamente.", "Aviso");
            }
            else
            {
                CreateWordDocument(plantilla, saveAs, datosFactura, reservasAsociadas);
            }

            File.Delete(plantilla);
        }

    }
}
