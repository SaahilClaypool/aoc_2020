using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day04 : Day {
        readonly List<string> RequiredFields = new List<string>() {
            "byr",
            "iyr",
            "eyr",
            "hgt",
            "hcl",
            "ecl",
            "pid",
        };

        public static IEnumerable<Dictionary<string, string>> ParseInput(string input) {
            var blocks = CommonExtensions.SplitAt(input.Split(null), line => line.Trim().Length == 0);
            var passports = blocks.Select(block => {
                var dict = new Dictionary<string, string>();
                foreach (var value in block) {
                    var key_value = value.Split(":");
                    dict[key_value[0]] = key_value[1];
                }
                return dict;
            });
            return passports;
        }


        readonly Dictionary<string, Func<string, bool>> Rules = new() {
            { "byr", byr => int.TryParse(byr, out var v) && v is >= 1920 and <= 2002 },
            { "iyr", iyr => int.TryParse(iyr, out var v) && v is >= 2010 and <= 2020 },
            { "eyr", eyr => int.TryParse(eyr, out var v) && v is >= 2020 and <= 2030 },
            {
                "hgt",
                hgt => {
                    try {
                        if (hgt.EndsWith("cm")) {
                            var height = int.Parse(hgt[0..^2]);
                            return height is >= 150 and <= 193;
                        }
                        else if (hgt.EndsWith("in")) {
                            var height = int.Parse(hgt[0..^2]);
                            return height is >= 59 and <= 76;
                        }
                        return false;
                    }
                    catch {
                        return false;
                    }
                }
            },
            {
                "hcl",
                hcl => new Regex(@"^#[0-9a-f]{6}").IsMatch(hcl)
            },
            {
                "ecl",
                ecl =>
           new List<string>() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.IndexOf(ecl) >= 0
            },
            {
                "pid",
                pid => pid.Length == 9 && int.TryParse(pid, out var v)
            },
            { "cid", cid => true }
        };


        public override string SolveA(string input) {
            var passports = ParseInput(input);
            var numberValid = passports.Where(passport =>
                    RequiredFields.All(field => passport.ContainsKey(field))
            ).Count();
            return numberValid.ToString();
        }

        public override string SolveB(string input) {
            var passports = ParseInput(input);
            return passports.Where(passport =>
            RequiredFields.All(field => passport.ContainsKey(field)) &&
            passport.All(kvp => {
                var res = Rules[kvp.Key](kvp.Value);
                if (!res) {
                    Console.WriteLine($"Failed: {kvp.Key} {kvp.Value}");
                }
                return res;
            }
            )).Count().ToString();
        }
        public Day04() {
            Tests = new()
            {
                new("byr valid", "2002", "True",
                    (input) => Rules["byr"](input).ToString()),
                new("byr invalid", "2003", "False",
                    (input) => Rules["byr"](input).ToString()),
                new("invalid", "invalid", "0", (input) => SolveB(GetInput(input))),
                new("valid", "valid", "4", (input) => SolveB(GetInput(input))),


                new("byr valid", "2002", "True", input => Rules["byr"](input).ToString()),
                new("byr invalid", "2003", "False", input => Rules["byr"](input).ToString()),
                new("hgt valid", "60in", "True", input => Rules["hgt"](input).ToString()),
                new("hgt valid", "190cm", "True", input => Rules["hgt"](input).ToString()),
                new("hgt invalid", "190in", "False", input => Rules["hgt"](input).ToString()),
                new("hgt invalid", "190", "False", input => Rules["hgt"](input).ToString()),
                new("hcl valid", "#123abc", "True", input => Rules["hcl"](input).ToString()),
                new("hcl invalid", "#123abz", "False", input => Rules["hcl"](input).ToString()),
                new("hcl invalid", "123abc", "False", input => Rules["hcl"](input).ToString()),
                new("ecl valid", "brn", "True", input => Rules["ecl"](input).ToString()),
                new("ecl invalid", "wat", "False", input => Rules["ecl"](input).ToString()),
                new("pid valid", "000000001", "True", input => Rules["pid"](input).ToString()),
                new("pid invalid", "0123456789", "False", input => Rules["pid"](input).ToString()),

            };

        }
    }
}