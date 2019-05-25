﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WireRodClassifer_1._0.Model;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public class WorkerClass : BackgroundWorker
    {
        #region Private Fields
        HTuple window;
        CancellationToken token;
        IMediator mediator;
        private HDevelopExport HDevExp;

        #endregion

        #region Constructor

        public WorkerClass()
        {
            HDevExp = new HDevelopExport();
        }

        #endregion

        #region Methods

        public void Action(ConfigAcquisitionDevice currentDevice, CancellationToken token, HTuple window)
        {
            this.token = token;
            this.window = window;
            //executeCapturingAndProcessing(currentDevice);
            HTuple hv_AcqHandle = null;
            hv_AcqHandle = setOpenFrameGrabber(currentDevice);
            if (hv_AcqHandle != null)
            {
                HDevExp.RunHalcon(window, hv_AcqHandle, token, currentDevice.FolderPath);
            }
            else
            {
                Thread.CurrentThread.Abort();
            }
            //!Pode ocorrer da thread não fechar quando a captura não for bem sucedida
        }

        /// <summary>
        /// Atenção, esse método é executado somente durante Design Time e não representa a o método de execução final
        /// </summary>
        /// <param name="currentDevice"></param>
        private void executeCapturingAndProcessing(ConfigAcquisitionDevice currentDevice)
        {
            // TODO: O opengramgrabber está recebendo uma informação pré-estabelecida, trocar pela informação enviada pelo usuário.
            // Local iconic variables 

            HObject ho_Image = null;

            // Local control variables 

            HTuple hv_AcqHandle = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            //Image Acquisition 01: Code generated by Image Acquisition 01
            hv_AcqHandle = setOpenFrameGrabber(currentDevice);
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

            while ((int)(1) != 0)
            {
                ho_Image.Dispose();
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                //Image Acquisition 01: Do something
                HOperatorSet.DispObj(ho_Image, window);
                //hWindowControlWPF1.HDisplayCurrentObject = ho_Image;
                if (token.IsCancellationRequested)
                {
                    HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                    ho_Image.Dispose();
                    break;
                }
            }
        }

        private HTuple setOpenFrameGrabber(ConfigAcquisitionDevice currentDevice)
        {
            HTuple hv_AcqHandle;
            if (currentDevice.IsDefault)
            {
                try
                {
                    HOperatorSet.OpenFramegrabber(currentDevice.DLLName, currentDevice.Defaults[0], currentDevice.Defaults[1], currentDevice.Defaults[2], currentDevice.Defaults[3],
                                currentDevice.Defaults[4], currentDevice.Defaults[5], currentDevice.Defaults[6], currentDevice.Defaults[7], currentDevice.Defaults[8],
                                currentDevice.Defaults[9], currentDevice.Defaults[10], currentDevice.Defaults[11], currentDevice.Defaults[12], currentDevice.Defaults[13], currentDevice.Defaults[14], out hv_AcqHandle);
                    return hv_AcqHandle;
                }
                catch (Exception e)
                {

                    MessageBox.Show("Não foi possível iniciar configurar a captura de imagem. Erro: " + e.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return hv_AcqHandle = null;
                }
            }
            else
            {
                try
                {
                    HOperatorSet.OpenFramegrabber(currentDevice.DLLName, currentDevice.Horizontal_resolution, currentDevice.Vertical_resolution, currentDevice.Image_width,
                                currentDevice.Image_height, currentDevice.Start_row, currentDevice.Start_column, currentDevice.Field, currentDevice.Bits_per_channel, currentDevice.Color_Space,
                                currentDevice.Generic, currentDevice.External_trigger, currentDevice.Camera_type, currentDevice.Device, currentDevice.Port, currentDevice.Line_in, out hv_AcqHandle);
                    return hv_AcqHandle;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Não foi possível iniciar configurar a captura de imagem. Erro: " + e.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return hv_AcqHandle = null;
                }
            }
        }

        #endregion
    }
}
