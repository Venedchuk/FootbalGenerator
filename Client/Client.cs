using System;
using System.Collections.Generic;
using System.ServiceModel;
using OperationWithTeams;

namespace Client
{
    class Client
    {
        static void Main()
        {
            Console.Title = "CLIENT";

            var adress = new Uri("http://localhost:8000/IContract");

            var binding = new BasicHttpBinding();


            var factory = new ChannelFactory<IContract>(binding, new EndpointAddress(adress));
            var channel = factory.CreateChannel();




            #region GetAllTeam

            var team = new List<Team>();

                team = channel.GetAllTeam();
                foreach (var item in team)
                {
                   var Memb = channel.GetTeamPlayers(item.Id);
                    item.Members =Memb;
                }

            

            foreach (var item in team)
            {
                Console.WriteLine(item.Name);
                foreach (var itemMembers in item.Members)
                {
                    Console.Write("    Name:" + itemMembers.Name + " Age:" + itemMembers.Age);
                }
                Console.WriteLine();
            }
            #endregion

 
            Console.ReadLine();
        }
    }
}

