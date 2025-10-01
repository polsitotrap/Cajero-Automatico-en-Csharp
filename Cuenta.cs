using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajeroJP
{
    // Clase que representa una cuenta bancaria dentro del sistema
    // Contiene la información del titular, su saldo y métodos para realizar operaciones.
    public class Cuenta
    {
        #region Rutas de Archivos

        // Archivo donde se almacenan todas las cuentas registradas
        private static readonly string ArchivoCuentas = Path.Combine(
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
            "cuentas.txt");

        // Archivo donde se almacenan todos los movimientos de todas las cuentas
        private static readonly string ArchivoMovimientos = Path.Combine(
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
            "movimientos.txt");

        #endregion

        #region Propiedades

        // Número único que identifica la cuenta
        public string NumeroCuenta { get; set; }

        // Nombre completo del titular de la cuenta
        public string NombreTitular { get; set; }

        // Clave de acceso (PIN) para ingresar a la cuenta
        public string Clave { get; set; }

        // Saldo disponible en la cuenta
        public decimal Saldo { get; set; }

        // Fecha en la que la cuenta fue creada
        public DateTime FechaCreacion { get; set; }

        // Indica si la cuenta está activa o inactiva
        public bool Activa { get; set; }

        #endregion

        #region Constructores

        // Constructor por defecto
        // Crea una cuenta nueva con fecha de creación actual y activa por defecto
        public Cuenta()
        {
            FechaCreacion = DateTime.Now;
            Activa = true;
        }

        // Constructor con parámetros
        // Permite crear una cuenta indicando el número, titular, clave y saldo inicial.
        public Cuenta(string numeroCuenta, string nombreTitular, string clave, decimal saldoInicial)
        {
            NumeroCuenta = numeroCuenta;
            NombreTitular = nombreTitular;
            Clave = clave;
            Saldo = saldoInicial;
            FechaCreacion = DateTime.Now;
            Activa = true;
        }

        #endregion

        #region Métodos de Autenticación

        // Inicia sesión validando el número de cuenta y la clave
        // Retorna la cuenta si las credenciales son correctas, o null si fallan
        public static Cuenta IniciarSesion(string numeroCuenta, string clave)
        {
            try
            {
                if (!File.Exists(ArchivoCuentas))
                {
                    Console.WriteLine("No existen cuentas registradas.");
                    return null;
                }

                string[] lineas = File.ReadAllLines(ArchivoCuentas);

                foreach (string linea in lineas)
                {
                    Cuenta cuenta = CrearDesdeTexto(linea);

                    if (cuenta.NumeroCuenta == numeroCuenta && cuenta.Clave == clave)
                    {
                        if (!cuenta.Activa)
                        {
                            Console.WriteLine("La cuenta se encuentra inactiva.");
                            return null;
                        }

                        return cuenta;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar sesión: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Operaciones Bancarias

        // Realiza un depósito en la cuenta
        // Aumenta el saldo, actualiza el archivo y registra el movimiento
        public bool Depositar(decimal monto)
        {
            try
            {
                if (monto <= 0)
                {
                    Console.WriteLine("El monto debe ser mayor a cero.");
                    return false;
                }

                Saldo += monto;
                ActualizarCuentaEnArchivo();
                RegistrarMovimiento("Depósito", monto);

                Console.WriteLine($"\n✓ Depósito exitoso!"); //Deposito de dinero
                Console.WriteLine($"Monto depositado: ${monto:N2}");//Monto depositado
                Console.WriteLine($"Nuevo saldo: ${Saldo:N2}");//Nuevo saldo

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar depósito: {ex.Message}");
                return false;
            }
        }

        // Realiza un retiro de la cuenta principal
        // Disminuye el saldo si hay fondos suficientes y registra el movimiento de retiro
        public bool Retirar(decimal monto)
        {
            try
            {
                if (monto <= 0)
                {
                    Console.WriteLine("El monto debe ser mayor a cero.");
                    return false;
                }

                if (monto > Saldo)
                {
                    Console.WriteLine("Saldo insuficiente para realizar el retiro.");
                    return false;
                }

                Saldo -= monto;
                ActualizarCuentaEnArchivo();
                RegistrarMovimiento("Retiro", monto);

                Console.WriteLine($"\n✓ Retiro exitoso!");
                Console.WriteLine($"Monto retirado: ${monto:N2}");
                Console.WriteLine($"Nuevo saldo: ${Saldo:N2}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar retiro: {ex.Message}");
                return false;
            }
        } //finaliza el retiro

        // Consulta el saldo actual y lo muestra en consola
        public void ConsultarSaldo()
        {
            try
            {
                RegistrarMovimiento("Consulta Saldo", 0);

                Console.WriteLine("\n╔═════════════════════════════════════╗");
                Console.WriteLine("║        CONSULTA DE SALDO            ║");
                Console.WriteLine("╠═════════════════════════════════════╣");
                Console.WriteLine($"║ Titular: {NombreTitular,-28} ║");
                Console.WriteLine($"║ Cuenta:  {NumeroCuenta,-28} ║");
                Console.WriteLine($"║ Saldo:   ${Saldo,-27:N2} ║");
                Console.WriteLine("╚═════════════════════════════════════╝");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar saldo: {ex.Message}");
            }
        }

        // Permite cambiar la clave de acceso de la cuenta
        public bool CambiarClave(string claveActual, string claveNueva)
        {
            try
            {
                if (Clave != claveActual)
                {
                    Console.WriteLine("La clave actual es incorrecta.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(claveNueva) || claveNueva.Length < 4)
                {
                    Console.WriteLine("La nueva clave debe tener al menos 4 caracteres.");
                    return false;
                }

                Clave = claveNueva;
                ActualizarCuentaEnArchivo();

                Console.WriteLine("\n✓ Clave cambiada exitosamente!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar clave: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Movimientos

        // Obtiene los últimos 5 movimientos realizados en esta cuenta
        public List<Movimiento> ObtenerUltimosMovimientos()
        {
            try
            {
                List<Movimiento> todosMovimientos = new List<Movimiento>();

                if (!File.Exists(ArchivoMovimientos))
                    return todosMovimientos;

                string[] lineas = File.ReadAllLines(ArchivoMovimientos);

                foreach (string linea in lineas)
                {
                    if (!string.IsNullOrWhiteSpace(linea))
                    {
                        Movimiento mov = Movimiento.CrearDesdeTexto(linea);
                        if (mov.NumeroCuenta == NumeroCuenta)
                        {
                            todosMovimientos.Add(mov);
                        }
                    }
                }

                return todosMovimientos.OrderByDescending(m => m.Fecha).Take(5).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener movimientos: {ex.Message}");
                return new List<Movimiento>();
            }
        }

        // Muestra en consola los últimos 5 movimientos de la cuenta
        public void MostrarUltimosMovimientos()
        {
            try
            {
                List<Movimiento> movimientos = ObtenerUltimosMovimientos();

                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                          ÚLTIMOS 5 MOVIMIENTOS                                 ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════════════════════════════╣");

                if (movimientos.Count == 0)
                {
                    Console.WriteLine("║  No hay movimientos registrados                                                ║");
                }
                else
                {
                    foreach (Movimiento mov in movimientos)
                    {
                        Console.WriteLine($"║ {mov.ToString(),-78} ║");
                    }
                }

                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar movimientos: {ex.Message}");
            }
        }

        // Registra un movimiento en el archivo de movimientos
        private void RegistrarMovimiento(string tipoMovimiento, decimal monto)
        {
            try
            {
                int nuevoId = ObtenerSiguienteIdMovimiento();
                Movimiento mov = new Movimiento(nuevoId, NumeroCuenta, tipoMovimiento, monto, Saldo);

                using (StreamWriter sw = File.AppendText(ArchivoMovimientos))
                {
                    sw.WriteLine(mov.ConvertirATexto());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar movimiento: {ex.Message}");
            }
        }

        // Obtiene el siguiente ID disponible para un movimiento
        private int ObtenerSiguienteIdMovimiento()
        {
            try
            {
                if (!File.Exists(ArchivoMovimientos))
                    return 1;

                string[] lineas = File.ReadAllLines(ArchivoMovimientos);

                if (lineas.Length == 0)
                    return 1;

                int maxId = 0;
                foreach (string linea in lineas)
                {
                    if (!string.IsNullOrWhiteSpace(linea))
                    {
                        string[] datos = linea.Split('|');
                        int id = int.Parse(datos[0]);
                        if (id > maxId)
                        {
                            maxId = id;
                        }
                    }
                }

                return maxId + 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion

        #region Persistencia en Archivos

        // Convierte la cuenta en un texto que puede guardarse en el archivo
        // Formato: NumeroCuenta|NombreTitular|Clave|Saldo|FechaCreacion|Activa
        public string ConvertirATexto()
        {
            return $"{NumeroCuenta}|{NombreTitular}|{Clave}|{Saldo}|{FechaCreacion:yyyy-MM-dd}|{Activa}";
        }

        // Crea una cuenta a partir de una línea de texto en formato plano
        public static Cuenta CrearDesdeTexto(string linea)
        {
            try
            {
                string[] datos = linea.Split('|');

                return new Cuenta
                {
                    NumeroCuenta = datos[0],
                    NombreTitular = datos[1],
                    Clave = datos[2],
                    Saldo = decimal.Parse(datos[3]),
                    FechaCreacion = DateTime.Parse(datos[4]),
                    Activa = bool.Parse(datos[5])
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear cuenta desde texto: {ex.Message}");
            }
        }

        // Actualiza los datos de esta cuenta en el archivo
        private void ActualizarCuentaEnArchivo()
        {
            try
            {
                if (!File.Exists(ArchivoCuentas))
                    return;

                string[] lineas = File.ReadAllLines(ArchivoCuentas);
                List<string> nuevasLineas = new List<string>();

                foreach (string linea in lineas)
                {
                    Cuenta cuenta = CrearDesdeTexto(linea);

                    if (cuenta.NumeroCuenta == NumeroCuenta)
                        nuevasLineas.Add(ConvertirATexto());
                    else
                        nuevasLineas.Add(linea);
                }

                File.WriteAllLines(ArchivoCuentas, nuevasLineas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar cuenta: {ex.Message}");
            }
        }

        // Crea una nueva cuenta y la guarda en el archivo
        public static bool CrearCuenta(Cuenta cuenta)
        {
            try
            {
                if (File.Exists(ArchivoCuentas))
                {
                    string[] lineas = File.ReadAllLines(ArchivoCuentas);
                    foreach (string linea in lineas)
                    {
                        Cuenta cuentaExistente = CrearDesdeTexto(linea);
                        if (cuentaExistente.NumeroCuenta == cuenta.NumeroCuenta)
                        {
                            Console.WriteLine("Ya existe una cuenta con ese número.");
                            return false;
                        }
                    }
                }

                using (StreamWriter sw = File.AppendText(ArchivoCuentas))
                {
                    sw.WriteLine(cuenta.ConvertirATexto());
                }

                Console.WriteLine("Cuenta creada exitosamente!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear cuenta: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}