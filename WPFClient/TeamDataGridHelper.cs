using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Controls;
using OperationWithTeams;

namespace WPFClient
{
    public class TeamDataGridHelper
    {
        public enum Result
        {
            Win,
            Lose,
            Tie,
            NotPlayedYet
        }

        private DataGrid _dataGrid;
        private Dictionary<SimpleTeam, IDictionary<string, object>> _team;
        
        public TeamDataGridHelper(DataGrid dataGrid)
        {
            this._dataGrid = dataGrid;
        }

        public void AddTeams(List<SimpleTeam> team)
        {
            
            this._team = new Dictionary<SimpleTeam, IDictionary<string, object>>();
            foreach (var item in team)
            {
                ExpandoObject dynamicObject = new ExpandoObject();
                IDictionary<string, object> dynObjectAsDictionary = (IDictionary<string, object>)dynamicObject;
                dynObjectAsDictionary.Add("TeamGrid", item.Name);
                foreach (var tm in team)
                {
                    if (tm == item)
                    {
                        dynObjectAsDictionary.Add(tm.Name, "------------");
                    }
                }
                this._team.Add(item, dynObjectAsDictionary);
            }


        }

        public void SetResult(SimpleTeam team1, SimpleTeam team2, Result result)
        {
            try
            {


                _team[team1].Add(team2.Name, result);
                switch (result)
                {
                    case Result.Lose:
                        _team[team2].Add(team1.Name, Result.Win);
                        break;
                    case Result.Win:
                        _team[team2].Add(team1.Name, Result.Lose);
                        break;
                    case Result.Tie:
                        _team[team2].Add(team1.Name, Result.Tie);
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        
        }

        public Result GetResult(SimpleTeam team1, SimpleTeam team2)
        {
            if (_team[team1].ContainsKey(team2.Name))
                return (Result)_team[team1][team2.Name];

            return Result.NotPlayedYet;
        }

        public void Render()
        {
           // _dataGrid.Items.Clear();
           // _dataGrid.Columns.Clear();
            foreach (var t in _team)
            {

                foreach (var t2 in _team)
                {
                    if (!_team[t.Key].ContainsKey(t2.Key.Name))
                        SetResult(t.Key,t2.Key, Result.NotPlayedYet); 
                }
                _dataGrid.Items.Add(t.Value);

            }
        }
    }
}
