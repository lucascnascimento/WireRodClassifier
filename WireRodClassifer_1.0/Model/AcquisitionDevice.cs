using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace WireRodClassifer_1._0.Model
{
    public class AcquisitionDevice
    {
        private HTuple _dLLName;
        private HTuple _device;
        private HTuple _bits_per_channel;
        private HTuple _camera_type;
        private HTuple _color_Space;
        private HTuple _defaults;
        private HTuple _external_trigger;
        private HTuple _field;
        private HTuple _general;
        private HTuple _generic;
        private HTuple _horizontal_resolution;
        private HTuple _image_height;
        private HTuple _image_width;
        private HTuple _info_boards;
        private HTuple _line_in;
        private HTuple _parameters;
        private HTuple _parameters_readonly;
        private HTuple _parameters_writeonly;
        private HTuple _port;
        private HTuple _revision;
        private HTuple _start_column;
        private HTuple _start_row;
        private HTuple _vertical_resolution;

        public AcquisitionDevice()
        {

        }

        /// <summary>
        /// Nome da interface de aquisição
        /// </summary>
        public HTuple DLLName { get => _dLLName; set => _dLLName = value; }

        public HTuple Bits_per_channel { get => _bits_per_channel; set => _bits_per_channel = value; }

        public HTuple Camera_type { get => _camera_type; set => _camera_type = value; }

        public HTuple Color_Space { get => _color_Space; set => _color_Space = value; }

        public HTuple Defaults { get => _defaults; set => _defaults = value; }

        /// <summary>
        /// Dispositivo de aquisição
        /// </summary>
        public HTuple Device { get => _device; set => _device = value; }

        public HTuple External_trigger { get => _external_trigger; set => _external_trigger = value; }

        public HTuple Field { get => _field; set => _field = value; }

        public HTuple General { get => _general; set => _general = value; }

        public HTuple Generic { get => _generic; set => _generic = value; }

        public HTuple Horizontal_resolution { get => _horizontal_resolution; set => _horizontal_resolution = value; }

        public HTuple Image_height { get => _image_height; set => _image_height = value; }

        public HTuple Image_width { get => _image_width; set => _image_width = value; }

        public HTuple Info_boards { get => _info_boards; set => _info_boards = value; }

        public HTuple Line_in { get => _line_in; set => _line_in = value; }

        public HTuple Parameters { get => _parameters; set => _parameters = value; }

        public HTuple Parameters_readonly { get => _parameters_readonly; set => _parameters_readonly = value; }

        public HTuple Parameters_writeonly { get => _parameters_writeonly; set => _parameters_writeonly = value; }

        public HTuple Port { get => _port; set => _port = value; }

        public HTuple Revision { get => _revision; set => _revision = value; }

        public HTuple Start_column { get => _start_column; set => _start_column = value; }

        public HTuple Start_row { get => _start_row; set => _start_row = value; }

        public HTuple Vertical_resolution { get => _vertical_resolution; set => _vertical_resolution = value; }
    }
}
