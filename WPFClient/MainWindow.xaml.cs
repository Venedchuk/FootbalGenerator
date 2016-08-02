using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFClient.Models;

namespace WPFClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            
        }

        public void TableResult(Guid SeasonGuid)
        {
            dataGrid.Items.Clear();
            dataGrid.Columns.Clear();
            dataGridPoint.Items.Clear();
            var dc = DataContext as MainWindowViewModel;
            var helper = new TeamDataGridHelper(dataGrid);
            
            var teams = MainWindowViewModel.channel.TeamGetFromSeason(SeasonGuid);             //here

            var col = new DataGridTextColumn();
            col.Header = "Team";
            col.Width = 50;
            col.Binding = new Binding("TeamGrid");
            col.FontWeight = FontWeights.Bold;
            dataGrid.Columns.Add(col);


            foreach (var item in teams)
            {
                var column = new DataGridTextColumn();
                column.Header = item.Name;
                column.Width = 80;
                column.Binding = new Binding(item.Name);
                dataGrid.Columns.Add(column);
            }
            helper.AddTeams(teams);


            var table = MainWindowViewModel.channel.GetAllMatches(SeasonGuid);
            foreach (var match in table)
            {
                foreach (var t1 in teams)
                {
                    foreach (var t2 in teams)
                    {
                        if (match.Home == t1.Name && match.Guest == t2.Name)
                        {
                            if (match.HomeTeamGoals == null || match.GuestTeamGoals == null)
                            {
                                helper.SetResult(t1, t2, TeamDataGridHelper.Result.NotPlayedYet);
                                continue;
                            }
                            if (match.HomeTeamGoals == match.GuestTeamGoals)
                            {
                                helper.SetResult(t1, t2, TeamDataGridHelper.Result.Tie);
                            }
                            else if (match.HomeTeamGoals > match.GuestTeamGoals)
                            {
                                helper.SetResult(t1, t2, TeamDataGridHelper.Result.Win);
                            }
                            else
                            {
                                helper.SetResult(t1, t2, TeamDataGridHelper.Result.Lose);
                            }


                        }
                    }
                }
            }
            helper.Render();






            foreach (var item in teams)
            {
                var forP = new TeamForGrid();
                forP.Point = 0;
                forP.TeamGrid = item.Name;
                foreach (var itm in teams)
                {

                    try
                    {
                        if (helper.GetResult(item, itm).ToString() == "Win")
                            forP.Point += 3;
                        if (helper.GetResult(item, itm).ToString() == "Tie")
                            forP.Point++;
                    }
                    catch (Exception)
                    {

                    }

                }
                dataGridPoint.Items.Add(forP);
            }
        }


        private void TreeView1_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var dc = DataContext as MainWindowViewModel;
            try
            {
                dc.SelectedMatch = (SimpleMatchClient)e.NewValue;
            }
            catch (Exception)
            {

                return;
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as MainWindowViewModel;
            TableResult(dc.SelectedSeasons.Id);
        }
    }
    public class TeamForGrid
    {
        public string TeamGrid { get; set; }
        public int Point { get; set; }
    }
}
