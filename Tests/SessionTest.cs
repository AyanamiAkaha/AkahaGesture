using System;
using System.Linq;
using NUnit.Framework;
using Akaha_Gesture.Stats;
using System.Data.SQLite;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace Akaha_Gesture.Tests {
    class SessionTest {
        private AkahaGestureDbContext _context;
        private DbConnection _connection;

        private string GetTempFilePath()
        {
            string tempFileName = $"akhg-{Path.GetRandomFileName()}.sqlite";
            string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
            return $"DataSource={tempFilePath}";
        }


        [SetUp]
        public void Setup()
        {
            _connection = new SQLiteConnection(GetTempFilePath());
            _connection.Open();

            _context = new AkahaGestureDbContext(_connection);
            _context.Database.CreateIfNotExists();

            var configuration = new Akaha_Gesture.Migrations.Configuration()
            {
                TargetDatabase = new DbConnectionInfo(_connection.ConnectionString, "System.Data.SQLite")
            };
            var migrator = new DbMigrator(configuration, _context);
            migrator.Update();
        }

        [TearDown]
        public void TearDown()
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{_connection.DataSource}.sqlite");
            _connection.Close();
            _context.Dispose();
            _connection.Dispose();
            SQLiteConnection.ClearAllPools();
            GC.Collect();
            if (File.Exists(tempFilePath)) {
                File.Delete(tempFilePath);
            }
        }

        [Test]
        public void TestAddImage()
        {
            var image = new Image { path = "test-image.jpg" };
            _context.Images.Add(image);
            _context.SaveChanges();

            Assert.AreEqual(1, _context.Images.Count());
        }

        [Test]
        public void TestAddSession()
        {
            var dt = new DateTime(1337);
            var image1 = new Image { path = "image1.jpg" };
            var image2 = new Image { path = "image2.jpg" };

            _context.Images.Add(image2);

            var session = new Session(dt, 2, 5);
            session.AddImage(image1);
            _context.Images.Add(image1);
            session.AddImage(image2);
            _context.Images.Add(image2);
            _context.Sessions.Add(session);
            _context.SaveChanges();

            Assert.AreEqual(1, _context.Sessions.Count());
            var savedSession = _context.Sessions.First();
            Console.WriteLine(savedSession.sessionImages.Count);
            Assert.AreEqual(2, _context.SessionImages.Count());
        }

    }
}
