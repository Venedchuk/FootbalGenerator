using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;


namespace OperationWithTeams
{
    [ServiceContract]
    public interface IContract
    {
        [OperationContract]
        StringBuilder GetAllTeamStr();

        [OperationContract]
        List<SimpleTeam> GetAllTeam();

        [OperationContract]
        List<Team> GetAllTeamForGenerate();

        [OperationContract]
        List<Player> GetTeamPlayers(Guid teamId);

        [OperationContract]
        void AddTeam(Team team,List<Player> players);

        [OperationContract]
        void AddSeason(Season season);

        [OperationContract]
        void RemoveSeason(Guid seasonGuid);

        [OperationContract]
        void StopServer();

        [OperationContract]
        void RemoveTeam(Guid guid);

        [OperationContract]
        void RemovePlayer(Guid playerId);

        [OperationContract]
        void AddPlayer(Guid id, Player player);

        [OperationContract]
        void AddTour(Tour tourForOperation, Guid seasonId,List<Match> matches );

        [OperationContract]
        void AddMatchGoal(Guid matchGuid, byte goalTeam1, byte goalTeam2);

        [OperationContract]
        List<SimpleMatch> GetAllMatches(Guid seasonGuid);

        [OperationContract]
        void TeamAddToSeason(Guid seasonId,Guid teamId);
        [OperationContract]
        List<SimpleTeam> TeamGetFromSeason(Guid seasonId);

        [OperationContract]
        void ChangeMatch(SimpleMatch match);

        [OperationContract]
        List<SimpleTour> GetAllTours(Guid seasonGuid);


        [OperationContract]
        List<SimpleMatch> GetMatches(Guid tourGuid);

        [OperationContract]
        List<SimpleSeason> GetSeasons();

        [OperationContract]
        List<SimpleChampionship> GetAllChampionships();


    }
}
