using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFClient.Models;
using System.Data.SqlClient;
using System.Data;
using OperationWithTeams;
using System.ServiceModel;

namespace WPFClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IContract Channel;
        public MainWindow()
        {

            InitializeComponent();
        }
        
        private void templateForQuery(string Query)
        {
            var adress = new Uri("http://localhost:8000/IContract");
            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IContract>(binding, new EndpointAddress(adress));
            Channel = factory.CreateChannel();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;

                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = Query;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                // var result = dt.DefaultView;
                //var a = dt.DefaultView;
                Guid IdTeam = (Guid)dt.Rows[0].ItemArray[0];//get id from Teams table
                Channel.GetAllTeamStr();
                QueryResult.Text = "";
                QueryResult.Text = String.Join(Environment.NewLine,Channel.GetMatchesOneTeam(IdTeam));
                

            }
        }

        private void FillQueryResult(DataView dv)
        {

           // QueryResult.ItemsSource = dv;

        }

        private string GetConnectionString()
        {
            return @"Data Source=METEOR\SQLEXPRESS;Initial Catalog=DbConnection;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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
                column.Width = 50;
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
                                helper.SetResult(t1, t2, TeamDataGridHelper.Result.NPlayedYet);
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
            if(dc.SelectedSeasons!=null)
            TableResult(dc.SelectedSeasons.Id);
            dataGrid.ColumnWidth = 15;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            var team = TeamQuery.SelectedItem;
            if(team!=null)
            templateForQuery("SELECT Id FROM Teams WHERE Name = '"+ team + "'");

        }

        private void OnDataGridPrinting(object sender, RoutedEventArgs e)
        {
           var print = dataGrid;
          //  var point = dataGridPoint.Columns[1];
          //  dataGridPoint = new DataGrid();
          //  point.Header = "Pointss";
            //print.Columns.Add(point);
        
            System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            {
                Size pageSize = new Size(Printdlg.PrintableAreaWidth+20, Printdlg.PrintableAreaHeight);
                // sizing of the element.
                
                print.Measure(pageSize);
                print.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                Printdlg.PrintVisual(print, Title);

            }
        }
    }
    public class TeamForGrid
    {
        public string TeamGrid { get; set; }
        public int Point { get; set; }
    }

}
