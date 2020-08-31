using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    class GetCameras
    {
        public List<KeyValuePair<int, string>> CameraList()
        {

            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();
            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);   //ищем камеры

            int _DeviceIndex = 0;   //записываем в лист, начиная индекс с 0
            foreach (DsDevice _Camera in _SystemCamereas)
            {
                ListCamerasData.Add(new KeyValuePair<int, string>(_DeviceIndex, _Camera.Name));
                _DeviceIndex++;
            }
            return ListCamerasData;    //возвращаем лист
        }
    }
}
