using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WireRodClassifer_1._0.Helper_Classes;
using WireRodClassifer_1._0.Model;
using HalconDotNet;

namespace WireRodClassifer_1._0.View
{
    /// <summary>
    /// Interaction logic for MainPageView.xaml
    /// </summary>
    public partial class InicioView : UserControl, IParticipant
    {
        #region Private Fields

        CancellationTokenSource cts;
        ConfigAcquisitionDevice configAcquisitionDevice;
        WorkerClass workerClass;
        HTuple window;
        IMediator mediator;
        bool isRunning = false;

        public event EventHandler<ChangeLabelStatusEventArgs> ChangeLabelEvent;


        #endregion

        #region Constructor

        public InicioView(HTuple window)
        {
            InitializeComponent();
            configAcquisitionDevice = ConfigAcquisitionDevice.Instance;
            workerClass = new WorkerClass();
            this.window = window;
            configAcquisitionDevice.IsConfiguredEvent += ConfigAcquisitionDevice_IsConfiguredEvent;
        }

        
        #endregion

        #region Helper Methods

        private void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            ChangeLabelEvent?.Invoke(this, new ChangeLabelStatusEventArgs("Processando..."));
            btnParar.IsEnabled = true;
            btnIniciar.IsEnabled = false;
            isRunning = true;
            cts = new CancellationTokenSource();
            //Thread Action = new Thread(new ThreadStart(action));
            Thread Action = new Thread(() => workerClass.Action(configAcquisitionDevice, cts.Token, window));
            Action.IsBackground = true;
            Action.Start();
        }

        private void BtnParar_Click(object sender, RoutedEventArgs e)
        {
            pararCaptura();
        }

        private void ConfigAcquisitionDevice_IsConfiguredEvent(object sender, IsConfiguredEventArgs e)
        {
            if (e.ConfigurationBool)
            {
                btnIniciar.IsEnabled = true;
                btnParar.IsEnabled = false;
            }
            else
            {
                btnIniciar.IsEnabled = false;
                btnParar.IsEnabled = false;
            }
        }

        public void ReceiveMessage(Message message)
        {
            if (message == Message.FechaCaptura)
            {
                pararCaptura();
            }
        }

        public void AddMediator(IMediator mediator)
        {
            this.mediator = mediator;
        }

        private void pararCaptura()
        {
            ChangeLabelEvent?.Invoke(this, new ChangeLabelStatusEventArgs("Pronto."));
            btnIniciar.IsEnabled = true;
            btnParar.IsEnabled = false;
            if (isRunning == true)
            {
                cts.Cancel();
                cts.Dispose();
                isRunning = false;
            }
        }

        #endregion
    }
}
