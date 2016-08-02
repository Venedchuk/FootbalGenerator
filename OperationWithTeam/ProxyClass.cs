using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;


namespace OperationWithTeams
{

    [DataContract]
    public class SimpleMatch

    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Home { get; set; }

        [DataMember]
        public string Guest { get; set; }

        [DataMember]
        public int? HomeTeamGoals { get; set; }
        
        [DataMember]
        public int? GuestTeamGoals { get; set; }

        [DataMember]
        public byte[] HomeImage { get; set; }
        [DataMember]
        public byte[] GuestImage { get; set; }

    }

    [DataContract]
    public class SimpleTour
    {

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string NameTour { get; set; }
        [DataMember]
        public Guid ChampionshipId { get; set; }
        [DataMember]
        public List<SimpleMatch> Matches { get; set; }
    }

    [DataContract]
    public class SimpleChampionship
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Year { get; set; }

        [DataMember]
        public List<Guid> TourGuids { get; set; }

    }

    [DataContract]
    public class SimpleTeam
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public byte[] ImageTeam { get; set; }
    }
    [DataContract]
    public class SimplePlayer
    {

        [DataMember]
        public Guid PlayerId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Age { get; set; }

    }
}
