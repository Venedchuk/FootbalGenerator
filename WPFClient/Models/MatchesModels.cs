using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace WPFClient.Models
{
    public class SimpleMatchClient : ObservableObject
    {
        public Guid Id { get; set; }
        private int? _homeTeamGoals;
        private int? _guestTeamGoals;
        public string Home { get; set; }
        public string Guest { get; set; }

        public BitmapImage HomeBitmapImage { get; set; }

        public BitmapImage GuestBitmapImage { get; set; }

        public int? HomeTeamGoals
        {

            get { return _homeTeamGoals; }
            set
            {
                _homeTeamGoals = value;
                RaisePropertyChanged(() => HomeTeamGoals);
                MainWindowViewModel.channel.ChangeMatch(MainWindowViewModel.ToSimpleMatch(this));
            }

        }

        public int? GuestTeamGoals
        {
            get { return _guestTeamGoals; }
            set
            {
                _guestTeamGoals = value;
                RaisePropertyChanged(() => GuestTeamGoals);
                MainWindowViewModel.channel.ChangeMatch(MainWindowViewModel.ToSimpleMatch(this));
            }

        }
        
    }
    public class SimpleTourClient : ObservableObject
    {
        public Guid Id { get; set; }

        public string NameTour { get; set; }

        public Guid  SeasonId { get; set; }

        public ObservableCollection<SimpleMatchClient> Matches { get; set; }
    }
    public class SimpleSeasonsClient : ObservableObject
    {
        public Guid Id { get; set; }
        public Guid ChampionshipId { get; set; }
        public string Name { get; set; }
        public ObservableCollection<SimpleTourClient> Tours { get; set; }
    }
    public class SimpleChampionshipClient : ObservableObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<SimpleSeasonsClient> Seasons { get; set; }
    }

}
