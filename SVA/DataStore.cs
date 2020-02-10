using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Logger;

namespace SVA
{
    public struct AddressRecord
    {
        public int cityname_id;
        public int streetname_id;
        public string street_address;
        public string apn_code;
        public string zip_code;
        public string first_name;
        public string last_name;
        public int precinct_id;
        public int party_id;
    }


    public class DataStore
    {
        public DataStore()
        {
        }

        static string LibraryPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");

        static string DataFilePath {
            get
            {
                return Path.Combine(@"Data", "address_data.txt");
            }
        }

        internal static void Save()
        {
            var data = new List<string> { };

            data.Add(String.Join('\t', Cities.Keys));
            data.Add(String.Join('\t', Streets.Keys));
            AddressRecords.ToList().ForEach(p => data.Add($"{p.cityname_id}\t{p.streetname_id}\t{p.street_address}\t{p.apn_code}"));

            File.WriteAllText(DataFilePath, String.Join('\n', data));
        }

        static void CreateFakeNames()
        {
            var rnd = new Random();

            var last_names = File.ReadAllLines(@"Data/family_names.txt")
                    .Select(p => { var items = p.Split('\t'); return (p0: double.Parse(items[2]), item: items[0]); }).ToArray();

            var last_p_max = last_names[last_names.Length - 1].p0;

            var first_names = File.ReadAllLines(@"Data/given_names.txt")
                    .Select(p => { var items = p.Split('\t'); return (p0: double.Parse(items[1]), item: items[0]); }).ToArray();

            var names = new string[300000];

            for (int i = 0; i < names.Length; i++)
            {
                var first_p_max = first_names[first_names.Length - 1].p0;

                var last_p = rnd.NextDouble() * last_p_max;
                var last_name = last_names.First(p => p.p0 >= last_p).item;
                var first_p = rnd.NextDouble() * first_p_max;
                var first_name = first_names.First(p => p.p0 >= first_p).item;

                var middle_name = Convert.ToChar(65 + rnd.Next(0, 26));

                names[i] = $"{last_name},{first_name} {middle_name}";
            }

            var name_path = Path.Combine(LibraryPath, "names.txt");
            File.WriteAllText(name_path, String.Join('\n', names));

        }

        static void Load()
        {
            var rnd = new Random();

            var full_names = File.ReadAllLines(@"Data/full_names.txt")
                .Select(p => { var items = p.Split(','); return (last: items[0], first: items[1]); })
                .ToArray();

            var data = File.ReadAllLines(DataFilePath);

            CityArray = data[0].Split('\t');
            StreetArray = data[1].Split('\t');


            var records = new List<AddressRecord> { };

            data.Skip(2).ToList().ForEach(p =>
            {
                var items = p.Split('\t');

                var address = new AddressRecord
                {
                    cityname_id = int.Parse(items[0]),
                    streetname_id = int.Parse(items[1]),
                    street_address = items[2],
                    apn_code = items[3],
                    zip_code = rnd.Next(93700, 93800).ToString(),
                    last_name = full_names[records.Count % full_names.Length].last,
                    first_name = full_names[records.Count % full_names.Length].first,
                    precinct_id = rnd.Next(1, 200),
                    party_id = rnd.Next(1, 3)
                };

                records.Add(address);
            });

            AddressRecords = records.OrderBy(p => p.last_name).ThenBy(p => p.first_name).ToArray();
        }

        static Dictionary<string, int> Cities { get; } = new Dictionary<string, int> { };
        static Dictionary<string, int> Streets { get; } = new Dictionary<string, int> { };

        static string[] PartyArray { get; } = new string[] { "DEMOCRATIC", "REPUBLICAN", "OTHER" };
        static string[] CityArray { get; set; } = new string[] { };
        static string[] StreetArray { get; set; } = new string[] { };

        static Dictionary<int, List<int>> StreetNameIndex { get; } = new Dictionary<int, List<int>> { };

        static AddressRecord[] AddressRecords { get; set; } = new AddressRecord[] { };

        static (string line_prefix, int city_id, string line_suffix) ParseLine(string line)
        {
            foreach (var city in Cities.Keys)
            {
                var padded_term = $" {city} ";

                var match = line.LastIndexOf(padded_term);

                if (match >= 0)
                    return (line.Substring(startIndex: 0, length: match), Cities[city], line.Substring(startIndex: match + padded_term.Length));

            }

            return ("", 0, "");
        }

        internal static void Init()
        {
            Load();
        }

        internal static string[] Match(string name, string street)
        {
            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(street))
                return FormatRecords(AddressRecords.Take(50));

            var matches = AddressRecords.Where(p => true);

            if (!String.IsNullOrEmpty(name))
                matches = matches.Where(p => p.first_name.StartsWith(name) || p.last_name.StartsWith(name));

            if (!String.IsNullOrEmpty(street))
                foreach (var street_item in street.Split(' '))
                {
                    if (Char.IsDigit(street_item[0]))
                        matches = matches.Where(p => p.street_address.StartsWith(street_item));
                    else
                        matches = matches.Where(p => StreetArray[p.streetname_id-1].Contains(street_item));
                }

            return FormatRecords(matches.ToList());
        }

        static string[] FormatRecords(IEnumerable<AddressRecord> records)
        {
            return records.Select(p =>
                    $"{p.last_name}, {p.first_name} -- PRECINCT {p.precinct_id:00000} -- {PartyArray[p.party_id-1]} -- {CityArray[p.cityname_id-1]} -- {p.zip_code} -- {p.street_address} {StreetArray[p.streetname_id-1]}")
                        .ToArray();
        }

        internal static void ParseSitus()
        {
            var city_list = @"FRESNO,CLOVIS,COALINGA,KINGSBURG,KERMAN,SELMA,FOWLER,SANGER,PARLIER,BIOLA,REEDLEY,DINKEY CREEK,PIEDRA,PINEDALE,PINEHURST,HIGHWAY CITY,DEL REY,HURON,AUBERRY,ORANGE COVE,SAN JOAQUIN,TRANQUILLITY,FIREBAUGH,DINUBA,DOS PALOS,RIVERDALE,MENDOTA,EASTON,SQUAW VALLEY,CARUTHERS,SHAVER LAKE,LATON,PRATHER,HUME LAKE,MALAGA,FRIANT,FIVE POINTS,TOLLHOUSE,RAISIN CITY,MIRAMONTE,CANTUA CREEK,HERNDON,BURRELL,MINERAL,HELM,BIG CREEK,DUNLAP,DUNBAR,HUNTINGTON LA".Split(',');

            city_list.ToList().ForEach(p => Cities.Add(p, Cities.Keys.Count + 1));

            var situs = File.ReadAllLines(@"Data/situs.txt");

            var records = new List<AddressRecord> { };

            foreach (var line in situs)
            {
                if (line.StartsWith("PAGE:"))
                    continue;

                (string prefix, int city_id, string suffix) = ParseLine(line);

                if (city_id == 0)
                {
                    msg($"[!] -- no city in: {line}");
                    continue;
                }

                var parts = suffix.Split(' ');

                // "W" "E" etc prefixes are after the city name:

                if (!Char.IsDigit(parts[0][0]))
                {
                    prefix = $"{parts[0][0]} {prefix}";
                    parts = parts.Skip(1).ToArray();
                }

                if (!Streets.ContainsKey(prefix))
                    Streets.Add(prefix, Streets.Keys.Count + 1);

                records.Add(new AddressRecord { cityname_id = city_id, streetname_id = Streets[prefix], street_address = parts[0], apn_code = parts[1] });
            }

            AddressRecords = records.ToArray();

            Save();

        }
    }
}
