using System;
using System.Data.SQLite;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;

namespace Akaha_Gesture.Stats
{
    internal class SessionRepository
    {
        const string DB_NAME = "akhg.sqlite";
        readonly static string DB_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AkahaGesture", DB_NAME);
        readonly static string CONNECTION_STRING = $"Data Source={DB_PATH}";

        public SessionRepository() {
            if (!File.Exists(DB_PATH)) {
                Directory.CreateDirectory(Path.GetDirectoryName(DB_PATH));
                SQLiteConnection.CreateFile(DB_PATH);
            }
            ensureMigrated();
        }

        private void ensureMigrated() {
            using (var connection = new SQLiteConnection(CONNECTION_STRING)) {
                connection.Open();
                using (var db = new AkahaGestureDbContext(connection))
                {
                    db.Database.CreateIfNotExists();
                    var configuration = new Migrations.Configuration() {
                        TargetDatabase = new DbConnectionInfo(db.Database.Connection.ConnectionString, "System.Data.SQLite")
                    };
                    var migrator = new DbMigrator(configuration, db);
                    migrator.Update();
                    db.SaveChanges();
                }
            }
        }

        public void addSession(Session session)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING)) {
                connection.Open();
                using (var db = new AkahaGestureDbContext(connection))
                {
                    db.Sessions.Add(session);
                    db.SaveChanges();
                }
            }
        }

        public Session getLastSession() {
            using (var connection = new SQLiteConnection(CONNECTION_STRING)) {
                connection.Open();
                using (var db = new AkahaGestureDbContext(connection))
                {
                    return db.Sessions.OrderByDescending(s => s.start).FirstOrDefault();
                }
            }
        }
    }
}
