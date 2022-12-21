using Linq.Model;

namespace Linq
{
    public class JoinSamples
    {
        public static void InnerJoin()
        {
            var racers = from r in Formula1.GetChampions()
                         from y in r.Years
                         select new
                         {
                             Year = y,
                             Name = r.FirstName + " " + r.LastName
                         };

            var teams = from t in Formula1.GetConstructorChampions()
                        from y in t.Years
                        select new
                        {
                            Year = y,
                            t.Name
                        };

            var racersAndTeams =
                  (from r in racers
                   join t in teams on r.Year equals t.Year
                   orderby t.Year
                   select new
                   {
                       r.Year,
                       Champion = r.Name,
                       Constructor = t.Name
                   }).Take(10);

            Console.WriteLine("Year  World Champion\t   Constructor Title");
            foreach (var item in racersAndTeams)
            {
                Console.WriteLine($"{item.Year}: {item.Champion,-20} {item.Constructor}");
            }
        }

        public static void InnerJoinWithMethods()
        {
            var racers = Formula1.GetChampions()
                .SelectMany(r => r.Years, (r1, year) =>
                new
                {
                    Year = year,
                    Name = $"{r1.FirstName} {r1.LastName}"
                });

            var teams = Formula1.GetConstructorChampions()
                .SelectMany(t => t.Years, (t, year) =>
                new
                {
                    Year = year,
                    t.Name
                });

            var racersAndTeams = racers.Join(
                teams,
                r => r.Year,
                t => t.Year,
                (r, t) =>
                    new
                    {
                        r.Year,
                        Champion = r.Name,
                        Constructor = t.Name
                    }).OrderBy(item => item.Year).Take(10);

            Console.WriteLine("Year  World Champion\t   Constructor Title");
            foreach (var item in racersAndTeams)
            {
                Console.WriteLine($"{item.Year}: {item.Champion,-20} {item.Constructor}");
            }
        }

        public static void GroupJoin()
        {
            IList<Racer> racers = Formula1.GetChampions();
            IEnumerable<Championship> ships = Formula1.GetChampionships();

            var shipsFlatten = from cs in ships
                               from r in new List<(int Year, int Position, string FirstName, string LastName)>()
                               {
                                 (cs.Year, Position: 1, FirstName: cs.First.FirstName(), LastName: cs.First.LastName()),
                                 (cs.Year, Position: 2, FirstName: cs.Second.FirstName(), LastName: cs.Second.LastName()),
                                 (cs.Year, Position: 3, FirstName: cs.Third.FirstName(), LastName: cs.Third.LastName())
                               }
                               select r;

            var groups = from r in racers
                         join s in shipsFlatten on
                         (
                             r.FirstName,
                             r.LastName
                         )
                         equals
                         (
                             s.FirstName,
                             s.LastName
                         )
                         into ss
                         select
                         (
                             r.FirstName,
                             r.LastName,
                             r.Wins,
                             r.Starts,
                             Results: ss
                         );

            foreach (var g in groups)
            {
                Console.WriteLine($"{g.FirstName} {g.LastName}");
                foreach (var results in g.Results)
                {
                    Console.WriteLine($"\t{results.Year} {results.Position}");
                }
            }
        }

        public static void GroupJoinWithMethods()
        {
            IList<Racer> racers = Formula1.GetChampions();
            IEnumerable<Championship> ships = Formula1.GetChampionships();
            IEnumerable<(int Year, int Position, string FirstName, string LastName)>
                shipsFlatten =
                    ships.SelectMany(s => new List<(int Year, int Position, string FirstName, string LastName)> {
                        (s.Year,Position:1,FirstName:s.First.FirstName(),LastName:s.First.LastName()),
                        (s.Year,Position:2,FirstName:s.Second.FirstName(),LastName:s.Second.LastName()),
                        (s.Year,Position:3,FirstName:s.Third.FirstName(),LastName:s.Third.LastName()),
            });

            var groups = racers.GroupJoin(shipsFlatten,
                        outerKeySelector: r => (r.FirstName, r.LastName),
                        innerKeySelector: s => (s.FirstName, s.LastName),
                        resultSelector: (r, ss) => (r.FirstName, r.LastName, r.Wins, r.Starts, Results: ss)
                     );

            foreach (var g in groups)
            {
                Console.WriteLine($"{g.FirstName} {g.LastName}");
                foreach (var r in g.Results)
                {
                    Console.WriteLine($"{r.Year} {r.Position}");
                }
            }
        }

        public static void LeftOuterJoin()
        {
            var racers = from r in Formula1.GetChampions()
                         from y in r.Years
                         select new
                         {
                             Year = y,
                             Name = r.FirstName + " " + r.LastName
                         };

            var teams = from t in Formula1.GetConstructorChampions()
                        from y in t.Years
                        select new
                        {
                            Year = y,
                            t.Name
                        };

            var racersAndTeams =
              (from r in racers
               join t in teams on r.Year equals t.Year
               into rt
               from t in rt.DefaultIfEmpty()
               orderby r.Year
               select new
               {
                   r.Year,
                   Champion = r.Name,
                   Constructor = t == null ? "no constructor championship" : t.Name
               }).Take(10);

            Console.WriteLine("Year  Champion\t\t   Constructor Title");
            foreach (var item in racersAndTeams)
            {
                Console.WriteLine($"{item.Year}: {item.Champion,-20} {item.Constructor}");
            }
        }

        public static void LeftOuterJoinWithMethods()
        {
            var racers = Formula1.GetChampions()
                .SelectMany(r => r.Years, (r1, year) =>
                new
                {
                    Year = year,
                    Name = $"{r1.FirstName} {r1.LastName}"
                });

            var teams = Formula1.GetConstructorChampions()
                .SelectMany(t => t.Years, (t, year) =>
                new
                {
                    Year = year,
                    t.Name
                });

            var racersAndTeams =
                racers.GroupJoin(
                    teams,
                    r => r.Year,
                    t => t.Year,
                    (r, ts) => new
                    {
                        r.Year,
                        Champion = r.Name,
                        Constructors = ts
                    })
                    .SelectMany(
                        item => item.Constructors.DefaultIfEmpty(),
                        (r, t) => new
                        {
                            r.Year,
                            r.Champion,
                            Constructor = t?.Name ?? "no constructor championship"
                        });

            Console.WriteLine("Year  Champion\t\t   Constructor Title");
            foreach (var item in racersAndTeams)
            {
                Console.WriteLine($"{item.Year}: {item.Champion,-20} {item.Constructor}");
            }
        }
    }
}
