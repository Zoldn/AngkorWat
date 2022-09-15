using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.TowerBuilder
{
    internal class TowerFloor
    {
        public string? HorizontalWord { get; set; }
        /// <summary>
        /// Ключ - позиция буквы горизонтального слова, к которой оно цепляется
        /// </summary>
        public Dictionary<int, string> VerticalWords { get; private set; }
        public int HorizontalWordShift { get; set; }
        internal int VerticalHeight { get; set; }

        public void SetVerticalWords(Dictionary<int, string> words)
        {
            VerticalWords = words;
            if (words.Any())
            {
                VerticalHeight = words.First().Value.Length;
            }
        }

        public HashSet<string> UsedWords
        {
            get
            {
                var words = new HashSet<string>();

                if (HorizontalWord != null)
                {
                    words.Add(HorizontalWord);
                }

                foreach (var (_, word) in VerticalWords)
                {
                    words.Add(word);
                }

                return words;
            }
        }

        public TowerFloor()
        {
            HorizontalWord = null;
            HorizontalWordShift = 0;
            VerticalWords = new();
        }

        public override string ToString()
        {
            return $"{HorizontalWord}, ({string.Join(",", VerticalWords.Select(e => e.Value))})";
        }
    }

    internal class TowerProject
    {
        public HashSet<string> UsedWords { get; set; }
        public List<TowerFloor> Floors { get; set; }
        public TowerProject()
        {
            Floors = new List<TowerFloor>();
            UsedWords = new HashSet<string>();
        }

        internal int GetTotalMass()
        {
            int mass = 0;

            for (int i = 0; i < Floors.Count; i++)
            {
                if (Floors[i].HorizontalWord != null && i > 0)
                {
                    mass += Floors[i].HorizontalWord.Length - Floors[i - 1].VerticalWords.Count;
                }

                mass += Floors[i].VerticalWords.Sum(kv => kv.Value.Length) - Floors[i].VerticalWords.Count;
            }

            return mass;
        }

        

        internal int GetTotalHeight 
        { 
            get
            {
                int height = 0;

                foreach (var floor in Floors)
                {
                    if (floor.HorizontalWord != null)
                    {
                        height += floor.VerticalHeight - 1;
                    }
                    else
                    {
                        height += floor.VerticalHeight;
                    }
                }

                return height;
            } 
        }

        internal Tower.Tower ToTower()
        {
            var tower = new Tower.Tower();

            int z = 0;
            int x = 0;

            int floorIndex = 0;

            foreach (var floor in Floors)
            {
                if (floor.HorizontalWord != null)
                {
                    var prevFloor = Floors[floorIndex-1];
                    var position = x + prevFloor.VerticalWords.Keys.Min();

                    x = position + floor.HorizontalWordShift;



                    for (int i = 0; i < floor.HorizontalWord.Length; i++)
                    {
                        if (!tower.NewPoints.ContainsKey((x + i, 0, z)))
                        {
                            tower.NewPoints.Add((x + i, 0, z), floor.HorizontalWord[i]);
                        }
                    }
                }

                foreach (var (shift, word) in floor.VerticalWords)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (!tower.NewPoints.ContainsKey((x + shift, 0, z - i)))
                        {
                            tower.NewPoints.Add((x + shift, 0, z - i), word[i]);
                        }
                    }
                }

                floorIndex++;
                z -= floor.VerticalHeight - 1;
            }

            int minX = tower.NewPoints.Min(kv => kv.Key.Item1);
            int minZ = tower.NewPoints.Min(kv => kv.Key.Item3);

            var changedPoints = new Dictionary<(int, int, int), char>();

            foreach (var ((xx, yy, zz), c) in tower.NewPoints)
            {
                changedPoints.Add((xx - minX, yy, zz - minZ), c);
            }

            tower.NewPoints = changedPoints;

            return tower;
        }
    }

    internal class TowerBuilder
    {
        public HashSet<string> Words { get; set; }
        public HashSet<char> CommonChars { get; set; }
        public Random Random { get; private set; }
        public TowerBuilder(List<string> words)
        {
            Words = words
                .Select(e => e.ToUpper())
                .ToHashSet();

            CommonChars = new HashSet<char>();

            Random = new Random(42);
        }
        public Tower.Tower SearchTower()
        {
            CalculateCommonChars();

            var currentTowerProject = new TowerProject();

            var bestTowerProject = new TowerProject();

            ///int iteration = 0;

            for (int i = 0; i < 10000; i++)
            {
                currentTowerProject = DropRandomBottomFloor(currentTowerProject);

                while (true)
                {
                    bool isFloorAdded = TryMakeNextFloor(currentTowerProject);

                    if (!isFloorAdded)
                    {
                        if (currentTowerProject.GetTotalHeight > bestTowerProject.GetTotalHeight)
                        {
                            bestTowerProject = currentTowerProject;
                        }

                        break;
                    }
                }

                Console.WriteLine($"Iteration: {i.ToString().PadLeft(4, ' ')} " +
                    $"Best height: {bestTowerProject.GetTotalHeight.ToString().PadLeft(4, ' ')} " +
                    $"Current height: {currentTowerProject.GetTotalHeight.ToString().PadLeft(4, ' ')} " +
                    $"Current words: {currentTowerProject.UsedWords.Count.ToString().PadLeft(4, ' ')} " +
                    $"Current mass: {currentTowerProject.GetTotalMass().ToString().PadLeft(4, ' ')} "
                    );
            }

            Tower.Tower tower = bestTowerProject.ToTower();

            tower.Print();

            int failLayer = tower.IsNotCrumbling();

            if (failLayer >= 0)
            {
                Console.WriteLine($"Tower is crumbling on floor {failLayer}");
            }

            if (!tower.IsNotFalling())
            {
                Console.WriteLine($"Tower is falling");
            }

            tower.UpdatePoints();

            return tower;
        }

        private void CalculateCommonChars()
        {
            var letters = Words
                .SelectMany(w => w.ToCharArray())
                .GroupBy(g => g)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Count()
                    );

            CommonChars = letters
                .OrderByDescending(kv => kv.Value)
                .Take(15)
                .Select(kv => kv.Key)
                .ToHashSet();
        }

        private TowerProject DropRandomBottomFloor(TowerProject currentTowerProject)
        {
            if (currentTowerProject.Floors.Count == 0)
            {
                return new TowerProject();
            }

            var towerProject = new TowerProject()
            {
                Floors = currentTowerProject.Floors.Take(Random.Next(1, currentTowerProject.Floors.Count - 1)).ToList(),
            };

            towerProject.UsedWords = towerProject.Floors
                .SelectMany(f => f.UsedWords)
                .ToHashSet();

            return towerProject;
        }

        /// <summary>
        /// Возвращает true и добавляет этаж на башню, если получилось его достроить
        /// иначе false
        /// </summary>
        /// <param name="towerProject"></param>
        private bool TryMakeNextFloor(TowerProject towerProject)
        {
            //Console.WriteLine($"Adding {towerProject.Floors.Count} floor to tower");

            /// Если первый этаж, то просто ищем слово по-длиннее и ставим его как первый этаж
            if (towerProject.Floors.Count == 0)
            {
                var longestWord = FindRandomNotUsedLongWord(towerProject);

                var firstFloor = new TowerFloor()
                {
                    HorizontalWord = null,
                    //VerticalWords = new Dictionary<int, string>()
                    //{
                    //    { 0, longestWord },
                    //},
                };

                firstFloor.SetVerticalWords(new Dictionary<int, string>()
                    {
                        { 0, longestWord },
                    });

                towerProject.Floors.Add(firstFloor);
                towerProject.UsedWords.Add(longestWord);

                return true;
            }
            else
            {
                var prevFloor = towerProject.Floors[^1];

                int minHorizontalLength = prevFloor.VerticalWords.Count * 2 + 1;

                if (!FindRandomNotUsedHorizontalWord(towerProject,
                    minHorizontalLength, prevFloor.VerticalWords,
                    out var horizontalWord, out var shift))
                {
                    return false;
                }

                var nextFloor = new TowerFloor()
                {
                    HorizontalWord = horizontalWord,
                    HorizontalWordShift = -shift,
                    //VerticalWords = new Dictionary<int, string>()
                    //{
                    //    //{ 0, longestWord },
                    //}
                };

                //nextFloor.SetVerticalWords()

                towerProject.UsedWords.Add(horizontalWord);
                towerProject.Floors.Add(nextFloor);

                if (!CalculateVerticalWordCount(towerProject, horizontalWord,
                    out int verticalWordCount, out int verticalWordMaxHeight))
                {
                    return false;
                }

                if (!TryFindVerticalWords(towerProject, horizontalWord, shift, verticalWordCount, verticalWordMaxHeight,
                    out var verticalWords))
                {
                    return false;
                }

                //nextFloor.VerticalWords = verticalWords;
                nextFloor.SetVerticalWords(verticalWords);

                foreach (var (_, word) in verticalWords)
                {
                    towerProject.UsedWords.Add(word);
                }

                return true;
            }
        }

        private bool TryFindVerticalWords(TowerProject towerProject, string horizontalWord, int horizontalShift,
            int verticalWordCount,
            int verticalWordMaxHeight, out Dictionary<int, string> verticalWords)
        {
            verticalWords = new();

            for (int verticalLength = verticalWordMaxHeight; verticalLength > 2; verticalLength--)
            {
                bool isFountVerticalWords = TryFindVerticalWordsWithLength(
                    towerProject,
                    horizontalWord, horizontalShift,
                    verticalWordCount, verticalLength, out
                    Dictionary<int, string> curVerticalWords
                    );

                if (!isFountVerticalWords)
                {
                    continue;
                }

                verticalWords = curVerticalWords;

                break;
            }

            return verticalWords.Count > 0;
        }

        private bool TryFindVerticalWordsWithLength(TowerProject towerProject, string horizontalWord, int horizontalShift,
            int verticalWordCount, 
            int verticalWordMaxHeight, out Dictionary<int, string> verticalWords)
        {
            HashSet<string> currentSelected = new();

            Dictionary<int, char> firstLetters = new();

            verticalWords = new();

            for (int i = 0; i < verticalWordCount; i++)
            {
                var x = horizontalShift - 1 + 2 * i;

                firstLetters.Add(x, horizontalWord[x]);

                var candidates = Words
                    .Where(w => !currentSelected.Contains(w)
                        && !towerProject.UsedWords.Contains(w)
                        && w[0] == horizontalWord[x]
                        && w.Length == verticalWordMaxHeight
                        && CommonChars.Contains(w[^1])
                    )
                    .ToList();

                if (candidates.Count == 0)
                {
                    return false;
                }

                var selectedWord = candidates[Random.Next(candidates.Count)];

                verticalWords.Add(x, selectedWord);

                currentSelected.Add(selectedWord);
            }

            return true;
        }

        private static bool CalculateVerticalWordCount(
            TowerProject towerProject, string horizontalWord, out int wordCount, out int maxHeight)
        {
            int massOfTower = towerProject.GetTotalMass();

            int maxVerticalWords = (int)Math.Ceiling(horizontalWord.Length / 2.0d);

            Dictionary<int, int> verticalWordsToMaxVerticalLength = new();

            for (int i = 1; i <= maxVerticalWords; i++)
            {
                var maxLengthOfVerticalWord = (int)Math.Floor(52 - (double)massOfTower / i);

                if (maxLengthOfVerticalWord < 3)
                {
                    continue;
                }

                verticalWordsToMaxVerticalLength.Add(i, maxLengthOfVerticalWord);
            }

            if (verticalWordsToMaxVerticalLength.Count == 0)
            {
                wordCount = 0;
                maxHeight = 0;
                return false;
            }

            int minCount = verticalWordsToMaxVerticalLength.Min(kv => kv.Key);

            var item = verticalWordsToMaxVerticalLength
                .Where(kv => kv.Key == minCount)
                .Select(kv => (kv.Key, kv.Value))
                .First();

            wordCount = item.Key;
            maxHeight = item.Value;

            return true;
        }

        private bool FindRandomNotUsedHorizontalWord(TowerProject towerProject, 
            int minHorizontalLength, Dictionary<int, string> verticalWords,
            out string? selectedWord, out int outShift)
        {
            var rawCandidates = Words
                .Where(w => !towerProject.UsedWords.Contains(w)
                    && w.Length >= minHorizontalLength
                )
                //.OrderBy(w => w.Length)
                .ToList();

            var candidates = new List<(string Word, int Shift)>();

            int minX = verticalWords.Min(kv => kv.Key);
            int maxX = verticalWords.Max(kv => kv.Key);

            Dictionary<int, char> lastCharacters = verticalWords
                .ToDictionary(
                    kv => kv.Key - minX,
                    kv => kv.Value[^1]
                    );

            foreach (var candidate in rawCandidates)
            {
                for (int shift = 1; shift < candidate.Length - maxX - 1; shift++)
                {
                    bool isWordMatch = true;

                    foreach (var (position, letter) in lastCharacters)
                    {
                        if (candidate[position + shift] != letter)
                        {
                            isWordMatch = false;
                            break;
                        }
                    }

                    if (isWordMatch)
                    {
                        candidates.Add((candidate, shift));
                    }
                }
            }

            if (candidates.Count == 0)
            {
                selectedWord = null;
                outShift = 0;
                return false;
            }

            var minLength = candidates.Min(e => e.Word.Length);

            candidates = candidates
                .Where(e => e.Word.Length <= minLength + 1)
                .ToList();

            int index = Random.Next(candidates.Count);

            selectedWord = candidates[index].Word;
            outShift = candidates[index].Shift;

            return true;
        }

        private string FindRandomNotUsedLongWord(TowerProject towerProject)
        {
            var candidates = Words
                .Where(w => w.Length >= 24
                    && !towerProject.UsedWords.Contains(w)
                )
                .ToList();

            int index = Random.Next(candidates.Count);

            return candidates[index];
        }
    }
}
