using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace WpfRedactor
{
    public class SimpleTeamClient : ObservableObject
    {

        public Guid Id { get; set; }

        public string Name { get; set; }



        public string Country { get; set; }

        public BitmapImage LogoTeam { get; set; }

        public ObservableCollection<SimplePlayerClient> Members { get; set; }

    }
    public class SimplePlayerClient : ObservableObject
    {

        public Guid PlayerId { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

    }
}
