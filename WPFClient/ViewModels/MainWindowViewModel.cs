
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OperationWithTeams;
using WPFClient.Models;


namespace WPFClient
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static IContract channel;
        private int _countMatches = 0;
      //  private List<Match> globalContainerMatches = new List<Match>();
        private List<Match> container = new List<Match>();
        private List<Match> _containerTour = new List<Match>();
        public ObservableCollection<SimpleChampionshipClient> Championships { get; set; }
        public ObservableCollection<SimpleSeasonsClient> Seasons { get; set; }
        public ObservableCollection<SimpleTourClient> Tours { get; set; }
        public ObservableCollection<SimpleTeam> TeamsToGenerate { get; set; }
        public ObservableCollection<SimpleTeam> Teams { get; set; }
        public SimpleTeam SelectedTeam { get; set; }
        public SimpleTeam SelectedTeamToGenerate { get; set; }
        public DataGrid dataGrid { get; set; }
        public DataGrid dataGridPoint { get; set; }
        private List<Team> teams = new List<Team>();
        

        public MainWindowViewModel()
        {
            var adress = new Uri("http://localhost:8000/IContract");
            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IContract>(binding, new EndpointAddress(adress));
            channel = factory.CreateChannel();
            Seasons = new ObservableCollection<SimpleSeasonsClient>();
            Championships = new ObservableCollection<SimpleChampionshipClient>();
           
            TeamsToGenerate = new ObservableCollection<SimpleTeam>();
            Teams = new ObservableCollection<SimpleTeam>(channel.GetAllTeam());
            SelectedTeam = new SimpleTeam();
            SelectedTeamToGenerate = new SimpleTeam();
            Tours = new ObservableCollection<SimpleTourClient>();
            Seasons = GetSelectedSeasons();
            //SelectedSeasons = Seasons[0];
           // Championships = GetAllChampionships();
            dataGrid = new DataGrid();
            dataGridPoint = new DataGrid();
        }

        private void MainWindowViewModelConstructor()
        {
            Seasons.Clear();
            Seasons = GetSelectedSeasons();
        }



        private SimpleSeasonsClient _selectedSeasonsClient;

        public SimpleSeasonsClient SelectedSeasons
        {
            get
            {
                return _selectedSeasonsClient;
            }
            set
            {
                if (_selectedSeasonsClient == value) return;
                
                _selectedSeasonsClient = value;

                
                RaisePropertyChanged("SelectedSeasons");
            }
            
        }


        private ObservableCollection<SimpleSeasonsClient> GetSelectedSeasons()
        {
            var championships = channel.GetAllChampionships();
            var seasons = channel.GetSeasons(
               // Guid.NewGuid()
                );//alert!!!!!!!1

            foreach (var itemSeason in seasons)
            {
                var toursToConvert = channel.GetAllTours(itemSeason.Id);
                Tours = new ObservableCollection<SimpleTourClient>();

                foreach (var item in toursToConvert)
                {
                    var tourMatches = channel.GetMatches(item.Id);
                    var tourMatchesToAdd = new ObservableCollection<SimpleMatchClient>();
                    foreach (var itm in tourMatches)
                    {
                        var tourMathcesClient = new SimpleMatchClient()
                        {
                            Id = itm.Id,
                            Guest = itm.Guest,
                            Home = itm.Home,
                            GuestTeamGoals = itm.GuestTeamGoals,
                            HomeTeamGoals = itm.HomeTeamGoals,
                            HomeBitmapImage = ImageHelper.LoadImage(itm.HomeImage),
                            GuestBitmapImage = ImageHelper.LoadImage(itm.GuestImage)
                        };
                    //    Matches.Add(tourMathcesClient);
                        tourMatchesToAdd.Add(tourMathcesClient);
                       
                    }
                    var tourinfo = new SimpleTourClient()
                    {
                        SeasonId = item.SeasonId,
                        Id = item.Id,
                        NameTour = item.NameTour,
                        Matches = tourMatchesToAdd
                    };
                    Tours.Add(tourinfo);
                   
                }
                Seasons.Add(new SimpleSeasonsClient()
                {
                    Id = itemSeason.Id,
                    Tours = Tours,
                    Name = itemSeason.Name,
                });
                
            }
            return Seasons;
        }

        private SimpleMatchClient _selectedMatch;

        public SimpleMatchClient SelectedMatch
        {
            get
            {
                return _selectedMatch;
            }
            set
            {
                if (_selectedMatch == value) return;
                _selectedMatch = value;
                RaisePropertyChanged("SelectedMatch");
            }
        }

        public static SimpleMatch ToSimpleMatch(SimpleMatchClient selectedMatch)
        {
            var selMatch = new SimpleMatch()
            {
                Guest = selectedMatch.Guest,
                Home = selectedMatch.Home,
                GuestTeamGoals = selectedMatch.GuestTeamGoals,
                HomeTeamGoals = selectedMatch.HomeTeamGoals,
                Id = selectedMatch.Id
            };
            return selMatch;
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


        private RelayCommand _toListTeam;
        public RelayCommand ToListTeam
        {
            get
            {
                return _toListTeam ?? (_toListTeam = new RelayCommand(() =>
                {

                    TeamsToGenerate.Add(SelectedTeam);
                    Teams.Remove(SelectedTeam);
                    try
                    {
                      //  SelectedTeam = Teams[0];//delete later
                    }
                    catch (Exception)
                    {

                    }
                    
                },
                 () => Validation(SelectedTeam)
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
                    Teams.Add(SelectedTeamToGenerate);
                    TeamsToGenerate.Remove(SelectedTeamToGenerate);
                    
                },
                () => Validation(SelectedTeamToGenerate)
                ));
            }
        }

        private RelayCommand _generateTour;
        public RelayCommand GenerateTour
        {
            get
            {
                return _generateTour ?? (_generateTour = new RelayCommand(() =>
                {
                    {
                        GoGenerate(SelectedSeasons.Id);
                    }
                }
                , () =>(TeamsToGenerate.Count>1&&SelectedSeasons!=null&&SelectedSeasons.Tours.Count==0)));

                // SelectedSeasons.Tours.Count == 0 && 
            }
        }

        private void GoGenerate(Guid seasonId)
        {
            _countMatches = 0;
            var tour = 0;
            container = new List<Match>();
            var teams = ToTeam(TeamsToGenerate);
            if (teams != null)

            {
                foreach (var item in teams)
                {
                    channel.TeamAddToSeason(seasonId,item.Id);
                }


                Console.WriteLine("-------------Start generation Matches------------");
                Console.WriteLine("Generetion for :"+SelectedSeasons.Name);
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
            _containerTour=new List<Match>();
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


                channel.AddTour(tourser, SelectedSeasons.Id, _containerTour);

                MainWindowViewModelConstructor();
                return;
            }
            else
            {
                var tourser = new Tour()
                {
                    NameTour = tourCounter.ToString(),
                    
                };

                
                channel.AddTour(tourser,SelectedSeasons.Id,_containerTour);
                FirstTour(teamWithKeys, tourCounter);
            }
        }
        private static bool Validation(SimpleTeam selectedTeam)
        {

            try
            {
                if (selectedTeam.Id == new Guid())
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }
    }

}

