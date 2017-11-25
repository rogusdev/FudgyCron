using System;
using Dapper;
using Nancy;

namespace FudgyCron.Web
{
    public class PostgresModule : NancyModule
    {
        public PostgresModule() : base("/postgres")
        {
            // this is very bad REST, I know
            Get("/{key}/{value}", args =>
            {
                var key = args.key.ToString();
                var value = args.value.ToString();

                var databaseUrlString = Environment.GetEnvironmentVariable("DATABASE_URL");
                Console.WriteLine($"DATABASE_URL from ENV: {databaseUrlString}");
                // https://devcenter.heroku.com/articles/connecting-to-relational-databases-on-heroku-with-java#using-the-database_url-in-plain-jdbc
                var dbUri = new Uri(databaseUrlString);
                Console.WriteLine($"parsed databaseUri: {dbUri}");
                var userInfo = dbUri.UserInfo.Split(':');
                var npgsqlConnString = $"Host={dbUri.Host};Port={dbUri.Port};Database={dbUri.LocalPath.Substring(1)};Username={userInfo[0]};Password={userInfo[1]}";
                Console.WriteLine($"constructed npgsqlConnString: {npgsqlConnString}");

                // https://github.com/StackExchange/Dapper
                // http://dapper-tutorial.net/dapper
                using (var dbConn = new Npgsql.NpgsqlConnection(npgsqlConnString))
                {
                    dbConn.Open();
                    // # https://stackoverflow.com/questions/8902674/manually-map-column-names-with-class-properties/34536863#34536863
                    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                    var now = DateTime.UtcNow;
                    var id = Guid.NewGuid();

                    // Insert some data
                    var newThing = new Thing()
                    {
                        Id = id,
                        Name = $"Hello world {now}: {key} = {value}",
                        Enabled = true,
                        CreatedAt = now.AddMinutes(-10),
                        UpdatedAt = now.AddMinutes(10),
                    };
                    dbConn.Execute(
                        "INSERT INTO things" +
                        " (id, name, enabled, created_at, updated_at)" +
                        " VALUES (@Id, @Name, @Enabled, @CreatedAt, @UpdatedAt)",
                        newThing
                    );
                    Console.WriteLine("inserted via dapper: {0}: {1}={2}", now, key, value);

                    // Retrieve all rows
                    var things = dbConn.Query<Thing>("SELECT * FROM things");
                    foreach (var thing in things)
                        Console.WriteLine(thing);
                    Console.WriteLine("read all dapper");

                    var addedThing = dbConn.QuerySingleOrDefault<Thing>(
                        "SELECT * FROM things WHERE id = @Id",
                        new { Id = id }
                    );
                    Console.WriteLine("added: {0}", addedThing);
                }

                return DateTime.UtcNow.ToString();
            });
        }

        private class Thing
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool Enabled { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }

            public override string ToString()
            {
                return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Enabled)}: {Enabled}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(UpdatedAt)}: {UpdatedAt}";
            }
        }
    }
}
