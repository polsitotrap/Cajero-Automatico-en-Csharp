using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajeroJP
{
    // Clase que representa un movimiento o transacción bancaria dentro del sistema
    // Cada movimiento puede ser un depósito, retiro o consulta de saldo.
    public class Movimiento
    {
        #region Propiedades

        // Identificador único del movimiento
        public int Id { get; set; }

        // Número de cuenta al cual está asociado este movimiento
        public string NumeroCuenta { get; set; }

        // Tipo de movimiento realizado: "Depósito", "Retiro", "Consulta", etc.
        public string TipoMovimiento { get; set; }

        // Monto de dinero involucrado en la transacción
        public decimal Monto { get; set; }

        // Fecha y hora en la que se registró el movimiento
        public DateTime Fecha { get; set; }

        // Saldo de la cuenta después de realizar este movimiento
        public decimal SaldoResultante { get; set; }

        #endregion

        #region Constructores

        // Constructor por defecto
        // Se utiliza cuando se crea un movimiento sin parámetros.
        // La fecha se asigna automáticamente al momento de instanciar el objeto.
        public Movimiento()
        {
            Fecha = DateTime.Now;
        }

        // Constructor con parámetros
        // Permite crear un movimiento indicando todos sus datos principales.
        public Movimiento(int id, string numeroCuenta, string tipoMovimiento, decimal monto, decimal saldoResultante)
        {
            Id = id;
            NumeroCuenta = numeroCuenta;
            TipoMovimiento = tipoMovimiento;
            Monto = monto;
            Fecha = DateTime.Now; // La fecha siempre se establece en el momento actual
            SaldoResultante = saldoResultante;
        }

        #endregion

        #region Métodos

        // Convierte el movimiento a una cadena de texto con formato
        // El formato es: Id|NumeroCuenta|TipoMovimiento|Monto|Fecha|SaldoResultante
        // Este texto es el que se guarda en el archivo de movimientos.
        public string ConvertirATexto()
        {
            return $"{Id}|{NumeroCuenta}|{TipoMovimiento}|{Monto}|{Fecha:yyyy-MM-dd HH:mm:ss}|{SaldoResultante}";
        }

        // Crea un objeto Movimiento a partir de una línea de texto
        // La línea debe estar en el mismo formato que el generado por ConvertirATexto().
        public static Movimiento CrearDesdeTexto(string Linea)
        {
            try
            {
                // Se separan los datos usando el carácter '|'
                string[] Datos = Linea.Split('|');

                // Se asignan los valores a las propiedades del nuevo objeto Movimiento
                return new Movimiento
                {
                    Id = int.Parse(Datos[0]),
                    NumeroCuenta = Datos[1],
                    TipoMovimiento = Datos[2],
                    Monto = decimal.Parse(Datos[3]),
                    Fecha = DateTime.Parse(Datos[4]),
                    SaldoResultante = decimal.Parse(Datos[5])
                };
            }
            catch (Exception ex)
            {
                // Si ocurre un error al leer la línea, se lanza una excepción con el detalle
                throw new Exception($"Error al crear movimiento desde texto: {ex.Message}");
            }
        }

        // Devuelve una representación legible del movimiento
        // Útil para mostrarlo en la consola o en reportes.
        // Ejemplo de salida: "25/09/2025 14:30 | Depósito        | Monto: $   50,000.00 | Saldo: $  200,000.00"
        public override string ToString()
        {
            return $"{Fecha:dd/MM/yyyy HH:mm} | {TipoMovimiento,-15} | Monto: ${Monto,10:N2} | Saldo: ${SaldoResultante,10:N2}";
        }

        #endregion
    }
}
