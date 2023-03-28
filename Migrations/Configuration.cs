using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;

namespace Akaha_Gesture.Migrations {
    internal sealed class Configuration : DbMigrationsConfiguration<Akaha_Gesture.Stats.AkahaGestureDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }
    }
}
