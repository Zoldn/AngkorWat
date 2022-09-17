using AngkorWat.TowerBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
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

            CommonChars = Utils.CalculateCommonChars(Words);

            Random = new Random(42);
        }
        public void BuildTower()
        {
            var currentTowerProject = new TowerProject();

            //for (int i = 0; i < 10000; i++)
            //{
            //currentTowerProject = DropRandomBottomFloor(currentTowerProject);

            int restartCount = 0;

            while (restartCount < 1000)
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

                    //break;
                }
                else
                {
                    Console.WriteLine($"Current floors {currentTowerProject.Floors.Count}");
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
        }

        private bool TryMakeNextFloor(TowerProject towerProject)
        {
            /// Если первый этаж, то просто ищем слово по-длиннее и ставим его как первый этаж
            if (towerProject.Floors.Count == 0)
            {
                var longestWord = FindRandomNotUsedLongWord(towerProject);

                var firstFloor = new TowerFloor()
                {
                    IsFirst = true,
                };

                firstFloor.ColumnWords.Add(Planks[0], new VerticalWord(Planks[0], 1, longestWord));

                towerProject.Floors.Add(firstFloor);
                towerProject.UsedWords.Add(longestWord);

                return true;
            }
            else
            {
                return TryMakeNotFirstFloor(towerProject);
            }
        }

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
