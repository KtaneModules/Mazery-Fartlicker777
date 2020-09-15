using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeryGraph
{
    public class State : IEquatable<State>
    {
        public string[][] Maze;
        public int[] Pos; //[X, Y] and Top left is [0, 0]
        public int[][] GoalLocations;
        public bool[] GoalStatus;
        public Directions ActionToHere;

        public string _simplifiedMaze;
        private string _simplifiedGoalLocations;
        public State(string[][] maze, int[] pos, int[][] goalLocations, bool[] goalStatus, Directions actionToHere)
        {
            Maze = maze;
            Pos = pos;
            GoalLocations = goalLocations;
            GoalStatus = goalStatus;
            ActionToHere = actionToHere;
            _simplifiedMaze = string.Join("", Maze.SelectMany(x => x).ToArray());
            _simplifiedGoalLocations = string.Join("", GoalLocations.SelectMany(x => x).Select(x => x.ToString()).ToArray());
        }

        public bool IsGoal() {
            return GoalStatus.All(x => x);
        }

        public List<State> GetSuccessors()
        {
            List<State> list = new List<State>();
            //Go West
            if (Pos[0] - 1 >= 0 && Maze[2 * Pos[1] + 1][2 * Pos[0]] == "░")
                list.Add(_generateNextState(Directions.West));
            //Go North
            if (Pos[1] - 1 >= 0 && Maze[2 * Pos[1]][2 * Pos[0] + 1] == "░")
                list.Add(_generateNextState(Directions.North));
            //Go East
            if (Pos[0] + 1 <= 4 && Maze[2 * Pos[1] + 1][2 * Pos[0] + 2] == "░")
                list.Add(_generateNextState(Directions.East));
            //Go South
            if (Pos[1] + 1 <= 4 && Maze[2 * Pos[1] + 2][2 * Pos[0] + 1] == "░")
                list.Add(_generateNextState(Directions.South));
            return list;
        }

        private State _generateNextState(Directions actionToHere)
        {
            int[] pos = new int[2];
            switch (actionToHere)
            {
                case Directions.West:
                    pos = new[] { Pos[0] - 1, Pos[1] };
                    break;
                case Directions.North:
                    pos = new[] { Pos[0], Pos[1] - 1 };
                    break;
                case Directions.East:
                    pos = new[] { Pos[0] + 1, Pos[1] };
                    break;
                case Directions.South:
                    pos = new[] { Pos[0], Pos[1] + 1 };
                    break;
            }
            bool[] newGoalStatus = GoalStatus.ToArray();
            for (int i = 0; i < GoalLocations.Length; i++)
                if (Enumerable.SequenceEqual(GoalLocations[i], pos))
                {
                    newGoalStatus[i] ^= true;
                    break;
                }

            return new State(Maze, pos, GoalLocations, newGoalStatus, actionToHere);
        }

        public static bool operator ==(State firstState, State otherState)
        {
            return firstState.Equals(otherState);
        }

        public static bool operator !=(State firstState, State otherState)
        {
            return !firstState.Equals(otherState);
        }

        public bool Equals(State otherState)
        {
            return _simplifiedMaze == otherState._simplifiedMaze && Enumerable.SequenceEqual(Pos, otherState.Pos) && GoalLocations.All(loc => otherState.GoalLocations.Any(oLoc => Enumerable.SequenceEqual(loc, oLoc))) && Enumerable.SequenceEqual(GoalStatus, otherState.GoalStatus);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
                return false;

            return Equals((State) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 31;
                hash = hash * 47 + _simplifiedMaze.GetHashCode();
                foreach (int num in Pos)
                    hash = hash * 47 + num.GetHashCode();
                hash = hash * 47 + _simplifiedGoalLocations.GetHashCode();
                foreach (bool val in GoalStatus)
                    hash = hash * 47 + val.GetHashCode();
                return hash;
            }
        }
    }
}
