using AngkorWat.TowerBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Tower3DBuilder
    {
        public List<Plank> Planks { get; set; }
        public HashSet<string> Words { get; set; }
        public HashSet<char> CommonChars { get; set; }
        public Random Random { get; private set; }
        public static int PREFERRABLE_HEIGHT = 8;

        public Tower3DBuilder(List<string> words) 
        {
            Planks = Utils.Initialize(5, 5, 100);

            Words = words
                .Select(e => e.ToUpper())
                .ToHashSet();

            var t = Words.GroupBy(w => w.Length)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderBy(kv => kv.Key);

            CommonChars = Utils.CalculateCommonChars(Words);

            Random = new Random(42424242);
        }
        public Tower BuildTower()
        {
            var currentTowerProject = new TowerProject();

            //for (int i = 0; i < 10000; i++)
            //{
            //currentTowerProject = DropRandomBottomFloor(currentTowerProject);

            int restartCount = 0;

            while (restartCount < 100)
            {
                bool isFloorAdded = TryMakeNextFloor(currentTowerProject);

                if (!isFloorAdded)
                {
                    //if (currentTowerProject.GetTotalHeight > bestTowerProject.GetTotalHeight)
                    //{
                    //    bestTowerProject = currentTowerProject;
                    //}

                    Console.WriteLine($"Restart {restartCount}, height = {currentTowerProject.Height}");

                    restartCount++;

                    RemoveLastFloor(currentTowerProject);

                    //currentTowerProject.CheckMass();

                    //currentTowerProject.CheckDublicateWords();
                }
                else
                {
                    Console.WriteLine($"Current height {currentTowerProject.Height}");
                }
            }

            //Console.WriteLine($"Iteration: {i.ToString().PadLeft(4, ' ')} " +
            //    $"Best height: {bestTowerProject.GetTotalHeight.ToString().PadLeft(4, ' ')} " +
            //    $"Current height: {currentTowerProject.GetTotalHeight.ToString().PadLeft(4, ' ')} " +
            //    $"Current words: {currentTowerProject.UsedWords.Count.ToString().PadLeft(4, ' ')} " +
            //    $"Current mass: {currentTowerProject.GetTotalMass().ToString().PadLeft(4, ' ')} "
            //    );
            //}
            Console.WriteLine("");

            var tower = currentTowerProject.ToTower();

            int y = currentTowerProject.CheckOverlaps();

            int c = tower.IsNotCrumbling();

            return tower;
        }

        private void RemoveLastFloor(TowerProject currentTowerProject)
        {
            int lastZ = currentTowerProject.TowerWords.Min(t => t.Z0);

            currentTowerProject.TowerWords = currentTowerProject.TowerWords
                .Where(t => t.Z0 > lastZ)
                .ToList();

            foreach (var towerWord in currentTowerProject.TowerWords)
            {
                towerWord.Overlaps = towerWord.Overlaps
                    .Where(ov => ov.Z0 > lastZ)
                    .ToList();
            }
        }

        private bool TryMakeNextFloor(TowerProject towerProject)
        {
            /// Если первый этаж, то просто ищем слово по-длиннее и ставим его как первый этаж
            if (towerProject.TowerWords.Count == 0)
            {
                var longestWord = FindRandomNotUsedLongWord(towerProject);

                var firstFloor = new TowerWord(longestWord, WordDirection.Z);

                towerProject.TowerWords.Add(firstFloor);

                return true;
            }
            else
            {
                return TryMakeNotFirstFloor(towerProject);
            }
        }

        private string FindRandomNotUsedLongWord(TowerProject towerProject)
        {
            var usedWords = towerProject.UsedWords;

            var candidates = Words
                .Where(w => w.Length >= 24
                    && !usedWords.Contains(w)
                )
                .ToList();

            int index = Random.Next(candidates.Count);

            return candidates[index];
        }

        private bool TryMakeNotFirstFloor(TowerProject towerProject)
        {
            var prevUsedWords = towerProject.UsedWords;

            int currentZ = towerProject.MinZ;

            var prevFloorColumnsWords = towerProject
                .TowerWords
                .Where(t => t.MinZ == currentZ)
                .ToList();

            if (!TryAddHorizontalForColumnsFromPrevFloor(towerProject, prevFloorColumnsWords, 
                prevUsedWords, currentZ))
            {
                Console.WriteLine($"\tFailed on adding planks");
                return false;
            }

            //towerProject.CheckDublicateWords();
            //towerProject.CheckMass();

            int height = GetMaxHeightOfColumnsForCurrentMass(towerProject, currentZ);

            while (height <= 10)
            {
                /// TODO: Add extension in Y
                if (!TryAddOneOutWord(towerProject, currentZ))
                {
                    Console.WriteLine($"\tFailed on adding outwards");
                    return false;
                }

                height = GetMaxHeightOfColumnsForCurrentMass(towerProject, currentZ);

                //towerProject.CheckDublicateWords();
                //towerProject.CheckMass();
            }

            if (!TryAddVerticalWords(towerProject, currentZ, height))
            {
                Console.WriteLine($"\tFailed on adding columns");
                return false;
            }

            //towerProject.CheckDublicateWords();
            //towerProject.CheckMass();

            return true;
        }

        private bool TryAddOneOutWord(TowerProject towerProject, int currentZ)
        {
            var prevUsedWords = towerProject.UsedWords;

            var lastPlank = towerProject.TowerWords
                .Where(t => t.Direction == WordDirection.X
                    && t.Z0 == currentZ
                )
                .OrderByDescending(t => t.Y0)
                .First();

            var currUsedWords = new HashSet<string>();

            TowerWord outWord = null;

            //towerProject.CheckMass();

            for (int length = 3; length < 15; length++)
            {
                for (int xShift = 0; xShift < lastPlank.Word.Length; xShift++)
                {
                    if (lastPlank.Overlaps.Any(ov => ov.X0 - lastPlank.X0 == xShift))
                    {
                        continue;
                    }

                    var candidates = Words
                        .Where(w => w.Length == length
                            && w[0] == lastPlank.Word[xShift]
                            && CommonChars.Contains(w[^1])
                            && !prevUsedWords.Contains(w)
                            && !currUsedWords.Contains(w)
                        )
                        .ToList();

                    if (candidates.Count > 0)
                    {
                        var word = SelectRandom(candidates);

                        outWord = new TowerWord(word, WordDirection.Y, lastPlank.X0 + xShift,
                            lastPlank.Y0, currentZ);

                        outWord.Overlaps.Add(lastPlank);
                        lastPlank.Overlaps.Add(outWord);

                        towerProject.TowerWords.Add(outWord);

                        currUsedWords.Add(word);

                        //towerProject.CheckMass();

                        break;
                    }
                }

                if (outWord != null)
                {
                    break;
                }
            }

            if (outWord == null)
            {
                return false;
            }

            for (int length = 3; length < 15; length++)
            {
                for (int xShift = 0; xShift < length; xShift++)
                {
                    var candidates = Words
                        .Where(w => w.Length == length
                            && w[xShift] == outWord.Word[^1]
                            //&& CommonChars.Contains(w[^1])
                            && !prevUsedWords.Contains(w)
                            && !currUsedWords.Contains(w)
                        )
                        .ToList();

                    if (candidates.Count > 0)
                    {
                        var word = SelectRandom(candidates);
                        xShift = -xShift;

                        var newPlank = new TowerWord(word, WordDirection.X, outWord.X0 + xShift,
                            outWord.Y0 + outWord.Word.Length - 1, currentZ);

                        outWord.Overlaps.Add(newPlank);
                        newPlank.Overlaps.Add(outWord);

                        towerProject.TowerWords.Add(newPlank);

                        //towerProject.CheckMass();

                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryAddVerticalWords(TowerProject towerProject, int currentZ, int maxHeight)
        {
            var currFloorPlankWords = towerProject
                .TowerWords
                .Where(t => t.Z0 == currentZ
                    && t.Direction == WordDirection.X
                )
                .ToList();

            var prevUsedWords = towerProject.UsedWords;

            for (int height = maxHeight; height >= 3; height--)
            {
                var currUsedWords = new HashSet<string>();

                Dictionary<TowerWord, (string, int)> selections = new();

                foreach (var plankWord in currFloorPlankWords)
                {
                    for (int xShift = 0; xShift < plankWord.Word.Length; xShift++)
                    {
                        if (plankWord.Overlaps.Any(ov => ov.X0 - plankWord.X0 == xShift))
                        {
                            continue;
                        }

                        var candidates = Words
                            .Where(w => w.Length == height
                                && w[0] == plankWord.Word[xShift]
                                && CommonChars.Contains(w[^1])
                                && !prevUsedWords.Contains(w)
                                && !currUsedWords.Contains(w)
                            )
                            .ToList();

                        if (candidates.Count == 0)
                        {
                            continue;
                        }

                        var selectedWord = SelectRandom(candidates);

                        selections.Add(plankWord, (selectedWord, xShift));

                        currUsedWords.Add(selectedWord);

                        break;
                    }
                }

                /// Если нашли не для всех, то ищем слова покороче
                if (selections.Count < currFloorPlankWords.Count)
                {
                    continue;
                }

                foreach (var (plank, (word, xShift)) in selections)
                {
                    var newTowerWord = new TowerWord(word, WordDirection.Z, plank.X0 + xShift, plank.Y0, plank.Z0);

                    newTowerWord.Overlaps.Add(plank);
                    plank.Overlaps.Add(newTowerWord);

                    towerProject.TowerWords.Add(newTowerWord);
                }

                return true;
            }

            return false;
        }

        private int GetMaxHeightOfColumnsForCurrentMass(TowerProject towerProject, int currentZ)
        {
            var currentXWords = towerProject
                .TowerWords
                .Where(w => w.Direction == WordDirection.X
                    && w.Z0 == currentZ
                )
                .ToList();

            return (int)Math.Floor(52 - (double)towerProject.Mass / currentXWords.Count);
        }

        public bool TryAddHorizontalForColumnsFromPrevFloor(TowerProject towerProject,
            List<TowerWord> prevFloorColumnsWords,
            HashSet<string> prevUsedWords, int currentZ)
        {
            HashSet<string> currUsedWords = new HashSet<string>();

            foreach (var columnWord in prevFloorColumnsWords)
            {
                if (!TryFindShortXWord(towerProject, columnWord, out string word, out int xShift,
                    prevUsedWords, currUsedWords))
                {
                    return false;
                }

                var newWord = new TowerWord(word, WordDirection.X, columnWord.X0 + xShift,
                    columnWord.Y0, currentZ);

                columnWord.Overlaps.Add(newWord);
                newWord.Overlaps.Add(columnWord);

                towerProject.TowerWords.Add(newWord);
            }

            return true;
        }

        private bool TryFindShortXWord(TowerProject towerProject, TowerWord columnWord, out string word, out int xShift, 
            HashSet<string> prevUsedWords, HashSet<string> currUsedWords)
        {
            int startLength = 3;

            //if (towerProject.Height > 130)
            //{
            //    startLength++;
            //}

            //if (towerProject.Height > 160)
            //{
            //    startLength++;
            //}

            for (int length = startLength; length < 15; length++)
            {
                for (int shift = 0; shift < length; shift++)
                {
                    var candidates = Words
                        .Where(w => w.Length == length
                            && w[shift] == columnWord.Word[^1]
                            && !prevUsedWords.Contains(w)
                            && !currUsedWords.Contains(w)
                        )
                        .ToList();

                    if (candidates.Count > 0)
                    {
                        word = SelectRandom(candidates);
                        currUsedWords.Add(word);
                        xShift = -shift;
                        return true;
                    }
                }
            }

            word = "";
            xShift = 0;
            return false;
        }

        private string SelectRandom(List<string> strings)
        {
            return strings[Random.Next(strings.Count)];
        }

        /*
        private bool TryMakeNotFirstFloor(TowerProject towerProject)
        {
            int plankCount = CalculatePlankForMass(towerProject);

            var prevFloor = towerProject.Floors[^1];

            var currentFloorWords = new HashSet<string>();

            var currentFloor = new TowerFloor()
            {
                IsFirst = false,
            };

            /// Ищем горизонтальные планки
            for (int plankId = 0; plankId < plankCount; plankId++)
            {
                var currentPlank = Planks[plankId];

                var constraints = new Dictionary<int, char>();

                if (prevFloor.ColumnWords.TryGetValue(currentPlank, out var prevColumn))
                {
                    constraints.Add(prevColumn.Shift, prevColumn.Word[^1]);
                }

                /// Ищем только слово сверху
                if (plankId > 0)
                {
                    var prevPlank = Planks[plankId - 1];
                    var prevPlankWord = currentFloor.PlankWords[prevPlank];

                    constraints.Add(0, prevPlankWord.Word[prevPlank.Length - 1 - prevPlankWord.Shift]);
                }

                if (!TryFindPlankWordWithConstraints(constraints, towerProject, currentFloorWords, currentPlank,
                    out string selectedWord, out int selectedShift))
                {
                    return false;
                }

                currentFloorWords.Add(selectedWord);
                currentFloor.PlankWords.Add(currentPlank, new HorizontalWord(currentPlank, selectedShift, selectedWord));
            }

            /// Ищем максимальную высоту вертикальных планок

            int maxVerticalHeight = GetMaxVerticalHeight(towerProject, currentFloor);

            /// От самого высокого этажа к низкому
            /// 
            bool isFoundColums = false;

            for (int verticalHeight = maxVerticalHeight; verticalHeight >= 3; verticalHeight--)
            {
                bool isHeightOk = true;

                HashSet<string> currentColumnWords = new();
                List<(Plank, string, int)> currentVerticalWords = new();
                /// Ищем вертиркальные планки
                for (int plankId = 0; plankId < plankCount; plankId++)
                {
                    var currentPlank = Planks[plankId];
                    var currentPlankWord = currentFloor.PlankWords[currentPlank];

                    if (!TryFindVerticalWord(towerProject, prevFloor, currentPlankWord,
                        verticalHeight,
                        currentColumnWords, currentFloorWords,
                        out string verticalWord, out int vertivalShift))
                    {
                        isHeightOk = false;
                        break;
                    }
                    else
                    {
                        currentVerticalWords.Add((currentPlank, verticalWord, vertivalShift));
                        currentColumnWords.Add(verticalWord);
                    }
                }

                if (!isHeightOk)
                {
                    continue;
                }

                isFoundColums = true;

                foreach (var (plank, word, shift) in currentVerticalWords)
                {
                    currentFloor.ColumnWords.Add(
                        plank,
                        new VerticalWord(plank, shift, word)
                        );
                }

                break;
            }

            if (!isFoundColums)
            {
                return false;
            }

            towerProject.Floors.Add(currentFloor);
            foreach (var (_, p) in currentFloor.PlankWords)
            {
                if (!towerProject.UsedWords.Add(p.Word))
                {
                    throw new Exception($"{p.Word} already used in tower");
                }
            }

            foreach (var (_, p) in currentFloor.ColumnWords)
            {
                if (!towerProject.UsedWords.Add(p.Word))
                {
                    throw new Exception($"{p.Word} already used in tower");
                }
            }

            return true;
        }

        private bool TryFindVerticalWord(TowerProject towerProject, TowerFloor prevFloor, 
            HorizontalWord currentPlankWord, 
            int verticalHeight, 
            HashSet<string> currentColumnWords, HashSet<string> currentFloorWords, 
            out string verticalWord, out int vertivalShift)
        {
            for (int shift = 1; shift <= currentPlankWord.Plank.Length - 1; shift++)
            {
                /// Если сверху есть колонна с таким же шифтом, то скипаем, чтобы слова
                /// не слиплись по вертикали
                if (prevFloor.ColumnWords.TryGetValue(currentPlankWord.Plank, out var upperColumn)
                    && upperColumn.Shift == shift
                    )
                {
                    continue;
                }

                var candidates = Words
                    .Where(w => w.Length == verticalHeight
                        && currentPlankWord.Word[shift - currentPlankWord.Shift] == w[0]
                        && CommonChars.Contains(w[^1])
                        && !towerProject.UsedWords.Contains(w)
                        && !currentColumnWords.Contains(w)
                        && !currentFloorWords.Contains(w)
                    )
                    .ToList();

                if (candidates.Count > 0)
                {
                    verticalWord = candidates[Random.Next(candidates.Count)];
                    vertivalShift = shift;
                    return true;
                }
            }

            verticalWord = "";
            vertivalShift = 0;
            return false;
        }

        private int GetMaxVerticalHeight(TowerProject towerProject, TowerFloor currentFloor)
        {
            int towerMass = towerProject.GetTotalMass();

            towerMass += currentFloor.GetPlankMass();
            towerMass -= towerProject.Floors[^1].ColumnWords.Count;

            return 51 - (int)Math.Floor((double)towerMass / currentFloor.PlankWords.Count);
        }

        private bool TryFindPlankWordWithConstraints(Dictionary<int, char> constraints,
            TowerProject towerProject, HashSet<string> currentFloorUsedWords, Plank plank,
            out string outWord, out int outShift)
        {
            var candidates = Words
                .Where(w => !towerProject.UsedWords.Contains(w)
                    && !currentFloorUsedWords.Contains(w)
                    && w.Length >= plank.Length
                )
                .ToList();

            var cleanCandidates = new List<(string, int)>();

            for (int shift = 0; shift >= -2; shift--)
            {
                foreach (var candidate in candidates)
                {
                    if (candidate == "МИНТАЙ")
                    {
                        int y = 1;
                    }

                    if (shift + candidate.Length >= plank.Length + 2)
                    {
                        continue;
                    }

                    if (shift + candidate.Length < plank.Length)
                    {
                        continue;
                    }

                    bool isOk = constraints.All(
                        kv => (kv.Key - shift >= 0)
                            && (kv.Key - shift < candidate.Length)
                            && (candidate[kv.Key - shift] == kv.Value)
                        );

                    if (isOk)
                    {
                        cleanCandidates.Add((candidate, shift));
                    }
                }
            }

            if (cleanCandidates.Count == 0)
            {
                outWord = "";
                outShift = 0;
                return false;
            }

            (outWord, outShift) = cleanCandidates[Random.Next(cleanCandidates.Count)];

            if (outWord == "МИНТАЙ")
            {
                int y = 1;
            }

            return true;
        }

        private static int CalculatePlankForMass(
            TowerProject towerProject)
        {
            int massOfTower = towerProject.GetTotalMass();

            return (int)Math.Ceiling(massOfTower / (42.0d - PREFERRABLE_HEIGHT));
        }

        

        */
    }
}
