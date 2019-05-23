using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireRodClassifer_1._0.Helper_Classes;
using System.Configuration;
using HalconDotNet;

namespace WireRodClassifer_1._0.Model
{
    public class ConfigAcquisitionDevice : AcquisitionDevice
    {

        #region Private Fields

        private static ConfigAcquisitionDevice _instance;
        private bool _isConfigured;
        private bool _isDefault;
        public event EventHandler<IsConfiguredEventArgs> IsConfiguredEvent;
        private HTuple _window;


        #endregion

        #region Singleton Constructor

        private ConfigAcquisitionDevice() : base()
        {
            IsConfigured = Properties.Settings.Default.IsConfiguredSetting;
            IsDefault = Properties.Settings.Default.IsDefaultSetting;
            configurationChanged(IsConfigured);
            Console.WriteLine("Configacquisitiondevice criado");
        }

        public static ConfigAcquisitionDevice Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigAcquisitionDevice();
                }
                return _instance;
            }
        }

        #endregion

        #region Properties

        public bool IsConfigured
        {
            get => _isConfigured;
            set
            {
                if (value != _isConfigured)
                {
                    _isConfigured = value;
                    Properties.Settings.Default.IsConfiguredSetting = value;
                    Properties.Settings.Default.Save();
                    configurationChanged(value);
                    Console.WriteLine("chamei a prop Isconfigured");
                }

            }
        }

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                Properties.Settings.Default.IsDefaultSetting = value;
                Properties.Settings.Default.Save();
                if (value == true)
                {
                    configurationChanged(value); // Avisa para a MainPageView que existe uma configuração
                }
            }
        }

        public HTuple Window { get => _window; set => _window = value; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Raise Configuration Changed Event
        /// </summary>
        /// <param name="value"></param>
        private void configurationChanged(bool value)
        {
            IsConfiguredEvent?.Invoke(this, new IsConfiguredEventArgs(value));
        }

        /// <summary>
        /// Ativa o evento para as classes que escutam o ConfigurationChanged
        /// </summary>
        public void FireConfigurationChangedEvent()
        {
            configurationChanged(IsConfigured);
        }

        #endregion
    }
}
