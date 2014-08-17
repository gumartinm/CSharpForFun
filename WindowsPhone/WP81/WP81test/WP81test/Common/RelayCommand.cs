using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WP81test.Common
{
    /// <summary>
    /// Comando cuyo único fin es transmitir su funcionalidad 
    /// a otros objetos mediante la invocación de delegados. 
    /// El valor devuelto predeterminado para el método CanExecute es 'true'.
    /// <see cref="RaiseCanExecuteChanged"/> debe llamarse siempre
    /// Se espera que <see cref="CanExecute"/> devuelva un valor distinto.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Se desencadena al llamar a RaiseCanExecuteChanged.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Crea un nuevo comando que puede ejecutarse siempre.
        /// </summary>
        /// <param name="execute">Lógica de ejecución.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Crea un nuevo comando.
        /// </summary>
        /// <param name="execute">Lógica de ejecución.</param>
        /// <param name="canExecute">Lógica de estado de ejecución.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si <see cref="RelayCommand"/> puede ejecutarse en su estado actual.
        /// </summary>
        /// <param name="parameter">
        /// Datos usados por el comando. Si el comando no requiere que se pasen datos, este objeto puede establecerse en null.
        /// </param>
        /// <returns>true si se puede ejecutar este comando; en caso contrario, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        /// <summary>
        /// Ejecuta <see cref="RelayCommand"/> en el destino del comando actual.
        /// </summary>
        /// <param name="parameter">
        /// Datos usados por el comando. Si el comando no requiere que se pasen datos, este objeto puede establecerse en null.
        /// </param>
        public void Execute(object parameter)
        {
            _execute();
        }

        /// <summary>
        /// Método usado para generar el evento <see cref="CanExecuteChanged"/>
        /// para indicar que el valor devuelto de <see cref="CanExecute"/>
        /// ha cambiado.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}