using System.Linq;
using Aoc.Runner;

namespace Aoc.Solutions
{
    // 2019 day 02
    public class Day02_2019 : Day
    {
        public override string SolveA()
        {
            var input = GetInput();
            var op = GetInput().Split(",").Select(el => int.Parse(el)!).ToList();
            op[1] = 12;
            op[2] = 2;
            return SolveA(string.Join(",", op));
        }
        public override string SolveA(string Input)
        {
            var op = Input.Split(",").Select(el => int.Parse(el)!).ToList();

            var ip = 0;
            var @continue = true;
            while (@continue)
            {
                var code = op[ip];
                var (a, b, c) = (op[ip + 1], op[ip + 2], op[ip + 3]);
                switch (code)
                {
                    case 1:
                        op[c] = op[a] + op[b];
                        break;
                    case 2:
                        op[c] = op[a] * op[b];
                        break;
                    case 99:
                        @continue = false;
                        break;
                }
                ip += 4;
            }

            return string.Join(", ", op.Select(o => o.ToString()));
        }
        public Day02_2019()
        {
        }
    }
}