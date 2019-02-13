using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace TicTacToe___ConsoleApp
{
    public static class MathInt
    {
        public static int Clamp(int value, int max) { return value > max ? max : value; }
    }

    public enum Alphabet { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z }

    public class Board : IEnumerable
    {
        // fields
        protected (int width, int height) dimensions;
        protected (int x, int y) position;

        public ResolveSymbol resolveSymbol;

        // properties
        public int[,] Tiles { get; protected set; }
        public int Width { get { return dimensions.width; } }
        public int Height { get { return dimensions.height; } }
        public int X { get { return position.x; } }
        public int Y { get { return position.y; } }

        public bool IsDrawn { get; set; }

        // delegates
        public delegate string ResolveSymbol(int value);

        // methods
        public Board(int width, int height, int x, int y, ResolveSymbol func)
        {
            Tiles = new int[height, width];
            dimensions = (width, height);
            position = (x, y);
            resolveSymbol = func;
        }
        #region Indexers
        public int this[int y, int x]
        {
            get { return Tiles[y, x]; }
            set
            {
                Tiles[y, x] = value;
                if (IsDrawn)
                {
                    var pos = GetBoardPosition(x, y);
                    WriteAt(GetStrSymbol(value), pos.x, pos.y);
                }
            }
        }
        public int this[int i]
        {
            get
            {
                var pos = XY2D(i);
                return this[pos.y, pos.x];
            }
            set
            {
                var pos = XY2D(i);
                this[pos.y, pos.x] = value;
            }
        }

        public int Get(int y, string letter)
        {
            int x = GetAlphabeticalIndex(letter);
            return this[y, x];
        }
        public void Set(int y, string letter, int value)
        {
            int x = GetAlphabeticalIndex(letter);
            this[y, x] = value;
        }
        #endregion
        public bool IsInRange(int x, int y)
        {
            if (x < Width && x > -1 && y < Height && y > -1)
                return true;
            else
                return false;
        }
        public bool IsInRange(int i) { return (i < Width * Height && i > -1) ? true : false; }

        public int XY1D(int x, int y) { return y * Width + x; }
        public (int x, int y) XY2D(int i) { return (i % Width, i / Width); }

        public static int GetAlphabeticalIndex(string letter)
        {
            if (letter.Length == 1)
            {
                string l = letter.ToUpper();
                Alphabet a;
                for (int i = 0; i < 24; i++)
                {
                    a = (Alphabet)i;
                    if (l == a.ToString())
                        return i;
                }
            }

            return -1;
        }
        public static string GetAlphabetLetter(int index)
        {
            if (index >= 0 && index < 25)
            {
                Alphabet a = (Alphabet)index;
                return a.ToString();
            }
            else
                return "";
        }

        public void ResetBoard()
        {
            for (int i = 0; i < Width * Height; i++)
            {
                this[i] = 0;
            }
            DrawPieces();
        }
        #region Drawing       
        public void DrawBoard()
        {
            DrawGrid();
            DrawPieces();
            IsDrawn = true;
        }
        protected void DrawGrid()
        {
            for (int y = 0; y < Height * 2 - 1; y++)
            {
                bool yEven = y % 2 == 0 ? true : false;
                Console.SetCursorPosition(X, Y + y);

                for (int x = 0; x < Width * 2 - 1; x++)
                {
                    bool xEven = x % 2 == 0 ? true : false;

                    if (yEven)
                    {
                        if (xEven)
                            Console.Write("   ");
                        else
                            Console.Write("│");
                    }
                    else
                    {
                        if (xEven)
                            Console.Write("───");
                        else
                            Console.Write("┼");
                    }
                }
                Console.WriteLine();
            }
        }
        protected void DrawPieces()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y] != 0)
                    {
                        var pos = GetBoardPosition(x, y);
                        var val = this[y, x];
                        WriteAt(GetStrSymbol(val), pos.x, pos.y);
                    }
                }
            }
        }
        #endregion
        public (int x, int y) GetBoardPosition(int x, int y)
        {
            int _x = position.x + (x * 4 + 1);
            int _y = position.y + (y * 2);
            // . 1 . | . 5 . | . 9 .
            // ─ ─ ─
            // . 2 .
            // ─ ─ ─
            // . 4 .
            return (_x, _y);
        }

        public string GetStrSymbol(int value)
        {
            if (resolveSymbol != null)
            {
                string symbol = resolveSymbol.Invoke(value);
                if (symbol != null && symbol != "")
                    return symbol;
                else
                    return " "; // Return empty tile
            }
            else
            {
                throw new NullReferenceException("getSymbol delegate is not setup");
            }
        }

        protected static void WriteAt(string s, int x, int y)
        {
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(s);
                Console.SetCursorPosition(oldX, oldY);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        public override string ToString()
        {
            string str = "Board { ";
            for (int y = 0; y < Height; y++)
            {
                str += "(";
                for (int x = 0; x < Width; x++)
                {
                    str += this[y, x] + ", ";
                }
                str += "\b\b) ";
            }
            return str += " }";
        }

        #region TileMethods
        public static (int x, int y) TileUp(int x, int y) { return (x, y - 1); }
        public static (int x, int y) TileDown(int x, int y) { return (x, y + 1); }
        public static (int x, int y) TileLeft(int x, int y) { return (x - 1, y); }
        public static (int x, int y) TileRight(int x, int y) { return (x + 1, y); }
        public static (int x, int y) TileLeftUp(int x, int y) { return (x - 1, y - 1); }
        public static (int x, int y) TileRightUp(int x, int y) { return (x + 1, y - 1); }
        public static (int x, int y) TileLeftDown(int x, int y) { return (x - 1, y + 1); }
        public static (int x, int y) TileRightDown(int x, int y) { return (x + 1, y + 1); }

        public int TileUp(int i) { return i - Width; } // (. . .)(. . .)(. . .)
        public int TileDown(int i) { return i + Width; }
        public int TileLeft(int i) { return i - 1; }
        public int TileRight(int i) { return i + 1; }
        public int TileLeftUp(int i) { return i - Width - 1; }
        public int TileRightUp(int i) { return i - Width + 1; }
        public int TileLeftDown(int i) { return i + Width - 1; }
        public int TileRightDown(int i) { return i + Width + 1; }
        #endregion

        public IEnumerator GetEnumerator() { return Tiles.GetEnumerator(); }

        protected void GetDiagonalLines(ref List<Line> list)
        {
            void GetDiagonals(ref List<Line> _list, bool axis, bool diagDir, int offset)
            {
                int correct = axis ? Height : Width;
                int lateral = axis ? Width : Height;

                for (int a = 0 + offset; a < correct; a++)
                {
                    int length = diagDir ? correct - (correct - a - 1) : correct - a;
                    length = MathInt.Clamp(length, lateral);
                    var lane = new Line(length);

                    int tilePos = XY1D(0, a);
                    for (int t = 0; t < lane.Length; t++)
                    {
                        lane.tiles[t].SetInfo(this[tilePos], tilePos);
                        tilePos = diagDir ? TileRightUp(tilePos) : TileRightDown(tilePos);
                    }

                    _list.Add(lane);
                }
            }

            GetDiagonals(ref list, true, false, 0);
            GetDiagonals(ref list, false, false, 1);
            GetDiagonals(ref list, true, true, 0);
            GetDiagonals(ref list, false, true, 1);
            // TODO: remove duplicate lanes
        }

        protected void GetStraightLines(ref List<Line> list, bool axis)
        {
            int correct = axis ? Height : Width;
            int lateral = axis ? Width : Height;

            for (int c = 0; c < correct; c++)
            {
                var lane = new Line(lateral);
                for (int l = 0; l < lateral; l++)
                {
                    int x = axis ? l : c;
                    int y = axis ? c : l;
                    lane.tiles[l].SetInfo(this[y, x], XY1D(x, y));
                }
                list.Add(lane);
            }
        }

        public List<Line> GetLines()
        {
            List<Line> lanes = new List<Line>();

            GetDiagonalLines(ref lanes);
            GetStraightLines(ref lanes, true);
            GetStraightLines(ref lanes, false);

            return lanes;
        }

        // classes
        public class Line : IEnumerable
        {
            public Tile[] tiles;

            public int Length { get { return tiles.Length; } }

            // methods
            public Line(int length) { tiles = new Tile[length]; }


            public static void UpdateLines(ref List<Line> list, Board board)
            {
                foreach (var lane in list)
                {
                    lane.UpdateLine(board);
                }
            }

            public static void UpdateLines(ref Line[] array, Board board)
            {
                foreach (var lane in array)
                {
                    lane.UpdateLine(board);
                }
            }

            public int Count(int value)
            {
                int count = 0;

                for (int i = 0; i < Length; i++)
                {
                    if (tiles[i].val == value)
                        count++;
                }

                return count;
            }

            public void UpdateLine(Board board)
            {
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].val = board[tiles[i].pos];
                }
            }

            public IEnumerator GetEnumerator() { return tiles.GetEnumerator(); }
        }

        public struct Tile
        {
            public int val;
            public int pos; // 1 dimensional board position

            public void SetInfo(int _val, int _pos)
            {
                val = _val;
                pos = _pos;
            }
        }
    }

    class Program
    {
        // fields
        const int X = -1;
        const int O = -X;

        const int PRE_GAME = int.MaxValue;
        const int GAME_IN_PROGRESS = int.MinValue;
        const int DRAW = 0;

        static int gameState = PRE_GAME;

        static Board tttBoard = new Board(3, 3, 2, 2, GetSymbol);
        static Board.Line[] lanes;

        static Random rand = new Random();

        static int human = O;
        static int turn = O;

        // methods
        static void Main(string[] args)
        {
            StartGame();

            while (true)
            {
                if (gameState == GAME_IN_PROGRESS)
                {
                    if (turn == human)
                        HumanTurn();
                    else
                    {
                        // Comp play
                        Thread.Sleep(rand.Next(1000, 2000));
                        int play = SmartPlay(tttBoard, -human);
                        tttBoard[play] = -human;
                    }

                    Board.Line.UpdateLines(ref lanes, tttBoard);
                    gameState = WinConditionMet(tttBoard);
                    turn = -turn;
                }
                else
                {
                    EndGame();
                    StartGame();
                }
            }
        }

        protected static void EndGame()
        {
            switch (gameState)
            {
                case DRAW:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("DRAW");
                    break;
                case X:
                case O:
                    if (gameState == human)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("You WIN!");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You LOSE");
                    }
                    break;
            }
            Console.ResetColor();

            Console.ReadKey();
        }

        protected static int HumanTurn()
        {
            int play = -1;
            while (play == -1)
            {
                string input = Console.ReadLine();
                string[] strPos = input.Split(' ');

                if (strPos != null && strPos.Length == 2)
                {
                    int y = Int32.Parse(strPos[0]);
                    int x = Int32.Parse(strPos[1]);

                    // Play position if its empty
                    if (tttBoard[y, x] == 0)
                    {
                        tttBoard[y, x] = human;
                        play = tttBoard.XY1D(x, y);
                    }
                }
            }

            return play;
        }

        protected static void StartGame()
        {
            // Reset everything
            Console.Clear();
            tttBoard.ResetBoard();

            if (lanes == null || lanes.Length < 1)
                lanes = tttBoard.GetLines().Where(l => l.Length == 3).ToArray();
            else
                Board.Line.UpdateLines(ref lanes, tttBoard);

            gameState = PRE_GAME;
            human = 0;

            Console.WriteLine("TicTacToe - by Sam");
            tttBoard.DrawBoard();
            Console.WriteLine();


            // Player must choose a side
            Console.WriteLine("Choose a side (X/O) — O opens.");
            Console.WriteLine("Input format: 'Y X'. Zero based.");
            while (gameState == PRE_GAME)
            {
                string choice = Console.ReadLine().ToUpper();

                if (choice == "X")
                    human = X;
                else if (choice == "O")
                    human = O;

                if (human != 0)
                    gameState = GAME_IN_PROGRESS;
            }

            turn = O;
        }

        public static string GetSymbol(int value)
        {
            switch (value)
            {
                case X:
                    return "x";

                case O:
                    return "o";

                default:
                    return null;
            }
        }

        public static int WinConditionMet(Board board)
        {
            int result = GAME_IN_PROGRESS;

            Board.Line[] lanes = board.GetLines().Where(l => l.Length == 3).ToArray();
            int xCount = 0;
            int oCount = 0;


            for (int i = 0; i < lanes.Length; i++)
            {
                xCount = lanes[i].Count(X);
                oCount = lanes[i].Count(O);
                if (xCount == 3 || oCount == 3)
                {
                    if (xCount == 3)
                        result = X;
                    else
                        result = O;
                    break;
                }
            }

            // Check if board is full
            if (result == GAME_IN_PROGRESS)
            {
                int freeTiles = 0;
                for (int j = 0; j < 9; j++)
                {
                    if (board[j] == 0)
                        freeTiles++;
                }
                if (freeTiles == 0)
                    result = DRAW;
            }

            return result;
        }


        public static int RandomPlay(Board board, int player)
        {
            List<int> options = new List<int>();
            int play = -1; // Tile position

            // List all empty tiles
            for (int i = 0; i < board.Width * board.Height; i++)
            {
                if (board[i] == 0)
                    options.Add(i);
            }

            // Pick a random tile
            if (options.Count > 0)
            {
                play = options[rand.Next(options.Count)];
                Console.WriteLine("Comp random play");
            }

            return play;
        }

        public static int SmartPlay(Board board, int player)
        {
            int GetRandomTileOfValue(Board.Line lane, int nr)
            {
                List<int> tileOptions = new List<int>();
                for (int i = 0; i < lane.tiles.Length; i++)
                {
                    if (lane.tiles[i].val == nr)
                        tileOptions.Add(lane.tiles[i].pos);

                }
                if (tileOptions.Count > 0)
                    return tileOptions[rand.Next(tileOptions.Count - 1)];
                else
                    return -1;
            }

            // Can I win?
            var winning = lanes.Where(l => l.Count(player) == 2 && l.Count(0) > 0);
            if (winning.Count() > 0)
            {
                var lane = winning.ElementAt(rand.Next(winning.Count()));
                Console.WriteLine("Comp winning play");
                return GetRandomTileOfValue(lane, 0);
            }

            // Must I block a lane?
            var losing = lanes.Where(l => l.Count(-player) == 2 && l.Count(0) > 0);
            if (losing.Count() > 0)
            {
                var lane = losing.ElementAt(rand.Next(losing.Count()));
                Console.WriteLine("Comp defensive play");
                return GetRandomTileOfValue(lane, 0);
            }

            // Play in a lane that has atleast one friendly piece and two free spots
            var options = lanes.Where(l => l.Count(player) > 0 && l.Count(0) >= 2);
            if (options.Count() > 0)
            {
                var lane = options.ElementAt(rand.Next(options.Count() - 1));
                Console.WriteLine("Comp tactical play");
                return GetRandomTileOfValue(lane, 0);
            }
            else
                return RandomPlay(board, player);
        }

        public static int GeniusPlay(Board board, int player)
        {
            // Win?

            // Block?

            // Constructive play
            // When you play in a lane that already has one of your pieces and two open tiles,
            // you have a potential win the next turn. But it should check for a tile that enables two potentials,
            // which is a sure win.

            // It should make a play at a position, cache that board, and count the potential winning lanes.
            // This produces nine possible plays max, and it should pick the one with highest winning lanes.

            return -1;
        }

        protected static void WriteAt(string s, int x, int y)
        {
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(s);
                Console.SetCursorPosition(oldX, oldY);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}
