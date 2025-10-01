using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CajeroJP
{
    // Clase principal del programa.
    // Aquí está el menú principal del cajero automático y toda la lógica
    // de interacción con el usuario (inicio de sesión, depósitos, retiros, etc.).
    class Program
    {
        // Esta variable guarda la cuenta actualmente activa (sesión iniciada).
        // Si es null, significa que nadie ha iniciado sesión.
        private static Cuenta cuentaActual = null;

        // Método principal (punto de entrada del programa).
        static void Main(string[] args)
        {
            // Se configura la consola para mostrar caracteres especiales (ej: tildes, símbolos).
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool salir = false;

            // Bucle principal que mantiene activo el programa hasta que el usuario decida salir.
            while (!salir)
            {
                if (cuentaActual == null)
                {
                    // Si no hay sesión iniciada, mostramos el menú principal.
                    salir = MostrarMenuPrincipal();
                }
                else
                {
                    // Si ya hay sesión iniciada, mostramos el menú del cajero.
                    MostrarMenuCajero();
                }
            }

            // Mensaje de despedida al cerrar el programa
            Console.WriteLine("\n¡Gracias por usar nuestro cajero automático!");
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }

        #region Menús

        // Muestra el menú principal (antes de iniciar sesión).
        // Retorna true si el usuario elige salir, false si desea seguir.
        private static bool MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗"); 
            Console.WriteLine("║   CAJERO AUTOMÁTICO - MENÚ PRINCIPAL   ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine("║  1. Iniciar Sesión                     ║");
            Console.WriteLine("║  2. Crear Cuenta Nueva                 ║");
            Console.WriteLine("║  3. Salir                              ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.Write("\nSeleccione una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ProcesarInicioSesion(); // Intentar iniciar sesión
                    return false;
                case "2":
                    CrearCuentaDemo(); // Crear una cuenta nueva
                    return false;
                case "3":
                    return true; // Salir del programa
                default:
                    Console.WriteLine("\nOpción inválida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return false;
            }
        }

        // Muestra el menú del cajero (después de iniciar sesión).
        // Aquí se encuentran las operaciones principales: consultar saldo, depositar, retirar, etc.
        private static void MostrarMenuCajero()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║      CAJERO AUTOMÁTICO - MENÚ          ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║ Titular: {cuentaActual.NombreTitular,-28} ║");
            Console.WriteLine($"║ Cuenta:  {cuentaActual.NumeroCuenta,-28} ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine("║  1. Consultar Saldo                    ║");
            Console.WriteLine("║  2. Depositar                          ║");
            Console.WriteLine("║  3. Retirar                            ║");
            Console.WriteLine("║  4. Últimos 5 Movimientos              ║");
            Console.WriteLine("║  5. Cambiar Clave                      ║");
            Console.WriteLine("║  6. Cerrar Sesión                      ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.Write("\nSeleccione una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    cuentaActual.ConsultarSaldo();
                    break;
                case "2":
                    ProcesarDeposito();
                    break;
                case "3":
                    ProcesarRetiro();
                    break;
                case "4":
                    cuentaActual.MostrarUltimosMovimientos();
                    break;
                case "5":
                    ProcesarCambioClave();
                    break;
                case "6":
                    CerrarSesion();
                    return;
                default:
                    Console.WriteLine("\nOpción inválida.");
                    break;
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        #endregion

        #region Procesamiento de Operaciones

        // Permite al usuario iniciar sesión con número de cuenta y clave.
        // Si las credenciales son correctas, se guarda la cuenta en "cuentaActual".
        private static void ProcesarInicioSesion()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║          INICIO DE SESIÓN              ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();

            Console.Write("Clave: ");
            string clave = LeerClaveOculta();

            Console.WriteLine("\n\nValidando credenciales...");

            cuentaActual = Cuenta.IniciarSesion(numeroCuenta, clave);

            if (cuentaActual == null)
            {
                Console.WriteLine("\n✗ Número de cuenta o clave incorrectos.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"\n✓ ¡Bienvenido/a {cuentaActual.NombreTitular}!");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        // Procesa un depósito en la cuenta activa.
        private static void ProcesarDeposito()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║              DEPÓSITO                  ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            Console.Write("Ingrese el monto a depositar: $");
            string montoStr = Console.ReadLine();

            if (decimal.TryParse(montoStr, out decimal monto))
            {
                cuentaActual.Depositar(monto);
            }
            else
            {
                Console.WriteLine("\n✗ Monto inválido.");
            }
        }

        // Procesa un retiro de la cuenta activa.
        private static void ProcesarRetiro()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║               RETIRO                   ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            Console.WriteLine($"Saldo disponible: ${cuentaActual.Saldo:N2}\n");
            Console.Write("Ingrese el monto a retirar: $");
            string montoStr = Console.ReadLine();

            if (decimal.TryParse(montoStr, out decimal monto))
            {
                cuentaActual.Retirar(monto);
            }
            else
            {
                Console.WriteLine("\n✗ Monto inválido.");
            }
        }

        // Procesa el cambio de clave de la cuenta activa.
        private static void ProcesarCambioClave()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║           CAMBIO DE CLAVE              ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            Console.Write("Clave actual: ");
            string claveActual = LeerClaveOculta();

            Console.Write("\nNueva clave: ");
            string claveNueva = LeerClaveOculta();

            Console.Write("\nConfirme nueva clave: ");
            string claveConfirmacion = LeerClaveOculta();

            Console.WriteLine("\n");

            if (claveNueva != claveConfirmacion)
            {
                Console.WriteLine("✗ Las claves no coinciden.");
                return;
            }

            cuentaActual.CambiarClave(claveActual, claveNueva);
        }

        // Cierra la sesión de la cuenta actual.
        private static void CerrarSesion()
        {
            Console.WriteLine("\n✓ Sesión cerrada exitosamente.");
            cuentaActual = null;
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        // Permite crear una cuenta nueva (modo demostración).
        // El usuario ingresa los datos básicos de la cuenta: número, nombre, clave y saldo inicial.
        private static void CrearCuentaDemo()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║         CREAR CUENTA NUEVA             ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();

            Console.Write("Nombre del titular: ");
            string nombreTitular = Console.ReadLine();

            Console.Write("Clave (mínimo 4 dígitos): ");
            string clave = LeerClaveOculta();

            Console.Write("\n\nSaldo inicial: $");
            string saldoStr = Console.ReadLine();

            if (decimal.TryParse(saldoStr, out decimal saldo) && saldo >= 0)
            {
                Cuenta nuevaCuenta = new Cuenta(numeroCuenta, nombreTitular, clave, saldo);
                Cuenta.CrearCuenta(nuevaCuenta);
            }
            else
            {
                Console.WriteLine("\n✗ Saldo inválido.");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        #endregion

        #region Métodos Auxiliares

        // Método auxiliar para leer la clave de forma oculta (muestra * en lugar de los dígitos).
        // Devuelve la clave escrita por el usuario.
        private static string LeerClaveOculta()
        {
            string clave = "";
            ConsoleKeyInfo tecla;

            do
            {
                tecla = Console.ReadKey(true);

                if (tecla.Key != ConsoleKey.Backspace && tecla.Key != ConsoleKey.Enter)
                {
                    clave += tecla.KeyChar;
                    Console.Write("*");
                }
                else if (tecla.Key == ConsoleKey.Backspace && clave.Length > 0)
                {
                    clave = clave.Substring(0, clave.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (tecla.Key != ConsoleKey.Enter);

            return clave;
        }

        #endregion
    }
}