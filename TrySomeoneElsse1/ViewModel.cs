
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace TrySomeoneElsse1
{
    public class ViewModel:ViewModelBase
    {
        public string Team { get { return "Team"; } }
        public ObservableCollection<Team> Teams { get; set; }

        public ViewModel()
        {
            Teams = new ObservableCollection<Team>()
            {
                new Team() {Count = 22, Name= "Dynamo"},
                new Team() {Count = 10, Name = "Shahtar"}
            };
        }



        Team _selectedTeam;
        public Team SelectedTeam
        {
            get
            {
                return _selectedTeam;
            }
            set
            {
                if (_selectedTeam != value)
                {
                    _selectedTeam = value;
                    RaisePropertyChanged("SelectedTeam");
                }
            }
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(() =>
                {
                    Teams.Add(new Team() { Count = 33, Name = "lastbook" });
                   // Teams.First().Count = 10;
                }, () => _selectedTeam != null));
            }
        }

        private ICommand _saveCommand;
        public ICommand Save
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(() =>
                {
                   
                }));
            }
        }
    }
}
