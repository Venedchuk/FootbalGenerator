
using GalaSoft.MvvmLight;

namespace TrySomeoneElsse1
{
    public class Team:ObservableObject
    {
        private int _count;
        public string Name { get; set; }

        public int Count
        {
            get
            {
                return _count;
                
            }
            set
            {
                _count = value;
                RaisePropertyChanged(() => Count);
            }
        }
    }
}
