using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OperationWithTeams;
// ReSharper disable CheckNamespace



namespace Server

{

    public class ConnectToDb : DbContext 
    {
        
        public ConnectToDb()  :base("DbConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ConnectToDb>());
            //  Database.SetInitializer(new DropCreateDatabaseAlways<ConnectToDb>());
           // this.Configuration.ProxyCreationEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            // ModelBuilder.Entity<MatchForDb>().HasOptional(m => m.Result).WithRequired(r => r.MatchForDb);
        }
        public DbSet<Team> Teams { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<TeamList> TeamLists { get; set; }
        public DbSet<Championship> Championships { get; set; }

    }
}
