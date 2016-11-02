using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace OperationWithTeams
{

    [DataContract]
    public class Championship
    {//not relised
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual List<Season> Seasons { get; set; }
    }


    [DataContract]
    public class Season
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual List<Tour> Tours { get; set; }

        [DataMember]
        public virtual Championship Championship { get; set; }

        [DataMember]
        public virtual List<TeamList> TeamGuids { get; set; }

    }

    [DataContract]
    public class TeamList
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid TeamGuid { get; set; }

        [DataMember]
        public virtual Season Season { get; set; }

    }

    [DataContract]
    public class Tour
    {

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string NameTour { get; set; }
        [DataMember]
        public virtual Season Season { get; set; }
        [DataMember]
        public virtual List<Match> Matches { get; set; }
    }

    public class Match
    {
        public virtual Guid HomeId { get; set; }
        public virtual Guid GuestId { get; set; }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public virtual Tour Tour { get; set; }

        [DataMember]
        public DateTime? DateMatch { get; set; }

        [ForeignKey("HomeId")]
        public virtual Team Home { get; set; }

        [ForeignKey("GuestId")]
        public virtual Team Guest { get; set; }

        [DataMember]
        public virtual Result Result { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public Guid Id { get; set; }

        public virtual List<Goal> Goals { get; set; }
        [DataMember]
       public int? HomeTeamGoals { get; set; }
       [DataMember]
       public int? GuestTeamGoals { get; set; }

    }


    [DataContract]
    public class Goal
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public DateTime? Time { get; set; }

        public virtual Guid PlayerId { get; set; }
        [DataMember]
        public virtual Result Results { get; set; }


    }
  
}
