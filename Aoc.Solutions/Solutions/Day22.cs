using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions.D22 {
    using Deck = LinkedList<long>;

    static class LinkedListExtensions {
        static public T Deq<T>(this LinkedList<T> self) {
            var first = self.First();
            self.RemoveFirst();
            return first;
        }
        static public void Enq<T>(this LinkedList<T> self, T item) {
            self.AddLast(item);
        }
    }

    class Game {
        public Deck Player1 { get; init; } = new();
        public Deck Player2 { get; init; } = new();

        public bool Round() {
            var card1 = Player1.Deq();
            var card2 = Player2.Deq();
            if (card1 > card2) {
                Player1.Enq(card1);
                Player1.Enq(card2);
            }
            else {
                Player2.Enq(card2);
                Player2.Enq(card1);
            }

            return Player1.Count == 0 || Player2.Count == 0;
        }

        public Deck Play() {
            while (!Round()) { }
            return Player1.Any() ? Player1 : Player2;
        }
    }


    class RecursiveGame {
        public RecursiveGame() { }
        public RecursiveGame(Game game) {
            Player1 = game.Player1;
            Player2 = game.Player2;
        }

        public Deck Player1 { get; init; } = new();
        public Deck Player2 { get; init; } = new();
        int RoundNumber = 0;


        readonly HashSet<string> PreviousStates = new();

        public bool Round(out bool player1Wins) {
            RoundNumber++;
            // Show();
            // end if state has been seen before
            var stateString = string.Join(", ", Player1) + "|" + string.Join(", ", Player2);
            if (PreviousStates.Contains(stateString)) {
                // "Reached previous state".Dbg();
                player1Wins = true;
                return true;
            }
            else {
                PreviousStates.Add(stateString);
            }

            // draw
            var card1 = Player1.Deq();
            var card2 = Player2.Deq();
            // $"Player 1 plays: {card1}\nPlayer 2 plays: {card2}".Dbg();

            // both have enough cards to play recursive game
            if (Player1.Count >= card1 && Player2.Count >= card2) {
                // $"entering recursive combat".Dbg();
                var newDeck1 = new Deck(Player1.Take((int)card1));
                var newDeck2 = new Deck(Player2.Take((int)card2));
                RecursiveGame subGame = new() { Player1 = newDeck1, Player2 = newDeck2 };
                subGame.Play(out player1Wins);
            }
            // someone doesn't have enough cards
            else {
                // bigger card wins this round
                player1Wins = card1 > card2;
                var winningPlayer = player1Wins ? 1 : 2;
                // $"Player {winningPlayer} wins".Dbg();
            }

            if (player1Wins) {
                Player1.Enq(card1);
                Player1.Enq(card2);
            }
            else {
                Player2.Enq(card2);
                Player2.Enq(card1);
            }

            return Player1.Count == 0 || Player2.Count == 0;
        }

        public Deck Play(out bool player1Wins) {
            while (!Round(out player1Wins)) { }
            return player1Wins ? Player1 : Player2;
        }

        public bool DecksEqual(Deck player1, Deck player2) {
            return
                Player1.Count == player1.Count &&
                Player2.Count == player2.Count &&
                player1.Zip(Player1).All(p => p.First == p.Second) &&
                player2.Zip(Player2).All(p => p.First == p.Second);
        }

        public void Show() {
            Console.WriteLine($"--- Round {RoundNumber}");
            Console.WriteLine($"Player 1's deck: {string.Join(", ", Player1)}");
            Console.WriteLine($"Player 2's deck: {string.Join(", ", Player2)}");
        }
    }



    public class Day22 : Day {
        public override string SolveA(string input) {
            var game = Parse(input);
            var winning = game.Play();
            // winning.Dbg();
            var result = winning.Reverse().WithIndex().Aggregate(
                0L,
                (acc, card) => acc += card.item * (card.index + 1)
            );


            return result.ToString();
        }

        public override string SolveB(string input) {
            var game = new RecursiveGame(Parse(input));
            var winning = game.Play(out var player1Wins);
            game.Show();
            // winning.Dbg();
            var result = winning.Reverse().WithIndex().Aggregate(
                0L,
                (acc, card) => acc += card.item * (card.index + 1)
            );
            return result.ToString();
        }
        static Game Parse(string input) {
            static Deck ParsePlayer(string playerInput) =>
                new(playerInput.Split("\n").Skip(1)
                    .Select(long.Parse)
                );

            var players = input.Split("\n\n").Select(ParsePlayer).ToList();
            return new() { Player1 = players[0], Player2 = players[1] };
        }
        public Day22() {
            Tests = new()
            {
                new("1", "sample", "306", input => GetInput(input).Then(SolveA)),
                new("2", "sample", "291", input => GetInput(input).Then(SolveB)),
                new("rec", "recsample", "0", input => GetInput(input).Then(SolveB))
            };
        }

    }
}
