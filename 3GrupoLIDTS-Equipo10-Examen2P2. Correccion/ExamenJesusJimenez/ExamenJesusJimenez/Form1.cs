using System;
using System.Data;
using System.IO.Ports;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ExamenJesusJimenez
{
    public partial class Form1 : Form
    {
        public SerialPort ArduinoPort { get; }
        public MySqlConnection _connection;
        private string connectionString = "server=localhost;database=formulario;uid=root;pwd=1993";

        public Form1()
        {
            InitializeComponent();

          
            _connection = new MySqlConnection(connectionString);

            ArduinoPort = new SerialPort("COM4", 9600);
            ArduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;

        
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Por favor, ingrese un nombre antes de guardar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          
            if (!ArduinoPort.IsOpen)
            {
                ArduinoPort.Open();
            }

            double temperatura;
            string estado = "Activo";

        
            if (double.TryParse(lblTemperatureFahrenheit.Text.Replace("Temperatura (°F): ", ""), out temperatura))
            {
                RegistrarEnBaseDeDatos(nombre, temperatura, estado); // Guardamos los datos en la base de datos
                MessageBox.Show("Datos guardados :)", "Bien hecho", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error al leer la temperatura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = ArduinoPort.ReadLine().Trim();
                double temperatura;

         
                Console.WriteLine("Datos recibidos: " + data);

                if (double.TryParse(data, out temperatura))
                {
                    lblTemperatureFahrenheit.Invoke((MethodInvoker)(() =>
                    {
                        lblTemperatureFahrenheit.Text = "Temperatura (°F): " + temperatura;
                    }));
                }
                else
                {
                    
                    Console.WriteLine("Error: No se pudo convertir a double. Dato recibido: " + data);
                }
            }
            catch (Exception ex)
            {
              
                Console.WriteLine("Error en DataReceivedHandler: " + ex.Message);
            }
        }

        private void RegistrarEnBaseDeDatos(string nombre, double temperatura, string estado)
        {
            string query = "INSERT INTO datos (nombre, temperatura, estado_servomotor, fecha) " +
                           "VALUES (@nombre, @temperatura, @estado, NOW()) " +
                           "ON DUPLICATE KEY UPDATE " +
                           "temperatura = @temperatura, estado_servomotor = @estado, fecha = NOW()";

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@temperatura", temperatura);
                cmd.Parameters.AddWithValue("@estado", estado);

                try
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al insertar datos: " + ex.Message);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            txtFecha.Clear();
            lblTemperatureFahrenheit.Text = "Temperatura (°F):";
        }

        private void txtNombre_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo de nombre no puede estar vacío.", "Alto ahí", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
            }
        }

        private void txtFecha_Leave(object sender, EventArgs e)
        {
            DateTime fecha;
            if (!DateTime.TryParseExact(txtFecha.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                MessageBox.Show("Ingrese la fecha en formato dd/mm/yyyy.", "Formato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFecha.Focus();
            }
        }

        private void servON_Click(object sender, EventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                ArduinoPort.Write("f"); // Enviar comando 'f' para encender el servomotor
                MessageBox.Show("Comando enviado: Encender servomotor");
            }
            else
            {
                MessageBox.Show("El puerto no está abierto. Asegúrese de conectar el Arduino.");
            }
        }

        private void servOFF_Click(object sender, EventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                ArduinoPort.Write("e"); // Enviar comando 'e' para apagar el servomotor
                MessageBox.Show("Comando enviado: Apagar servomotor");
            }
            else
            {
                MessageBox.Show("El puerto no está abierto. Asegúrese de conectar el Arduino.");
            }
        }
    }
}



