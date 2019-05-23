using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WireRodClassifer_1._0.Model;
using WireRodClassifer_1._0.Helper_Classes;
using System.Collections.Specialized;

namespace WireRodClassifer_1._0.View
{
    /// <summary>
    /// Interaction logic for ConfigDispAquiView.xaml
    /// </summary>
    public partial class ConfigDispAquiView : UserControl, IParticipant
    {
        #region Private Fields

        private string[] acqDLLNames;
        private string[] infoQueries;
        HTuple valueList;
        ObservableCollection<AcquisitionDevice> observableAcquisitionDevices;
        private AcquisitionDevice _currentDevice;
        private ObservableCollection<string> _device;
        private ObservableCollection<int> _port;
        private ObservableCollection<string> _cameraType;
        private ObservableCollection<int> _resolutionX;
        private ObservableCollection<int> _resolutionY;
        private ObservableCollection<string> _colorSpace;
        private ObservableCollection<string> _field;
        private ObservableCollection<int> _bitDepth;
        private ObservableCollection<string> _hInterfacesDLLName;
        ConfigAcquisitionDevice configAcquisitionDevice;
        private ObservableCollection<double> _generic;
        private IMediator mediator;

        public event EventHandler<ChangePageEventArgs> ChangePageEvent;
        public event EventHandler<ChangeLabelStatusEventArgs> ChangeLabelEvent;
        //public event EventHandler<SendMessageEventArgs> SendMessage;

        #endregion

        #region Constructor

        public ConfigDispAquiView()
        {
            InitializeComponent();
            DataContext = this;

            initializeAcqDLLNames();
            initializeInfoQueries();

            configAcquisitionDevice = ConfigAcquisitionDevice.Instance;
            Console.WriteLine("Child view " + configAcquisitionDevice.GetHashCode());

            observableAcquisitionDevices = new ObservableCollection<AcquisitionDevice>();
            CurrentDevice = new AcquisitionDevice();
            Device = new ObservableCollection<string>();
            Port = new ObservableCollection<int>();
            CameraType = new ObservableCollection<string>();
            ResolutionX = new ObservableCollection<int>();
            ResolutionY = new ObservableCollection<int>();
            ColorSpace = new ObservableCollection<string>();
            Field = new ObservableCollection<string>();
            BitDepth = new ObservableCollection<int>();
            HInterfacesDLLName = new ObservableCollection<string>();
            Generic = new ObservableCollection<double>();
            loadSettings();
        }

        #endregion

        #region Properties

        public ObservableCollection<AcquisitionDevice> ObservableAcquisitionDevices { get => observableAcquisitionDevices; set => observableAcquisitionDevices = value; }
        public AcquisitionDevice CurrentDevice { get => _currentDevice; set => _currentDevice = value; }

        //Campos do ComboBox
        public ObservableCollection<string> Device { get => _device; set => _device = value; }
        public ObservableCollection<int> Port { get => _port; set => _port = value; }
        public ObservableCollection<string> CameraType { get => _cameraType; set => _cameraType = value; }
        public ObservableCollection<int> ResolutionX { get => _resolutionX; set => _resolutionX = value; }
        public ObservableCollection<int> ResolutionY { get => _resolutionY; set => _resolutionY = value; }
        public ObservableCollection<string> ColorSpace { get => _colorSpace; set => _colorSpace = value; }
        public ObservableCollection<string> Field { get => _field; set => _field = value; }
        public ObservableCollection<int> BitDepth { get => _bitDepth; set => _bitDepth = value; }
        public ObservableCollection<string> HInterfacesDLLName { get => _hInterfacesDLLName; set => _hInterfacesDLLName = value; }
        public ObservableCollection<double> Generic { get => _generic; set => _generic = value; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Auto detecção das interfaces
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAutoDetect_Click(object sender, RoutedEventArgs e)
        {
            clearComboBoxesCollections();
            gpBoxConexao.IsEnabled = false;
            btnAutoDetect.IsEnabled = false;
            ChangeLabelEvent?.Invoke(this, new ChangeLabelStatusEventArgs("Procurando..."));
            List<AcquisitionDevice> result = await Task.Run(() => getInfo(acqDLLNames));
            if (result == null)
                MessageBox.Show("Nenhuma interface de aquisição encontrada", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                listToObservableCollection(ObservableAcquisitionDevices, result);
                foreach (AcquisitionDevice item in ObservableAcquisitionDevices)
                {
                    for (int i = 0; i < item.DLLName.Length; i++)
                    {
                        HInterfacesDLLName.Add(item.DLLName[i]);
                    }
                }
            }
            btnAutoDetect.IsEnabled = true;
            ChangeLabelEvent?.Invoke(this, new ChangeLabelStatusEventArgs("Pronto."));
        }

        /// <summary>
        /// Quando o usuário seleciona uma interface este método copia as informações para o singleton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbboxInterfaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string CurrentInterfaceName;
            gpBoxConexao.IsEnabled = true;
            CurrentInterfaceName = cbboxInterfaces.SelectedItem as string;
            foreach (AcquisitionDevice item in ObservableAcquisitionDevices) // Procura qual acquisition device possui o nome da Interface escolhida pelo usuário
            {
                for (int i = 0; i < item.DLLName.Length; i++)
                {
                    if (item.DLLName[i] == CurrentInterfaceName)
                    {
                        CurrentDevice = item;
                        copyDeviceToSingleton(configAcquisitionDevice, CurrentDevice);
                    }
                }
            }
            copyInfoToObservableCollections(CurrentDevice);
            btnConectar.IsEnabled = true;
        }

        /// <summary>
        /// Trata o evento de marcar e desmarcar o CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkboxDefault_Click(object sender, RoutedEventArgs e)
        {
            bool aux = (bool)chkboxDefault.IsChecked;
            if (aux)
            {
                enableDisableComboBoxes(false);
            }
            else
            {
                enableDisableComboBoxes(true);
            }
        }

        /// <summary>
        /// Limpa as ObservableCollections e deleta as informações do singleton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            clearComboBoxesCollections();
            clearSingleton();
            gpBoxConexao.IsEnabled = false;
            cbboxInterfaces.IsEnabled = true;
            btnAutoDetect.IsEnabled = true;
            chkboxDefault.IsEnabled = true;
            sendMessage(Message.FechaCaptura);
        }

        /// <summary>
        /// Copia as informações escolhidas pelo usuário para o singleton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            configAcquisitionDevice.IsDefault = (bool)chkboxDefault.IsChecked;
            configAcquisitionDevice.IsConfigured = true;
            fromUserToSingleton();
            ChangePageEvent?.Invoke(this, new ChangePageEventArgs());
        }

        //TODO: fazer pegar o valor default caso seja null
        //!Este método não foi implementado em nenhuma instância de combobox
        private void GetAllComboBoxes_Click(object sender, RoutedEventArgs e)
        {
            fromUserToSingleton();
        }

        /// <summary>
        /// Inicializa os nomes das bibliotecas DLL suportadas pelo Halcon
        /// </summary>
        private void initializeAcqDLLNames()
        {
            /* As interfaces "DirectFile" e "File" foram excluídas devido ao fato de não serem necessárias para a realização do
             * produto final, tendo em vista que se tratam de interfaces para concatenar o openframegrabber à arquivos do tipo
             * de vídeo e fotos salvos no HD do computador*/
            acqDLLNames = new string[38]{ "1394IIDC", "ABS", "ADLINK", "Andor", "Argos3D-P1xx", "BitFlow",
                "Crevis", "DahengCAM", "DirectShow", "Ensenso-NxLib", "GenICamTL",
                "GigEVision2", "Ginga++", "GingaDG", "heliCamC3", "LinX", "LPS36", "LuCam", "MatrixVisionAcquire",
                "MILLite", "MultiCam", "O3D3xx", "Opteon", "PixeLINK", "pylon", "SaperaLT", "Sentech", "ShapeDrive",
                "SICK-3DCamera", "SICK-ScanningRuler", "SiliconSoftware", "Slink", "SwissRanger", "TWAIN", "uEye",
                "USB3Vision", "Video4Linux2", "VRmUsbCam"};
        }

        /// <summary>
        /// Inicializa os parâmetros de pesquisa do operador InfoFrameGrabber
        /// </summary>
        private void initializeInfoQueries()
        {
            infoQueries = new string[21] { "bits_per_channel", "camera_type", "color_space", "defaults", "device",
                "external_trigger", "field", "general", "generic", "horizontal_resolution", "image_height", "image_width",
                "info_boards", "parameters", "parameters_readonly", "parameters_writeonly", "port", "revision", "start_column",
                "start_row", "vertical_resolution" };
        }

        /// <summary>
        /// Adquire as informações das câmeras disponíveis
        /// </summary>
        /// <param name="acqDLLNames">Vetor com o nome das DLL's aceitas pelo Halcon</param>
        private List<AcquisitionDevice> getInfo(string[] acqDLLNames)
        {
            List<AcquisitionDevice> acquisitionDevices = new List<AcquisitionDevice>();
            for (int i = 0; i < acqDLLNames.Length; i++)
            {
                try
                {
                    HInfo.InfoFramegrabber(acqDLLNames[i], "info_boards", out valueList);  // Pesquisa interfaces de aquisição instaladas
                    if (valueList != null)
                    {
                        acquisitionDevices.Add(new AcquisitionDevice { DLLName = acqDLLNames[i] });
                        for (int j = 0; j < infoQueries.Length; j++)
                        {
                            HInfo.InfoFramegrabber(acqDLLNames[i], infoQueries[j], out valueList);
                            switch (infoQueries[j]) //! Consertar as àspas na string do Device
                            {
                                case "bits_per_channel":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Bits_per_channel = valueList;
                                    break;
                                case "camera_type":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Camera_type = valueList;
                                    break;
                                case "color_space":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Color_Space = valueList;
                                    break;
                                case "defaults":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Defaults = valueList;
                                    break;
                                case "device":
                                    //string aux = acqDLLNames[i].Replace("\"", "");
                                    acquisitionDevices[acquisitionDevices.Count - 1].Device = valueList;
                                    break;
                                case "external_trigger":
                                    acquisitionDevices[acquisitionDevices.Count - 1].External_trigger = valueList;
                                    break;
                                case "field":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Field = valueList;
                                    break;
                                case "general":
                                    acquisitionDevices[acquisitionDevices.Count - 1].General = valueList;
                                    break;
                                case "generic":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Generic = valueList;
                                    break;
                                case "horizontal_resolution":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Horizontal_resolution = valueList;
                                    break;
                                case "image_height":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Image_height = valueList;
                                    break;
                                case "image_width":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Image_width = valueList;
                                    break;
                                case "info_boards":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Info_boards = valueList;
                                    break;
                                case "parameters":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Parameters = valueList;
                                    break;
                                case "parameters_readonly":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Parameters_readonly = valueList;
                                    break;
                                case "parameters_writeonly":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Parameters_writeonly = valueList;
                                    break;
                                case "port":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Port = valueList;
                                    break;
                                case "revision":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Revision = valueList;
                                    break;
                                case "start_column":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Start_column = valueList;
                                    break;
                                case "start_row":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Start_row = valueList;
                                    break;
                                case "vertical_resolution":
                                    acquisitionDevices[acquisitionDevices.Count - 1].Vertical_resolution = valueList;
                                    break;
                                default:
                                    Console.WriteLine("Couldn't find: " + infoQueries[j]);
                                    break;
                            }
                        }
                        if (acquisitionDevices[acquisitionDevices.Count - 1].Info_boards.TupleLength() == 0) // Deleta as interfaces instaladas que não tem nenhum device conectado
                            acquisitionDevices.RemoveAt(acquisitionDevices.Count - 1);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(acqDLLNames[i] + " not installed.");
                }
            }
            return acquisitionDevices;
        }

        /// <summary>
        /// Transforma uma Lista do tipo AcquisitionDevice em uma ObservableCollection do tipo AcquisitionDevice
        /// </summary>
        /// <param name="acqDevOB"></param>
        /// <param name="acqDevLst"></param>
        private void listToObservableCollection(ObservableCollection<AcquisitionDevice> acqDevOB, List<AcquisitionDevice> acqDevLst)
        {
            if (acqDevLst != null)
            {
                foreach (AcquisitionDevice item in acqDevLst)
                {
                    acqDevOB.Add(item);
                }
            }
        }

        /// <summary>
        /// Recebe a interface escolhida pelo usuário através de um objeto <see cref="AcquisitionDevice"/> e prepara os dados para exibição em um combobox
        /// </summary>
        /// <param name="acquisitionDevice"></param>
        private void copyInfoToObservableCollections(AcquisitionDevice acquisitionDevice)
        {
            //Device
            if (acquisitionDevice.Device.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Device.Length; i++)
                {
                    Device.Add(acquisitionDevice.Device[i]);
                    cbboxDevice.SelectedIndex = 0;
                    cbboxDevice.IsEnabled = true;
                }
            }
            else
            {
                cbboxDevice.IsEnabled = false;
                Device.Clear();
            }

            //Port
            if (acquisitionDevice.Port.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Port.Length; i++)
                {
                    Port.Add(acquisitionDevice.Port[i]);
                    cbboxPort.IsEnabled = true;
                    cbboxPort.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxPort.IsEnabled = false;
                Port.Clear();
            }

            //Camera Type
            if (acquisitionDevice.Camera_type.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Camera_type.Length; i++)
                {
                    CameraType.Add(acquisitionDevice.Camera_type[i]);
                    cbboxCamType.IsEnabled = true;
                    cbboxCamType.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxCamType.IsEnabled = false;
                CameraType.Clear();
            }

            //Horizontal Resolution
            if (acquisitionDevice.Horizontal_resolution.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Horizontal_resolution.Length; i++)
                {
                    ResolutionX.Add(acquisitionDevice.Horizontal_resolution[i]);
                    cbboxX.IsEnabled = true;
                    cbboxX.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxX.IsEnabled = false;
                ResolutionX.Clear();
            }

            //Vertical Resolution
            if (acquisitionDevice.Vertical_resolution.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Vertical_resolution.Length; i++)
                {
                    ResolutionY.Add(acquisitionDevice.Vertical_resolution[i]);
                    cbboxY.IsEnabled = true;
                    cbboxY.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxY.IsEnabled = false;
                ResolutionY.Clear();
            }

            //Color Space
            if (acquisitionDevice.Color_Space.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Color_Space.Length; i++)
                {
                    ColorSpace.Add(acquisitionDevice.Color_Space[i]);
                    cbboxColorSpace.IsEnabled = true;
                    cbboxColorSpace.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxColorSpace.IsEnabled = false;
                ColorSpace.Clear();
            }

            //Field
            if (acquisitionDevice.Field.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Field.Length; i++)
                {
                    Field.Add(acquisitionDevice.Field[i]);
                    cbboxField.IsEnabled = true;
                    cbboxField.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxField.IsEnabled = false;
                Field.Clear();
            }

            //Bits per channel/depth
            if (acquisitionDevice.Bits_per_channel.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Bits_per_channel.Length; i++)
                {
                    BitDepth.Add(acquisitionDevice.Bits_per_channel[i]);
                    cbboxBitDepth.IsEnabled = true;
                    cbboxBitDepth.SelectedIndex = 0;
                }
            }
            else
            {
                cbboxBitDepth.IsEnabled = false;
                BitDepth.Clear();
            }

            //Generic
            if (acquisitionDevice.Generic.Length > 0)
            {
                for (int i = 0; i < acquisitionDevice.Generic.Length; i++)
                {
                    Generic.Add(acquisitionDevice.Generic[i]);
                }
                cbboxGeneric.IsEnabled = true;
                cbboxGeneric.SelectedIndex = 0;
            }
            else
            {
                cbboxGeneric.IsEnabled = false;
                Generic.Clear();
            }
        }

        /// <summary>
        /// Limpa as propriedades que estão conectadas aos comboboxes
        /// </summary>
        private void clearComboBoxesCollections()
        {
            ObservableAcquisitionDevices.Clear();
            HInterfacesDLLName.Clear();
            Device.Clear();
            Port.Clear();
            CameraType.Clear();
            ResolutionX.Clear();
            ResolutionY.Clear();
            ColorSpace.Clear();
            Field.Clear();
            BitDepth.Clear();
            Generic.Clear();
        }

        /// <summary>
        /// Habilita e desabilita os ComboBoxes
        /// </summary>
        /// <param name="isEnabled"> Se for true habilita, se for false desabilita</param>
        private void enableDisableComboBoxes(bool isEnabled)
        {
            cbboxBitDepth.IsEnabled = isEnabled;
            cbboxCamType.IsEnabled = isEnabled;
            cbboxColorSpace.IsEnabled = isEnabled;
            cbboxDevice.IsEnabled = isEnabled;
            cbboxField.IsEnabled = isEnabled;
            cbboxGeneric.IsEnabled = isEnabled;
            cbboxPort.IsEnabled = isEnabled;
            cbboxX.IsEnabled = isEnabled;
            cbboxY.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Copia os parâmetros da Interface de acquisição escolhida pelo usuário para o singleton que guarda as configurações gerais
        /// </summary>
        /// <param name="configAcquisitionDevice"></param>
        /// <param name="acquisitionDevice"></param>
        private void copyDeviceToSingleton(ConfigAcquisitionDevice configAcquisitionDevice, AcquisitionDevice acquisitionDevice)
        {
            configAcquisitionDevice.Bits_per_channel = acquisitionDevice.Bits_per_channel;
            configAcquisitionDevice.Camera_type = acquisitionDevice.Camera_type;
            configAcquisitionDevice.Color_Space = acquisitionDevice.Color_Space;
            configAcquisitionDevice.Defaults = acquisitionDevice.Defaults;
            configAcquisitionDevice.Device = acquisitionDevice.Device;
            configAcquisitionDevice.DLLName = acquisitionDevice.DLLName;
            configAcquisitionDevice.External_trigger = acquisitionDevice.External_trigger;
            configAcquisitionDevice.Field = acquisitionDevice.Field;
            configAcquisitionDevice.General = acquisitionDevice.General;
            configAcquisitionDevice.Generic = acquisitionDevice.Generic;
            configAcquisitionDevice.Horizontal_resolution = acquisitionDevice.Horizontal_resolution;
            configAcquisitionDevice.Image_height = acquisitionDevice.Image_height;
            configAcquisitionDevice.Image_width = acquisitionDevice.Image_width;
            configAcquisitionDevice.Info_boards = acquisitionDevice.Info_boards;
            configAcquisitionDevice.Line_in = acquisitionDevice.Line_in;
            configAcquisitionDevice.Parameters = acquisitionDevice.Parameters;
            configAcquisitionDevice.Parameters_readonly = acquisitionDevice.Parameters_readonly;
            configAcquisitionDevice.Parameters_writeonly = acquisitionDevice.Parameters_writeonly;
            configAcquisitionDevice.Port = acquisitionDevice.Port;
            configAcquisitionDevice.Revision = acquisitionDevice.Revision;
            configAcquisitionDevice.Start_column = acquisitionDevice.Start_column;
            configAcquisitionDevice.Start_row = acquisitionDevice.Start_row;
            configAcquisitionDevice.Vertical_resolution = acquisitionDevice.Vertical_resolution;
        }

        /// <summary>
        /// Copia os dados inseridos pelo usuário para o singleton. Se não tiver o dado ele copia o valor default
        /// </summary>
        private void fromUserToSingleton()
        {
            //Bits per channel
            if (BitDepth.Count > 0)
            {
                configAcquisitionDevice.Bits_per_channel = Convert.ToInt32(cbboxBitDepth.Text);
                Properties.Settings.Default.BitDeptSetting = configAcquisitionDevice.Bits_per_channel;
            }
            else
            {
                configAcquisitionDevice.Bits_per_channel = configAcquisitionDevice.Defaults[7];
                Properties.Settings.Default.BitDeptSetting = configAcquisitionDevice.Bits_per_channel;
            }

            //CameraType
            if (CameraType.Count > 0)
            {
                configAcquisitionDevice.Camera_type = (cbboxCamType.Text);
                Properties.Settings.Default.CameraTypeSetting = configAcquisitionDevice.Camera_type;
            }
            else
            {
                configAcquisitionDevice.Camera_type = configAcquisitionDevice.Defaults[11];
                Properties.Settings.Default.CameraTypeSetting = configAcquisitionDevice.Camera_type;
            }

            //ColorSpace
            if (ColorSpace.Count > 0)
            {
                configAcquisitionDevice.Color_Space = (cbboxColorSpace.Text);
                Properties.Settings.Default.ColorSpaceSetting = configAcquisitionDevice.Color_Space;
            }
            else
            {
                configAcquisitionDevice.Color_Space = configAcquisitionDevice.Defaults[8];
                Properties.Settings.Default.ColorSpaceSetting = configAcquisitionDevice.Color_Space;
            }

            //Device
            if (Device.Count > 0)
            {
                configAcquisitionDevice.Device = (cbboxDevice.Text);
                Properties.Settings.Default.DeviceSetting = configAcquisitionDevice.Device;
            }
            else
            {
                configAcquisitionDevice.Device = configAcquisitionDevice.Defaults[12];
                Properties.Settings.Default.DeviceSetting = configAcquisitionDevice.Device;
            }

            //DLLName
            if(HInterfacesDLLName.Count > 0)
            {
                configAcquisitionDevice.DLLName = (cbboxInterfaces.Text);
                Properties.Settings.Default.DLLNameSetting = configAcquisitionDevice.DLLName;
            }

            //Field
            if (Field.Count > 0)
            {
                configAcquisitionDevice.Field = (cbboxField.Text);
                Properties.Settings.Default.FieldSetting = configAcquisitionDevice.Field;
            }
            else
            {
                configAcquisitionDevice.Field = configAcquisitionDevice.Defaults[6];
                Properties.Settings.Default.FieldSetting = configAcquisitionDevice.Field;
            }

            //Generic
            if (Generic.Count > 0)
            {
                configAcquisitionDevice.Generic = Convert.ToInt64(cbboxGeneric.Text);
                Properties.Settings.Default.GenericSetting = configAcquisitionDevice.Generic;
            }
            else
            {
                configAcquisitionDevice.Generic = configAcquisitionDevice.Defaults[9];
                Properties.Settings.Default.GenericSetting = configAcquisitionDevice.Generic;
            }

            //HorizontalResolution
            if (ResolutionX.Count > 0)
            {
                configAcquisitionDevice.Horizontal_resolution = Convert.ToInt32(cbboxX.Text);
                Properties.Settings.Default.ResolutionXSetting = configAcquisitionDevice.Horizontal_resolution;
            }
            else
            {
                configAcquisitionDevice.Horizontal_resolution = configAcquisitionDevice.Defaults[0];
                Properties.Settings.Default.ResolutionXSetting = configAcquisitionDevice.Horizontal_resolution;
            }

            //Port
            if (Port.Count > 0)
            {
                configAcquisitionDevice.Port = Convert.ToInt32(cbboxPort.Text);
                Properties.Settings.Default.PortSetting = configAcquisitionDevice.Port;
            }
            else
            {
                configAcquisitionDevice.Port = configAcquisitionDevice.Defaults[13];
                Properties.Settings.Default.PortSetting = configAcquisitionDevice.Port;
            }

            //VerticalResolution
            if (ResolutionY.Count > 0)
            {
                configAcquisitionDevice.Vertical_resolution = Convert.ToInt32(cbboxY.Text);
                Properties.Settings.Default.ResolutionYSetting = configAcquisitionDevice.Vertical_resolution;
            }
            else
            {
                configAcquisitionDevice.Vertical_resolution = configAcquisitionDevice.Defaults[1];
                Properties.Settings.Default.ResolutionYSetting = configAcquisitionDevice.Vertical_resolution;
            }

            //Image width
            configAcquisitionDevice.Image_width = configAcquisitionDevice.Defaults[2];
            Properties.Settings.Default.ImageWidthSetting = configAcquisitionDevice.Image_width;

            //Image height
            configAcquisitionDevice.Image_height = configAcquisitionDevice.Defaults[3];
            Properties.Settings.Default.ImageHeightSetting = configAcquisitionDevice.Image_height;

            //Start Row
            configAcquisitionDevice.Start_row = configAcquisitionDevice.Defaults[4];
            Properties.Settings.Default.StartRowSetting = configAcquisitionDevice.Start_row;

            //Start Column
            configAcquisitionDevice.Start_column = configAcquisitionDevice.Defaults[5];
            Properties.Settings.Default.StartColumnSetting = configAcquisitionDevice.Start_column;

            //External Trigger
            configAcquisitionDevice.External_trigger = configAcquisitionDevice.Defaults[10];
            Properties.Settings.Default.ExternalTriggerSetting = configAcquisitionDevice.External_trigger;

            //Line In
            configAcquisitionDevice.Line_in = configAcquisitionDevice.Defaults[14];
            Properties.Settings.Default.LineInSetting = configAcquisitionDevice.Line_in;

            Properties.Settings.Default.default0 = configAcquisitionDevice.Defaults[0];
            Properties.Settings.Default.default1 = configAcquisitionDevice.Defaults[1];
            Properties.Settings.Default.default2 = configAcquisitionDevice.Defaults[2];
            Properties.Settings.Default.default3 = configAcquisitionDevice.Defaults[3];
            Properties.Settings.Default.default4 = configAcquisitionDevice.Defaults[4];
            Properties.Settings.Default.default5 = configAcquisitionDevice.Defaults[5];
            Properties.Settings.Default.default6 = configAcquisitionDevice.Defaults[6];
            Properties.Settings.Default.default7 = configAcquisitionDevice.Defaults[7];
            Properties.Settings.Default.default8 = configAcquisitionDevice.Defaults[8];
            Properties.Settings.Default.default9 = configAcquisitionDevice.Defaults[9];
            Properties.Settings.Default.default10 = configAcquisitionDevice.Defaults[10];
            Properties.Settings.Default.default11 = configAcquisitionDevice.Defaults[11];
            Properties.Settings.Default.default12 = configAcquisitionDevice.Defaults[12];
            Properties.Settings.Default.default13 = configAcquisitionDevice.Defaults[13];
            Properties.Settings.Default.default14 = configAcquisitionDevice.Defaults[14];

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Limpa todos os campos HTuple do singleton e seu campos particulares
        /// </summary>
        private void clearSingleton()
        {
            configAcquisitionDevice.Bits_per_channel = null;
            configAcquisitionDevice.Camera_type = null;
            configAcquisitionDevice.Color_Space = null;
            configAcquisitionDevice.Defaults = null;
            configAcquisitionDevice.Device = null;
            configAcquisitionDevice.DLLName = null;
            configAcquisitionDevice.External_trigger = null;
            configAcquisitionDevice.Field = null;
            configAcquisitionDevice.General = null;
            configAcquisitionDevice.Generic = null;
            configAcquisitionDevice.Horizontal_resolution = null;
            configAcquisitionDevice.Image_height = null;
            configAcquisitionDevice.Image_width = null;
            configAcquisitionDevice.Info_boards = null;
            configAcquisitionDevice.Line_in = null;
            configAcquisitionDevice.Parameters = null;
            configAcquisitionDevice.Parameters_readonly = null;
            configAcquisitionDevice.Parameters_writeonly = null;
            configAcquisitionDevice.Port = null;
            configAcquisitionDevice.Revision = null;
            configAcquisitionDevice.Start_column = null;
            configAcquisitionDevice.Start_row = null;
            configAcquisitionDevice.Vertical_resolution = null;

            configAcquisitionDevice.IsConfigured = false;
            Properties.Settings.Default.IsConfiguredSetting = false;
            Properties.Settings.Default.Save();

            configAcquisitionDevice.IsDefault = false;
            Properties.Settings.Default.IsConfiguredSetting = false;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Habilita/Desabilita o groupBox dependendo das configurações salvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfiDispAquiView_Loaded(object sender, RoutedEventArgs e)
        {
            //loadSettings();
            if (configAcquisitionDevice.IsConfigured)
            {
                copyInfoToObservableCollections(CurrentDevice);
            }
        }

        /// <summary>
        /// Carrega as cnfigurações salvas no HD
        /// </summary>
        private void loadSettings()
        {
            if (configAcquisitionDevice.IsConfigured)
            {
                CurrentDevice.Device = (Properties.Settings.Default.DeviceSetting);
                CurrentDevice.Port = (Properties.Settings.Default.PortSetting);
                CurrentDevice.Camera_type = (Properties.Settings.Default.CameraTypeSetting);
                CurrentDevice.Horizontal_resolution = (Properties.Settings.Default.ResolutionXSetting);
                CurrentDevice.Vertical_resolution = (Properties.Settings.Default.ResolutionYSetting);
                CurrentDevice.Color_Space = (Properties.Settings.Default.ColorSpaceSetting);
                CurrentDevice.Field = (Properties.Settings.Default.FieldSetting);
                CurrentDevice.Bits_per_channel = (Properties.Settings.Default.BitDeptSetting);
                CurrentDevice.DLLName = (Properties.Settings.Default.DLLNameSetting);
                CurrentDevice.Generic = (Properties.Settings.Default.GenericSetting);
                CurrentDevice.Defaults = new HTuple();
                CurrentDevice.Defaults[0] = Properties.Settings.Default.default0;
                CurrentDevice.Defaults[1] = Properties.Settings.Default.default1;
                CurrentDevice.Defaults[2] = Properties.Settings.Default.default2;
                CurrentDevice.Defaults[3] = Properties.Settings.Default.default3;
                CurrentDevice.Defaults[4] = Properties.Settings.Default.default4;
                CurrentDevice.Defaults[5] = Properties.Settings.Default.default5;
                CurrentDevice.Defaults[6] = Properties.Settings.Default.default6;
                CurrentDevice.Defaults[7] = Properties.Settings.Default.default7;
                CurrentDevice.Defaults[8] = Properties.Settings.Default.default8;
                CurrentDevice.Defaults[9] = Properties.Settings.Default.default9;
                CurrentDevice.Defaults[10] = Properties.Settings.Default.default10;
                CurrentDevice.Defaults[11] = Properties.Settings.Default.default11;
                CurrentDevice.Defaults[12] = Properties.Settings.Default.default12;
                CurrentDevice.Defaults[13] = Properties.Settings.Default.default13;
                CurrentDevice.Defaults[14] = Properties.Settings.Default.default14;


                copyDeviceToSingleton(configAcquisitionDevice, CurrentDevice);
                //copyInfoToObservableCollections(CurrentDevice);
                fromUserToSingleton();

                enableDisableComboBoxes(false);
                chkboxDefault.IsEnabled = false;
                btnAutoDetect.IsEnabled = false;
                cbboxInterfaces.IsEnabled = false;
                btnConectar.IsEnabled = false;
            }
            else
            {
                gpBoxConexao.IsEnabled = false;
            }
        }

        /// <summary>
        /// Invoca o evento SendMessage
        /// </summary>
        /// <param name="message"></param>
        private void sendMessage(Message message)
        {
            //SendMessage?.Invoke(this, new SendMessageEventArgs(message));
            mediator.BroadcastMessage(message);
        }

        public void ReceiveMessage(Message message)
        {
            //SendMessage?.Invoke(this, new SendMessageEventArgs(message));
        }

        public void AddMediator(IMediator mediator)
        {
            this.mediator = mediator;
        }
        #endregion
    }
}
