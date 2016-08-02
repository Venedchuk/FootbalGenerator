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
        void AddChampionship(Championship champ);

        [OperationContract]
        void RemoveChamp(Guid champGuid);

        [OperationContract]
        void StopServer();

        [OperationContract]
        void RemoveTeam(Guid guid);

        [OperationContract]
        void RemovePlayer(Guid playerId);

        [OperationContract]
        void AddPlayer(Guid id, Player player);

        [OperationContract]
        void AddTour(Tour tourForOperation, Guid champId,List<Match> matches );

        [OperationContract]
        void AddMatchGoal(Guid matchGuid, byte goalTeam1, byte goalTeam2);

        [OperationContract]
        List<SimpleMatch> GetAllMatches(Guid champGuid);

        [OperationContract]
        void TeamAddToChamp(Guid champId,Guid teamId);
        [OperationContract]
        List<SimpleTeam> TeamGetFromChamp(Guid champId);

        [OperationContract]
        void ChangeMatch(SimpleMatch match);

        [OperationContract]
        List<SimpleTour> GetAllTours(Guid championshipGuid);


        [OperationContract]
        List<SimpleMatch> GetMatches(Guid tourGuid);

        [OperationContract]
        List<SimpleChampionship> GetChampionships();


    }
}
