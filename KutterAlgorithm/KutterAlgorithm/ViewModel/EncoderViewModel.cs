using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using KutterAlgorithm.Encoders;

namespace KutterAlgorithm.ViewModel
{
    public class EncoderViewModel : ViewModelBase
    {
        public string Name { get; private set; }

        private readonly Func<IEncoder> _getInstanceFunc;

        public EncoderViewModel(string name, Func<IEncoder> getInstanceFunc)
        {
            Name = name;
            _getInstanceFunc = getInstanceFunc;
        }

        public IEncoder GetEncoderInstance()
        {
            if (_getInstanceFunc != null)
            {
                return _getInstanceFunc();
            }
            return null;
        }
    }
}
