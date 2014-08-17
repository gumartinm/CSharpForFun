using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WP81test.Common
{
    /// <summary>
    /// NavigationHelper es útil para la navegación entre páginas.  Proporciona comandos que se usan para 
    /// navegar hacia atrás y hacia adelante, además de registros de los accesos directos 
    /// de mouse y teclado estándar usados para retroceder y avanzar en Windows y el botón del hardware para retroceder en
    /// Windows Phone. Además, integra SuspensionManager para controlar la administración del
    /// estado y de la duración de los procesos al navegar entre páginas.
    /// </summary>
    /// <example>
    /// Para usar NavigationHelper, realice los dos pasos siguientes o bien
    /// empiece a usar BasicPage o cualquier otra plantilla de elemento de página distinta de BlankPage.
    /// 
    /// 1) Crear una instancia de NavigationHelper en algún lugar como 
    ///     el constructor de la página y registrar una devolución de llamada para los eventos LoadState y 
    ///     SaveState.
    /// <code>
    ///     public MyPage()
    ///     {
    ///         this.InitializeComponent();
    ///         var navigationHelper = new NavigationHelper(this);
    ///         this.navigationHelper.LoadState += navigationHelper_LoadState;
    ///         this.navigationHelper.SaveState += navigationHelper_SaveState;
    ///     }
    ///     
    ///     private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    ///     { }
    ///     private async void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
    ///     { }
    /// </code>
    /// 
    /// 2) Registrar la página para llamar a NavigationHelper siempre que la página participe 
    ///     en la navegación invalidando los eventos <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo"/> 
    ///     y <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom"/>.
    /// <code>
    ///     protected override void OnNavigatedTo(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedTo(e);
    ///     }
    ///     
    ///     protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedFrom(e);
    ///     }
    /// </code>
    /// </example>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame { get { return this.Page.Frame; } }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NavigationHelper"/>.
        /// </summary>
        /// <param name="page">Referencia a la página actual usada para la navegación.  
        /// Esta referencia permite la manipulación de marcos y ayuda a garantizar que las solicitudes 
        /// de navegación con el teclado solo se producen cuando la página ocupa la ventana completa.</param>
        public NavigationHelper(Page page)
        {
            this.Page = page;

            // Cuando esta página forma parte del árbol visual, realizar dos cambios:
            // 1) Asignar el estado de vista de la aplicación a un estado visual para la página
            // 2) Controlar las solicitudes de navegación del hardware
            this.Page.Loaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#else
                // La navegación con el teclado y el mouse se aplica únicamente cuando se ocupa toda la ventana
                if (this.Page.ActualHeight == Window.Current.Bounds.Height &&
                    this.Page.ActualWidth == Window.Current.Bounds.Width)
                {
                    // Escuchar a la ventana directamente de forma que el foco no es necesario
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
#endif
            };

            // Deshacer los cambios cuando la página ya no está visible
            this.Page.Unloaded += (sender, e) =>
            {
#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#else
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
#endif
            };
        }

        #region Admitir navegación

        RelayCommand _goBackCommand;
        RelayCommand _goForwardCommand;

        /// <summary>
        /// <see cref="RelayCommand"/> usado para enlazar a la propiedad Command del botón Atrás
        /// para navegar al elemento más reciente del historial de navegación hacia atrás, si un objeto Frame
        /// administra su propio historial de navegación.
        /// 
        /// El objeto <see cref="RelayCommand"/> está configurado para usar el método virtual <see cref="GoBack"/>
        /// como acción de ejecución y <see cref="CanGoBack"/> para CanExecute.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }
                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }
        /// <summary>
        /// <see cref="RelayCommand"/> usado para navegar al elemento más reciente en 
        /// el historial de navegación hacia delante, si un objeto Frame administra su propio historial de navegación.
        /// 
        /// El objeto <see cref="RelayCommand"/> está configurado para usar el método virtual <see cref="GoForward"/>
        /// como acción de ejecución y <see cref="CanGoForward"/> para CanExecute.
        /// </summary>
        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }
                return _goForwardCommand;
            }
        }

        /// <summary>
        /// Método virtual usado por la propiedad <see cref="GoBackCommand"/>
        /// para determinar si el objeto <see cref="Frame"/> puede retroceder.
        /// </summary>
        /// <returns>
        /// true si el objeto <see cref="Frame"/> tiene al menos una entrada 
        /// en el historial de navegación hacia atrás.
        /// </returns>
        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }
        /// <summary>
        /// Método virtual usado por la propiedad <see cref="GoForwardCommand"/>
        /// para determinar si el objeto <see cref="Frame"/> puede avanzar.
        /// </summary>
        /// <returns>
        /// true si el objeto <see cref="Frame"/> tiene al menos una entrada 
        /// en el historial de navegación hacia delante.
        /// </returns>
        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        /// <summary>
        /// Método virtual usado por la propiedad <see cref="GoBackCommand"/>
        /// para invocar el método <see cref="Windows.UI.Xaml.Controls.Frame.GoBack"/>.
        /// </summary>
        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
        /// <summary>
        /// Método virtual usado por la propiedad <see cref="GoForwardCommand"/>
        /// para invocar el método <see cref="Windows.UI.Xaml.Controls.Frame.GoForward"/>.
        /// </summary>
        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Se invoca cuando se presiona el botón para retroceder del hardware. Solo para Windows Phone.
        /// </summary>
        /// <param name="sender">Instancia que desencadena el evento.</param>
        /// <param name="e">Datos de evento que describen las condiciones que dan lugar al evento.</param>
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                this.GoBackCommand.Execute(null);
            }
        }
#else
        /// <summary>
        /// Se invoca en cada pulsación de tecla, incluidas las teclas del sistema como combinaciones de teclas Alt, cuando
        /// esta página está activa y ocupa toda la ventana.  Se usa para detectar la navegación con el teclado
        /// entre páginas incluso cuando la página no tiene el foco.
        /// </summary>
        /// <param name="sender">Instancia que desencadena el evento.</param>
        /// <param name="e">Datos de evento que describen las condiciones que dan lugar al evento.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Investigar más solo cuando se presionan las teclas Izquierda, Derecha o las teclas
            // dedicadas Repág o Avpág
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // Cuando se presionan las teclas Repág o Alt+Izquierda, navegar hacia atrás
                    e.Handled = true;
                    this.GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // Cuando se presionan las teclas Avpág o Alt+Derecha, navegar hacia delante
                    e.Handled = true;
                    this.GoForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Se invoca en cada clic del mouse, punteo en la pantalla táctil o una interacción equivalente cuando esta
        /// página está activa y ocupa toda la ventana.  Se usa para detectar los clics de botón del mouse
        /// siguiente y anterior del estilo del explorador para navegar entre páginas.
        /// </summary>
        /// <param name="sender">Instancia que desencadena el evento.</param>
        /// <param name="e">Datos de evento que describen las condiciones que dan lugar al evento.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Omitir la presión simultánea de botones con los botones Izquierda, Derecha y Medio
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // Si se presiona Repág o Avpág (pero no ambos), navegar adecuadamente
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.GoBackCommand.Execute(null);
                if (forwardPressed) this.GoForwardCommand.Execute(null);
            }
        }
#endif

        #endregion

        #region Administración de la duración de los procesos

        private String _pageKey;

        /// <summary>
        /// Registre este evento en la página actual para rellenar la página
        /// con contenido pasado durante la navegación, así como cualquier estado
        /// guardado proporcionado al recrear una página a partir de una sesión anterior.
        /// </summary>
        public event LoadStateEventHandler LoadState;
        /// <summary>
        /// Registre este evento en la página actual para mantener
        /// el estado asociado a la página actual en caso de que
        /// la aplicación se suspenda o la página se descarte de
        /// la memoria caché de navegación.
        /// </summary>
        public event SaveStateEventHandler SaveState;

        /// <summary>
        /// Se invoca cuando esta página se va a mostrar en un objeto Frame.  
        /// Este método llama a <see cref="LoadState"/>, donde debe incluirse
        /// toda la lógica de navegación y administración de la duración de los procesos específica de la página.
        /// </summary>
        /// <param name="e">Datos de evento que describen cómo se llegó a esta página.  La propiedad Parameter
        /// proporciona el grupo que se va a mostrar.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Borrar el estado existente para la navegación hacia delante cuando se agregue una nueva página a la
                // pila de navegación
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Pasar el parámetro de navegación a la nueva página
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                // Pasar el parámetro de navegación y el estado de página mantenido a la página usando
                // la misma estrategia para cargar el estado suspendido y volver a crear las páginas descartadas
                // a partir de la memoria caché
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]));
                }
            }
        }

        /// <summary>
        /// Se invoca cuando esta página deja de estar visible en un marco.
        /// Este método llama a <see cref="SaveState"/>, donde debe incluirse
        /// toda la lógica de navegación y administración de la duración de los procesos específica de la página.
        /// </summary>
        /// <param name="e">Datos de evento que describen cómo se llegó a esta página.  La propiedad Parameter
        /// proporciona el grupo que se va a mostrar.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            if (this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// Representa el método que controlará el evento <see cref="NavigationHelper.LoadState"/>
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Representa el método que controlará el evento <see cref="NavigationHelper.SaveState"/>
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Clase usada para contener los datos de evento requeridos cuando una página intenta cargar el estado.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// Valor de parámetro pasado a <see cref="Frame.Navigate(Type, Object)"/> 
        /// cuando se solicitó inicialmente esta página.
        /// </summary>
        public Object NavigationParameter { get; private set; }
        /// <summary>
        /// Diccionario del estado mantenido por esta página durante una sesión
        /// anterior.  Será null la primera vez que se visite una página.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LoadStateEventArgs"/>.
        /// </summary>
        /// <param name="navigationParameter">
        /// Valor de parámetro pasado a <see cref="Frame.Navigate(Type, Object)"/> 
        /// cuando se solicitó inicialmente esta página.
        /// </param>
        /// <param name="pageState">
        /// Diccionario del estado mantenido por esta página durante una sesión
        /// anterior.  Será null la primera vez que se visite una página.
        /// </param>
        public LoadStateEventArgs(Object navigationParameter, Dictionary<string, Object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }
    /// <summary>
    /// Clase usada para contener los datos de evento requeridos cuando una página intenta guardar el estado.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Diccionario vacío para rellenar con un estado serializable.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SaveStateEventArgs"/>.
        /// </summary>
        /// <param name="pageState">Diccionario vacío para rellenar con un estado serializable.</param>
        public SaveStateEventArgs(Dictionary<string, Object> pageState)
            : base()
        {
            this.PageState = pageState;
        }
    }
}
