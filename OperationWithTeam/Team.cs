using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OperationWithTeams
{
    [DataContract]
    public class Team
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public byte[] ImageTeam { get; set; }

        public virtual List<Player> Members { get; set; }

        public virtual List<Match> Mathces { get; set; }
    }
    public class ProductImage
    {
        public int ProductId { get; private set; }
        public byte[] Image { get; set; }
    }

    [DataContract]
    public class Player
    {

        [DataMember]
        public Guid PlayerId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Age { get; set; }

        public virtual Team Team { get; set; }
    }


}