using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OperationWithTeams;
using WPFClient.Models;
using System.Collections.ObjectModel;

namespace WpfRedactor
{
   public partial class MainWindowRedactorViewModel 
    {
        private RelayCommand _toListTeam;
        public RelayCommand ToListTeam
        {
            get
            {
                return _toListTeam ?? (_toListTeam = new RelayCommand(() =>
                {

                    TeamsToGenerate.Add(SelectedTeamPorted);
                    TeamsFromClient.Remove(SelectedTeamPorted);
                    try
                    {
                        //  SelectedTeam = Teams[0];//delete later
                    }
                    catch (Exception)
                    {

                    }

                },
                 () => Validation(SelectedTeamPorted)
                ));


            }
        }

        private RelayCommand _fromList;
        public RelayCommand FromList
        {
            get
            {
                return _fromList ?? (_fromList = new RelayCommand(() =>
                {
                    TeamsFromClient.Add(SelectedTeamToGenerate);
                    TeamsToGenerate.Remove(SelectedTeamToGenerate);

                },
                () => Validation(SelectedTeamToGenerate)
                ));


            }
        }
        private SimpleChampionshipClient _selectedChampionshipClient;

        public SimpleChampionshipClient SelectedChampionship
        {
            get
            {
                return _selectedChampionshipClient;
            }
            set
            {
                if (_selectedChampionshipClient == value) return;

                _selectedChampionshipClient = value;


                RaisePropertyChanged("SelectedChampionship");
            }

        }
        private RelayCommand _generateTour;
        public RelayCommand GenerateTour
        {
            get
            {
                return _generateTour ?? (_generateTour = new RelayCommand(() =>
                {

                    GoGenerate(SelectedChamp.Id);
                    MainWindowRedactorViewModelConstruct();

                }
                , () => { return SelectedChamp != null && (TeamsToGenerate.Count > 1 && SelectedChamp.Tours == null); }));

                // SelectedChampionship.Tours.Count == 0 && 
            }
        }

        private void GoGenerate(Guid champId)
        {
            _countMatches = 0;
            var tour = 0;
            container = new List<Match>();
            var teams = ToTeam(TeamsToGenerate);
            if (teams != null)

            {
                foreach (var item in teams)
                {
                    Channel.TeamAddToChamp(champId, item.Id);
                }


                Console.WriteLine("-------------Start generation Matches------------");
                Console.WriteLine("Generetion for :" + SelectedChamp.Name);
                var rnd = new Random();

                var teamWithKeys = new Dictionary<Team, List<bool>>();

                for (var i = 0; i < teams.Count; i++)
                {
                    var value = new List<bool>();
                    for (var j = 0; j < teams.Count; j++)
                    {
                        if (teams[j] == teams[i])
                            value.Add(true);
                        else
                            value.Add(false);
                    }
                    teamWithKeys.Add(teams[i], value);
                    //full template

                }
                FirstTour(teamWithKeys, tour);


            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Teams empty, do you want to close this window?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }

            }
        }

        private static int CountMatches(Dictionary<Team, List<bool>> teamWithKeys)
        {
            var a = Factorial(teamWithKeys.Count) / (2 * Factorial(teamWithKeys.Count - 2));
            return a;
        }

        private static int Factorial(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            else if (x == 1 || x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }
        private void FirstTour(Dictionary<Team, List<bool>> teamWithKeys, int tourCounter)
        {
            _containerTour = new List<Match>();
            tourCounter++;
            var rnd = new Random();
            var alreadyPlay = new bool[teamWithKeys.Count];
            Array.Clear(alreadyPlay, 0, alreadyPlay.Length);


            var antivis = 0;
            while (true)
            {

                if (antivis == 3000)
                {
                    antivis = 0;
                    break;
                }

                antivis++;
                var scnd = rnd.Next(alreadyPlay.Length);
                var third = rnd.Next(alreadyPlay.Length);

                if ((alreadyPlay[scnd] == false) && (alreadyPlay[third] == false) &&
                    (teamWithKeys.ElementAt(scnd).Value[third] == false) &&
                    (teamWithKeys.ElementAt(third).Value[scnd] == false))
                {
                    alreadyPlay[scnd] = true;
                    alreadyPlay[third] = true;
                    teamWithKeys.ElementAt(scnd).Value[third] = true;
                    teamWithKeys.ElementAt(third).Value[scnd] = true;

                    var match = new Match();
                    match.Home = teamWithKeys.ElementAt(scnd).Key;
                    match.Guest = teamWithKeys.ElementAt(third).Key;

                    match.Id = Guid.NewGuid();
                    container.Add(match);
                    _containerTour.Add(match);
                    Console.WriteLine(match.Home.Name + " vs " + match.Guest.Name);
                    _countMatches++;

                }


            }
            if (_countMatches == CountMatches(teamWithKeys))
            {
                var tourser = new Tour()
                {
                    NameTour = tourCounter.ToString(),

                };


                Channel.AddTour(tourser, SelectedChamp.Id, _containerTour);

                MainWindowRedactorViewModelConstruct();
                return;
            }
            else
            {
                var tourser = new Tour()
                {
                    NameTour = tourCounter.ToString(),

                };


                Channel.AddTour(tourser, SelectedChamp.Id, _containerTour);
                FirstTour(teamWithKeys, tourCounter);
            }
        }
        private static bool Validation(SimpleTeam selectedTeam)
        {
            if (selectedTeam == null)
                return false;
            if (selectedTeam.Id == new Guid())
                    return false;

            return true;
        }

       public static List<Team> ToTeam(ObservableCollection<SimpleTeam> selectedTeams)
        {
            var teams = new List<Team>();
            foreach (var item in selectedTeams)
            {

                var oneTeam = new Team()
                {
                    Name = item.Name,
                    Id = item.Id,
                    Country = item.Country

                };
                teams.Add(oneTeam);
            }

            return teams;
        }
    }
}
