using System;
using System.Collections.Generic;
using System.Linq;
using OperationWithTeams;

// ReSharper disable once CheckNamespace
namespace Server
{
    partial class Program
    {
        private static void TemplateDb()
        {

            using (var db = new ConnectToDb())
            {

                var playerOne = db.Players.Add(new Player()
                {

                    Age = 20,
                    Name = "Vasya",
                    PlayerId = Guid.NewGuid(),
                });

                var playerTwo = db.Players.Add(new Player()
                {
                    Age = 35,
                    Name = "Petya",
                    PlayerId = Guid.NewGuid(),
                });
                var listPlayer = new List<Player>();
                listPlayer.Add(playerOne);

                db.Teams.Add(new Team()
                {
                    Country = "Ukraine",
                    Id = Guid.NewGuid(),
                    Members = listPlayer,
                    Name = "Kyiv",
                });
                listPlayer = new List<Player>();
                listPlayer.Add(playerTwo);
                db.Teams.Add(new Team()
                {
                    Country = "Ukraine",
                    Id = Guid.NewGuid(),
                    Name = "Harkiv",
                    Members = listPlayer
                });
                db.SaveChanges();

                var champ = db.Championships.Add(new Championship());
                champ.Id = Guid.NewGuid();
                champ.Name = "Ukrainian champ";
                champ.Year = DateTime.Today;
                db.Championships.Add(champ);
                db.SaveChanges();

                db.Tours.Add(new Tour()
                {
                    Id = Guid.NewGuid(),
                    Championship = db.Championships.ToList().Find(item => item.Name == "Ukrainian champ")
                });

                db.SaveChanges();

                db.Matches.Add(new Match()
                {
                    DateMatch = DateTime.Today,
                    Id = Guid.NewGuid(),
                    Home = db.Teams.ToList()[0],
                    Guest = db.Teams.ToList()[1],
                    Tour = db.Tours.ToList()[0],
                });

                db.SaveChanges();

                db.Matches.ToList()[0].Result =
                    new Result()
                    {
                        Id = Guid.NewGuid(),


                        Goals = new List<Goal>()
                        {
                            new Goal()
                            {
                                Id = Guid.NewGuid(),
                                PlayerId = db.Players.ToList()[0].PlayerId,
                                Time = DateTime.Now,
                            },
                            new Goal()

                            {
                                Id = Guid.NewGuid(),
                                PlayerId = db.Players.ToList()[0].PlayerId,

                                Time = DateTime.Now
                            }
                        }


                    };
                db.SaveChanges();


                db.SaveChanges();


            }
        }

        private static void AddNormDb()
        {
            using (var db = new ConnectToDb())
            {
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Harkiv"
                });
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Lviv"
                });
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Zhytomyr"
                });
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Dnipro"
                });
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Donets'k"
                });
                db.Teams.Add(new Team()
                {
                    Id = Guid.NewGuid(),
                    Country = "Ukraine",
                    Name = "Karpaty"
                });
                db.SaveChanges();
                Console.WriteLine("Teamplate Team added");

            }
        }
    }
}
