using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using OperationWithTeams;
using WPFClient.Models;


namespace WpfRedactor
{
    public class MainWindowRedactorViewModel : ViewModelBase
    {
        public static IContract Channel;
        
        
        public MainWindowRedactorViewModel()
        {
            
            var adress = new Uri("http://localhost:8000/IContract");
            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IContract>(binding, new EndpointAddress(adress));
            Channel = factory.CreateChannel();
            

            MainWindowRedactorViewModelConstruct();

        }

        private void MainWindowRedactorViewModelConstruct()
        {
 
            Seasons = new ObservableCollection<SimpleSeasonsClient>();

                Teams = new ObservableCollection<SimpleTeamClient>(GetAllTeamsToShow());

            
            SelectedTeam = new SimpleTeamClient();

            Seasons = GetAllSeasons();
        }

        public string AddTeamName { get; set; }
        public string AddPlayerName { get; set; }
        public int AddPlayerAge { get; set; }
        public string AddTeamCountry { get; set; }
        public string AddSeasonName { get; set; }
        public BitmapImage ImageTeam { get; set; }


        private ObservableCollection<SimpleSeasonsClient> GetAllSeasons()
        {
            var seasons = Channel.GetSeasons();



            foreach (var itemSeason in seasons)
            {
                Seasons.Add(new SimpleSeasonsClient()
                {
                    Id = itemSeason.Id,
                    Name = itemSeason.Name,
                });
            }
            return Seasons;
        }

        private static ObservableCollection<SimpleTeamClient> GetAllTeamsToShow()
        {
            var teamsToReturn = new ObservableCollection<SimpleTeamClient>();

            foreach (var tm in Channel.GetAllTeam())
            {
                var players = new ObservableCollection<SimplePlayerClient>();
                foreach (var player in Channel.GetTeamPlayers(tm.Id))
                {
                    players.Add(new SimplePlayerClient()
                    {
                        Age = player.Age,
                        Name = player.Name,
                        PlayerId = player.PlayerId,

                    });

                }
                teamsToReturn.Add(new SimpleTeamClient()
                {
                    Id = tm.Id,
                    Country = tm.Country,
                    Members = players,
                    Name = tm.Name,
                    LogoTeam = ImageHelper.LoadImage(tm.ImageTeam)

                });
            }

            return teamsToReturn;
        }

        private RelayCommand _addPlayer;

        public RelayCommand AddPlayer
        {
            get
            {
                return _addPlayer ?? (_addPlayer = new RelayCommand(() =>
                {
                    Channel.AddPlayer(SelectedTeam.Id, new Player()
                    {
                        Age = AddPlayerAge,
                        Name = AddPlayerName

                    });

                    RaisePropertyChanged("SelectedTeam");
                    MainWindowRedactorViewModelConstruct();
                    InfoAddPlayer = "Added Player: " + AddPlayerName;
                }
                    , () => (AddPlayerName != null && AddPlayerAge != 0 && SelectedTeam.Id != Guid.NewGuid())));


            }
        }

        private RelayCommand _deletePlayer;

        public RelayCommand DeletePlayer
        {
            get
            {
                return _deletePlayer ?? (_deletePlayer = new RelayCommand(() =>
                {
                    InfoAddPlayer = "Deleted Player: " + SelectedPlayer.Name;
                    Channel.RemovePlayer(SelectedPlayer.PlayerId);

                    RaisePropertyChanged("SelectedPlayer");
                    MainWindowRedactorViewModelConstruct();

                }
                    , () => (SelectedPlayer != null && SelectedTeam.Id != Guid.NewGuid())));


            }
        }

        private RelayCommand _deleteSeason;

        public RelayCommand DeleteSeason
        {
            get
            {
                return _deleteSeason ?? (_deleteSeason = new RelayCommand(() =>
                {

                    Channel.RemoveSeason(SelectedSeason.Id);

                    RaisePropertyChanged("SelectedSeason");
                    MainWindowRedactorViewModelConstruct();

                }
                    , () => (SelectedSeason != null)));


            }
        }

        private RelayCommand _addTeam;

        public RelayCommand AddTeam
        {
            get
            {
                return _addTeam ?? (_addTeam = new RelayCommand(() =>
                {
                    AddTeamInfo = "Added Team: " + AddTeamName;
                var team = new Team()
                {
                    Country = AddTeamCountry,
                    Name = AddTeamName,
                    //ImageTeam = ImageToByteArray(BitmapImage2Bitmap(ImageTeam))
                };

                    if (ImageTeam == null)
                        team.ImageTeam = ImageToByteArray(BitmapImage2Bitmap(new BitmapImage(new Uri("D:/FootbalGenerator/S.jpg"))));
                    else
                    {
                        team.ImageTeam = ImageToByteArray(BitmapImage2Bitmap(ImageTeam));
                    }
                    Channel.AddTeam(team, new List<Player>());
                    

                    RaisePropertyChanged("AddTeamInfo");
                    MainWindowRedactorViewModelConstruct();

                }
                    , () => (AddTeamName != null && AddTeamCountry != null)));


            }
        }

        private RelayCommand _addSeason;

        public RelayCommand AddSeason
        {
            get
            {
                return _addSeason ?? (_addSeason = new RelayCommand(() =>
                {
                    AddSeasonInfo = "Added Season: " + AddSeasonName;
                    Channel.AddSeason(new Season()
                    {
                        Name = AddSeasonName,
                    });

                    RaisePropertyChanged("AddSeasonInfo");
                    MainWindowRedactorViewModelConstruct();

                }
                    , () => (AddSeasonName != null)));


            }
        }

        private RelayCommand _openImg;

        public RelayCommand OpenImg
        {
            get
            {
                return _openImg ?? (_openImg = new RelayCommand(() =>
                {
                     ImageTeam = BitmapToImageSource(LoadImage());
                    RaisePropertyChanged("ImageTeam");
                }

                    ));


            }
        }

        private RelayCommand _removeTeam;

        public RelayCommand RemoveTeam
        {
            get
            {
                return _removeTeam ?? (_removeTeam = new RelayCommand(() =>
                {
                    AddTeamInfo = "Removed Team: " + SelectedTeam.Name;
                    Channel.RemoveTeam(SelectedTeam.Id);

                    RaisePropertyChanged("AddTeamInfo");
                    MainWindowRedactorViewModelConstruct();

                }
                    , () => (SelectedTeam.Name != null)));


            }
        }

        private string _infoAddPlayer;

        public string InfoAddPlayer
        {
            get { return _infoAddPlayer; }
            set
            {
                if (_infoAddPlayer == value) return;
                _infoAddPlayer = value;

                RaisePropertyChanged("InfoAddPlayer");

            }
        }

        private string _addTeamInfo;

        public string AddTeamInfo
        {
            get { return _addTeamInfo; }
            set
            {
                if (_addTeamInfo == value) return;
                _addTeamInfo = value;

                RaisePropertyChanged("AddTeamInfo");

            }
        }

        private string _addSeasonInfo;

        public string AddSeasonInfo
        {
            get { return _addSeasonInfo; }
            set
            {
                if (_addSeasonInfo == value) return;
                _addSeasonInfo = value;

                RaisePropertyChanged("AddSeasonInfo");

            }
        }

        private SimpleTeamClient _selectedTeam;

        public SimpleTeamClient SelectedTeam
        {
            get { return _selectedTeam; }
            set
            {
                if (_selectedTeam == value) return;
                _selectedTeam = value;

                RaisePropertyChanged("SelectedTeam");

            }
        }

        private SimplePlayerClient _selectedPlayer;

        public SimplePlayerClient SelectedPlayer
        {
            get { return _selectedPlayer; }
            set
            {
                if (_selectedPlayer == value) return;
                _selectedPlayer = value;

                RaisePropertyChanged("SelectedPlayer");

            }
        }

        private SimpleSeasonsClient _selectedSeason;

        public SimpleSeasonsClient SelectedSeason
        {
            get { return _selectedSeason; }

            set
            {
                if (_selectedSeason == value) return;
                _selectedSeason = value;
                RaisePropertyChanged("SelectedSeason");

            }
        }

        private ObservableCollection<SimpleTeamClient> _teams;

        public ObservableCollection<SimpleTeamClient> Teams
        {
            get { return _teams; }

            set
            {
                if (_teams == value) return;
                _teams = value;
                RaisePropertyChanged("Teams");

            }
        }

        private ObservableCollection<SimpleSeasonsClient> _seasons;

        public ObservableCollection<SimpleSeasonsClient> Seasons
        {

            get { return _seasons; }

            set
            {
                if (_seasons == value) return;
                _seasons = value;
                RaisePropertyChanged("Seasons");
                AddSeasonName = null;
            }
        }

        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public static Bitmap LoadImage()
        {
            {
                
                
                    OpenFileDialog open = new OpenFileDialog();
                    open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                    open.ShowDialog();
                    {
                    var bit = new Bitmap(30,30);
                        if (open.FileName=="")
                        bit = new Bitmap(open.FileName = @"B:\FotbalGenerator\S.jpg");
                        else
                         bit = new Bitmap(open.FileName);
                        return bit;
                    }
                

                
            }

        }
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}