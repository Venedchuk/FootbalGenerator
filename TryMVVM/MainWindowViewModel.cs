

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace TryMVVM
{
    public class MainWindowViewModel:ViewModelBase
    {
        private int _z { get; set;}
        public int X { get; set; }
        public int Y { get; set; }

        public int Z
        {
            get { return _z; }
            set
            {
                
                _z = value;
                RaisePropertyChanged(()=>Z);
            }
        }

        //public MainWindowViewModel()
        //{
        //    X = 5;
        //    Y = 10;
        //    Z = X + Y;
        //}

        private ICommand _calc;


        public ICommand Calc
        {
            get
            {
                if (_calc == null)
                {
                   return _calc = new RelayCommand((() =>
                   {
                       Z = X + Y;

                   }));
                }
                return _calc;
            }
        }
    }
}
