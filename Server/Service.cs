using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using OperationWithTeams;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace Server
{
    public class Service : IContract
    {
        public void TeamAddToSeason(Guid seasonId, Guid teamId)
        {

            using (var db = new ConnectToDb())
            {
                foreach (var item in db.Seasons)
                {
                    item.TeamGuids = new List<TeamList>();
                    if (item.Id == seasonId)
                    {
                        item.TeamGuids.Add(new TeamList()
                        {
                            TeamGuid = teamId,
                            Season = item,
                            Id = Guid.NewGuid()
                        });
                    }
                }
                db.SaveChanges();

            }

        }

        public List<SimpleTeam> TeamGetFromSeason(Guid seasonId)
        {
            var teamsToReturn = new List<SimpleTeam>();
            var teamsid = new List<Guid>();
            using (var db = new ConnectToDb())
            {
                foreach (var teamListItem in db.TeamLists.Include(x=>x.Season))
                {
                    if (teamListItem.Season.Id==seasonId)
                    {
                        teamsid.Add(teamListItem.TeamGuid);
                    }

                }
                foreach (var team in db.Teams)
                {

                    foreach (var teamId in teamsid)
                    {
                        if (team.Id == teamId)
                        {
                            teamsToReturn.Add( new SimpleTeam()
                            {
                                Country = team.Country,
                                Id = team.Id,
                                Name = team.Name
                            });
                        }
                    }
                }
            }
            return teamsToReturn;

        }

        public StringBuilder GetAllTeamStr()
        {
            Console.WriteLine("Use GetAllTeamSTR method");

            var teamString = new StringBuilder();
            using (var db = new ConnectToDb())
            {




                foreach (var item in db.Teams)
                {
                    teamString.AppendLine("Name Team:" + item.Name);

                    foreach (var count in item.Members)
                    {

                        teamString.AppendLine("Name player:" + count.Name + " Age:" + count.Age);

                    }
                    teamString.AppendLine();

                }
                return teamString;
            }


        }

        public List<SimpleTeam> GetAllTeam()
        {
            Console.WriteLine("Use GetAllTeam method");

            using (var db = new ConnectToDb())
            {


                var teamsToReturn = new List<SimpleTeam>();

                if (db.Teams.Any())
                {
                    foreach (var item in db.Teams)
                    {
                        var teamToReturn = new SimpleTeam()
                        {
                            Id = item.Id,
                            Country = item.Country,
                            Name = item.Name,
                            ImageTeam = item.ImageTeam
                        };


                        teamsToReturn.Add(teamToReturn);

                    }
                }
                return teamsToReturn;


            }


        }

        public List<Team> GetAllTeamForGenerate()
        {

           // var team
            using (var db = new ConnectToDb())
            {
                

                return db.Teams.ToList();

            }

        }

        public List<Player> GetTeamPlayers(Guid teamId)
        {
            Console.WriteLine("Get Team Players");
            var listPlayer = new List<Player>();


            using (var db = new ConnectToDb())
            {
                foreach (var item in db.Players.Include(x=>x.Team))
                {
                    if (item.Team.Id == teamId)
                    {
                        listPlayer.Add(new Player {Name = item.Name, Age = item.Age, PlayerId = item.PlayerId});
                    }
                }

            }

            return listPlayer;
        }

        public void AddTeam(Team t1, List<Player> players)
        {
            if (t1 == null)
                throw new ArgumentException("t1");


            Console.WriteLine("Used method AddTeam");
            t1.Members = players;
            t1.Id = Guid.NewGuid();
            if (t1.Members != null)
            {
                // foreach (var item in t1.Members)
                // {
                //     item.PlayerId = Guid.NewGuid();
                // }
            }
            using (var db = new ConnectToDb())
            {



                db.Teams.Add(t1);

                db.SaveChanges();
                Console.WriteLine("Team " + t1.Name + " added");

            }



        }

        public void RemoveTeam(Guid guid)
        {
            var playerdel = false;
            Console.WriteLine("Used method RemoveTeam");
            using (var db = new ConnectToDb())
            {
                foreach (var item in db.Teams)
                {
                    if (db.Players != null && playerdel == false)
                    {
                        foreach (var player in db.Players.Include(x => x.Team))
                        {
                            
                            if (player.Team.Id == guid)
                            {
                                db.Players.Remove(player);
                            }
                        }
                        playerdel = true;
                    }
                    if (item.Id == guid)
                    {
                        db.Teams.Remove(item);
                        break;
                    }
                }


                db.SaveChanges();


            }
        }

        public void RemovePlayer(Guid playerId)
        {
            Console.WriteLine("Used method RemovePlayer");
            using (var db = new ConnectToDb())
            {
                foreach (var playerFind in db.Players)
                {
                    if (playerFind.PlayerId == playerId)
                    {

                        db.Players.Remove(playerFind);

                        break;
                    }
                }
                db.SaveChanges();

            }
        }

        public void AddPlayer(Guid id, Player player)
        {
            Console.WriteLine("Used method AddPlayer");
            player.PlayerId = Guid.NewGuid();
            using (var db = new ConnectToDb())
            {
                foreach (var item in db.Teams.Include(x=>x.Members))
                {
                    if (item.Id == id)
                    {
                        item.Members.Add(player);

                        break;
                    }
                }
                db.SaveChanges();

            }
        }

        public void AddTour(Tour tour, Guid seasonId, List<Match> matches)
        {
            Console.WriteLine("Used method AddTour");


            using (var db = new ConnectToDb())
            {
                tour.Season = db.Seasons.ToList().Find(x => x.Id == seasonId);
                tour.Id = Guid.NewGuid();
                tour.Matches = matches;
                for (var i = 0; i < matches.Count; i++)
                {

                    tour.Matches[i].GuestId = new Guid(matches[i].Guest.Id.ToString());
                    tour.Matches[i].HomeId = new Guid(matches[i].Home.Id.ToString());
                    tour.Matches[i].Guest = null;
                    tour.Matches[i].Home = null;
                    tour.Matches[i].Result = matches[i].Result;
                }
                db.Tours.Add(tour);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception a)
                {
                    Console.Write("Add Tour Trouble:" + a.InnerException.InnerException);

                }

            }
        }

        public void AddSeason(Season season)
        {
            season.Id = Guid.NewGuid();
           // season.Tours = new List<Tour>();
            using (var db = new ConnectToDb())
            {
                db.Seasons.Add(season);

                
                    db.SaveChanges();
                

                
            }
        }

        public void RemoveSeason(Guid seasonGuid)
        {

            var matchesToDelete = new List<Match>();
            var tourToDelete = new List<Tour>();
            var teamListToDelete = new List<TeamList>();
            var season = new Season();
            using (var db = new ConnectToDb())
            {
                foreach (var item in db.Seasons)
                {
                    if (item.Id == seasonGuid)
                    {
                        season = item;
                        foreach (var tour in db.Tours.Include(x=>x.Season))
                        {
                            if (tour.Season.Id == item.Id)
                            {
                                
                                try
                                {


                                    foreach (var match in db.Matches.Include(x => x.Tour))
                                    {
                                        if (match.Tour.Id == tour.Id)
                                        {
                                            matchesToDelete.Add(match);
                                            
                                        }

                                    }


                                    tourToDelete.Add(tour);
                                     }
                                catch (Exception a)
                                {
                                  Console.WriteLine(a);
                                }
                                
                            }
                        }


                        teamListToDelete.AddRange(db.TeamLists.Where(teamItem => teamItem.Season.Id == item.Id));
                        Console.WriteLine("Season " + item.Name + " removed");
                        break;
                    }
                    

                }
                db.Matches.RemoveRange(matchesToDelete);
                db.Tours.RemoveRange(tourToDelete);
                db.TeamLists.RemoveRange(teamListToDelete);
                db.Seasons.Remove(season);

                    db.SaveChanges();
                   

                

            }
        }

        public void AddMatchGoal(Guid matchGuid, byte goalTeam1, byte goalTeam2)
        {
            Console.WriteLine("Add Match Goal");
            using (var db = new ConnectToDb())
            {
                var matches = db.Matches;
                foreach (var item in matches)
                {
                    if (item.Id == matchGuid)
                    {
                        // item.HomeTeamGoals = goalTeam1;
                        //  item.GuestTeamGoals = goalTeam2;
                        db.SaveChanges();
                        break;
                    }
                }
            }

        }

        public List<SimpleMatch> GetAllMatches(Guid seasonGuid)
        {
            Console.WriteLine("Get All Matches");
            using (var db = new ConnectToDb())
            {
                if (!db.Matches.Any()) return new List<SimpleMatch>();
                var matches = db.Matches;

                var matchesToReturn = new List<SimpleMatch>();
                foreach (
                    var item in
                        matches.Include(x => x.Home).Include(x => x.Guest).Include(x => x.Result).Include(x => x.Tour).Include(x =>x.Tour.Season))
                {
                    if (item.Tour.Season.Id == seasonGuid)
                    {
                        var match = new SimpleMatch() {Id = item.Id, Home = item.Home.Name, Guest = item.Guest.Name};
                        //  match.Id = item.Id;
                        // match.Home = item.Home.Name;
                        // match.Guest = item.Guest.Name;
                        if (item.Result != null)
                        {
                            //   var res = db.Results.Include(x=>x.Id).ToList().Find(x => x.Id);
                            match.GuestTeamGoals = item.Result.GuestTeamGoals;
                            match.HomeTeamGoals = item.Result.HomeTeamGoals;
                        }

                        matchesToReturn.Add(match);
                    }
                }

                return matchesToReturn;
            }
        }

        public void ChangeMatch(SimpleMatch match)
        {
            if (match == null) return;
            Console.WriteLine("Change match: "+match.Home+" vs "+match.Guest);
            using (var db = new ConnectToDb())
            {
                var res = db.Matches.Include(x => x.Result).ToList().Find(item => item.Id == match.Id);
                if (res.Result != null)
                {
                    res.Result.HomeTeamGoals = match.HomeTeamGoals;
                    res.Result.GuestTeamGoals = match.GuestTeamGoals;
                }
                else
                {
                    res.Result = new Result()
                    {
                        Id = Guid.NewGuid(),
                        HomeTeamGoals = match.HomeTeamGoals,
                        GuestTeamGoals = match.GuestTeamGoals
                    };
                    //res.Id = Guid.NewGuid();
                    //res.Result.HomeTeamGoals = new int();
                    //res.Result.HomeTeamGoals = match.HomeTeamGoals;

                    // res.Result.GuestTeamGoals = new int();
                    //res.Result.GuestTeamGoals = match.GuestTeamGoals;
                    db.Results.Add(res.Result);

                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception a)
                {
                    Console.WriteLine("ChangeMatch trouble: " + a.InnerException);

                }

            }
        }

        public List<SimpleTour> GetAllTours(Guid seasonGuid)
        {
            using (var db = new ConnectToDb())
            {
                var tours = db.Tours.Include(x=>x.Season).ToList();
                var toursToReturn = new List<SimpleTour>();
                foreach (var item in tours)
                {
                    if(item.Season==null)continue;
                    if (item.Season.Id == seasonGuid)
                    {


                        var tour = new SimpleTour()
                        {
                            Id = item.Id,
                            SeasonId = item.Season.Id,
                            NameTour = item.NameTour
                        };
                        toursToReturn.Add(tour);
                    }
                }

                return toursToReturn;

            }


        }

        public List<SimpleMatch> GetMatches(Guid tour)
        {
            Console.WriteLine("Get All Matches");
            using (var db = new ConnectToDb())
            {
                
                if (!db.Matches.Any()) return new List<SimpleMatch>();
                var matches = db.Matches;
                var matchesToReturn = new List<SimpleMatch>();
                foreach (
                    var item in
                        matches.Include(x => x.Home).Include(x => x.Guest).Include(x => x.Result).Include(x => x.Tour))
                {
                    if (item.Tour.Id != tour)
                        continue;

                    var match = new SimpleMatch()
                    {
                        Id = item.Id,
                        Home = item.Home.Name,
                        Guest = item.Guest.Name,
                        HomeImage = item.Home.ImageTeam,
                        GuestImage = item.Guest.ImageTeam
                    };

                    if (item.Result != null)
                    {
                        //   var res = db.Results.Include(x=>x.Id).ToList().Find(x => x.Id);
                        match.GuestTeamGoals = item.Result.GuestTeamGoals;
                        match.HomeTeamGoals = item.Result.HomeTeamGoals;
                    }



                    matchesToReturn.Add(match);

                }

                return matchesToReturn;
            }

        }

        public List<SimpleSeason> GetSeasons()
        {
            Console.WriteLine("Get All Matches");

             
           
            using (var db = new ConnectToDb())
            {


                try
                {
                    db.Seasons.Include(x => x.Tours).ToList();
                }
                catch (Exception)
                {

                    return new List<SimpleSeason>();
                }
                 
                var seasonToReturn = new List<SimpleSeason>();
                
                {


                    foreach (var item in db.Seasons.Include(x => x.Tours).Include(x => x.Championship).ToList())
                    {

                        var listTour = new List<Guid>();
                        foreach (var itm in item.Tours)
                        {
                            listTour.Add(itm.Id);
                        }
                        var season = new SimpleSeason()
                        {
                            Name = item.Name,
                            Id = item.Id,
                            TourGuids = listTour
                        };
                        seasonToReturn.Add(season);
                    }
                }
                return seasonToReturn;
            }
        }

      

        public List<SimpleChampionship> GetAllChampionships()
        {
            using(var db = new ConnectToDb())
            {
                var Champs = db.Championships.Include(x => x.Seasons);
                

                return new List<SimpleChampionship>();    
            }
            
        }

    public void StopServer()
        {
            using (var db = new ConnectToDb())
            {
                db.SaveChanges();
                Console.WriteLine("Used method StopServer");
            }
            Environment.Exit(0);   
        }

        
    }
}
