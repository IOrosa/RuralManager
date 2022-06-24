using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace RuralManager
{
    public partial class panelPrincipal : Form
    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );


        // Variables privadas de la clase principal del programa

        private List<Label> listaDias = new List<Label>();
        private List<Label> listaMeses = new List<Label>();
        private List<Panel> panelesApartamentos = new List<Panel>();

        // Lista de reservas, apartamentos y tarifas
        private List<Reserva> Reservas = new List<Reserva>();
        private List<Apartamento> Apartamentos = new List<Apartamento>();
        private List<Factura> Facturas = new List<Factura>();

        // Lista de los paneles que en el momento se muestran en pantalla y son clicables
        private List<BotonReserva> botonesReservas = new List<BotonReserva>();
        private List<Panel> panelesBlancos = new List<Panel>();
        private DateTime timePrimeraCaja;

        public panelPrincipal()
        {
            // Para poner los bordes de la ventana principal redondos y cambiar el color.
            // PnlNav es la barra azul claro que se pone al seleccionar las secciones principales del programa
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            indicadorSeleccion.Height = btnCalendario.Height;
            indicadorSeleccion.Top = btnCalendario.Top;
            indicadorSeleccion.Left = btnCalendario.Left;
            btnCalendario.BackColor = Color.FromArgb(46, 51, 73);
            panelCalendario.Size = new Size(1348, 679);
            close.BringToFront();
            btnCalendario.PerformClick();

            panelInfoReserva.Hide();
            panelBuscadorReservas.Hide();
            panelFacturas.Hide();
            panelTarifas.Hide();
            panelPromociones.Hide();
            inicializarTabla();
            aniadirApartamentos();
                       

            // Obtener todas las reservas de la base de datos y imprimirlas en el calendario
            actualizarReservas();
            imprimirCasillasEnBlanco();

            // IDEA: Los fines de semana cambiar el color azul a otro mas oscuro para marcar los fines de semana

        }


        //
        // Listeners de click de botones menú principal
        //

        private void btnCalendario_Click(object sender, EventArgs e)
        {
            btnCalendario.BackColor = Color.FromArgb(46, 51, 73);
            panelCalendario.Size = new Size(1348, 679);
            btnVueltaCalendario_Click(null, null);
            mostrarPanel(panelCalendario, btnCalendario);            
        }

        private void btnReservas_Click(object sender, EventArgs e)
        {
            btnCalendario.BackColor = Color.FromArgb(24, 30, 54);
            mostrarPanel(panelBuscadorReservas, btnReservas);
            actualizarReservas();
            mostrarBuscadorReservas(true);

            btnReservas.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnTarifas_Click(object sender, EventArgs e)
        {
            mostrarPanel(panelTarifas, btnTarifas);
            mostrarTarifas(true);
        }

        private void btnFacturas_Click(object sender, EventArgs e)
        {
            mostrarPanel(panelFacturas, btnFacturas);
            mostrarFacturas(true);                      
        }

        private void btnPromociones_Click(object sender, EventArgs e)
        {
            btnCalendario.BackColor = Color.FromArgb(24, 30, 54);
            mostrarPanel(panelPromociones, btnPromociones);
            actualizarReservas();
            mostrarPromociones(true);

            btnPromociones.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void close_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void mostrarPanel(Panel panel, Button boton)
        {
            // Llamar a esta función en el click del boton del panel
            // Mueve el panel a mostrar delante con bringtofront (panel.bringtofront)
            panelCalendario.Hide();
            panelBuscadorReservas.Hide();
            panelFacturas.Hide();
            panelTarifas.Hide();
            panelPromociones.Hide();

            mostrarBuscadorReservas(false);
            mostrarTarifas(false);
            mostrarFacturas(false);
            mostrarPromociones(false);

            panel.Show();
            panel.BringToFront();

            // Eliminar los leave, haciendo un if que compruebe cual es el boton clicado y ponga los otros de color azul
            btnCalendario.BackColor = Color.FromArgb(24, 30, 54);
            btnReservas.BackColor = Color.FromArgb(24, 30, 54);
            btnTarifas.BackColor = Color.FromArgb(24, 30, 54);
            btnFacturas.BackColor = Color.FromArgb(24, 30, 54);
            btnPromociones.BackColor = Color.FromArgb(24, 30, 54);

            // mover el indicadorseleccion a los valores de panel
            indicadorSeleccion.Height = boton.Height;
            indicadorSeleccion.Top = boton.Top;
            indicadorSeleccion.Left = boton.Left;
            boton.BackColor = Color.FromArgb(46, 51, 73);
            indicadorSeleccion.BringToFront();
            close.BringToFront();
        }

        //
        // Funciones botones flechas calendario
        //

        private void flechaIzquierda_Click(object sender, EventArgs e)
        {
            DateTime diaPrimero = timePrimeraCaja.AddDays(-15);

            for (int i = 0; i < 15; i++)
            {
                listaDias[i].Text = "" + (diaPrimero.AddDays(i).Day);
                listaMeses[i].Text = ObtenerMes(diaPrimero.AddDays(i).Month);
            }

            timePrimeraCaja = diaPrimero;

            tablaCalendario.Hide();
            limpiarPanelesBlancos();
            imprimirReservas();
            imprimirCasillasEnBlanco();
            tablaCalendario.Show();

            panelMesAnio.Text = ObtenerMes(timePrimeraCaja.Month) + " " + timePrimeraCaja.Year;
        }

        private void flechaDerecha_Click(object sender, EventArgs e)
        {
            DateTime diaPrimero = timePrimeraCaja.AddDays(+15);

            for (int i = 0; i < 15; i++)
            {
                listaDias[i].Text = "" + (diaPrimero.AddDays(i).Day);
                listaMeses[i].Text = ObtenerMes(diaPrimero.AddDays(i).Month);
            }

            timePrimeraCaja = diaPrimero;

            tablaCalendario.Hide();
            limpiarPanelesBlancos();
            imprimirReservas();
            imprimirCasillasEnBlanco();
            tablaCalendario.Show();

            panelMesAnio.Text = ObtenerMes(timePrimeraCaja.Month) + " " + timePrimeraCaja.Year;
        }

        //
        // Funciones para iniciar la aplicación
        //

        private void actualizarReservas()
        {
            ConexionBD conn = new ConexionBD();
            Reservas = conn.obtenerReservas();
            
            imprimirReservas();
        }

        private void inicializarTabla()
        {
            //Obtener dia y mes actuales para rellenar la primera fila de la tabla con los dias
            int anioActual = DateTime.Today.Year;
            int mesActual = DateTime.Today.Month;
            string diaActual = DateTime.Today.Day.ToString();

            timePrimeraCaja = DateTime.Today;

            // conseguir el número de días del mes
            int diasEnElMes = DateTime.DaysInMonth(anioActual, mesActual);


            tablaCalendario.Size = new Size(1330, 505);

            //Añado los textbox de los días para poder modificarlos más facilmente

            listaDias.Add(cajaDia1);
            listaDias.Add(cajaDia2);
            listaDias.Add(cajaDia3);
            listaDias.Add(cajaDia4);
            listaDias.Add(cajaDia5);
            listaDias.Add(cajaDia6);
            listaDias.Add(cajaDia7);
            listaDias.Add(cajaDia8);
            listaDias.Add(cajaDia9);
            listaDias.Add(cajaDia10);
            listaDias.Add(cajaDia11);
            listaDias.Add(cajaDia12);
            listaDias.Add(cajaDia13);
            listaDias.Add(cajaDia14);
            listaDias.Add(cajaDia15);

            listaMeses.Add(cajaMes1);
            listaMeses.Add(cajaMes2);
            listaMeses.Add(cajaMes3);
            listaMeses.Add(cajaMes4);
            listaMeses.Add(cajaMes5);
            listaMeses.Add(cajaMes6);
            listaMeses.Add(cajaMes7);
            listaMeses.Add(cajaMes8);
            listaMeses.Add(cajaMes9);
            listaMeses.Add(cajaMes10);
            listaMeses.Add(cajaMes11);
            listaMeses.Add(cajaMes12);
            listaMeses.Add(cajaMes13);
            listaMeses.Add(cajaMes14);
            listaMeses.Add(cajaMes15);

            // Bucle asignar días a los paneles de día a partir del actual
            int dia = Int32.Parse(diaActual);
            int diaMesSiguiente = 1, j = 0;

            for(int i=0; i < 15; i++)
            {
                if ((dia + i) <= diasEnElMes)
                {
                    listaDias[i].Text = "" + (dia + i);
                    listaMeses[i].Text = ObtenerMes(mesActual);
                }
                else
                {
                    listaDias[i].Text = "" + (diaMesSiguiente + j);
                    listaMeses[i].Text = ObtenerMes(mesActual + 1);
                    j++;
                }
            }

            // Cambiar el evento del calendario para pintar en rojo los fines de semana
            eventoPintarFinesDeSemana();

            panelMesAnio.Text = ObtenerMes(timePrimeraCaja.Month) + " " + timePrimeraCaja.Year;
        }

        private string ObtenerMes(int numMes)
        {
            string nombreMes = "";

            switch (numMes)
            {
                case 1:
                    nombreMes = "Enero";
                    break;
                case 2:
                    nombreMes = "Febrero";
                    break;
                case 3:
                    nombreMes = "Marzo";
                    break;
                case 4:
                    nombreMes = "Abril";
                    break;
                case 5:
                    nombreMes = "Mayo";
                    break;
                case 6:
                    nombreMes = "Junio";
                    break;
                case 7:
                    nombreMes = "Julio";
                    break;
                case 8:
                    nombreMes = "Agosto";
                    break;
                case 9:
                    nombreMes = "Septiembre";
                    break;
                case 10:
                    nombreMes = "Octubre";
                    break;
                case 11:
                    nombreMes = "Noviembre";
                    break;
                case 12:
                    nombreMes = "Diciembre";
                    break;

            }
            return nombreMes;
        }

        private void aniadirApartamentos()
        {
            ConexionBD conn = new ConexionBD();
            Apartamentos = conn.obtenerApartamentos();

            for (int i = 0; i < Apartamentos.Count; i++)
            {
                Panel panelApartamento = new Panel();
                TextBox textBoxApartamento = new TextBox();

                //Modelo el estilo de el texto
                textBoxApartamento.Text = Apartamentos.ElementAt(i).GetNombre;
                textBoxApartamento.BackColor = Color.Coral;
                textBoxApartamento.Width = 133;
                textBoxApartamento.Height = 30;
                textBoxApartamento.Location = new Point(4, 9);
                textBoxApartamento.Font = new Font("Nirmala UI", 11.25f, FontStyle.Bold);
                textBoxApartamento.BorderStyle = BorderStyle.None;
                panelApartamento.Controls.Add(textBoxApartamento);

                //Modelo el estilo del panel
                panelApartamento.BackColor = Color.Coral;
                panelApartamento.Width = 133;
                panelApartamento.Height = 38;
                panelesApartamentos.Add(panelApartamento);
            }

            for (int i = 0; i < Apartamentos.Count; i++)
            {
                tablaCalendario.RowCount++;
                tablaCalendario.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
                tablaCalendario.Controls.Add(panelesApartamentos[i], 0, tablaCalendario.RowCount - 1);
            }
            tablaCalendario.RowCount++;
            tablaCalendario.RowStyles.Add(new RowStyle(SizeType.Absolute, 1F));
        }

        private void imprimirCasillasEnBlanco()
        {
            // Recorrer la tabla a partir del 1,1 y rellenarla de paneles en azul oscuro
            for (int i = 1; i < 16; i++)
            {
                for (int k = 1; k < Apartamentos.Count()+1; k++)
                {
                    // Si no hay ningun control en la casilla, imprimo un panel
                    if (tablaCalendario.GetControlFromPosition(i, k) == null)
                    {
                        Panel panelVacio = new Panel();
                        panelVacio.BackColor = Color.FromArgb(0, 0, 0, 0);
                        panelVacio.Size = new Size(1500, 44);

                        panelesBlancos.Add(panelVacio);

                        panelVacio.MouseDown += (sender, e) => {
                            int columna = tablaCalendario.GetPositionFromControl(panelVacio).Column;
                            int fila = tablaCalendario.GetPositionFromControl(panelVacio).Row;
                            int tamanioBoton = 1;
                            int[] datosReserva = new int[7];

                            // crear botón
                            BotonReserva btnReserva = new BotonReserva();
                            btnReserva.Text = "Nueva reserva";
                            btnReserva.Size = new Size(150,38);

                            btnReserva.Click += (sender, e) =>
                            {
                                tablaCalendario.Controls.Remove(tablaCalendario.GetControlFromPosition(columna, fila));

                                // 0: Apartamento, 1: AñoCIn, 2: MesCIn, 3: DiaCin, 4:AñoCOut, 5: MesCOut, 6: DiaCOut
                                DateTime diaCheckin = timePrimeraCaja.AddDays(tablaCalendario.GetPositionFromControl(panelVacio).Column - 1);
                                DateTime diaCheckout = diaCheckin.AddDays(tamanioBoton);

                                datosReserva[0] = (fila - 1);
                                datosReserva[1] = diaCheckin.Year;
                                datosReserva[2] = diaCheckin.Month;
                                datosReserva[3] = diaCheckin.Day;
                                datosReserva[4] = diaCheckout.Year;
                                datosReserva[5] = diaCheckout.Month;
                                datosReserva[6] = diaCheckout.Day;

                                crearReserva(datosReserva);
                            };
                            
                            tablaCalendario.SetColumnSpan(panelVacio, tamanioBoton);
                            tablaCalendario.GetControlFromPosition(columna, fila).Controls.Add(btnReserva);

                            // recorre los siguientes y crea los eventos
                            for (int j = columna + 1; j < 16; j++)
                            {
                                if (!(tablaCalendario.GetControlFromPosition(j, fila) is BotonReserva))
                                {
                                    try { 
                                        tablaCalendario.GetControlFromPosition(j, fila).MouseMove += (sender, e) =>
                                        {
                                            //MessageBox.Show("Estoy sobre el " + columna + ", "+ fila);
                                            // Borro el siguiente
                                            tablaCalendario.Controls.Remove(tablaCalendario.GetControlFromPosition(columna+1, fila));

                                            tamanioBoton += 1;
                                            columna += 1;
                                            btnReserva.Size = new Size((77*tamanioBoton), 38);
                                            tablaCalendario.SetColumnSpan(panelVacio, tamanioBoton);
                                        };
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                        };
                        
                        tablaCalendario.Controls.Add(panelVacio, i, k);
                    }
                }
            }
        }

        private void limpiarPanelesBlancos()
        {
            for (int j = 0; j < panelesBlancos.Count; j++)
            {
                tablaCalendario.Controls.Remove(panelesBlancos.ElementAt(j));
            }

            panelesBlancos.Clear();
        }

        private void imprimirReservas()
        {
            // Método para mostrar las reservas visibles en los días que aparecen en el calendario
            BotonReserva botonReserva;
            int columnaDia = 0, indice = 0;

            // borrar los botones que hay en el momento en la tabla
            for (int j = 0; j < botonesReservas.Count; j++)
            {
                tablaCalendario.Controls.Remove(botonesReservas.ElementAt(j));
            }
            
            botonesReservas.Clear();


            for (int i = 0; i < Reservas.Count; i++)
            {
                if ((Reservas.ElementAt(i).GetSetCheckin >= timePrimeraCaja && Reservas.ElementAt(i).GetSetCheckin < timePrimeraCaja.AddDays(+15)) || 
                    (Reservas.ElementAt(i).GetSetCheckout > timePrimeraCaja && Reservas.ElementAt(i).GetSetCheckout < timePrimeraCaja.AddDays(+15)))
                {
                    botonReserva = new BotonReserva();
                    botonReserva.Text = Reservas.ElementAt(i).GetSetNombre + " " + Reservas.ElementAt(i).GetSetApellidos;
                    botonReserva.GetSetID = Reservas.ElementAt(i).GetId;

                    if (Reservas.ElementAt(i).GetFactura != -1)
                    {
                        botonReserva.BackColor = Color.FromArgb(164, 157, 53);
                    }
                    else if(Reservas.ElementAt(i).GetPagado == true)
                    {
                        botonReserva.BackColor = Color.FromArgb(63, 156, 88);
                    }

                    // Crear ventana reserva con la información de la reserva al clicar en el botón
                    botonReserva.Click += (sender, e) => mostrarDatosReserva(sender, e);

                    botonesReservas.Add(botonReserva);

                    //buscar el día en el que empieza la reserva y guardarlo en columnaDia
                    for (int k = 0; k < listaDias.Count; k++)
                    {
                        if (Reservas.ElementAt(i).GetSetCheckin.Day == Int32.Parse(listaDias.ElementAt(k).Text))
                        {
                            columnaDia = k + 1;
                        }
                    }
                    // buscar el índice de reserva en la lista de reservas y guardarlo en indice
                    for(int l = 0; l < botonesReservas.Count; l++)
                    {
                        if (botonesReservas.ElementAt(l).GetSetID == Reservas.ElementAt(i).GetId)
                        {
                            indice = l;
                        }
                    }

                    // Poner la duración de la reserva
                    // Si el checkout es mayor que la ultima casilla, dibuja hasta la ultima casilla

                    if (Reservas.ElementAt(i).GetSetCheckout >= timePrimeraCaja.AddDays(+15))
                    {
                        tablaCalendario.Controls.Add(botonesReservas.ElementAt(indice), columnaDia,
                                Reservas.ElementAt(i).GetSetApartamento + 1);
                        tablaCalendario.SetColumnSpan(botonesReservas.ElementAt(indice),
                            (timePrimeraCaja.AddDays(+15) - Reservas.ElementAt(i).GetSetCheckin).Days);
                    }
                    // Si el checkin es menor que la primera casilla, dibuja desde la primera casilla
                    else if(Reservas.ElementAt(i).GetSetCheckin < timePrimeraCaja)
                    {
                        tablaCalendario.Controls.Add(botonesReservas.ElementAt(indice), 1,
                                Reservas.ElementAt(i).GetSetApartamento + 1);
                        tablaCalendario.SetColumnSpan(botonesReservas.ElementAt(indice),
                            (Reservas.ElementAt(i).GetSetCheckout - timePrimeraCaja).Days);
                    }
                    else
                    {
                        tablaCalendario.Controls.Add(botonesReservas.ElementAt(indice), columnaDia,
                                Reservas.ElementAt(i).GetSetApartamento + 1);
                        tablaCalendario.SetColumnSpan(botonesReservas.ElementAt(indice), 
                            (Reservas.ElementAt(i).GetSetCheckout - Reservas.ElementAt(i).GetSetCheckin).Days);
                    }                    
                }
            }
        }
                
        private void eventoPintarFinesDeSemana()
        {
            tablaCalendario.CellPaint += (sender, e) =>
            {            
                DateTime dia = timePrimeraCaja;

                for (int i = 0; i < 15; i++)
                {
                    // Domingo: 0, Lunes: 1, Martes: 2, Miercoles: 3, Jueves: 4, Viernes: 5, Sabado: 6
                    if (dia.DayOfWeek.ToString("d") == "6" || dia.DayOfWeek.ToString("d") == "0")
                    {
                        // la columna i+1 se pinta de rojo
                        if (e.Column == (i+1) && e.Row != 0)
                        {
                            var brush = new SolidBrush(Color.FromArgb(100, 144, 12, 63));
                            e.Graphics.FillRectangle(brush, e.CellBounds);
                        }                        
                    }
                    dia = dia.AddDays(1);
                }
            };            
        }

        private void btnVueltaCalendario_Click(object sender, EventArgs e)
        {
            panelInfoReserva.Controls.Clear();
            panelInfoReserva.Hide();

            actualizarReservas();
            limpiarPanelesBlancos();
            imprimirCasillasEnBlanco();

            tablaCalendario.Show();
            panelMesAnio.Show();
            flechaDerecha.Show();
            flechaIzquierda.Show();
            nuevaReserva.Show();
        }

        //
        // Funciones para mostrar ventana de reservas y sus datos
        //

        private List<textBoxRedonda> crearUIReserva()
        {
            string[] datos = { "Nombre", "Apellidos", "Telefono", "Código Postal", "E-mail", "Apartamento", "Personas" };
            // crear list de textbox con todos
            List<textBoxRedonda> textBoxDatosReserva = new List<textBoxRedonda>();

            textBoxDatosReserva.Clear();

            // Esconde los paneles que muestran el calendario
            tablaCalendario.Hide();
            panelMesAnio.Hide();
            flechaDerecha.Hide();
            flechaIzquierda.Hide();

            panelInfoReserva.Size = new Size(1368, 679);
            panelInfoReserva.Location = new Point(209, 12);
            panelInfoReserva.Controls.Add(btnVueltaCalendario);

            // etiqueta datos reserva
            Label labelReserva = new Label();
            labelReserva.Font = new Font("Segoe UI", 15.0f);
            labelReserva.ForeColor = Color.CornflowerBlue;
            labelReserva.Text = "Datos de la reserva: ";
            labelReserva.Size = new Size(300, 30);
            labelReserva.Location = new Point(50, 80);

            panelInfoReserva.Controls.Add(labelReserva);

            // línea naranja
            Panel lineaNaranja = new Panel();
            lineaNaranja.BackColor = Color.DarkOrange;
            lineaNaranja.Location = new Point(50, 115);
            lineaNaranja.Size = new Size(1050, 4);

            panelInfoReserva.Controls.Add(lineaNaranja);

            // Añadir primeras casillas: Nombre, Apellidos, etc..
            for (int i = 0; i < 7; i++)
            {                
                // Estilo textbox de datos (Nombre, telefono, etc..)
                textBoxRedonda textBoxNueva = new textBoxRedonda();
                textBoxNueva.BackColor = Color.RoyalBlue;
                textBoxNueva.Size = new Size(300, 10);
                textBoxNueva.BorderStyle = BorderStyle.None;
                textBoxNueva.Font = new Font("Segoe UI", 12.0f);
                textBoxNueva.ForeColor = Color.White;
                textBoxNueva.TextAlign = HorizontalAlignment.Center;

                textBoxDatosReserva.Add(textBoxNueva);
                // Si es el textbox de apartamentos, no se añade a la vista ya que luego se añade el boton desplegable
                // pero si que se añade el textbox para guardar el dato de apartamento seleccionado
                if(i != 5)
                {
                    panelInfoReserva.Controls.Add(textBoxDatosReserva.ElementAt(i));
                }                

                // Estilo labels de datos

                Label labelNueva = new Label();
                labelNueva.Font = new Font("Segoe UI", 11.0f);
                labelNueva.ForeColor = Color.CornflowerBlue;
                labelNueva.Text = datos[i];

                panelInfoReserva.Controls.Add(labelNueva);

                //Coloco la etiqueta y su cuadro
                if (i >= 5)
                {                    
                    labelNueva.Location = new Point(50, 190 + (i * 40));
                    textBoxDatosReserva.ElementAt(i).Location = new Point(150, 190 + (i * 40));
                }
                else
                {
                    labelNueva.Location = new Point(50, 150 + (i * 40));
                    textBoxDatosReserva.ElementAt(i).Location = new Point(150, 150 + (i * 40));
                }
            }

            // Añadir casillas de checkin y checkout

            // Etiquetas Checkin
            Label labelCheckin = new Label();
            labelCheckin.Font = new Font("Segoe UI", 11.0f);
            labelCheckin.ForeColor = Color.CornflowerBlue;
            labelCheckin.Text = "Check-in";
            labelCheckin.Location = new Point(50, 470);

            panelInfoReserva.Controls.Add(labelCheckin);

            textBoxRedonda textBoxCheckinDia = new textBoxRedonda();
            textBoxCheckinDia.BackColor = Color.Navy;
            textBoxCheckinDia.Size = new Size(50, 10);
            textBoxCheckinDia.BorderStyle = BorderStyle.None;
            textBoxCheckinDia.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckinDia.ForeColor = Color.White;
            textBoxCheckinDia.TextAlign = HorizontalAlignment.Center;
            textBoxCheckinDia.Location = new Point(150, 470);

            textBoxDatosReserva.Add(textBoxCheckinDia);
            panelInfoReserva.Controls.Add(textBoxCheckinDia);

            Label labelDiaCheckin = new Label();
            labelDiaCheckin.Font = new Font("Segoe UI", 11.0f);
            labelDiaCheckin.ForeColor = Color.CornflowerBlue;
            labelDiaCheckin.Text = "Dia";
            labelDiaCheckin.Size = new Size(35, 30);
            labelDiaCheckin.Location = new Point(200, 470);

            panelInfoReserva.Controls.Add(labelDiaCheckin);

            textBoxRedonda textBoxCheckinMes = new textBoxRedonda();
            textBoxCheckinMes.BackColor = Color.Navy;
            textBoxCheckinMes.Size = new Size(50, 10);
            textBoxCheckinMes.BorderStyle = BorderStyle.None;
            textBoxCheckinMes.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckinMes.ForeColor = Color.White;
            textBoxCheckinMes.TextAlign = HorizontalAlignment.Center;
            textBoxCheckinMes.Location = new Point(240, 470);

            textBoxDatosReserva.Add(textBoxCheckinMes);
            panelInfoReserva.Controls.Add(textBoxCheckinMes);

            Label labelMesCheckin = new Label();
            labelMesCheckin.Font = new Font("Segoe UI", 11.0f);
            labelMesCheckin.ForeColor = Color.CornflowerBlue;
            labelMesCheckin.Text = "Mes";
            labelMesCheckin.Size = new Size(40, 30);
            labelMesCheckin.Location = new Point(290, 470);

            panelInfoReserva.Controls.Add(labelMesCheckin);

            textBoxRedonda textBoxCheckinAnio = new textBoxRedonda();
            textBoxCheckinAnio.BackColor = Color.Navy;
            textBoxCheckinAnio.Size = new Size(80, 10);
            textBoxCheckinAnio.BorderStyle = BorderStyle.None;
            textBoxCheckinAnio.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckinAnio.ForeColor = Color.White;
            textBoxCheckinAnio.TextAlign = HorizontalAlignment.Center;
            textBoxCheckinAnio.Location = new Point(330, 470);

            textBoxDatosReserva.Add(textBoxCheckinAnio);
            panelInfoReserva.Controls.Add(textBoxCheckinAnio);

            Label labelAnioCheckin = new Label();
            labelAnioCheckin.Font = new Font("Segoe UI", 11.0f);
            labelAnioCheckin.ForeColor = Color.CornflowerBlue;
            labelAnioCheckin.Text = "Año";
            labelAnioCheckin.Size = new Size(40, 30);
            labelAnioCheckin.Location = new Point(410, 470);

            panelInfoReserva.Controls.Add(labelAnioCheckin);

            // Etiquetas Checkout
            Label labelCheckout = new Label();
            labelCheckout.Font = new Font("Segoe UI", 11.0f);
            labelCheckout.ForeColor = Color.CornflowerBlue;
            labelCheckout.Text = "Check-out";
            labelCheckout.Location = new Point(50, 510);

            panelInfoReserva.Controls.Add(labelCheckout);

            textBoxRedonda textBoxCheckoutDia = new textBoxRedonda();
            textBoxCheckoutDia.BackColor = Color.Navy;
            textBoxCheckoutDia.Size = new Size(50, 10);
            textBoxCheckoutDia.BorderStyle = BorderStyle.None;
            textBoxCheckoutDia.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckoutDia.ForeColor = Color.White;
            textBoxCheckoutDia.TextAlign = HorizontalAlignment.Center;
            textBoxCheckoutDia.Location = new Point(150, 510);

            textBoxDatosReserva.Add(textBoxCheckoutDia);
            panelInfoReserva.Controls.Add(textBoxCheckoutDia);

            Label labelDiaCheckout = new Label();
            labelDiaCheckout.Font = new Font("Segoe UI", 11.0f);
            labelDiaCheckout.ForeColor = Color.CornflowerBlue;
            labelDiaCheckout.Text = "Dia";
            labelDiaCheckout.Size = new Size(35, 30);
            labelDiaCheckout.Location = new Point(200, 510);

            panelInfoReserva.Controls.Add(labelDiaCheckout);

            textBoxRedonda textBoxCheckoutMes = new textBoxRedonda();
            textBoxCheckoutMes.BackColor = Color.Navy;
            textBoxCheckoutMes.Size = new Size(50, 10);
            textBoxCheckoutMes.BorderStyle = BorderStyle.None;
            textBoxCheckoutMes.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckoutMes.ForeColor = Color.White;
            textBoxCheckoutMes.TextAlign = HorizontalAlignment.Center;
            textBoxCheckoutMes.Location = new Point(240, 510);

            textBoxDatosReserva.Add(textBoxCheckoutMes);
            panelInfoReserva.Controls.Add(textBoxCheckoutMes);

            Label labelMesCheckout = new Label();
            labelMesCheckout.Font = new Font("Segoe UI", 11.0f);
            labelMesCheckout.ForeColor = Color.CornflowerBlue;
            labelMesCheckout.Text = "Mes";
            labelMesCheckout.Size = new Size(40, 30);
            labelMesCheckout.Location = new Point(290, 510);

            panelInfoReserva.Controls.Add(labelMesCheckout);

            textBoxRedonda textBoxCheckoutAnio = new textBoxRedonda();
            textBoxCheckoutAnio.BackColor = Color.Navy;
            textBoxCheckoutAnio.Size = new Size(80, 10);
            textBoxCheckoutAnio.BorderStyle = BorderStyle.None;
            textBoxCheckoutAnio.Font = new Font("Segoe UI", 12.0f);
            textBoxCheckoutAnio.ForeColor = Color.White;
            textBoxCheckoutAnio.TextAlign = HorizontalAlignment.Center;
            textBoxCheckoutAnio.Location = new Point(330, 510);

            textBoxDatosReserva.Add(textBoxCheckoutAnio);
            panelInfoReserva.Controls.Add(textBoxCheckoutAnio);

            Label labelAnioCheckout = new Label();
            labelAnioCheckout.Font = new Font("Segoe UI", 11.0f);
            labelAnioCheckout.ForeColor = Color.CornflowerBlue;
            labelAnioCheckout.Text = "Año";
            labelAnioCheckout.Size = new Size(40, 30);
            labelAnioCheckout.Location = new Point(410, 510);

            panelInfoReserva.Controls.Add(labelAnioCheckout);

            
            // casilla de importe y botón de cambio de estado
            Label labelImporte = new Label();
            labelImporte.Font = new Font("Segoe UI", 12.0f);
            labelImporte.ForeColor = Color.CornflowerBlue;
            labelImporte.Text = "Importe";
            labelImporte.Location = new Point(600, 420);

            panelInfoReserva.Controls.Add(labelImporte);

            textBoxRedonda textBoxImporte = new textBoxRedonda();
            textBoxImporte.BackColor = Color.RoyalBlue;
            textBoxImporte.Size = new Size(150, 10);
            textBoxImporte.BorderStyle = BorderStyle.None;
            textBoxImporte.Font = new Font("Segoe UI", 24.0f);
            textBoxImporte.ForeColor = Color.White;
            textBoxImporte.TextAlign = HorizontalAlignment.Center;
            textBoxImporte.Location = new Point(700, 420);

            textBoxDatosReserva.Add(textBoxImporte);
            panelInfoReserva.Controls.Add(textBoxImporte);

            Label labelEuro = new Label();
            labelEuro.Font = new Font("Segoe UI", 24.0f);
            labelEuro.ForeColor = Color.CornflowerBlue;
            labelEuro.Text = "€";
            labelEuro.Size = new Size(50,50);
            labelEuro.Location = new Point(850, 420);

            panelInfoReserva.Controls.Add(labelEuro);
                                               
            // etiquetas y casillas de tarjeta de credito
            textBoxRedonda textBoxTarjeta = new textBoxRedonda();
            textBoxTarjeta.BackColor = Color.RoyalBlue;
            textBoxTarjeta.Size = new Size(300, 30);
            textBoxTarjeta.BorderStyle = BorderStyle.None;
            textBoxTarjeta.Font = new Font("Segoe UI", 12.0f);
            textBoxTarjeta.ForeColor = Color.White;
            textBoxTarjeta.TextAlign = HorizontalAlignment.Center;
            textBoxTarjeta.Location = new Point(750, 480);

            textBoxDatosReserva.Add(textBoxTarjeta);
            panelInfoReserva.Controls.Add(textBoxTarjeta);

            Label labelTarjeta = new Label();
            labelTarjeta.Font = new Font("Segoe UI", 12.0f);
            labelTarjeta.ForeColor = Color.CornflowerBlue;
            labelTarjeta.Text = "Número de tarjeta";
            labelTarjeta.Size = new Size(180, 30);
            labelTarjeta.Location = new Point(600, 480);

            panelInfoReserva.Controls.Add(labelTarjeta);

            textBoxRedonda textBoxFecha = new textBoxRedonda();
            textBoxFecha.BackColor = Color.RoyalBlue;
            textBoxFecha.Size = new Size(300, 30);
            textBoxFecha.BorderStyle = BorderStyle.None;
            textBoxFecha.Font = new Font("Segoe UI", 12.0f);
            textBoxFecha.ForeColor = Color.White;
            textBoxFecha.TextAlign = HorizontalAlignment.Center;
            textBoxFecha.Location = new Point(750, 520);

            textBoxDatosReserva.Add(textBoxFecha);
            panelInfoReserva.Controls.Add(textBoxFecha);

            Label labelFecha = new Label();
            labelFecha.Font = new Font("Segoe UI", 12.0f);
            labelFecha.ForeColor = Color.CornflowerBlue;
            labelFecha.Text = "Fecha caducidad";
            labelFecha.Size = new Size(180, 30);
            labelFecha.Location = new Point(600, 520);

            panelInfoReserva.Controls.Add(labelFecha);


            panelInfoReserva.Show();

            // Devuelve lista con todas las textbox añadidas
            //
         

            return textBoxDatosReserva;
        }

        private List<BotonReserva> crearUIReservaBotones()
        {
            List<BotonReserva> botones = new List<BotonReserva>();
                        
            BotonReserva btnCambioEstado = new BotonReserva();
            btnCambioEstado.Text = "Cambiar estado";
            btnCambioEstado.Size = new Size(140, 20);
            btnCambioEstado.Location = new Point(900, 450);

            botones.Add(btnCambioEstado);
            panelInfoReserva.Controls.Add(btnCambioEstado);

            BotonReserva btnGuardarDatos = new BotonReserva();
            btnGuardarDatos.Text = "Guardar cambios";
            btnGuardarDatos.Size = new Size(200, 80);
            btnGuardarDatos.Location = new Point(1100, 420);

            botones.Add(btnGuardarDatos);
            panelInfoReserva.Controls.Add(btnGuardarDatos);

            BotonReserva btnApartamento = new BotonReserva();
            btnApartamento.Location = new Point(150, 390);
            btnApartamento.BackColor = Color.RoyalBlue;
            btnApartamento.Size = new Size(300, 28);
            btnApartamento.Font = new Font("Segoe UI", 12.0f);
            btnApartamento.ForeColor = Color.White;
            btnApartamento.TextAlign = ContentAlignment.TopCenter;

            botones.Add(btnApartamento);
            panelInfoReserva.Controls.Add(btnApartamento);

            BotonReserva btnCalcularImporteAuto = new BotonReserva();
            btnCalcularImporteAuto.Location = new Point(600, 450);
            btnCalcularImporteAuto.Size = new Size(100, 25);
            btnCalcularImporteAuto.Text = "Importe tarifa";
            btnCalcularImporteAuto.Font = new Font("Segoe UI", 8.0f);
            btnCalcularImporteAuto.ForeColor = Color.White;
            btnCalcularImporteAuto.TextAlign = ContentAlignment.TopCenter;

            botones.Add(btnCalcularImporteAuto);
            panelInfoReserva.Controls.Add(btnCalcularImporteAuto);


            return botones;
        }

        private RichTextBox crearUIReservaNotas()
        {
            Label labelNotas = new Label();
            labelNotas.Font = new Font("Segoe UI", 15.0f);
            labelNotas.ForeColor = Color.CornflowerBlue;
            labelNotas.Text = "Notas";
            labelNotas.Location = new Point(600, 120);

            panelInfoReserva.Controls.Add(labelNotas);

            RichTextBox textBoxNotas = new RichTextBox();
            textBoxNotas.BackColor = Color.SlateGray;
            textBoxNotas.Size = new Size(500, 240);
            textBoxNotas.BorderStyle = BorderStyle.None;
            textBoxNotas.Font = new Font("Segoe UI", 12.0f);
            textBoxNotas.ForeColor = Color.White;
            textBoxNotas.Location = new Point(600, 150);

            panelInfoReserva.Controls.Add(textBoxNotas);

            return textBoxNotas;
        }

        // Se usa para generar una factura y para modificar los datos de una factura existente
        // Si se da una reserva, se crea una nueva.
        // Si se da un numFactura se modifica la factura
        private Form crearUIFactura(Reserva reserva, int numFactura, List<Reserva> reservasAsociadas)
        {
            List<TextBox> textBoxes = new List<TextBox>();
            List<Label> labels = new List<Label>();

            Form ventanaFactura = new Form();
            ventanaFactura.Location = new Point(500, 500);
            ventanaFactura.Size = new Size(500, 800);
            ventanaFactura.BackColor = Color.DarkGray;
            ventanaFactura.Text = "Factura";
            ventanaFactura.StartPosition = FormStartPosition.Manual;
            ventanaFactura.Left = 700;
            ventanaFactura.Top = 200;

            string[] valoresLabel = { "Nombre completo", "DNI", "Dirección", "Código Postal", "Fecha de factura", "Check-in", "Check-out", "Apartamento", "Personas", "Precio",
                    "Observaciones", "Reserva asociada"};

            if (numFactura == -1)
            {
                string[] valoresTextBox = { "", "", "", reserva.GetCodigoPostal.ToString(), DateTime.Now.ToString("yyyy-MMM-dd"),
                reserva.GetSetCheckin.ToString("yyyy-MMM-dd"), reserva.GetSetCheckout.ToString("yyyy-MMM-dd"), Apartamentos.ElementAt(reserva.GetSetApartamento).GetNombre,
                reserva.GetSetPersonas.ToString(), reserva.GetSetImporte.ToString(), "", reserva.GetId.ToString()};

                for (int i = 0; i < 12; i++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Location = new Point(140, 40 + 40 * i);
                    textBox.Size = new Size(260, 50);
                    textBox.Text = valoresTextBox[i];
                    textBoxes.Add(textBox);
                    ventanaFactura.Controls.Add(textBoxes[i]);

                    Label label = new Label();
                    label.Location = new Point(10, 43 + 40 * i);
                    label.Size = new Size(120, 30);
                    label.Text = valoresLabel[i];
                    labels.Add(label);
                    ventanaFactura.Controls.Add(labels[i]);
                }
            }
            else
            {
                if(Facturas.ElementAt(numFactura-1).GetSetReservasAsociadas.Count == 1)
                {
                    int idReserva = Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas.ElementAt(0);

                    // Coger datos de la factura
                    string[] valoresTextBox = {  Facturas.ElementAt(numFactura-1).GetSetNombreCompleto, Facturas.ElementAt(numFactura-1).GetSetDNI, Facturas.ElementAt(numFactura-1).GetSetDireccion, 
                        Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetCodigoPostal.ToString(), Facturas.ElementAt(numFactura-1).GetSetFecha,
                        Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetCheckin.ToString("yyyy-MMM-dd"), Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetCheckout.ToString("yyyy-MMM-dd"), 
                        Apartamentos.ElementAt(Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetApartamento).GetNombre,
                        Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetPersonas.ToString(), Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetImporte.ToString(),
                        Facturas.ElementAt(numFactura-1).GetSetObservaciones, Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetId.ToString()};

                    for (int i = 0; i < 12; i++)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Location = new Point(140, 40 + 40 * i);
                        textBox.Size = new Size(260, 50);
                        textBox.Text = valoresTextBox[i];
                        textBoxes.Add(textBox);
                        ventanaFactura.Controls.Add(textBoxes[i]);

                        Label label = new Label();
                        label.Location = new Point(10, 43 + 40 * i);
                        label.Size = new Size(120, 30);
                        label.Text = valoresLabel[i];
                        labels.Add(label);
                        ventanaFactura.Controls.Add(labels[i]);
                    }
                }
                else
                {
                    DateTime[] checkinYCheckout = buscarCheckinYCheckoutFactura(Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas);
                    int numTotalPersonas = calcularPersonasTotales(Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas);
                    float importeTotal = calcularImporteTotal(Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas);
                    string apartamentosAsociados = concatenarApartamentos(Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas);

                    // está asociado a más de una reserva
                    // coger los datos de reservasAsociadas
                    string[] valoresTextBox = {  Facturas.ElementAt(numFactura-1).GetSetNombreCompleto, Facturas.ElementAt(numFactura-1).GetSetDNI, Facturas.ElementAt(numFactura-1).GetSetDireccion,
                        Facturas.ElementAt(numFactura-1).GetSetCodigoPostal.ToString(), Facturas.ElementAt(numFactura-1).GetSetFecha,
                        checkinYCheckout[0].ToString("yyyy-MMM-dd"), checkinYCheckout[1].ToString("yyyy-MMM-dd"), apartamentosAsociados, numTotalPersonas.ToString(), importeTotal.ToString(),
                        Facturas.ElementAt(numFactura-1).GetSetObservaciones, "Múltiples"};

                    for (int i = 0; i < 12; i++)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Location = new Point(140, 40 + 40 * i);
                        textBox.Size = new Size(260, 50);
                        textBox.Text = valoresTextBox[i];
                        textBoxes.Add(textBox);
                        ventanaFactura.Controls.Add(textBoxes[i]);

                        Label label = new Label();
                        label.Location = new Point(10, 43 + 40 * i);
                        label.Size = new Size(120, 30);
                        label.Text = valoresLabel[i];
                        labels.Add(label);
                        ventanaFactura.Controls.Add(labels[i]);
                    }
                }           
                                                
            }

            

            BotonReserva btnAceptar = new BotonReserva();
            btnAceptar.Text = "Generar factura";
            btnAceptar.Size = new Size(120, 30);
            btnAceptar.Location = new Point(170, 500);
            btnAceptar.BackColor = Color.Gray;
            btnAceptar.ForeColor = Color.Black;
            ventanaFactura.Controls.Add(btnAceptar);
            btnAceptar.Click += (sender, e) =>
            {
                // Recoger datos de las textboxes y comprobar que ninguna esté vacía.
                // Si alguna está vacía, cambia su color a rojo claro
                int comprobadorCasillasBlanco = 0;

                for (int i = 0; i < textBoxes.Count(); i++)
                {
                    if (textBoxes.ElementAt(i).Text == "" && i != 10)
                    {
                        textBoxes.ElementAt(i).BackColor = Color.IndianRed;
                        comprobadorCasillasBlanco = 1;
                    }
                    else
                    {
                        textBoxes.ElementAt(i).BackColor = Color.White;
                    }

                }

                if (comprobadorCasillasBlanco == 0)
                {
                    if(numFactura != -1)
                    {
                        // Modificar factura y volver a imprimir
                        string[] datosFactura = { textBoxes.ElementAt(0).Text, textBoxes.ElementAt(1).Text, textBoxes.ElementAt(2).Text, textBoxes.ElementAt(3).Text,
                        textBoxes.ElementAt(4).Text, textBoxes.ElementAt(5).Text, textBoxes.ElementAt(6).Text, textBoxes.ElementAt(7).Text, textBoxes.ElementAt(8).Text,
                        textBoxes.ElementAt(9).Text, textBoxes.ElementAt(10).Text, textBoxes.ElementAt(11).Text, numFactura.ToString()};


                        Facturas.ElementAt(numFactura - 1).modificarDatosFactura(datosFactura, reservasAsociadas);
                        actualizarFacturas();
                        dibujarFacturas(null, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1));

                        ventanaFactura.Close();
                    }
                    else
                    {
                        // Crear nueva factura
                        // 0: NombreCompleto, 1: DNI, 2: Direccion, 3: Codigo Postal, 4: fechaActual, 5: checkin, 6: checkout, 7: apartamento, 8: personas, 9: precio
                        // 10: observaciones, 11: ReservaAsociada, 12: NumSiguienteFactura (para imprimirla en el PDF)
                        ConexionBD conn = new ConexionBD();

                        string[] datosFactura = { textBoxes.ElementAt(0).Text, textBoxes.ElementAt(1).Text, textBoxes.ElementAt(2).Text, textBoxes.ElementAt(3).Text,
                        textBoxes.ElementAt(4).Text, textBoxes.ElementAt(5).Text, textBoxes.ElementAt(6).Text, textBoxes.ElementAt(7).Text, textBoxes.ElementAt(8).Text,
                        textBoxes.ElementAt(9).Text, textBoxes.ElementAt(10).Text, textBoxes.ElementAt(11).Text, conn.comprobarNFacturas()+1.ToString() };

                        Factura f = new Factura(datosFactura);

                        ventanaFactura.Close();
                    }                   
                }
                else
                {
                    MessageBox.Show("Debe rellenar todas las casillas para poder generar la factura.", "Aviso");
                }


            };

            return ventanaFactura;
        }

        private void nuevaReserva_Click(object sender, EventArgs e)
        {
            nuevaReserva.Hide();
            crearReserva(null);
        }

        private void crearReserva(int[] datosReservaClick)
        {
            List<textBoxRedonda> textBoxes = crearUIReserva();
            List<BotonReserva> botones = crearUIReservaBotones();
            RichTextBox notas = crearUIReservaNotas();
            string[] datosReserva = new string[15];
            bool aparecePagado = false;

            panelInfoReserva.BringToFront();
            panelInfoReserva.Controls.Add(textBoxNumReserva);
            textBoxNumReserva.Text = "Nueva reserva";           

            Label Pagado = new Label();
            Pagado.Font = new Font("Segoe UI", 12.0f);
            Pagado.Size = new Size(140, 25);
            Pagado.Text = "Pago realizado";
            Pagado.Location = new Point(900, 420);
            Pagado.ForeColor = Color.ForestGreen;

            TableLayoutPanel desplegableApartamentos = new TableLayoutPanel();
            desplegableApartamentos.Size = new Size(350, 900);
            desplegableApartamentos.MaximumSize = new Size(330, 200);
            desplegableApartamentos.AutoScroll = true;
            desplegableApartamentos.ColumnCount = 1;
            desplegableApartamentos.RowCount = Apartamentos.Count();
            desplegableApartamentos.Location = new Point(150, 390);

            for (int i = 0; i < Apartamentos.Count(); i++)
            {
                BotonReserva botonReserva = new BotonReserva();
                botonReserva.BackColor = Color.CornflowerBlue;
                botonReserva.Text = Apartamentos.ElementAt(i).GetNombre;
                botonReserva.Size = new Size(300, 25);

                botonReserva.Click += (sender, e) =>
                {
                    botones.ElementAt(2).Text = botonReserva.Text;
                    textBoxes.ElementAt(5).Text = botonReserva.Text;
                    panelInfoReserva.Controls.Remove(desplegableApartamentos);
                };
                desplegableApartamentos.Controls.Add(botonReserva, 0, i);
            }

            // 0: Apartamento, 1: AñoCIn, 2: MesCIn, 3: DiaCin, 4:AñoCOut, 5: MesCOut, 6: DiaCOut

            if (datosReservaClick != null)
            {
                // Apartamento
                botones.ElementAt(2).Text = Apartamentos.ElementAt(datosReservaClick[0]).GetNombre;
                textBoxes.ElementAt(5).Text = Apartamentos.ElementAt(datosReservaClick[0]).GetNombre;

                textBoxes.ElementAt(6).Text = Apartamentos.ElementAt(datosReservaClick[0]).GetSetCapacidadMin.ToString();
                textBoxes.ElementAt(7).Text = datosReservaClick[3].ToString();
                textBoxes.ElementAt(8).Text = datosReservaClick[2].ToString();
                textBoxes.ElementAt(9).Text = datosReservaClick[1].ToString();
                textBoxes.ElementAt(10).Text = datosReservaClick[6].ToString();
                textBoxes.ElementAt(11).Text = datosReservaClick[5].ToString();
                textBoxes.ElementAt(12).Text = datosReservaClick[4].ToString();

                // calcular importe
                int indiceApartamento = 0;
                DateTime Checkin = new DateTime(Int32.Parse(textBoxes.ElementAt(9).Text), Int32.Parse(textBoxes.ElementAt(8).Text), Int32.Parse(textBoxes.ElementAt(7).Text));
                DateTime Checkout = new DateTime(Int32.Parse(textBoxes.ElementAt(12).Text), Int32.Parse(textBoxes.ElementAt(11).Text), Int32.Parse(textBoxes.ElementAt(10).Text));

                for (int k = 0; k < Apartamentos.Count(); k++)
                {
                    if (Apartamentos.ElementAt(k).GetNombre == textBoxes.ElementAt(5).Text)
                    {
                        indiceApartamento = k;
                    }
                }

                // Calcular importe con tarifas
                ConexionBD conn = new ConexionBD();
                textBoxes.ElementAt(13).Text = conn.calcularImporteReserva(datosReservaClick[0], Checkin, Checkout,
                    Apartamentos.ElementAt(indiceApartamento).GetSetPrecioBase).ToString();
            }

            botones.ElementAt(2).Click += (sender, e) =>
            {
                panelInfoReserva.Controls.Add(desplegableApartamentos);
                desplegableApartamentos.BringToFront();
            };

            botones.ElementAt(0).Click += (sender, e) =>
            {
                if (aparecePagado == false)
                {
                    aparecePagado = true;
                    panelInfoReserva.Controls.Add(Pagado);
                }
                else
                {
                    aparecePagado = false;
                    panelInfoReserva.Controls.Remove(Pagado);
                }
            };

            botones.ElementAt(1).Click += (sender, e) =>
            {
                // Si clica en guardar datos
                if (MessageBox.Show("¿Está seguro de que desea realizar los cambios?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Apartamento, cambiar de nombre a numero de índice
                    int indiceApartamento = 0;

                    for (int k = 0; k < Apartamentos.Count(); k++)
                    {
                        if (Apartamentos.ElementAt(k).GetNombre == textBoxes.ElementAt(5).Text)
                        {
                            indiceApartamento = k;
                        }
                    }

                    if (textBoxes.ElementAt(0).Text != "" && textBoxes.ElementAt(2).Text != "" && textBoxes.ElementAt(5).Text != "" &&
                        textBoxes.ElementAt(6).Text != "" && textBoxes.ElementAt(7).Text != "" && textBoxes.ElementAt(8).Text != "" && textBoxes.ElementAt(9).Text != "" &&
                        textBoxes.ElementAt(10).Text != "" && textBoxes.ElementAt(11).Text != "" && textBoxes.ElementAt(12).Text != "")
                    {
                        DateTime Checkin = new DateTime(Int32.Parse(textBoxes.ElementAt(9).Text), Int32.Parse(textBoxes.ElementAt(8).Text), Int32.Parse(textBoxes.ElementAt(7).Text));
                        DateTime Checkout = new DateTime(Int32.Parse(textBoxes.ElementAt(12).Text), Int32.Parse(textBoxes.ElementAt(11).Text), Int32.Parse(textBoxes.ElementAt(10).Text));
                        if (comprobarDisponibilidadFechas(-1, Checkin, Checkout, indiceApartamento))
                        {
                            if (Checkout > Checkin)
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    if (i == 5)
                                    {
                                        datosReserva[i] = indiceApartamento.ToString();
                                    }
                                    else if (i == 7)
                                    {
                                        // si hay que meter el checkin
                                        datosReserva[i] = textBoxes.ElementAt(9).Text + "-" + textBoxes.ElementAt(8).Text + "-" + textBoxes.ElementAt(7).Text;
                                    }
                                    else if (i == 8)
                                    {
                                        // si hay que meter el checkout
                                        datosReserva[i] = textBoxes.ElementAt(12).Text + "-" + textBoxes.ElementAt(11).Text + "-" + textBoxes.ElementAt(10).Text;
                                    }
                                    else if (i == 9)
                                    {
                                        // Importe, tarjeta y fechacad
                                        datosReserva[i] = textBoxes.ElementAt(13).Text;
                                        datosReserva[10] = textBoxes.ElementAt(14).Text;
                                        datosReserva[11] = textBoxes.ElementAt(15).Text;
                                        i = 12;
                                    }
                                    else if (i == 12)
                                    {
                                        // Pagado
                                        datosReserva[i] = Convert.ToInt32(aparecePagado).ToString();                                        
                                    }
                                    else if (i == 13)
                                    {
                                        // Notas
                                        datosReserva[i] = notas.Text;
                                    }
                                    else
                                    {
                                        datosReserva[i] = textBoxes.ElementAt(i).Text;
                                    }

                                }

                                ConexionBD conn = new ConexionBD();
                                conn.crearReserva(datosReserva);
                            }
                            else
                            {
                                MessageBox.Show("El día de checkout no puede ser el mismo o mayor que el de checkin", "Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pueden realizar los cambios de fecha ya que solapan otra reserva en el mismo apartamento.", "Error");
                        }


                    }
                    else
                    {
                        MessageBox.Show("Asegúrese de rellenar los datos obligatorios: Nombre, Teléfono, Apartamento, Personas, Checkin, Checkout", "Error");
                    }
                }

                

            };

            botones.ElementAt(3).Click += (sender, e) =>
            {
                int indiceApartamento = 0;
                                
                DateTime Checkin = new DateTime(Int32.Parse(textBoxes.ElementAt(9).Text), Int32.Parse(textBoxes.ElementAt(8).Text), Int32.Parse(textBoxes.ElementAt(7).Text));
                DateTime Checkout = new DateTime(Int32.Parse(textBoxes.ElementAt(12).Text), Int32.Parse(textBoxes.ElementAt(11).Text), Int32.Parse(textBoxes.ElementAt(10).Text));

                for (int k = 0; k < Apartamentos.Count(); k++)
                {
                    if (Apartamentos.ElementAt(k).GetNombre == textBoxes.ElementAt(5).Text)
                    {
                        indiceApartamento = k;
                    }
                }

                // Calcular importe con tarifas
                ConexionBD conn = new ConexionBD();
                textBoxes.ElementAt(13).Text = conn.calcularImporteReserva(indiceApartamento, Checkin, Checkout, Apartamentos.ElementAt(indiceApartamento).GetSetPrecioBase).ToString();
            };

        }

        private void mostrarDatosReserva(object sender, EventArgs e)
        {
            List<textBoxRedonda> textBoxes = crearUIReserva();
            List<BotonReserva> botones = crearUIReservaBotones();
            RichTextBox notas = crearUIReservaNotas();
            int idReserva;

            try
            {
                idReserva = (sender as BotonReserva).GetSetID;
            }
            catch
            {
                // si se ha accedido desde el buscador de reservas
                idReserva = (sender as Reserva).GetId;
            }
            
            String[] datosReserva = Reservas.ElementAt(buscarIndiceReserva(idReserva)).getDatosReserva();            
            DateTime[] CheckinAndOut = Reservas.ElementAt(buscarIndiceReserva(idReserva)).getCheckInAndOut();
            bool aparecePagado = false;

            textBoxNumReserva.Text = "Nº Reserva: " + idReserva;
            panelInfoReserva.Controls.Add(textBoxNumReserva);
            panelInfoReserva.BringToFront(); 

            // Rellenar las textbox con los datos de la reserva
            for (int i = 0; i < 13; i++)
            {                
                // Comprobar si es de casillas de checkin
                if(i == 5)
                {
                    // Apartamento mostrar nombre 
                    textBoxes.ElementAt(i).Text = Apartamentos.ElementAt(Int32.Parse(datosReserva[i])).GetNombre;
                    i++;
                }
                if(i == 7)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[0].Day.ToString();
                }
                else if(i == 8)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[0].Month.ToString();
                }
                else if (i == 9)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[0].Year.ToString();
                }
                else if (i == 10)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[1].Day.ToString();
                }
                else if (i == 11)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[1].Month.ToString();
                }
                else if (i == 12)
                {
                    textBoxes.ElementAt(i).Text = CheckinAndOut[1].Year.ToString();
                    textBoxes.ElementAt(13).Text = datosReserva[9];
                    textBoxes.ElementAt(14).Text = datosReserva[10];
                    textBoxes.ElementAt(15).Text = datosReserva[11];
                }
                else
                {
                    textBoxes.ElementAt(i).Text = datosReserva[i];
                }
            }
            botones.ElementAt(2).Text = Apartamentos.ElementAt(Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetApartamento).GetNombre;
            notas.Text = Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetNotas;
            aparecePagado = Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetPagado;

            Label Pagado = new Label();
            Pagado.Font = new Font("Segoe UI", 12.0f);
            Pagado.Size = new Size(140, 25);
            Pagado.Text = "Pago realizado";
            Pagado.Location = new Point(900, 420);
            Pagado.ForeColor = Color.ForestGreen;

            
            TableLayoutPanel desplegableApartamentos = new TableLayoutPanel();
            desplegableApartamentos.Size = new Size(350, 900);
            desplegableApartamentos.MaximumSize = new Size(330, 200);
            desplegableApartamentos.AutoScroll = true;
            desplegableApartamentos.ColumnCount = 1;
            desplegableApartamentos.RowCount = Apartamentos.Count();
            desplegableApartamentos.Location = new Point(150, 390);

            for (int i = 0; i < Apartamentos.Count(); i++)
            {
                BotonReserva botonReserva = new BotonReserva();
                botonReserva.BackColor = Color.CornflowerBlue;
                botonReserva.Text = Apartamentos.ElementAt(i).GetNombre;
                botonReserva.Size = new Size(300, 25);

                botonReserva.Click += (sender, e) =>
                {
                    botones.ElementAt(2).Text = botonReserva.Text;
                    textBoxes.ElementAt(5).Text = botonReserva.Text;
                    panelInfoReserva.Controls.Remove(desplegableApartamentos);
                };
                desplegableApartamentos.Controls.Add(botonReserva, 0, i);
            }

            if (Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetPagado == true)
            {
                panelInfoReserva.Controls.Add(Pagado);
            }

            // Boton para desplegable apartamentos
            botones.ElementAt(2).Click += (sender, e) =>
            {
                panelInfoReserva.Controls.Add(desplegableApartamentos);
                desplegableApartamentos.BringToFront();
            };

            // Boton para cambiar estado pagado
            botones.ElementAt(0).Click += (sender, e) =>
            {
                if (aparecePagado == false)
                {
                    aparecePagado = true;
                    panelInfoReserva.Controls.Add(Pagado);
                }
                else
                {
                    aparecePagado = false;
                    panelInfoReserva.Controls.Remove(Pagado);
                }
            };

            // Boton guardar cambios
            botones.ElementAt(1).Click += (sender, e) =>
            {
                // Si clica en guardar datos
                if (MessageBox.Show("¿Está seguro de que desea realizar los cambios?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Apartamento, cambiar de nombre a numero de índice
                    int indiceApartamento = 0;

                    for (int k = 0; k < Apartamentos.Count(); k++)
                    {
                        if (Apartamentos.ElementAt(k).GetNombre == textBoxes.ElementAt(5).Text)
                        {
                            indiceApartamento = k;
                        }
                    }

                    if (textBoxes.ElementAt(0).Text != "" && textBoxes.ElementAt(2).Text != "" && textBoxes.ElementAt(5).Text != "" &&
                        textBoxes.ElementAt(6).Text != "" && textBoxes.ElementAt(7).Text != "" && textBoxes.ElementAt(8).Text != "" && textBoxes.ElementAt(9).Text != "" &&
                        textBoxes.ElementAt(10).Text != "" && textBoxes.ElementAt(11).Text != "" && textBoxes.ElementAt(12).Text != "")
                    {
                        DateTime Checkin = new DateTime(Int32.Parse(textBoxes.ElementAt(9).Text), Int32.Parse(textBoxes.ElementAt(8).Text), Int32.Parse(textBoxes.ElementAt(7).Text));
                        DateTime Checkout = new DateTime(Int32.Parse(textBoxes.ElementAt(12).Text), Int32.Parse(textBoxes.ElementAt(11).Text), Int32.Parse(textBoxes.ElementAt(10).Text));
                        if (comprobarDisponibilidadFechas(idReserva, Checkin, Checkout, indiceApartamento))
                        {
                            if (Checkout > Checkin)
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    if (i == 5)
                                    {
                                        datosReserva[i] = indiceApartamento.ToString();
                                    }
                                    else if (i == 7)
                                    {
                                        // si hay que meter el checkin
                                        datosReserva[i] = textBoxes.ElementAt(9).Text + "-" + textBoxes.ElementAt(8).Text + "-" + textBoxes.ElementAt(7).Text;
                                    }
                                    else if (i == 8)
                                    {
                                        // si hay que meter el checkout
                                        datosReserva[i] = textBoxes.ElementAt(12).Text + "-" + textBoxes.ElementAt(11).Text + "-" + textBoxes.ElementAt(10).Text;
                                    }
                                    else if (i == 9)
                                    {
                                        // Importe, tarjeta y fechacad
                                        datosReserva[i] = textBoxes.ElementAt(13).Text;
                                        datosReserva[10] = textBoxes.ElementAt(14).Text;
                                        datosReserva[11] = textBoxes.ElementAt(15).Text;
                                        i = 11;
                                    }
                                    else if (i == 12)
                                    {
                                        // Pagado
                                        datosReserva[i] = Convert.ToInt32(aparecePagado).ToString();
                                    }
                                    else if (i == 13)
                                    {
                                        // Notas
                                        datosReserva[i] = notas.Text;
                                    }
                                    else
                                    {
                                        datosReserva[i] = textBoxes.ElementAt(i).Text;
                                    }

                                }
                                ConexionBD conn = new ConexionBD();
                                conn.modificarReserva(datosReserva, idReserva);
                            }
                            else
                            {
                                MessageBox.Show("El día de checkout no puede ser el mismo o mayor que el de checkin", "Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pueden realizar los cambios de fecha ya que solapan otra reserva en el mismo apartamento.", "Error");
                        }


                    }
                    else
                    {
                        MessageBox.Show("Asegúrese de rellenar los datos obligatorios: Nombre, Teléfono, Apartamento, Personas, Checkin, Checkout", "Error");
                    }
                }



            };

            // Botón calcular importe automatico
            botones.ElementAt(3).Click += (sender, e) =>
            {
                int indiceApartamento = 0;
                DateTime Checkin = new DateTime(Int32.Parse(textBoxes.ElementAt(9).Text), Int32.Parse(textBoxes.ElementAt(8).Text), Int32.Parse(textBoxes.ElementAt(7).Text));
                DateTime Checkout = new DateTime(Int32.Parse(textBoxes.ElementAt(12).Text), Int32.Parse(textBoxes.ElementAt(11).Text), Int32.Parse(textBoxes.ElementAt(10).Text));

                for (int k = 0; k < Apartamentos.Count(); k++)
                {
                    if (Apartamentos.ElementAt(k).GetNombre == textBoxes.ElementAt(5).Text)
                    {
                        indiceApartamento = k;
                    }
                }

                // Calcular importe con tarifas
                ConexionBD conn = new ConexionBD();
                textBoxes.ElementAt(13).Text = conn.calcularImporteReserva(Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetSetApartamento, Checkin, Checkout,
                    Apartamentos.ElementAt(indiceApartamento).GetSetPrecioBase).ToString();
            };

            BotonReserva btnEliminarReserva = new BotonReserva();
            btnEliminarReserva.Text = "Eliminar reserva";
            btnEliminarReserva.Size = new Size(200, 40);
            btnEliminarReserva.Location = new Point(1100, 520);
            btnEliminarReserva.BackColor = Color.DarkRed;
            btnEliminarReserva.Click += (sender, e) =>
            {
                if (Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetFactura == -1)
                {
                    if (MessageBox.Show("Está seguro de que desea eliminar la reserva", "Eliminar reserva", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ConexionBD conn = new ConexionBD();
                        conn.eliminarReserva(idReserva);
                    }
                }
                else
                {
                    MessageBox.Show("No se puede eliminar una reserva ya facturada.");
                }
            };

            panelInfoReserva.Controls.Add(btnEliminarReserva);

            // Label factura existente
            Label facturaYaExiste = new Label();
            facturaYaExiste.Text = "Factura Nº " + Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetFactura;
            facturaYaExiste.Size = new Size(200, 40);
            facturaYaExiste.Font = new Font("Segoe UI", 20.0f);
            facturaYaExiste.ForeColor = Color.CornflowerBlue;
            facturaYaExiste.Location = new Point(1130, 150);

            if (Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetFactura == -1)
            {
                BotonReserva btnCrearFactura = new BotonReserva();
                btnCrearFactura.Text = "Crear factura";
                btnCrearFactura.Size = new Size(200, 40);
                btnCrearFactura.Location = new Point(1100, 150);
                btnCrearFactura.Click += (sender, e) =>
                {
                    actualizarReservas();
                    if (Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetFactura == -1)
                    {                        
                        if (Reservas.ElementAt(buscarIndiceReserva(idReserva)).GetPagado == true)
                        {
                            crearUIFactura(Reservas.ElementAt(buscarIndiceReserva(idReserva)), -1, null).Show();

                            botones.ElementAt(0).Hide();
                            //btnCrearFactura.Hide();
                        }
                        else
                        {
                            MessageBox.Show("No se puede crear la factura si la reserva no está pagada.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("La factura para esta reserva ya ha sido creada");
                    }
                    
                };

                panelInfoReserva.Controls.Add(btnCrearFactura);
            }
            else
            {
                botones.ElementAt(0).Hide();
                panelInfoReserva.Controls.Add(facturaYaExiste);
            }
            
        }

        private bool comprobarDisponibilidadFechas(int idReserva, DateTime CheckinNuevo, DateTime CheckoutNuevo, int apartamento)
        {
            // Hacer que se mande el apartamento nuevo si en la modificación se cambia de apartamento y lo hará bien
            int indice = -1;
            bool disponibilidad = true;

            if(idReserva != -1)
            {
                // Entonces se busca modificar una reserva existente                
                indice = buscarIndiceReserva(idReserva);
            }

            for(int i = 0; i < Reservas.Count(); i++)
            {
                // Comprobar si se puede realizar el cambio sin solapar otras reservas

                //Comprueba que la reserva a comparar sea del mismo apartamento y no sea la misma reserva
                // Y luego comprueba si esta completamente delante o atras de la reserva que está comparando y no solapa

                if (apartamento == Reservas.ElementAt(i).GetSetApartamento && indice != i )
                {
                    if (!((CheckoutNuevo <= Reservas.ElementAt(i).GetSetCheckin) ||
                       (CheckinNuevo >= Reservas.ElementAt(i).GetSetCheckout))) 
                    {                        
                        MessageBox.Show("La reserva que da conflicto es: " + Reservas.ElementAt(i).GetId + "\r\nQue dura desde " + Reservas.ElementAt(i).GetSetCheckin + " hasta " +
                            Reservas.ElementAt(i).GetSetCheckout);
                        disponibilidad = false;
                    }
                }                    
            }          

            return disponibilidad;
        }

        private void actualizarApartamentos()
        {
            ConexionBD conn = new ConexionBD();
            Apartamentos = conn.obtenerApartamentos();
        }

        private int buscarIndiceReserva(int idReserva)
        {
            int indice = 0;

            for (int k = 0; k < Reservas.Count; k++)
            {
                if (Reservas.ElementAt(k).GetId == idReserva)
                {
                    indice = k;
                }
            }

            return indice;
        }

        //
        // Funciones para mostrar ventana de búsqueda de reservas
        //

        public void crearUIBuscadorReservas()
        {
            Label labelReservas = new Label();
            labelReservas.Text = "Buscar reserva";
            labelReservas.Font = new Font("Segoe UI", 24.0f);
            labelReservas.ForeColor = Color.FromArgb(0, 126, 249);
            labelReservas.Location = new Point(20, 20);
            labelReservas.AutoSize = true;
            panelBuscadorReservas.Controls.Add(labelReservas);


            Label labelBuscar = new Label();
            labelBuscar.Text = "Buscar por nombre:";
            labelBuscar.Font = new Font("Segoe UI", 12.0f);
            labelBuscar.ForeColor = Color.CornflowerBlue;
            labelBuscar.Location = new Point(50, 80);
            labelBuscar.AutoSize = true;
            panelBuscadorReservas.Controls.Add(labelBuscar);

            Panel lineaNaranja = new Panel();
            lineaNaranja.BackColor = Color.DarkOrange;
            lineaNaranja.Location = new Point(50, 115);
            lineaNaranja.Size = new Size(800, 4);

            panelBuscadorReservas.Controls.Add(lineaNaranja);

            //return panelBuscadorReservas;

        }

        public void mostrarBuscadorReservas(bool mostrar)
        {
            if (mostrar == true)
            {
                panelBuscadorReservas.Size = new Size(1368, 679);
                panelBuscadorReservas.Location = new Point(195, 12);
                crearUIBuscadorReservas();

                // Añadir buscador
                textBoxRedonda buscador = new textBoxRedonda();
                buscador.BackColor = Color.RoyalBlue;
                buscador.Location = new Point(250, 80);
                buscador.Size = new Size(300, 10);
                buscador.Font = new Font("Segoe UI", 12.0f);
                buscador.ForeColor = Color.White;
                buscador.BorderStyle = BorderStyle.None;
                buscador.TextAlign = HorizontalAlignment.Center;
                panelBuscadorReservas.Controls.Add(buscador);

                // Boton actualizar
                BotonReserva actualizar = new BotonReserva();
                actualizar.BackColor = Color.RoyalBlue;
                actualizar.Location = new Point(650, 50);
                actualizar.Size = new Size(150, 30);
                actualizar.Text = "Actualizar";
                actualizar.Font = new Font("Segoe UI", 12.0f);
                actualizar.ForeColor = Color.White;
                panelBuscadorReservas.Controls.Add(actualizar);

                actualizar.Click += (sender, e) =>
                {
                    actualizarReservas();
                    // Para activar el cambio de texto y que se actualize la tabla
                    string texto = buscador.Text;
                    buscador.Text = "123456";
                    buscador.Text = texto;
                };

                // Añadir tabla de resultados
                TableLayoutPanel tablaResultados = new TableLayoutPanel();
                tablaResultados.Size = new Size(800, 500);
                tablaResultados.Location = new Point(50,130);
                tablaResultados.BackColor = Color.FromArgb(24, 30, 54);

                tablaResultados.BorderStyle = BorderStyle.FixedSingle;
                tablaResultados.ColumnCount = 1;
                tablaResultados.RowCount = 0;
                tablaResultados.AutoScroll = false;
                tablaResultados.HorizontalScroll.Enabled = false;
                tablaResultados.HorizontalScroll.Visible = false;
                tablaResultados.HorizontalScroll.Maximum = 0;
                tablaResultados.AutoScroll = true;

                buscador.TextChanged += (sender, e) =>
                {
                    tablaResultados.Controls.Clear();
                    tablaResultados.RowCount = 0;

                    for (int i = 0; i < Reservas.Count; i++)
                    {
                        string nombrecompleto = Reservas.ElementAt(i).GetSetNombre + " " + Reservas.ElementAt(i).GetSetApellidos;
                        if (nombrecompleto.Contains(buscador.Text))
                        {
                            Panel nuevaFila = crearNuevaFila(i, false);

                            tablaResultados.RowCount += 1;
                            tablaResultados.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
                            tablaResultados.Controls.Add(nuevaFila, 0, tablaResultados.RowCount - 1);
                        }
                    }

                };

                // Para activar el cambio de texto y que se actualize la tabla
                string texto = buscador.Text;
                buscador.Text = "123456";
                buscador.Text = texto;

                panelBuscadorReservas.Controls.Add(tablaResultados);

                panelBuscadorReservas.Show();
            }
            else
            {
                panelBuscadorReservas.Controls.Clear();
                panelBuscadorReservas.Hide();
            }          
        }

        // También se accede desde facturas
        public Panel crearNuevaFila(int indiceReserva, bool marcarAFacturar)
        {
            
            Panel nuevaFila = new Panel();
            nuevaFila.Size = new Size(750, 60);
            nuevaFila.BackColor = Color.FromArgb(150, 0, 126, 249);
            nuevaFila.BorderStyle = BorderStyle.FixedSingle;                     
            
            Label nombreReserva = new Label();
            nombreReserva.Font = new Font("Segoe UI", 12.0f);
            nombreReserva.Location = new Point(0, 0);
            nombreReserva.AutoSize = true;
            nombreReserva.ForeColor = Color.White;
            nombreReserva.BackColor = Color.FromArgb(0, 0, 126, 249);            
            nombreReserva.Text = Reservas.ElementAt(indiceReserva).GetSetNombre + " " + Reservas.ElementAt(indiceReserva).GetSetApellidos;            
            nuevaFila.Controls.Add(nombreReserva);

            Label Apartamento = new Label();
            Apartamento.Font = new Font("Segoe UI", 12.0f);
            Apartamento.Location = new Point(0, 35);
            Apartamento.AutoSize = true;
            Apartamento.ForeColor = Color.White;
            Apartamento.BackColor = Color.FromArgb(0, 0, 126, 249);
            Apartamento.Text = Apartamentos.ElementAt(Reservas.ElementAt(indiceReserva).GetSetApartamento).GetNombre;
            nuevaFila.Controls.Add(Apartamento);

            Label CheckinYOut = new Label();
            CheckinYOut.Font = new Font("Segoe UI", 12.0f);
            CheckinYOut.Location = new Point(520, 0);
            CheckinYOut.AutoSize = true;
            CheckinYOut.ForeColor = Color.White;
            CheckinYOut.BackColor = Color.FromArgb(0, 0, 126, 249);
            CheckinYOut.Text = Reservas.ElementAt(indiceReserva).GetSetCheckin.ToString("yyyy-MMM-dd") + " - " + Reservas.ElementAt(indiceReserva).GetSetCheckout.ToString("yyyy-MMM-dd");
            nuevaFila.Controls.Add(CheckinYOut);

            Label Importe = new Label();
            Importe.Font = new Font("Segoe UI", 12.0f);
            Importe.Location = new Point(650, 35);
            Importe.AutoSize = true;
            Importe.ForeColor = Color.White;
            Importe.BackColor = Color.FromArgb(0, 0, 126, 249);
            Importe.Text = Reservas.ElementAt(indiceReserva).GetSetImporte + " €";
            nuevaFila.Controls.Add(Importe);
                        

            if(marcarAFacturar == true)
            {
                // Únicamente es para recoger el ID del panel clicado en las facturas
                Button id = new Button();
                id.Text = indiceReserva.ToString();
                id.Location = new Point(-200,-200);
                nuevaFila.Controls.Add(id);

                nuevaFila.Size = new Size(400, 50);
                nombreReserva.Font = new Font("Segoe UI", 10.0f);
                Apartamento.Font = new Font("Segoe UI", 10.0f);
                Apartamento.Location = new Point(0, 25);
                CheckinYOut.Font = new Font("Segoe UI", 12.0f);
                CheckinYOut.Location = new Point(170, 0);
                Importe.Font = new Font("Segoe UI", 12.0f);
                Importe.Location = new Point(320, 25);

                nuevaFila.Click += clicarReserva;
                nombreReserva.Click += clicarReserva;
                Apartamento.Click += clicarReserva;
                CheckinYOut.Click += clicarReserva;
                Importe.Click += clicarReserva;

                void clicarReserva(object sender, EventArgs e)
                {
                    if (nuevaFila.BackColor == Color.FromArgb(150, 0, 126, 249))
                    {
                        nuevaFila.BackColor = Color.FromArgb(255, 0, 126, 249);
                    }
                    else
                    {
                        nuevaFila.BackColor = Color.FromArgb(150, 0, 126, 249);
                    }
                };

            }
            else
            {
                nuevaFila.Click += clicarReserva;
                nombreReserva.Click += clicarReserva;
                Apartamento.Click += clicarReserva;
                CheckinYOut.Click += clicarReserva;
                Importe.Click += clicarReserva;

                nuevaFila.MouseHover += mouseHoverReserva;
                nombreReserva.MouseHover += mouseHoverReserva;
                Apartamento.MouseHover += mouseHoverReserva;
                CheckinYOut.MouseHover += mouseHoverReserva;
                Importe.MouseHover += mouseHoverReserva;

                nuevaFila.MouseLeave += mouseLeaveReserva;
                nombreReserva.MouseLeave += mouseLeaveReserva;
                Apartamento.MouseLeave += mouseLeaveReserva;
                CheckinYOut.MouseLeave += mouseLeaveReserva;
                Importe.MouseLeave += mouseLeaveReserva;

                void clicarReserva(object sender, EventArgs e)
                {
                    mostrarDatosReserva(Reservas.ElementAt(indiceReserva), e);
                };

                void mouseHoverReserva(object sender, EventArgs e)
                {
                    nuevaFila.BackColor = Color.FromArgb(255, 0, 126, 249);
                };

                void mouseLeaveReserva(object sender, EventArgs e)
                {
                    nuevaFila.BackColor = Color.FromArgb(150, 0, 126, 249);
                };
            }

            
                        

            return nuevaFila;
        }

        //
        // Funciones para mostrar ventana de tarifas
        //

        public void crearUITarifas()
        {
            Label labelTarifas = new Label();
            labelTarifas.Text = "Personalizar tarifas";
            labelTarifas.Font = new Font("Segoe UI", 24.0f);
            labelTarifas.ForeColor = Color.FromArgb(0, 126, 249);
            labelTarifas.Location = new Point(20, 20);
            labelTarifas.AutoSize = true;
            panelTarifas.Controls.Add(labelTarifas);

            Panel lineaNaranja = new Panel();
            lineaNaranja.BackColor = Color.DarkOrange;
            lineaNaranja.Location = new Point(50, 125);
            lineaNaranja.Size = new Size(715, 4);

            panelTarifas.Controls.Add(lineaNaranja);

            // Etiquetas dias calendario
            string[] dias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            int pixelesExtra = 0;

            foreach(string dia in dias)
            {
                Label labelDia = new Label();
                labelDia.Text = dia;
                labelDia.ForeColor = Color.CornflowerBlue;
                labelDia.Font = new Font("Segoe UI", 12.0f);
                labelDia.AutoSize = true;
                labelDia.Location = new Point(50 + pixelesExtra, 150);
                pixelesExtra += 102;
                panelTarifas.Controls.Add(labelDia);
            }                              

        }

        private List<Panel> crearCalendarioMes(TableLayoutPanel tablaCalendarioTarifas, DateTime actual)
        {
            List<Panel> panelesDias = new List<Panel>();          

            for (int k = 0; k < 5; k++)
            {
                tablaCalendarioTarifas.ColumnCount++;
                tablaCalendarioTarifas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));

                tablaCalendarioTarifas.RowCount++;
                tablaCalendarioTarifas.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            }
            tablaCalendarioTarifas.ColumnCount++;
            tablaCalendarioTarifas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));

            // Meter paneles blancos hasta el primer dia de la semana
            
            DateTime primeroDelMes = new DateTime(actual.Year, actual.Month, 1);
            int diasEnElMes = DateTime.DaysInMonth(actual.Year, actual.Month);
            int primerDiaDelMes = Int32.Parse(primeroDelMes.DayOfWeek.ToString("d"));

            // Domingo: 0, Lunes: 1, Martes: 2, Miercoles: 3, Jueves: 4, Viernes: 5, Sabado: 6
            if (primerDiaDelMes != 0)
            {
                primerDiaDelMes -= 1;
            }
            else
            {
                primerDiaDelMes = 6;
            }

            bool comprobador = false;
            int diaAImprimir = 1;

            for (int i = 0; i < tablaCalendarioTarifas.RowCount; i++)
            {
                for (int k = 0; k < tablaCalendarioTarifas.ColumnCount; k++)
                {
                    if (comprobador == false)
                    {
                        // Se empiezan a imprimir días a partir del primer dia de la semana del mes
                        k = primerDiaDelMes;
                        comprobador = true;
                    }

                    if (diasEnElMes > 0)
                    {
                        // añadir panel en fila i, columna k
                        Panel panelDia = new Panel();
                        panelDia.Size = new Size(100, 80);
                        panelDia.BackColor = Color.FromArgb(24, 30, 54);

                        Label etiquetaDia = new Label();
                        etiquetaDia.Text = diaAImprimir.ToString();
                        etiquetaDia.Font = new Font("Segoe UI", 24.0f);
                        etiquetaDia.AutoSize = true;
                        etiquetaDia.ForeColor = Color.White;
                        etiquetaDia.BackColor = Color.FromArgb(24, 30, 54);
                        etiquetaDia.Location = new Point(0, 0);
                        panelDia.Controls.Add(etiquetaDia);

                        tablaCalendarioTarifas.Controls.Add(panelDia, k, i);
                        panelesDias.Add(panelDia);
                        diaAImprimir++;
                        diasEnElMes--;
                    }
                }
            }

            panelTarifas.Controls.Add(tablaCalendarioTarifas);

            return panelesDias;
        }

        private void mostrarTarifas(bool mostrar)
        {            
            if (mostrar == true)
            {
                //List<TextBox> textBoxPrecios = new List<TextBox>();

                panelTarifas.Size = new Size(1368, 679);
                panelTarifas.Location = new Point(195, 12);
                crearUITarifas();

                // Crear tabla calendario
                TableLayoutPanel tablaCalendarioTarifas = new TableLayoutPanel();
                tablaCalendarioTarifas.Size = new Size(715, 442);
                tablaCalendarioTarifas.Location = new Point(50, 180);
                tablaCalendarioTarifas.BackColor = Color.FromArgb(24, 30, 54);

                tablaCalendarioTarifas.BorderStyle = BorderStyle.FixedSingle;
                tablaCalendarioTarifas.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                tablaCalendarioTarifas.ColumnCount = 1;
                tablaCalendarioTarifas.RowCount = 1;

                DateTime actual = DateTime.Now;
                List<Panel> listaPanelesDias = crearCalendarioMes(tablaCalendarioTarifas, actual);               

                Label labelMes = new Label();
                labelMes.Text = actual.ToString("MMMM-yyyy").First().ToString().ToUpper() + actual.ToString("MMMM-yyyy").Substring(1);
                labelMes.ForeColor = Color.FromArgb(0, 126, 249);
                labelMes.Font = new Font("Segoe UI", 20.0f);
                labelMes.AutoSize = true;
                labelMes.Location = new Point(50, 80);

                panelTarifas.Controls.Add(labelMes);

                // Tabla de precios, que de momento no se añade hasta que no se clica en un dia.
                TableLayoutPanel tablaPrecios = new TableLayoutPanel();
                tablaPrecios.Size = new Size(300, 442);
                tablaPrecios.Location = new Point(850, 180);
                tablaPrecios.BackColor = Color.FromArgb(24, 30, 54);

                tablaPrecios.AutoScroll = false;
                tablaPrecios.HorizontalScroll.Enabled = false;
                tablaPrecios.HorizontalScroll.Visible = false;
                tablaPrecios.HorizontalScroll.Maximum = 0;
                tablaPrecios.AutoScroll = true;
                tablaPrecios.BorderStyle = BorderStyle.FixedSingle;
                tablaPrecios.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                tablaPrecios.ColumnCount = 2;
                tablaPrecios.RowCount = 0;
                panelTarifas.Controls.Add(tablaPrecios);

                // Botones navegar calendario izq y der
                Button btnIzquierda = new Button();
                btnIzquierda.BackgroundImage = RuralManager.Properties.Resources.left;
                btnIzquierda.Size = new Size(50, 50);
                btnIzquierda.BackColor = Color.CornflowerBlue;
                btnIzquierda.Location = new Point(650,75);

                panelTarifas.Controls.Add(btnIzquierda);

                Button btnDerecha = new Button();
                btnDerecha.BackgroundImage = RuralManager.Properties.Resources.right;
                btnDerecha.Size = new Size(50, 50);
                btnDerecha.BackColor = Color.CornflowerBlue;
                btnDerecha.Location = new Point(710, 75);

                panelTarifas.Controls.Add(btnDerecha);

                btnIzquierda.Click += (sender, e) =>
                {
                    if(actual.Month == 1)
                    {
                        actual = new DateTime(actual.Year - 1, 12, 1);
                    }
                    else 
                    {
                        actual = new DateTime(actual.Year, actual.Month - 1, 1);
                    }
                    

                    labelMes.Text = actual.ToString("MMMM-yyyy").First().ToString().ToUpper() + actual.ToString("MMMM-yyyy").Substring(1);

                    tablaCalendarioTarifas.Hide();
                    tablaCalendarioTarifas.Controls.Clear();
                    tablaCalendarioTarifas.ColumnCount = 1;
                    tablaCalendarioTarifas.RowCount = 1;
                    listaPanelesDias = crearCalendarioMes(tablaCalendarioTarifas, actual);
                    listaPanelesDias = cambiarColorPaneles(listaPanelesDias, actual, tablaPrecios, false);
                    tablaCalendarioTarifas.Show();
                };

                btnDerecha.Click += (sender, e) =>
                {
                    if (actual.Month == 12)
                    {
                        actual = new DateTime(actual.Year + 1, 1, 1);
                    }
                    else
                    {
                        actual = new DateTime(actual.Year, actual.Month + 1, 1);
                    }

                    labelMes.Text = actual.ToString("MMMM-yyyy").First().ToString().ToUpper() + actual.ToString("MMMM-yyyy").Substring(1);

                    tablaCalendarioTarifas.Hide();
                    tablaCalendarioTarifas.Controls.Clear();
                    tablaCalendarioTarifas.ColumnCount = 1;
                    tablaCalendarioTarifas.RowCount = 1;
                    listaPanelesDias = crearCalendarioMes(tablaCalendarioTarifas, actual);
                    listaPanelesDias = cambiarColorPaneles(listaPanelesDias, actual, tablaPrecios, false);
                    tablaCalendarioTarifas.Show();
                };

                listaPanelesDias = cambiarColorPaneles(listaPanelesDias, actual, tablaPrecios, false);

                // Boton guardar cambios
                BotonReserva btnGuardar = new BotonReserva();
                btnGuardar.Size = new Size(200,40);
                btnGuardar.Text = "Guardar cambios";
                btnGuardar.BackColor = Color.CornflowerBlue;
                btnGuardar.Location = new Point(950, 630);

                panelTarifas.Controls.Add(btnGuardar);

                btnGuardar.Click += (sender, e) =>
                {
                    float[] precio = new float[Apartamentos.Count()];
                    int[] apartamento = new int[Apartamentos.Count()];
                    List<DateTime> dias = new List<DateTime>();
                    bool comprobadorCambioTarifa = false;

                    // recojo los datos de los precios introducidos
                    for (int i = 0; i < Apartamentos.Count(); i++)
                    {
                        try
                        {
                            if (tablaPrecios.GetControlFromPosition(1, i).Text != "0" && tablaPrecios.GetControlFromPosition(1, i).Text  != "" &&
                            tablaPrecios.GetControlFromPosition(1, i).Text != Apartamentos.ElementAt(i).GetSetPrecioBase.ToString())
                            {
                            
                                    comprobadorCambioTarifa = true;
                                    precio[i] = float.Parse(tablaPrecios.GetControlFromPosition(1, i).Text);
                                    apartamento[i] = i;                           
                            }
                        }
                        catch
                        {
                            MessageBox.Show("El formato introducido en el importe tiene caracteres erróneos.");
                        }
                    }

                    // ver qué dia está seleccionado en la tabla
                    foreach(Control control in tablaCalendarioTarifas.Controls)
                    {
                        if (control is Panel)
                        {
                            foreach(Control label in control.Controls)
                            {
                                if (label is Label && label.BackColor == Color.LightGreen)
                                {
                                    dias.Add(new DateTime(actual.Year, actual.Month, Int32.Parse(label.Text)));
                                }
                            }                            
                        }
                        
                    }

                    if (dias.Count == 0)
                    {
                        MessageBox.Show("No hay días seleccionados");
                    }
                    else if (comprobadorCambioTarifa == false)
                    {
                        MessageBox.Show("Los precios de la tarifa son similares a la tarifa base.");
                    }
                    else
                    {
                        ConexionBD conn = new ConexionBD();
                        conn.editarTarifaDia(dias, precio, apartamento);
                    }
                    

                };

                // Boton modificar tarifa base
                BotonReserva btnModificarTarifaBase = new BotonReserva();
                btnModificarTarifaBase.Size = new Size(300, 40);
                btnModificarTarifaBase.Text = "Modificar tarifa base";
                btnModificarTarifaBase.BackColor = Color.CornflowerBlue;
                btnModificarTarifaBase.Location = new Point(250, 630);

                panelTarifas.Controls.Add(btnModificarTarifaBase);

                btnModificarTarifaBase.Click += (sender, e) =>
                {
                    ventanaModificarTarifaBase().Show();
                };

                //Boton cambiar modo selección para seleccionar varios
                BotonReserva btnCambiarModo = new BotonReserva();
                btnCambiarModo.Size = new Size(200, 40);
                btnCambiarModo.Text = "Selección múltiple";
                btnCambiarModo.BackColor = Color.CornflowerBlue;
                btnCambiarModo.Location = new Point(300, 80);

                panelTarifas.Controls.Add(btnCambiarModo);

                btnCambiarModo.Click += (sender, e) =>
                {
                    if(btnCambiarModo.BackColor == Color.CornflowerBlue)
                    {
                        btnCambiarModo.BackColor = Color.DarkGreen;

                        // Modo selección múltiple
                        tablaCalendarioTarifas.Hide();
                        tablaCalendarioTarifas.Controls.Clear();
                        tablaCalendarioTarifas.ColumnCount = 1;
                        tablaCalendarioTarifas.RowCount = 1;
                        listaPanelesDias = crearCalendarioMes(tablaCalendarioTarifas, actual);
                        listaPanelesDias = cambiarColorPaneles(listaPanelesDias, actual, tablaPrecios, true);
                        tablaCalendarioTarifas.Show();
                    }
                    else
                    {
                        btnCambiarModo.BackColor = Color.CornflowerBlue;

                        // Modo selección normal
                        tablaCalendarioTarifas.Hide();
                        tablaCalendarioTarifas.Controls.Clear();
                        tablaCalendarioTarifas.ColumnCount = 1;
                        tablaCalendarioTarifas.RowCount = 1;
                        listaPanelesDias = crearCalendarioMes(tablaCalendarioTarifas, actual);
                        listaPanelesDias = cambiarColorPaneles(listaPanelesDias, actual, tablaPrecios, false);
                        tablaCalendarioTarifas.Show();
                    }

                    // La tabla precios se establece con los precios base
                    generarUIPrecios(new DateTime(2000, 1, 1), tablaPrecios);

                };


                //Boton cambiar modo selección para seleccionar varios
                BotonReserva btnEliminarTarifa = new BotonReserva();
                btnEliminarTarifa.Size = new Size(170, 40);
                btnEliminarTarifa.Text = "Eliminar tarifa";
                btnEliminarTarifa.BackColor = Color.DarkRed;
                btnEliminarTarifa.Location = new Point(600, 630);

                panelTarifas.Controls.Add(btnEliminarTarifa);

                btnEliminarTarifa.Click += (sender, e) =>
                {
                    List<DateTime> dias = new List<DateTime>();

                    DialogResult dialogResult = MessageBox.Show("¿Está seguro de que desea eliminar las tarifas para los días seleccionados ? ", "Aviso", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // ver qué dia está seleccionado en la tabla
                        foreach (Control control in tablaCalendarioTarifas.Controls)
                        {
                            if (control is Panel)
                            {
                                foreach (Control label in control.Controls)
                                {
                                    if (label is Label && label.BackColor == Color.LightGreen)
                                    {
                                        dias.Add(new DateTime(actual.Year, actual.Month, Int32.Parse(label.Text)));
                                    }
                                }
                            }

                        }

                        if (dias.Count == 0)
                        {
                            MessageBox.Show("No hay días seleccionados");
                            return;
                        }
                        else
                        {
                            ConexionBD conn = new ConexionBD();
                            conn.eliminarTarifas(dias);
                        }
                    }
                };
                // IDEA: poner para introducir un rango de fechas y que se seleccionen para cambiarlo mas rapidamente

                panelTarifas.Show();
            }
            else
            {
                panelTarifas.Controls.Clear();
                panelFacturas.Hide();
            }
        }

        private List<Panel> cambiarColorPaneles(List<Panel> listaPanelesDias, DateTime mesActual, TableLayoutPanel tablaPrecios, bool seleccionMultiple)
        {
            // Hacer los días clicables y seleccionables
            foreach (Panel panel in listaPanelesDias)
            {
                foreach (Control control in panel.Controls)
                {
                    if (control is Label)
                    {
                        panel.Click += clicarDia;
                        control.Click += clicarDia;

                        void clicarDia(object sender, EventArgs e)
                        {
                            if (seleccionMultiple == false)
                            {
                                // deseleccionar los demás paneles
                                foreach (Panel panelN in listaPanelesDias)
                                {
                                    foreach (Control controlN in panelN.Controls)
                                    {
                                        panelN.BackColor = Color.FromArgb(24, 30, 54);
                                        controlN.BackColor = Color.FromArgb(24, 30, 54);
                                        controlN.ForeColor = Color.White;
                                    }
                                }

                                // seleccionar el actual
                                panel.BackColor = Color.LightGreen;
                                control.BackColor = Color.LightGreen;
                                control.ForeColor = Color.Black;

                                // Llamar a función que añada la UI de los precios de la tarifa para este dia clicado                                                        
                                generarUIPrecios(new DateTime(mesActual.Year, mesActual.Month, Int32.Parse(control.Text)), tablaPrecios);
                            }
                            else
                            {
                                // Con selección múltiple, no se limpian los demás paneles
                                if (panel.BackColor == Color.FromArgb(24, 30, 54))
                                {
                                    // Si el actual no está pulsado, se pulsa
                                    panel.BackColor = Color.LightGreen;
                                    control.BackColor = Color.LightGreen;
                                    control.ForeColor = Color.Black;
                                }
                                else
                                {
                                    panel.BackColor = Color.FromArgb(24, 30, 54);
                                    control.BackColor = Color.FromArgb(24, 30, 54);
                                    control.ForeColor = Color.White;
                                }
                                
                            }
                        };
                    }
                }
            }

            return listaPanelesDias;
        }

        private void generarUIPrecios(DateTime dia, TableLayoutPanel tablaPrecios) 
        {
            tablaPrecios.Hide();
            tablaPrecios.Controls.Clear();            
            tablaPrecios.RowCount = 0;            

            ConexionBD conn = new ConexionBD();
            List<Tarifa> tarifasDia = conn.obtenerTarifasDeDia(dia);
            
            for (int i = 0; i < Apartamentos.Count(); i++)
            {
                tablaPrecios.RowCount += 1;

                Label labelApartamento = new Label();
                labelApartamento.Text = Apartamentos.ElementAt(i).GetNombre;
                labelApartamento.Font = new Font("Segoe UI", 14.0f);
                labelApartamento.Size = new Size(200, 30);
                labelApartamento.ForeColor = Color.Black;
                labelApartamento.BackColor = Color.Coral;
                tablaPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                tablaPrecios.Controls.Add(labelApartamento, 0, tablaPrecios.RowCount - 1);

                textBoxRedonda textboxPrecio = new textBoxRedonda();
                textboxPrecio.Size = new Size(80, 30);
                textboxPrecio.ForeColor = Color.White;
                textboxPrecio.BackColor = Color.FromArgb(24, 30, 54);
                textboxPrecio.BorderStyle = BorderStyle.None;
                textboxPrecio.Font = new Font("Segoe UI", 14.0f);
                textboxPrecio.TextAlign = HorizontalAlignment.Right;
                textboxPrecio.Text = buscarPrecioTarifaApartamento(tarifasDia, i).ToString();
                tablaPrecios.Controls.Add(textboxPrecio, 1, tablaPrecios.RowCount - 1);
                
            }
            tablaPrecios.Show();
        }

        private float buscarPrecioTarifaApartamento(List<Tarifa> diasTarifa, int indiceApartamento)
        {
            float precio = 0;                        

            foreach (Tarifa tarifa in diasTarifa)
            {               
                if (tarifa.GetSetApartamento == indiceApartamento)
                {
                    return tarifa.GetSetPrecio;
                }
            }

            // si no hay precio definido para la tarifa:
            precio = Apartamentos.ElementAt(indiceApartamento).GetSetPrecioBase;

            return precio;
        }

        private Form ventanaModificarTarifaBase()
        {
            Form ventanaTarifaBase = new Form();
            ventanaTarifaBase.Location = new Point(300, 600);
            ventanaTarifaBase.Size = new Size(450, 700);
            ventanaTarifaBase.BackColor = Color.FromArgb(46, 51, 73);
            ventanaTarifaBase.Text = "Modificar tarifa base";
            ventanaTarifaBase.StartPosition = FormStartPosition.Manual;
            ventanaTarifaBase.Left = 600;
            ventanaTarifaBase.Top = 200;

            Label etiquetaModificar = new Label();
            etiquetaModificar.Text = "Modificar Tarifa Base";
            etiquetaModificar.Location = new Point(20, 20);
            etiquetaModificar.ForeColor = Color.FromArgb(0, 126, 249);
            etiquetaModificar.BackColor = Color.FromArgb(46, 51, 73);
            etiquetaModificar.AutoSize = true;
            etiquetaModificar.Font = new Font("Segoe UI", 16.0f);

            ventanaTarifaBase.Controls.Add(etiquetaModificar);

            TableLayoutPanel tablaPrecios = new TableLayoutPanel();
            tablaPrecios.Size = new Size(300, 442);
            tablaPrecios.Location = new Point(60, 60);
            tablaPrecios.BackColor = Color.FromArgb(24, 30, 54);

            tablaPrecios.AutoScroll = false;
            tablaPrecios.HorizontalScroll.Enabled = false;
            tablaPrecios.HorizontalScroll.Visible = false;
            tablaPrecios.HorizontalScroll.Maximum = 0;
            tablaPrecios.AutoScroll = true;
            tablaPrecios.BorderStyle = BorderStyle.FixedSingle;
            tablaPrecios.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablaPrecios.ColumnCount = 2;
            tablaPrecios.RowCount = 0;

            // inicializar tabla
            for (int i = 0; i < Apartamentos.Count(); i++)
            {
                tablaPrecios.RowCount += 1;

                Label labelApartamento = new Label();
                labelApartamento.Text = Apartamentos.ElementAt(i).GetNombre;
                labelApartamento.Font = new Font("Segoe UI", 14.0f);
                labelApartamento.Size = new Size(200, 30);
                labelApartamento.ForeColor = Color.Black;
                labelApartamento.BackColor = Color.Coral;
                tablaPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                tablaPrecios.Controls.Add(labelApartamento, 0, tablaPrecios.RowCount - 1);

                textBoxRedonda textboxPrecio = new textBoxRedonda();
                textboxPrecio.Size = new Size(80, 30);
                textboxPrecio.ForeColor = Color.White;
                textboxPrecio.BackColor = Color.FromArgb(24, 30, 54);
                textboxPrecio.BorderStyle = BorderStyle.None;
                textboxPrecio.Font = new Font("Segoe UI", 14.0f);
                textboxPrecio.TextAlign = HorizontalAlignment.Right;
                textboxPrecio.Text = Apartamentos.ElementAt(i).GetSetPrecioBase.ToString();
                tablaPrecios.Controls.Add(textboxPrecio, 1, tablaPrecios.RowCount - 1);

            }

            ventanaTarifaBase.Controls.Add(tablaPrecios);

            BotonReserva btnGuardar = new BotonReserva();
            btnGuardar.Size = new Size(200, 40);
            btnGuardar.Text = "Guardar cambios";
            btnGuardar.Location = new Point(120, 550);

            ventanaTarifaBase.Controls.Add(btnGuardar);

            btnGuardar.Click += (sender, e) =>
            {
                float[] precio = new float[Apartamentos.Count()];
                int[] apartamento = new int[Apartamentos.Count()];

                try
                {
                    for (int i = 0; i < Apartamentos.Count(); i++)
                    {
                        if (tablaPrecios.GetControlFromPosition(1, i).Text != "0" && tablaPrecios.GetControlFromPosition(1, i).Text != "")
                        {
                            precio[i] = float.Parse(tablaPrecios.GetControlFromPosition(1, i).Text);
                            apartamento[i] = i;
                        }
                    }
                    ConexionBD conn = new ConexionBD();
                    conn.actualizarPreciosBaseTarifa(precio, apartamento);

                    actualizarApartamentos();
                    ventanaTarifaBase.Close();
                }
                catch
                {
                    MessageBox.Show("El formato introducido en el importe tiene caracteres erróneos.");
                }
                
            };


            return ventanaTarifaBase;
        }

        //
        // Funciones para mostrar ventana de facturas
        //

        public void actualizarFacturas()
        {
            ConexionBD conn = new ConexionBD();
            Facturas = conn.obtenerFacturas();          

        }

        public void dibujarFacturas(string buscaNombre, DateTime buscaFechaDe, DateTime buscaFechaHasta)
        {
            bool mostrar = true;
            tablaFacturas.Hide();
            tablaFacturas.Controls.Clear();            
            tablaFacturas.RowCount = 1;


            for (int i = 0; i < Facturas.Count(); i++)
            {
                
                if (buscaNombre != null && buscaFechaDe != new DateTime(2000, 1, 1) && buscaFechaHasta != new DateTime(2000, 1, 1))
                {
                    // Sólo se busca por rango de fechas
                    if (comprobarFechaReservaEnFactura(Facturas.ElementAt(i).GetSetNumeroFactura, buscaFechaDe, buscaFechaHasta))
                    {
                        mostrar = true;
                    }
                    else
                    {
                        mostrar = false;
                    }
                }                
                else if (buscaNombre != null)
                {
                    // Sólo se busca por nombre
                    //MessageBox.Show("Busca por nombre");
                    if (Facturas.ElementAt(i).GetSetNombreCompleto.Contains(buscaNombre))
                    {
                        mostrar = true;
                    }
                    else
                    {
                        mostrar = false;
                    }
                }
                else if (buscaNombre != null && buscaFechaDe != new DateTime(2000, 1, 1) && buscaFechaHasta != new DateTime(2000, 1, 1))
                {
                    // Se busca por los dos filtros
                    MessageBox.Show("Busca por los dos filtros");
                    if (Facturas.ElementAt(i).GetSetNombreCompleto.Contains(buscaNombre) &&
                        comprobarFechaReservaEnFactura(Facturas.ElementAt(i).GetSetNumeroFactura, buscaFechaDe, buscaFechaHasta))
                    {
                        mostrar = true;
                    }
                    else
                    {
                        mostrar = false;
                    }

                }

                if (mostrar == true)
                {
                    Label numFactura = new Label();
                    numFactura.Text = Facturas.ElementAt(i).GetSetNumeroFactura.ToString();
                    numFactura.Font = new Font("Segoe UI", 11.0f);
                    numFactura.ForeColor = Color.White;
                    numFactura.Size = new Size(350, 30);

                    Label nombreCompleto = new Label();
                    nombreCompleto.Text = Facturas.ElementAt(i).GetSetNombreCompleto;
                    nombreCompleto.Font = new Font("Segoe UI", 11.0f);
                    nombreCompleto.ForeColor = Color.White;
                    nombreCompleto.Size = new Size(350, 30);

                    Label DNI = new Label();
                    DNI.Text = Facturas.ElementAt(i).GetSetDNI;
                    DNI.Font = new Font("Segoe UI", 11.0f);
                    DNI.ForeColor = Color.White;
                    DNI.Size = new Size(350, 30);

                    Label fecha = new Label();
                    fecha.Text = Facturas.ElementAt(i).GetSetFecha;
                    fecha.Font = new Font("Segoe UI", 11.0f);
                    fecha.ForeColor = Color.White;
                    fecha.Size = new Size(350, 30);


                    if (Facturas.ElementAt(i).GetSetReservasAsociadas.Count == 1)
                    {
                        Label apartamentoAsociado = new Label();
                        int indice = buscarIndiceReserva(Facturas.ElementAt(i).GetSetReservasAsociadas.ElementAt(0));
                        apartamentoAsociado.Text = "Reserva: " + Facturas.ElementAt(i).GetSetReservasAsociadas.ElementAt(0).ToString() + " | " +
                                Reservas.ElementAt(indice).GetSetCheckin.ToString("yyyy-MMM-dd") + " - " +
                                Reservas.ElementAt(indice).GetSetCheckout.ToString("yyyy-MMM-dd"); ;
                        apartamentoAsociado.Font = new Font("Segoe UI", 10.0f);
                        apartamentoAsociado.ForeColor = Color.White;
                        apartamentoAsociado.Size = new Size(350, 30);
                        tablaFacturas.Controls.Add(apartamentoAsociado, 4, tablaFacturas.RowCount);
                    }
                    else
                    {
                        // La factura está ligada a varias reservas

                        // crear desplegable apartamentos
                        Label desplegable = new Label();
                        desplegable.Text = "Múltiples";
                        desplegable.Font = new Font("Segoe UI", 11.0f);
                        desplegable.ForeColor = Color.White;
                        desplegable.BackColor = Color.FromArgb(74, 30, 54);
                        desplegable.Size = new Size(500, 30);
                        tablaFacturas.Controls.Add(desplegable, 4, tablaFacturas.RowCount);

                        Panel desplegableReservas = new Panel();
                        desplegableReservas.AutoSize = true;
                        desplegableReservas.BackColor = Color.FromArgb(0, 126, 249);
                        desplegableReservas.Size = new Size(150, 30);

                        for (int k = 0; k < Facturas.ElementAt(i).GetSetReservasAsociadas.Count; k++)
                        {
                            Label reservaAsociada = new Label();
                            reservaAsociada.AutoSize = true;
                            reservaAsociada.Font = new Font("Segoe UI", 16.0f);

                            int indice = buscarIndiceReserva(Facturas.ElementAt(i).GetSetReservasAsociadas.ElementAt(k));
                            reservaAsociada.Text = "Reserva: " + Facturas.ElementAt(i).GetSetReservasAsociadas.ElementAt(k).ToString() + " | " +
                                Reservas.ElementAt(indice).GetSetCheckin.ToString("yyyy-MMM-dd") + " - " +
                                Reservas.ElementAt(indice).GetSetCheckout.ToString("yyyy-MMM-dd");
                            reservaAsociada.Location = new Point(0, 0 + (30 * k));
                            desplegableReservas.Controls.Add(reservaAsociada);
                        }


                        desplegable.MouseEnter += (sender, e) =>
                        {
                            // mostrar panel desplegable
                            desplegableReservas.Location = new Point(Cursor.Position.X - 170, Cursor.Position.Y - 170);
                            Controls.Add(desplegableReservas);
                            desplegableReservas.BringToFront();
                        };

                        desplegable.MouseLeave += (sender, e) =>
                        {
                            Controls.Remove(desplegableReservas);
                        };


                    }

                    // añadir iconos de PDF, Modificar y añadir reserva
                    anadirIconosFacturas(Facturas.ElementAt(i).GetSetNumeroFactura);
                                    
                    tablaFacturas.RowCount = tablaFacturas.RowCount + 1;
                    tablaFacturas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                    tablaFacturas.Controls.Add(numFactura, 0, tablaFacturas.RowCount - 1);
                    tablaFacturas.Controls.Add(nombreCompleto, 1, tablaFacturas.RowCount - 1);
                    tablaFacturas.Controls.Add(DNI, 2, tablaFacturas.RowCount - 1);
                    tablaFacturas.Controls.Add(fecha, 3, tablaFacturas.RowCount - 1);
                }               

            }
            tablaFacturas.RowCount = tablaFacturas.RowCount + 1;
            tablaFacturas.Show();

        }

        public void crearUIFacturas()
        {
            Label labelFacturas = new Label();
            labelFacturas.Text = "Facturación";
            labelFacturas.Font = new Font("Segoe UI", 24.0f);
            labelFacturas.ForeColor = Color.FromArgb(0, 126, 249);
            labelFacturas.Location = new Point(20, 20);
            labelFacturas.AutoSize = true;
            panelFacturas.Controls.Add(labelFacturas);

            Label labelListaFacturas = new Label();
            labelListaFacturas.Text = "Lista facturas";
            labelListaFacturas.Font = new Font("Segoe UI", 15.0f);
            labelListaFacturas.ForeColor = Color.CornflowerBlue;
            labelListaFacturas.Location = new Point(50, 80);
            labelListaFacturas.AutoSize = true;
            panelFacturas.Controls.Add(labelListaFacturas);

            Panel lineaNaranja = new Panel();
            lineaNaranja.BackColor = Color.DarkOrange;
            lineaNaranja.Location = new Point(50, 115);
            lineaNaranja.Size = new Size(1250, 4);

            panelFacturas.Controls.Add(lineaNaranja);

            // Mover tabla
            tablaFacturasColumnas.Location = new Point(50, 130);
            tablaFacturas.Location = new Point(50, 160);

            primeraColFactura.Text = "Nº";
            segundaColFactura.Text = "Nombre completo";
            terceraColFactura.Text = "DNI";
            cuartaColFactura.Text = "Fecha de factura";
            quintaColFactura.Text = "Nº Reserva asociada";
            sextaColFactura.Text = "PDF";
            septimaColFactura.Text = "Modificar";

        }

        public Form crearUIAnadirReservasAFactura(int numFactura)
        {
            TableLayoutPanel tablaTemporalBuscador = new TableLayoutPanel();

            Form ventanaAnadirApartamento = new Form();
            ventanaAnadirApartamento.Location = new Point(500, 500);
            ventanaAnadirApartamento.Size = new Size(960, 800);
            ventanaAnadirApartamento.BackColor = Color.DarkGray;
            ventanaAnadirApartamento.Text = "Añadir reservas a factura";
            ventanaAnadirApartamento.StartPosition = FormStartPosition.Manual;
            ventanaAnadirApartamento.Left = 600;
            ventanaAnadirApartamento.Top = 200;

            Label labelAniadirReserva = new Label();
            labelAniadirReserva.Text = "Selecciona las reservas a añadir a la factura:";
            labelAniadirReserva.Font = new Font("Segoe UI", 16.0f, FontStyle.Bold);
            labelAniadirReserva.ForeColor = Color.Black;
            labelAniadirReserva.Location = new Point(20, 20);
            labelAniadirReserva.AutoSize = true;

            ventanaAnadirApartamento.Controls.Add(labelAniadirReserva);

            Label labelReservasDisponibles = new Label();
            labelReservasDisponibles.Text = "Reservas disponibles a facturar:";
            labelReservasDisponibles.Font = new Font("Segoe UI", 12.0f, FontStyle.Bold);
            labelReservasDisponibles.ForeColor = Color.Black;
            labelReservasDisponibles.Location = new Point(20, 80);
            labelReservasDisponibles.AutoSize = true;

            ventanaAnadirApartamento.Controls.Add(labelReservasDisponibles);

            Label labelReservasEnFactura = new Label();
            labelReservasEnFactura.Text = "Reservas en la factura " + numFactura + ":";
            labelReservasEnFactura.Font = new Font("Segoe UI", 12.0f, FontStyle.Bold);
            labelReservasEnFactura.ForeColor = Color.Black;
            labelReservasEnFactura.Location = new Point(520, 100);
            labelReservasEnFactura.AutoSize = true;

            ventanaAnadirApartamento.Controls.Add(labelReservasEnFactura);

            // Añadir buscador
            Label labelBuscador = new Label();
            labelBuscador.Text = "Buscador ";
            labelBuscador.Font = new Font("Segoe UI", 12.0f);
            labelBuscador.ForeColor = Color.Black;
            labelBuscador.Location = new Point(20, 110);
            labelBuscador.AutoSize = true;

            ventanaAnadirApartamento.Controls.Add(labelBuscador);

            textBoxRedonda buscador = new textBoxRedonda();
            buscador.BackColor = Color.RoyalBlue;
            buscador.Location = new Point(120, 110);
            buscador.Size = new Size(300, 10);
            buscador.Font = new Font("Segoe UI", 12.0f);
            buscador.ForeColor = Color.White;
            buscador.BorderStyle = BorderStyle.None;
            buscador.TextAlign = HorizontalAlignment.Center;
            buscador.BringToFront();
            ventanaAnadirApartamento.Controls.Add(buscador);                      

            // Devolver un array de ints de los numeros de reserva seleccionados

            // Tabla izquierda
            TableLayoutPanel tablaReservasDisponibles = new TableLayoutPanel();
            tablaReservasDisponibles.Size = new Size(410, 600);
            tablaReservasDisponibles.Location = new Point(20, 140);
            tablaReservasDisponibles.BackColor = Color.FromArgb(24, 30, 54);

            tablaReservasDisponibles.BorderStyle = BorderStyle.FixedSingle;
            tablaReservasDisponibles.ColumnCount = 1;
            tablaReservasDisponibles.RowCount = 0;
            tablaReservasDisponibles.AutoScroll = false;
            tablaReservasDisponibles.HorizontalScroll.Enabled = false;
            tablaReservasDisponibles.HorizontalScroll.Visible = false;
            tablaReservasDisponibles.HorizontalScroll.Maximum = 0;
            tablaReservasDisponibles.AutoScroll = true;            

            // Añadir las filas de apartamentos NO facturados aún

            for(int i = 0; i < Reservas.Count(); i++)
            {
                if(Reservas.ElementAt(i).GetFactura == -1)
                {
                    Panel nuevaFila = crearNuevaFila(i, true);

                    tablaReservasDisponibles.RowCount += 1;
                    tablaReservasDisponibles.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
                    tablaReservasDisponibles.Controls.Add(nuevaFila, 0, tablaReservasDisponibles.RowCount - 1);
                }
                
            }
            ventanaAnadirApartamento.Controls.Add(tablaReservasDisponibles);

            // Tabla derecha
            TableLayoutPanel tablaReservasAFacturar = new TableLayoutPanel();
            tablaReservasAFacturar.Size = new Size(410, 600);
            tablaReservasAFacturar.Location = new Point(520, 140);
            tablaReservasAFacturar.BackColor = Color.FromArgb(24, 30, 54);

            tablaReservasAFacturar.BorderStyle = BorderStyle.FixedSingle;
            tablaReservasAFacturar.ColumnCount = 1;
            tablaReservasAFacturar.RowCount = 0;
            tablaReservasAFacturar.AutoScroll = false;
            tablaReservasAFacturar.HorizontalScroll.Enabled = false;
            tablaReservasAFacturar.HorizontalScroll.Visible = false;
            tablaReservasAFacturar.HorizontalScroll.Maximum = 0;
            tablaReservasAFacturar.AutoScroll = true;

            // Añadir las filas de apartamentos que aparecen en la factura
            for (int i = 0; i < Reservas.Count(); i++)
            {
                if (Reservas.ElementAt(i).GetFactura == numFactura)
                {
                    Panel nuevaFila = crearNuevaFila(i, true);

                    tablaReservasAFacturar.RowCount += 1;
                    tablaReservasAFacturar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
                    tablaReservasAFacturar.Controls.Add(nuevaFila, 0, tablaReservasAFacturar.RowCount - 1);
                }

            }
            ventanaAnadirApartamento.Controls.Add(tablaReservasAFacturar);

            buscador.TextChanged += (sender, e) =>
            {
                // se repite para mover las que están en la tabla temporal que SI que se tienen que mostrar
                foreach (Control panel in tablaTemporalBuscador.Controls)
                {
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Button)
                        {
                            string nombrecompleto = Reservas.ElementAt(Int32.Parse(control.Text)).GetSetNombre + " " + Reservas.ElementAt(Int32.Parse(control.Text)).GetSetApellidos;
                            if (nombrecompleto.Contains(buscador.Text))
                            {
                                // Si no contiene el texto buscado, se mueve al panel temporal
                                TableLayoutPanel[] tablas = moverPanelATabla(tablaTemporalBuscador, tablaReservasDisponibles, control);

                                tablaTemporalBuscador = tablas[0];
                                tablaReservasDisponibles = tablas[1];
                            }
                        }
                    }
                }

                foreach (Control panel in tablaReservasDisponibles.Controls)
                {
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Button)
                        {
                            string nombrecompleto = Reservas.ElementAt(Int32.Parse(control.Text)).GetSetNombre + " " + Reservas.ElementAt(Int32.Parse(control.Text)).GetSetApellidos;
                            if (!nombrecompleto.Contains(buscador.Text))
                            {
                                // Si no contiene el texto buscado, se mueve al panel temporal
                                TableLayoutPanel[] tablas = moverPanelATabla(tablaReservasDisponibles, tablaTemporalBuscador, control);

                                tablaReservasDisponibles = tablas[0];
                                tablaTemporalBuscador = tablas[1];
                            }
                        }
                    }
                }                
            };


            // Botones flechas izquierda y derecha
            Button botonDerecha = new Button();
            botonDerecha.Size = new Size(50, 50);
            botonDerecha.Text = "";
            botonDerecha.BackColor = Color.CornflowerBlue;
            botonDerecha.Location = new Point(450, 320);

            botonDerecha.Click += (sender, e) =>
            {
                // Bucle que comprueba las casillas seleccionadas y las mueve a la tabla de la derecha

                foreach(Control panel in tablaReservasDisponibles.Controls)
                {
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Button && panel.BackColor == Color.FromArgb(255, 0, 126, 249))
                        {
                            TableLayoutPanel[] tablas = moverPanelATabla(tablaReservasDisponibles, tablaReservasAFacturar, control);

                            tablaReservasDisponibles = tablas[0];
                            tablaReservasAFacturar = tablas[1];
                        }
                    }
                }

            };

            Image botonDer = new Bitmap(@"C:\Users\ivior\Desktop\TFG\Iconos\right.png");
            botonDerecha.BackgroundImage = botonDer;
            ventanaAnadirApartamento.Controls.Add(botonDerecha);


            Button botonIzquierda = new Button();
            botonIzquierda.Size = new Size(50, 50);
            botonIzquierda.Text = "";
            botonIzquierda.BackColor = Color.CornflowerBlue;
            botonIzquierda.Location = new Point(450, 400);

            botonIzquierda.Click += (sender, e) =>
            {
                // Bucle que comprueba las casillas seleccionadas y las mueve a la tabla de la izquierda
                // Tener en cuenta que si ya está en la factura no se puede mover

                foreach (Control panel in tablaReservasAFacturar.Controls)
                {
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Button && panel.BackColor == Color.FromArgb(255, 0, 126, 249))
                        {
                            if(numFactura == Reservas.ElementAt(Int32.Parse(control.Text)).GetFactura)
                            {
                                MessageBox.Show("No se puede mover la reserva porque ya se encontraba anteriormente en la factura.");
                            }
                            else
                            {
                                TableLayoutPanel[] tablas = moverPanelATabla(tablaReservasAFacturar, tablaReservasDisponibles, control);

                                tablaReservasAFacturar = tablas[0];
                                tablaReservasDisponibles = tablas[1];
                            }                            
                        }
                    }
                }
            };

            Image botonIzq = new Bitmap(@"C:\Users\ivior\Desktop\TFG\Iconos\left.png");
            botonIzquierda.BackgroundImage = botonIzq;
            ventanaAnadirApartamento.Controls.Add(botonIzquierda);

            BotonReserva botonGuardarCambios = new BotonReserva();
            botonGuardarCambios.Size = new Size(250, 40);
            botonGuardarCambios.Location = new Point(650,20);
            botonGuardarCambios.BackColor = Color.CornflowerBlue;
            botonGuardarCambios.Text = "Guardar cambios";
            botonGuardarCambios.Font = new Font("Segoe UI", 12.0f);
            botonGuardarCambios.ForeColor = Color.White;

            ventanaAnadirApartamento.Controls.Add(botonGuardarCambios);

            botonGuardarCambios.Click += (sender, e) =>
            {
                List<Reserva> reservasAsociadas = new List<Reserva>();

                foreach (Control panel in tablaReservasAFacturar.Controls)
                {
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Button)
                        {
                            // Recoger las reservas a facturar
                            reservasAsociadas.Add(Reservas.ElementAt(Int32.Parse(control.Text)));
                        }
                    }
                }
                // Cambiar la factura asignada de las reservas en la BD haciendo una llamada
                ConexionBD conn = new ConexionBD();
                conn.asignarNuevasFacturas(reservasAsociadas, numFactura);

                actualizarReservas();
                actualizarFacturas();                                             

                Factura fact = Facturas.ElementAt(BuscarIndiceFactura(numFactura));
                DateTime[] checkinYCheckout = buscarCheckinYCheckoutFactura(fact.GetSetReservasAsociadas);
                int numTotalPersonas = calcularPersonasTotales(fact.GetSetReservasAsociadas);
                float importeTotal = calcularImporteTotal(fact.GetSetReservasAsociadas);
                string apartamentosAsociados = concatenarApartamentos(fact.GetSetReservasAsociadas);

                // 0: NombreCompleto, 1: DNI, 2: Direccion, 3: Codigo Postal, 4: fechaActual, 5: checkin, 6: checkout, 7: apartamento, 8: personas, 9: precio
                // 10: observaciones, 11: ReservaAsociada, 12: NumSiguienteFactura (para imprimirla en el PDF)
                string[] datosFactura = { fact.GetSetNombreCompleto, fact.GetSetDNI, fact.GetSetDireccion, fact.GetSetCodigoPostal.ToString(), DateTime.Now.ToString("yyyy-MMM-dd"),
                checkinYCheckout[0].ToString("yyyy-MMM-dd"), checkinYCheckout[1].ToString("yyyy-MMM-dd"), apartamentosAsociados, numTotalPersonas.ToString(), importeTotal.ToString(), 
                fact.GetSetObservaciones, "Múltiples", numFactura.ToString()};


                fact.asignarNuevasReservas(reservasAsociadas, datosFactura);
                dibujarFacturas(null, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1));
                ventanaAnadirApartamento.Close();
            };



            return ventanaAnadirApartamento;
        }

        public TableLayoutPanel[] moverPanelATabla(TableLayoutPanel inicio, TableLayoutPanel final, Control id)
        {            
            int lineaABorrar = -1;

            // Eliminar el panel con el id enviado
            foreach (Control control in inicio.Controls)
            {
                foreach (Control panel in control.Controls)
                {
                    if (panel is Button && panel.Text == id.Text)
                    {
                        //MessageBox.Show("La linea a borrar es: " + inicio.GetPositionFromControl(control).ToString());
                        inicio.Controls.Remove(control);
                        inicio.RowCount = inicio.RowCount - 1;
                    }
                }                    
            }

            // Añadir en la otra tabla el panel llamando a nuevafila
            Panel nuevaFila = crearNuevaFila(Int32.Parse(id.Text), true);

            final.RowCount += 1;
            final.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            final.Controls.Add(nuevaFila, 0, final.RowCount - 1);

            TableLayoutPanel[] inicioYFinal = { inicio, final};

            return inicioYFinal;
        }

        public void anadirIconosFacturas(int numFactura)
        {
            Label botonPDF = new Label();
            botonPDF.Image = RuralManager.Properties.Resources.pdf;
            botonPDF.FlatStyle = FlatStyle.Flat;
            tablaFacturas.Controls.Add(botonPDF, 5, tablaFacturas.RowCount);
            botonPDF.Click += (sender, e) =>
            {
                string filePath = @"C:\Users\ivior\Desktop\facturas\Factura" + numFactura + ".pdf";
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("La factura no existe en el directorio de facturas", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string argument = "/select, \"" + filePath + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            };

            botonPDF.MouseEnter += (sender, e) =>
            {
                botonPDF.BackColor = Color.FromName("ActiveCaption");
            };

            botonPDF.MouseLeave += (sender, e) =>
            {
                botonPDF.BackColor = Color.FromArgb(24, 30, 54);
            };

            // Crear panel para añadir los dos iconos de editar y añadir
            Panel panelModificar = new Panel();
            panelModificar.BackColor = Color.FromArgb(24, 30, 54);
            panelModificar.Size = new Size(150, 30);
            tablaFacturas.Controls.Add(panelModificar, 6, tablaFacturas.RowCount);

            // Editar
            Label botonEditar = new Label();
            botonEditar.Image = RuralManager.Properties.Resources.editar;
            botonEditar.FlatStyle = FlatStyle.Flat;
            botonEditar.Size = new Size(50, 30);
            botonEditar.Location = new Point(0, 0);
            panelModificar.Controls.Add(botonEditar);

            botonEditar.Click += (sender, e) =>
            {
                // Mandar lista de reservas asociadas si es múltiple

                if (Facturas.ElementAt(numFactura-1).GetSetReservasAsociadas.Count > 1)
                {
                    // es multiple
                    List<Reserva> reservasAsociadas = new List<Reserva>();

                    for(int i = 0; i < Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas.Count; i++)
                    {
                        reservasAsociadas.Add(Reservas.ElementAt(buscarIndiceReserva(Facturas.ElementAt(numFactura - 1).GetSetReservasAsociadas[i])));
                    }

                    crearUIFactura(null, numFactura, reservasAsociadas).Show();
                }
                else
                {
                    crearUIFactura(null, numFactura, null).Show();
                }
                
            };

            botonEditar.MouseEnter += (sender, e) =>
            {
                botonEditar.BackColor = Color.FromName("ActiveCaption");
            };

            botonEditar.MouseLeave += (sender, e) =>
            {
                botonEditar.BackColor = Color.FromArgb(24, 30, 54);
            };

            // Añadir reserva

            Label botonAnadir = new Label();
            botonAnadir.Image = RuralManager.Properties.Resources.anadir;
            botonAnadir.FlatStyle = FlatStyle.Flat;
            botonAnadir.Size = new Size(50, 30);
            botonEditar.Location = new Point(50, 0);
            panelModificar.Controls.Add(botonAnadir);

            botonAnadir.Click += (sender, e) =>
            {
                Form panelAnadirapartamento = crearUIAnadirReservasAFactura(numFactura);
                panelAnadirapartamento.Show();

            };

            botonAnadir.MouseEnter += (sender, e) =>
            {
                botonAnadir.BackColor = Color.FromName("ActiveCaption");
            };

            botonAnadir.MouseLeave += (sender, e) =>
            {
                botonAnadir.BackColor = Color.FromArgb(24, 30, 54);
            };


        }

        public DateTime[] buscarCheckinYCheckoutFactura(List<int> reservasAsociadas)
        {
            // Busca el checkin menor y checkout mayor de las reservas asociadas en la factura
            DateTime[] checkinYCheckout = { Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(0))).GetSetCheckin, 
                Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(0))).GetSetCheckout};

            for (int i = 1; i < reservasAsociadas.Count; i++)
            {
                if (Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckin < checkinYCheckout[0])
                {
                    checkinYCheckout[0] = Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckin;
                }

                if (Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckout > checkinYCheckout[1])
                {
                    checkinYCheckout[1] = Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckout;
                }
            }           


            return checkinYCheckout;
        }

        public int calcularPersonasTotales(List<int> reservasAsociadas)
        {
            int total = 0;

            for (int i = 0; i < reservasAsociadas.Count; i++)
            {
                total += Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetPersonas;
            }

            return total;
        }

        public float calcularImporteTotal(List<int> reservasAsociadas)
        {
            float importe = 0;

            for (int i = 0; i < reservasAsociadas.Count; i++)
            {
                importe += Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetImporte;
            }

            return importe;
        }

        public string concatenarApartamentos(List<int> reservasAsociadas)
        {
            string apartamentos = Apartamentos.ElementAt((Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(0))).GetSetApartamento)).GetNombre;

            for (int i = 1; i < reservasAsociadas.Count; i++)
            {
                apartamentos += "," + Apartamentos.ElementAt((Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetApartamento)).GetNombre;
            }

            return apartamentos;
        }

        public bool comprobarFechaReservaEnFactura(int numFactura, DateTime buscaFechaDe, DateTime buscaFechaHasta)
        {
            bool comprobarFecha = false;
            List<int> reservasAsociadas = Facturas.ElementAt(BuscarIndiceFactura(numFactura)).GetSetReservasAsociadas;

            // recorrer la lista de reservasAsociadas para la factura comprobando si alguna de las reservas está dentro del rango
            for (int i = 0; i < reservasAsociadas.Count(); i++)
            {
                if (buscaFechaHasta >= Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckin &&
                    buscaFechaDe <= Reservas.ElementAt(buscarIndiceReserva(reservasAsociadas.ElementAt(i))).GetSetCheckout)
                {
                    //MessageBox.Show("Se muestra");
                    comprobarFecha = true;
                }
            }

            return comprobarFecha;
        }

        public void mostrarFacturas(bool mostrar)
        {
            if(mostrar == true)
            {
                DateTime fechaEnBuscadorDe = new DateTime(2000, 1, 1);
                DateTime fechaEnBuscadorHasta = new DateTime(2000, 1, 1); 

                panelFacturas.Size = new Size(1368, 679);
                panelFacturas.Location = new Point(195, 12);
                tablaFacturas.Hide();
                tablaFacturas.Controls.Clear();

                // Rellenar el panel secundario con las etiquetas y la tabla
                crearUIFacturas();

                // Rellenar datos tabla
                actualizarReservas();
                actualizarFacturas();

                dibujarFacturas(null, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1));

                // Buscador de facturas por nombre
                Label labelBuscadorNombre = new Label();
                labelBuscadorNombre.AutoSize = true;
                labelBuscadorNombre.Text = "Nombre: ";
                labelBuscadorNombre.BackColor = Color.FromArgb(46, 51, 73);
                labelBuscadorNombre.Font = new Font("Segoe UI", 12.0f);
                labelBuscadorNombre.ForeColor = Color.CornflowerBlue;
                labelBuscadorNombre.Location = new Point(250, 85);

                panelFacturas.Controls.Add(labelBuscadorNombre);

                textBoxRedonda buscadorNombre = new textBoxRedonda();
                buscadorNombre.Size = new Size(300, 25);
                buscadorNombre.BackColor = Color.FromArgb(0, 126, 249);
                buscadorNombre.BorderStyle = BorderStyle.None;
                buscadorNombre.TextAlign = HorizontalAlignment.Center;
                buscadorNombre.Font = new Font("Segoe UI", 12.0f);
                buscadorNombre.ForeColor = Color.White;
                buscadorNombre.Location = new Point(330, 85);

                panelFacturas.Controls.Add(buscadorNombre);                              

                // Buscador de facturas por fechas
                Label labelBuscadorFechaDe = new Label();
                labelBuscadorFechaDe.AutoSize = true;
                labelBuscadorFechaDe.Text = "Fechas de: ";
                labelBuscadorFechaDe.BackColor = Color.FromArgb(46, 51, 73);
                labelBuscadorFechaDe.Font = new Font("Segoe UI", 12.0f);
                labelBuscadorFechaDe.ForeColor = Color.CornflowerBlue;
                labelBuscadorFechaDe.Location = new Point(650, 85);

                panelFacturas.Controls.Add(labelBuscadorFechaDe);

                textBoxRedonda buscadorFechasDe = new textBoxRedonda();
                buscadorFechasDe.Size = new Size(120, 25);
                buscadorFechasDe.BackColor = Color.FromArgb(0, 126, 249);
                buscadorFechasDe.BorderStyle = BorderStyle.None;
                buscadorFechasDe.TextAlign = HorizontalAlignment.Center;
                buscadorFechasDe.Font = new Font("Segoe UI", 12.0f);
                buscadorFechasDe.ForeColor = Color.White;
                buscadorFechasDe.Location = new Point(740, 85);

                panelFacturas.Controls.Add(buscadorFechasDe);

                // Añadir icono de calendario
                Button elegirFechaDe = new Button();
                elegirFechaDe.Text = "W";
                elegirFechaDe.Size = new Size(25,25);
                elegirFechaDe.Location = new Point(860, 85);

                panelFacturas.Controls.Add(elegirFechaDe);

                elegirFechaDe.Click += (sender, e) =>
                {
                    MonthCalendar calendarioDe = new MonthCalendar();
                    calendarioDe.Location = new Point(960, 85);
                    calendarioDe.ForeColor = Color.White;
                    calendarioDe.BackColor = Color.CornflowerBlue;
                    calendarioDe.FirstDayOfWeek = Day.Monday;
                    calendarioDe.MaxSelectionCount = 1;

                    panelFacturas.Controls.Add(calendarioDe);
                    calendarioDe.BringToFront();

                    calendarioDe.MouseUp += (sender, e) => 
                    {
                        switch (calendarioDe.HitTest(e.Location).HitArea)
                        {
                            case MonthCalendar.HitArea.Date: break;
                            case MonthCalendar.HitArea.NextMonthDate: break;
                            case MonthCalendar.HitArea.PrevMonthDate: break;
                            default: return;
                        }
                        fechaEnBuscadorDe = calendarioDe.SelectionStart;
                        buscadorFechasDe.Text = calendarioDe.SelectionStart.ToString("yyyy-MMM-dd");                        
                        panelFacturas.Controls.Remove(calendarioDe);
                    };
                };

                buscadorFechasDe.TextChanged += (sender, e) =>
                {
                    dibujarFacturas(buscadorNombre.Text, fechaEnBuscadorDe, fechaEnBuscadorHasta);
                };

                // Buscador fecha hasta
                Label labelBuscadorFechaHasta = new Label();
                labelBuscadorFechaHasta.AutoSize = true;
                labelBuscadorFechaHasta.Text = "Hasta: ";
                labelBuscadorFechaHasta.BackColor = Color.FromArgb(46, 51, 73);
                labelBuscadorFechaHasta.Font = new Font("Segoe UI", 12.0f);
                labelBuscadorFechaHasta.ForeColor = Color.CornflowerBlue;
                labelBuscadorFechaHasta.Location = new Point(930, 85);

                panelFacturas.Controls.Add(labelBuscadorFechaHasta);

                textBoxRedonda buscadorFechasHasta = new textBoxRedonda();
                buscadorFechasHasta.Size = new Size(120, 25);
                buscadorFechasHasta.BackColor = Color.FromArgb(0, 126, 249);
                buscadorFechasHasta.BorderStyle = BorderStyle.None;
                buscadorFechasHasta.TextAlign = HorizontalAlignment.Center;
                buscadorFechasHasta.Font = new Font("Segoe UI", 12.0f);
                buscadorFechasHasta.ForeColor = Color.White;
                buscadorFechasHasta.Location = new Point(1000, 85);

                panelFacturas.Controls.Add(buscadorFechasHasta);

                // Añadir icono de calendario
                Button elegirFechaHasta = new Button();
                elegirFechaHasta.Text = "W";
                elegirFechaHasta.Size = new Size(25, 25);
                elegirFechaHasta.Location = new Point(1120, 85);

                panelFacturas.Controls.Add(elegirFechaHasta);

                elegirFechaHasta.Click += (sender, e) =>
                {
                    MonthCalendar calendarioHasta = new MonthCalendar();
                    calendarioHasta.Location = new Point(1120, 85);
                    calendarioHasta.ForeColor = Color.White;
                    calendarioHasta.BackColor = Color.CornflowerBlue;
                    calendarioHasta.FirstDayOfWeek = Day.Monday;
                    calendarioHasta.MaxSelectionCount = 1;

                    panelFacturas.Controls.Add(calendarioHasta);
                    calendarioHasta.BringToFront();

                    calendarioHasta.MouseUp += (sender, e) =>
                    {
                        switch (calendarioHasta.HitTest(e.Location).HitArea)
                        {
                            case MonthCalendar.HitArea.Date: break;
                            case MonthCalendar.HitArea.NextMonthDate: break;
                            case MonthCalendar.HitArea.PrevMonthDate: break;
                            default: return;
                        }
                        fechaEnBuscadorHasta = calendarioHasta.SelectionStart;
                        buscadorFechasHasta.Text = calendarioHasta.SelectionStart.ToString("yyyy-MMM-dd");                        
                        panelFacturas.Controls.Remove(calendarioHasta);
                    };
                };

                buscadorFechasHasta.TextChanged += (sender, e) =>
                {
                    dibujarFacturas(buscadorNombre.Text, fechaEnBuscadorDe, fechaEnBuscadorHasta);
                };

                // Buscar cuando se escribe algo en el buscador de nombre
                buscadorNombre.TextChanged += (sender, e) =>
                {
                    dibujarFacturas(buscadorNombre.Text, fechaEnBuscadorDe, fechaEnBuscadorHasta);
                };



                // Añadir icono de calendario
                BotonReserva btnLimpiar = new BotonReserva();
                btnLimpiar.Text = "Limpiar";
                btnLimpiar.Font = new Font("Segoe UI", 12.0f);
                btnLimpiar.ForeColor = Color.White;
                btnLimpiar.BackColor = Color.CornflowerBlue;
                btnLimpiar.Size = new Size(100, 30);
                btnLimpiar.Location = new Point(1170, 80);

                panelFacturas.Controls.Add(btnLimpiar);

                btnLimpiar.Click += (sender, e) =>
                {
                    buscadorNombre.Text = "";
                    buscadorFechasDe.Text = "";
                    buscadorFechasHasta.Text = "";

                    fechaEnBuscadorDe = new DateTime(2000, 1, 1);
                    fechaEnBuscadorHasta = new DateTime(2000, 1, 1);

                    dibujarFacturas(null, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1));
                };


                tablaFacturas.Show();
            }
            else
            {
                tablaFacturas.Controls.Clear();
                tablaFacturas.Hide();
            }
            

        }

        public int BuscarIndiceFactura(int numFactura)
        {
            int indice = 0;

            for(int i = 0; i < Facturas.Count(); i++)
            {
                if(Facturas.ElementAt(i).GetSetNumeroFactura == numFactura)
                {
                    indice = i;
                }
            }

            return indice;
        }

        //
        // Funciones para mostrar ventana de promociones
        //

        public void crearUIPromociones()
        {
            Label labelReservas = new Label();
            labelReservas.Text = "Promociones";
            labelReservas.Font = new Font("Segoe UI", 24.0f);
            labelReservas.ForeColor = Color.FromArgb(0, 126, 249);
            labelReservas.Location = new Point(20, 20);
            labelReservas.AutoSize = true;
            panelPromociones.Controls.Add(labelReservas);


            Label labelBuscar = new Label();
            labelBuscar.Text = "Buscar por nombre:";
            labelBuscar.Font = new Font("Segoe UI", 12.0f);
            labelBuscar.ForeColor = Color.CornflowerBlue;
            labelBuscar.Location = new Point(50, 80);
            labelBuscar.AutoSize = true;
            panelPromociones.Controls.Add(labelBuscar);

            Panel lineaNaranja = new Panel();
            lineaNaranja.BackColor = Color.DarkOrange;
            lineaNaranja.Location = new Point(50, 115);
            lineaNaranja.Size = new Size(800, 4);

            panelPromociones.Controls.Add(lineaNaranja);

        }

        public void mostrarPromociones(bool mostrar)
        {
            if (mostrar == true)
            {
                List<string> nombreUsuario = new List<string>();
                List<string> correoUsuario = new List<string>();

                panelPromociones.Size = new Size(1368, 679);
                panelPromociones.Location = new Point(195, 12);
                crearUIPromociones();
                actualizarReservas();

                // Añadir buscador
                textBoxRedonda buscador = new textBoxRedonda();
                buscador.BackColor = Color.RoyalBlue;
                buscador.Location = new Point(250, 80);
                buscador.Size = new Size(300, 10);
                buscador.Font = new Font("Segoe UI", 12.0f);
                buscador.ForeColor = Color.White;
                buscador.BorderStyle = BorderStyle.None;
                buscador.TextAlign = HorizontalAlignment.Center;
                panelPromociones.Controls.Add(buscador);

                // Añadir boton enviar a todos
                BotonReserva btnEnviarATodos = new BotonReserva();
                btnEnviarATodos.Size = new Size(300, 60);
                btnEnviarATodos.Location = new Point(950, 350);
                btnEnviarATodos.Text = "Enviar a todos los usuarios";
                btnEnviarATodos.ForeColor = Color.White;
                btnEnviarATodos.Font = new Font("Segoe UI", 12.0f);
                btnEnviarATodos.BackColor = Color.RoyalBlue;
                panelPromociones.Controls.Add(btnEnviarATodos);

                btnEnviarATodos.Click += (sender, e) =>
                {
                    string correos = "";

                    // bucle añadiendo todos con un ;

                    foreach(string correo in correoUsuario)
                    {
                        correos += correo + ";";
                    }

                    System.Diagnostics.Process.Start(new ProcessStartInfo("mailto:" + correos) { UseShellExecute = true });
                };


                // Añadir tabla de resultados
                TableLayoutPanel tablaPromociones = new TableLayoutPanel();
                tablaPromociones.Size = new Size(800, 500);
                tablaPromociones.Location = new Point(50, 130);
                tablaPromociones.BackColor = Color.FromArgb(24, 30, 54);

                tablaPromociones.BorderStyle = BorderStyle.FixedSingle;
                tablaPromociones.ColumnCount = 1;
                tablaPromociones.RowCount = 0;
                tablaPromociones.AutoScroll = false;
                tablaPromociones.HorizontalScroll.Enabled = false;
                tablaPromociones.HorizontalScroll.Visible = false;
                tablaPromociones.HorizontalScroll.Maximum = 0;
                tablaPromociones.AutoScroll = true;

                buscador.TextChanged += (sender, e) =>
                {
                    nombreUsuario.Clear();
                    correoUsuario.Clear();

                    tablaPromociones.Controls.Clear();
                    tablaPromociones.RowCount = 0;

                    for (int i = 0; i < Reservas.Count; i++)
                    {
                        // Comprobar que el nombre y el correo no hayan sido ya añadidos
                        // Si no lo son, añadir a los resultados
                        string nombrecompleto = Reservas.ElementAt(i).GetSetNombre + " " + Reservas.ElementAt(i).GetSetApellidos;
                        string correo = Reservas.ElementAt(i).GetSetEmail;


                        if (nombrecompleto.Contains(buscador.Text))
                        {
                            if(!nombreUsuario.Contains(nombrecompleto) || !correoUsuario.Contains(correo))
                            {
                                // Si no tenemos el correo o el nombre del siguiente, se añade

                                nombreUsuario.Add(nombrecompleto);
                                correoUsuario.Add(correo);

                                Panel nuevaFila = crearNuevaFilaUsuario(i, false);

                                tablaPromociones.RowCount += 1;
                                tablaPromociones.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                                tablaPromociones.Controls.Add(nuevaFila, 0, tablaPromociones.RowCount - 1);
                            }                            
                        }
                    }                  

                };

                // Para activar el cambio de texto y que se actualize la tabla
                string texto = buscador.Text;
                buscador.Text = "123456";
                buscador.Text = texto;

                panelPromociones.Controls.Add(tablaPromociones);

                panelPromociones.Show();
            }
            else
            {
                panelPromociones.Controls.Clear();
                panelPromociones.Hide();
            }



        }

        public Panel crearNuevaFilaUsuario(int indiceReserva, bool marcarAFacturar)
        {
            Panel nuevaFila = new Panel();
            nuevaFila.Size = new Size(750, 25);
            nuevaFila.BackColor = Color.FromArgb(150, 0, 126, 249);
            nuevaFila.BorderStyle = BorderStyle.FixedSingle;

            Label nombreUsuario = new Label();
            nombreUsuario.Font = new Font("Segoe UI", 12.0f);
            nombreUsuario.Location = new Point(0, 0);
            nombreUsuario.AutoSize = true;
            nombreUsuario.ForeColor = Color.White;
            nombreUsuario.BackColor = Color.FromArgb(0, 0, 126, 249);
            nombreUsuario.Text = Reservas.ElementAt(indiceReserva).GetSetNombre + " " + Reservas.ElementAt(indiceReserva).GetSetApellidos;
            nuevaFila.Controls.Add(nombreUsuario);

            Label correoUsuario = new Label();
            correoUsuario.Font = new Font("Segoe UI", 12.0f);
            correoUsuario.Location = new Point(320, 0);
            correoUsuario.AutoSize = true;
            correoUsuario.ForeColor = Color.White;
            correoUsuario.BackColor = Color.FromArgb(0, 0, 126, 249);
            correoUsuario.Text = Reservas.ElementAt(indiceReserva).GetSetEmail;
            nuevaFila.Controls.Add(correoUsuario);


            nuevaFila.Click += clicarUsuario;
            nombreUsuario.Click += clicarUsuario;
            correoUsuario.Click += clicarUsuario;

            nuevaFila.MouseHover += mouseHoverUsuario;
            nombreUsuario.MouseHover += mouseHoverUsuario;
            correoUsuario.MouseHover += mouseHoverUsuario;

            nuevaFila.MouseLeave += mouseLeaveUsuario;
            nombreUsuario.MouseLeave += mouseLeaveUsuario;
            correoUsuario.MouseLeave += mouseLeaveUsuario;

            void clicarUsuario(object sender, EventArgs e)
            {
                // MAILTO
                System.Diagnostics.Process.Start(new ProcessStartInfo("mailto:" + correoUsuario.Text) { UseShellExecute = true });                
            };

            void mouseHoverUsuario(object sender, EventArgs e)
            {
                nuevaFila.BackColor = Color.FromArgb(255, 0, 126, 249);
            };

            void mouseLeaveUsuario(object sender, EventArgs e)
            {
                nuevaFila.BackColor = Color.FromArgb(150, 0, 126, 249);
            };


            return nuevaFila;
        }
    }
}