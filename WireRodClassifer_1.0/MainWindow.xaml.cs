using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HalconDotNet;
using WireRodClassifer_1._0.View;
using WireRodClassifer_1._0.Model;
using WireRodClassifer_1._0.Helper_Classes;

namespace WireRodClassifer_1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Private Fields

        HTuple window;
        ConfigDispAquiView ConfigDispAquiView; // Configuration Display Aquisition View
        InicioView InicioView;
        ConfigAcquisitionDevice configAcquisitionDevice;
        ConcreteMediator concreteMediator;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            concreteMediator = new ConcreteMediator();
            ConfigDispAquiView = new ConfigDispAquiView();
            configAcquisitionDevice = ConfigAcquisitionDevice.Instance;
            Console.WriteLine("Main view "+configAcquisitionDevice.GetHashCode());
            ConfigDispAquiView.ChangePageEvent += GoToMain_ChangePageEvent;
            ConfigDispAquiView.ChangeLabelEvent += ConfigDispAquiView_ChangeLabelEvent;
            Console.WriteLine("Main window criada");
            //contentMain.Content = ConfigDispAquiView;

            concreteMediator.AddParticipant(ConfigDispAquiView);
            ConfigDispAquiView.AddMediator(concreteMediator);
        }

        #endregion

        #region Helper Methods

        private void HalconWindow_HInitWindow(object sender, EventArgs e)
        {
            window = HalconWindow.HalconID;
            InicioView = new InicioView(window); //Main Page View
            InicioView.ChangeLabelEvent += ConfigDispAquiView_ChangeLabelEvent;
            InicioView.AddMediator(concreteMediator);
            concreteMediator.AddParticipant(InicioView);
            configAcquisitionDevice.FireConfigurationChangedEvent();
            if (HalconAPI.isWindows) // Necessário para fazer o gerenciamento de threads das janelas
                HOperatorSet.SetSystem("use_window_thread", "true");
            Console.WriteLine("Halcon Window loaded");
            contentMain.Content = InicioView;
        }

        private void MenuItemDispAqui_Click(object sender, RoutedEventArgs e)
        {
            contentMain.Content = ConfigDispAquiView;
        }

        private void MenuHeaderInicio_Click(object sender, RoutedEventArgs e)
        {
            contentMain.Content = InicioView;
        }

        private void GoToMain_ChangePageEvent(object sender, Helper_Classes.ChangePageEventArgs e)
        {
            contentMain.Content = InicioView;
        }

        private void ConfigDispAquiView_ChangeLabelEvent(object sender, Helper_Classes.ChangeLabelStatusEventArgs e)
        {
            labelStatus.Content = e.Status;
        }


        #endregion
    }
   
}
