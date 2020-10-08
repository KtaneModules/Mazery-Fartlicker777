using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using MazeryGraph;

public class Mazery : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Buttons;
    public Material[] Colors;
    public GameObject[] Nodes;
    public KMColorblindMode ColorblindMode;
    public TextMesh[] ColorblindText;
    private bool colorblindMode = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    int[] Initalthingslol = {12, 14, 16, 18, 20, 34, 36, 38, 40, 42, 56, 58, 60, 62, 64, 78, 80, 82, 84, 86, 100, 102, 104, 106, 108};
    int[] Posibilities = {12, 14, 16, 18, 20, 34, 36, 38, 40, 42, 56, 58, 60, 62, 64, 78, 80, 82, 84, 86, 100, 102, 104, 106, 108};
    int[] NodesPosition = {0,0,0};
    int Counter = 0;
    int ColorCounter = 0;
    int Traveling = 0;

    private string[][] Maze = new string[11][]
      {
          new string[] { "█", "█", "█", "█", "█", "█", "█", "█", "█", "█", "█" },
          new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
          new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
          new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
          new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
          new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
          new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
          new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
          new string[] { "█", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "▓", "█" },
          new string[] { "█", "░", "▓", "░", "▓", "░", "▓", "░", "▓", "░", "█" },
          new string[] { "█", "█", "█", "█", "█", "█", "█", "█", "█", "█", "█" },
      };
      string VennDiagramOfColors = "MIKWCTLGNOPRBYA";
      string MazeForTraversingSinceIAmBigDumbass = "";

      private bool[][] visited = new bool[5][]
      {
          new bool[] { false, false, false, false, false },
          new bool[] { false, false, false, false, false },
          new bool[] { false, false, false, false, false },
          new bool[] { false, false, false, false, false },
          new bool[] { false, false, false, false, false }
      };
      List<int> GetAvenues(int x, int y)
      {
          List<int> temp = new List<int>();
          if (y - 1 >= 0)
              if (visited[x][y - 1])
                  temp.Add(0);
          if (y + 1 <= 4)
              if (visited[x][y + 1])
                  temp.Add(1);
          if (x - 1 >= 0)
              if (visited[x - 1][y])
                  temp.Add(2);
          if (x + 1 <= 4)
              if (visited[x + 1][y])
                  temp.Add(3);
          return temp;
      }
      bool HasVisitors(int x, int y)
      {
          if (y - 1 >= 0)
              if (visited[x][y - 1])
                  return true;
          if (y + 1 <= 4)
              if (visited[x][y + 1])
                  return true;
          if (x - 1 >= 0)
              if (visited[x - 1][y])
                  return true;
          if (x + 1 <= 4)
              if (visited[x + 1][y])
                  return true;
          return false;
      }
      bool ContainsFalse()
      {
          for (int i = 0; i < 5; i++)
          {
              for (int j = 0; j < 5; j++)
              {
                  if (visited[i][j] == false)
                      return true;
              }
          }
          return false;
      }
      bool[] NodesTraveled = {false, false, false};
      bool Active = false;
      bool OnTarget = false;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Button in Buttons) {
            Button.OnInteract += delegate () { ButtonPress(Button); return false; };
        }
        colorblindMode = ColorblindMode.ColorblindModeActive;
    }

    void Start () {
      MazeDisplay();
    }

    void Update () {
      for (int i = 0; i < Nodes.Length; i++) {
        if (colorblindMode) {
          string nodeColor = Regex.Match(Nodes[i].GetComponent<MeshRenderer>().material.ToString(), @"^([\w\-]+)").Value;
          switch (nodeColor) {
            //MIKWCTLGNOPRBYA
            case "maroon":  ColorblindText[i].text = "M"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "pink":    ColorblindText[i].text = "I"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "black":   ColorblindText[i].text = "K"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "white":   ColorblindText[i].text = "W"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "cyan":    ColorblindText[i].text = "C"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "tan":     ColorblindText[i].text = "T"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "lime":    ColorblindText[i].text = "L"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "green":   ColorblindText[i].text = "G"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "brown":   ColorblindText[i].text = "N"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "orange":  ColorblindText[i].text = "O"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "purple":  ColorblindText[i].text = "P"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "red":     ColorblindText[i].text = "R"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "blue":    ColorblindText[i].text = "B"; ColorblindText[i].color = new Color(255, 255, 255);break;
            case "yellow":  ColorblindText[i].text = "Y"; ColorblindText[i].color = new Color(0, 0, 0)      ;break;
            case "grey":    ColorblindText[i].text = "A"; ColorblindText[i].color = new Color(255, 255, 255);break;
          }
        }
        else ColorblindText[i].text = "";
      }
    }

    void ButtonPress (KMSelectable Button) {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
      if (moduleSolved) {
        return;
      }
      if (!Active) {
        Active = true;
        Counter = 0;
        for (int i = 0; i < Nodes.Length; i++) {
          Nodes[Counter].GetComponent<MeshRenderer>().material = Colors[3];
          Counter++;
        }
        Posibilities.Shuffle();
        Traveling = Posibilities[0];
        Nodes[Array.IndexOf(Initalthingslol, Posibilities[0])].GetComponent<MeshRenderer>().material = Colors[9];
        for (int i = 0; i < NodesPosition.Length; i++) {
          while ((MazeForTraversingSinceIAmBigDumbass[NodesPosition[i]] != '░') || (NodesPosition[i] == NodesPosition[0] && i != 0 || NodesPosition[i] == NodesPosition[1] && i != 1 || NodesPosition[i] == NodesPosition[2] && i != 2)) {
            NodesPosition[i] = Posibilities[i + 1];
            Nodes[Array.IndexOf(Initalthingslol, Posibilities[i + 1])].GetComponent<MeshRenderer>().material = Colors[4];
          }
        }
      }
      else {
        if (Button == Buttons[0]) {
          if (MazeForTraversingSinceIAmBigDumbass[Traveling + 11] != '░') {
            GetComponent<KMBombModule>().HandleStrike();
            MazeDisplay();
            Active = false;
            OnTarget = false;
            for (int i = 0; i < 3; i++) {
              NodesTraveled[i] = false;
              NodesPosition[i] = 0;
            }
          }
          else {
            if (OnTarget) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[4];
              OnTarget = false;
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[3];
            }
            Traveling += 22;
            bool[] JustCheckingIn = {false, false, false};
            for (int i = 1; i < 4; i++) {
              if (Traveling == Posibilities[i]) {
                NodesTraveled[i - 1] = !NodesTraveled[i - 1];
                OnTarget = true;
              }
              else {
                JustCheckingIn[i - 1] = true;
              }
            }
            if (JustCheckingIn[0] && JustCheckingIn[1] && JustCheckingIn[2]) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[9];
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[10];
            }
          }
        }
        else if (Button == Buttons[1]) {
          if (MazeForTraversingSinceIAmBigDumbass[Traveling - 11] != '░') {
            GetComponent<KMBombModule>().HandleStrike();
            MazeDisplay();
            Active = false;
            OnTarget = false;
            for (int i = 0; i < 3; i++) {
              NodesTraveled[i] = false;
              NodesPosition[i] = 0;
            }
          }
          else {
            if (OnTarget) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[4];
              OnTarget = false;
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[3];
            }
            Traveling -= 22;
            bool[] JustCheckingIn = {false, false, false};
            for (int i = 1; i < 4; i++) {
              if (Traveling == Posibilities[i]) {
                NodesTraveled[i - 1] = !NodesTraveled[i - 1];
                OnTarget = true;
              }
              else {
                JustCheckingIn[i - 1] = true;
              }
            }
            if (JustCheckingIn[0] && JustCheckingIn[1] && JustCheckingIn[2]) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[9];
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[10];
            }
          }
        }
        else if (Button == Buttons[2]) {
          if (MazeForTraversingSinceIAmBigDumbass[Traveling + 1] != '░') {
            GetComponent<KMBombModule>().HandleStrike();
            MazeDisplay();
            Active = false;
            OnTarget = false;
            for (int i = 0; i < 3; i++) {
              NodesTraveled[i] = false;
              NodesPosition[i] = 0;
            }
          }
          else {
            if (OnTarget) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[4];
              OnTarget = false;
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[3];
            }
            Traveling += 2;
            bool[] JustCheckingIn = {false, false, false};
            for (int i = 1; i < 4; i++) {
              if (Traveling == Posibilities[i]) {
                NodesTraveled[i - 1] = !NodesTraveled[i - 1];
                OnTarget = true;
              }
              else {
                JustCheckingIn[i - 1] = true;
              }
            }
            if (JustCheckingIn[0] && JustCheckingIn[1] && JustCheckingIn[2]) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[9];
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[10];
            }
          }
        }
        else if (Button == Buttons[3]) {
          if (MazeForTraversingSinceIAmBigDumbass[Traveling - 1] != '░') {
            GetComponent<KMBombModule>().HandleStrike();
            MazeDisplay();
            Active = false;
            OnTarget = false;
            for (int i = 0; i < 3; i++) {
              NodesTraveled[i] = false;
              NodesPosition[i] = 0;
            }
          }
          else {
            if (OnTarget) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[4];
              OnTarget = false;
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[3];
            }
            Traveling -= 2;
            bool[] JustCheckingIn = {false, false, false};
            for (int i = 1; i < 4; i++) {
              if (Traveling == Posibilities[i]) {
                NodesTraveled[i - 1] = !NodesTraveled[i - 1];
                OnTarget = true;
              }
              else {
                JustCheckingIn[i - 1] = true;
              }
            }
            if (JustCheckingIn[0] && JustCheckingIn[1] && JustCheckingIn[2]) {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[9];
            }
            else {
              Nodes[Array.IndexOf(Initalthingslol, Traveling)].GetComponent<MeshRenderer>().material = Colors[10];
            }
          }
        }
        if (NodesTraveled[0] && NodesTraveled[1] && NodesTraveled[2]) {
          GetComponent<KMBombModule>().HandlePass();
          moduleSolved = true;
        }
      }
    }

    void GenerateMaze()
      {
          int[] vals = new int[] { 1, 3, 5, 7, 9 };
          int x = UnityEngine.Random.Range(0, 5);
          int y = UnityEngine.Random.Range(0, 5);
          Debug.Log("Starting position: (" + x + ", " + y + ")");
          while (ContainsFalse())
          {
              visited[x][y] = true;
              // U, D, L, R
              string[] moves = new string[4];
              List<int> posDirs = new List<int>();
              moves[0] = Maze[vals[y] - 1][vals[x]];
              moves[1] = Maze[vals[y] + 1][vals[x]];
              moves[2] = Maze[vals[y]][vals[x] - 1];
              moves[3] = Maze[vals[y]][vals[x] + 1];
              if (moves[0] == "▓")
              {
                  if (!visited[x][y - 1])
                      posDirs.Add(0);
              }
              if (moves[1] == "▓")
              {
                  if (!visited[x][y + 1])
                      posDirs.Add(1);
              }
              if (moves[2] == "▓")
              {
                  if (!visited[x - 1][y])
                      posDirs.Add(2);
              }
              if (moves[3] == "▓")
              {
                  if (!visited[x + 1][y])
                      posDirs.Add(3);
              }
              if (posDirs.Count == 0)
              {
                  Debug.Log("No possible moves, getting new start");
                  bool found = false;
                  for (int i = 0; i < 5; i++)
                  {
                      for (int j = 0; j < 5; j++)
                      {
                          if (!visited[i][j] && HasVisitors(i, j))
                          {
                              x = i;
                              y = j;
                              Debug.Log("New start: (" + x + ", " + y + ")");
                              List<int> newAvs = GetAvenues(x, y);
                              int num = UnityEngine.Random.Range(1, newAvs.Count);
                              for (int k = 0; k < num; k++)
                              {
                                  int choice = UnityEngine.Random.Range(0, newAvs.Count);
                                  if (newAvs[choice] == 0)
                                  {
                                      Maze[vals[y] - 1][vals[x]] = "░";
                                  }
                                  else if (newAvs[choice] == 1)
                                  {
                                      Maze[vals[y] + 1][vals[x]] = "░";
                                  }
                                  else if (newAvs[choice] == 2)
                                  {
                                      Maze[vals[y]][vals[x] - 1] = "░";
                                  }
                                  else if (newAvs[choice] == 3)
                                  {
                                      Maze[vals[y]][vals[x] + 1] = "░";
                                  }
                                  string[] logNames = new string[] { "UP", "DOWN", "LEFT", "RIGHT" };
                                  Debug.Log("Removing a wall " + logNames[newAvs[choice]] + " from start");
                                  newAvs.RemoveAt(choice);
                              }
                              visited[x][y] = true;
                              found = true;
                              break;
                          }
                      }
                      if (found)
                          break;
                  }
              }
              else
              {
                  int choice = UnityEngine.Random.Range(0, posDirs.Count);
                  if (posDirs[choice] == 0)
                  {
                      Maze[vals[y] - 1][vals[x]] = "░";
                      y--;
                  }
                  else if (posDirs[choice] == 1)
                  {
                      Maze[vals[y] + 1][vals[x]] = "░";
                      y++;
                  }
                  else if (posDirs[choice] == 2)
                  {
                      Maze[vals[y]][vals[x] - 1] = "░";
                      x--;
                  }
                  else if (posDirs[choice] == 3)
                  {
                      Maze[vals[y]][vals[x] + 1] = "░";
                      x++;
                  }
                  string[] logNames = new string[] { "UP", "DOWN", "LEFT", "RIGHT" };
                  Debug.Log("Found moves, going " + logNames[posDirs[choice]] + " to (" + x + ", " + y + ")");
              }
          }
          string Mazelog = "";
          for (int i = 0; i < 11; i++)
          {
              for (int j = 0; j < 11; j++)
              {
                  Mazelog += Maze[i][j];
              }
              if (i != 10)
                  Mazelog += "\n";
          }
          Debug.LogFormat("[Mazery #{0}] The maze is\n{1}", moduleId, Mazelog);
      }

    void MazeDisplay () {
      GenerateMaze();
      Counter = 0;
      for (int i = 0; i < 11; i++)
        if (i % 2 == 1)
          for (int j = 0; j < Maze[i].Length; j++) {
            if (Maze[i][j] == "░" && j % 2 == 1) {
              ColorCounter = 0;
              if (Maze[i - 1][j] != "░")
                ColorCounter += 1;
              if (Maze[i][j - 1] != "░")
                ColorCounter += 2;
              if (Maze[i][j + 1] != "░")
                ColorCounter += 4;
              if (Maze[i + 1][j] != "░")
                ColorCounter += 8;
              Nodes[Counter].GetComponent<MeshRenderer>().material = Colors[ColorCounter];
              Counter++;
            }
          }
      for (int i = 0; i < 11; i++)
        for (int j = 0; j < 11; j++)
          MazeForTraversingSinceIAmBigDumbass += Maze[i][j];
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} activate to start the module. Use !{0} u/l/d/r to move up/left/down/right. You can chain commands like !{0} lluullddrduld.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      if (Command == "COLORBLIND")
        colorblindMode = true;
      else if (Command == "ACTIVATE")
        Buttons[UnityEngine.Random.Range(0,4)].OnInteract();
      else {
        yield return null;
        for (int i = 0; i < Command.Length; i++)
          if (Command[i] != 'U' && Command[i] != 'L' && Command[i] != 'D' && Command[i] != 'R') {
            yield return "sendtochaterror I don't understand!";
            yield break;
          }
        for (int i = 0; i < Command.Length; i++) {
          if (Command.Length > 1) yield return "strikemessage move #" + (i + 1);
          if (Command[i] == 'D')
            Buttons[0].OnInteract();
          else if (Command[i] == 'U')
            Buttons[1].OnInteract();
          else if (Command[i] == 'R')
            Buttons[2].OnInteract();
          else if (Command[i] == 'L')
            Buttons[3].OnInteract();
          yield return new WaitForSeconds(.1f);
        }
      }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!Active)
        {
            Buttons[0].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        List<Directions> moves = BFS();
        if (moves == null)
            throw new Exception("Somehow there is no valid moves to the goal state.");
        foreach(Directions move in moves)
        {
            Buttons[(int) move].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
    }

    List<Directions> BFS()
    {
        List<State> visited = new List<State>();
        State start = new State(Maze, new[] { Traveling % 11 / 2, Traveling / 22 }, NodesPosition.Select(num => new[] { num % 11 / 2, num / 22 }).ToArray(), NodesTraveled.ToArray(), Directions.None);
        Queue<State> queue = new Queue<State>();
        Dictionary<State, State> previous = new Dictionary<State, State>();
        List<Directions> moves = new List<Directions>();
        queue.Enqueue(start);
        while (queue.Count != 0)
        {
            State currentState = queue.Dequeue();
            visited.Add(currentState);
            if (currentState.IsGoal())
            {
                while (currentState.ActionToHere != Directions.None)
                {
                    moves.Insert(0, currentState.ActionToHere);
                    currentState = previous[currentState];
                }
                if (moves.Count == 0) moves.Add(Directions.None);
                queue.Clear();
                visited.Clear();
                previous.Clear();
                return moves;
            }

            List<State> successors = currentState.GetSuccessors();
            foreach (State successor in successors)
                if (!queue.Contains(successor) && !visited.Contains(successor))
                {
                    queue.Enqueue(successor);
                    previous.Add(successor, currentState);
                }
        }
        visited.Clear();
        previous.Clear();
        return null;
    }
}
