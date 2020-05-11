using System;
using System.Collections.Generic;
using Android.Widget;

namespace VisualST
{
    /// <summary>
    /// Моноид
    /// </summary>
    [Serializable]
    public class Monoid
    {
        public static readonly Random rnd = new Random();

        // Отображение текста
        public event Action<string> MakeText;

        // Функция
        public event Func<int, int, int> Fun;

        // Элементы
        public List<int> groupoid;

        // Проверки на нейтральный / ассоциативность
        public bool neutralTest, associativityTest;

        // Нейтральный элемент
        public int neutral;

        // Таблица Кэли
        public int[,] Cayley;

        // Хранит длину группоида (моноида)
        private readonly TextView number;

        public Monoid(TextView number)
        {
            Cayley = new int[0, 0];
            groupoid = new List<int>();
            neutralTest = associativityTest = false;

            this.number = number;
        }

        public void Clear()
        {
            Cayley = new int[0, 0];
            groupoid.Clear();
            neutralTest = associativityTest = false;

            UpdateSize();
        }

        public void UpdateSize() =>
            number.Text = Count.ToString();

        public int Count { get => groupoid.Count; }

        public int GetRandom() => groupoid[rnd.Next(groupoid.Count)];

        public int GetCayley(int x, int y) => Cayley[x, y];

        public bool Contains(int x) => groupoid.Contains(x);

        /// <summary>
        /// Генерация группоида
        /// </summary>
        /// <param name="generating_set"> Порождающее множество </param>
        /// <param name="p"> Модуль операции </param>
        public void Generate(int[] generating_set, int p)
        {
            groupoid = new List<int>();

            List<Tuple<int, int, int>> ToCayley = new List<Tuple<int, int, int>>();

            bool[] visited = new bool[p];

            for (int i = 0; i < generating_set.Length; ++i)
            {
                if (generating_set[i] >= p) // generating_set[i] < 0 проверяется сразу же
                {
                    MakeText("Элемент порождающего множества некорректен");

                    Clear();
                    return;
                }
                visited[generating_set[i]] = true;
                groupoid.Add(generating_set[i]);
            }

            for (int i = 0; i < groupoid.Count; ++i)
            {
                for (int j = 0; j <= i; ++j)
                {
                    try
                    {
                        int cur_value = Fun(groupoid[i], groupoid[j]);
                        ToCayley.Add(new Tuple<int, int, int>(groupoid[i], groupoid[j], cur_value));
                        if (!visited[cur_value])
                        {
                            visited[cur_value] = true;
                            groupoid.Add(cur_value);
                        }

                        if (j == i)
                            continue;

                        cur_value = Fun(groupoid[j], groupoid[i]);
                        ToCayley.Add(new Tuple<int, int, int>(groupoid[j], groupoid[i], cur_value));
                        if (!visited[cur_value])
                        {
                            visited[cur_value] = true;
                            groupoid.Add(cur_value);
                        }
                        continue;
                    }
                    catch (DivideByZeroException ex)
                    {
                        MakeText($"Divide by zero:{Environment.NewLine}{ex.Message}");
                    }
                    catch (NullReferenceException ex)
                    {
                        MakeText($"Error in function:{Environment.NewLine}{ex.Message}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        MakeText($"Error in function (invalid operation):{Environment.NewLine}{ex.Message}");
                    }
                    catch (ArgumentException ex)
                    {
                        MakeText($"Error in function (invalid argument):{Environment.NewLine}{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MakeText($"Something strange occurred:{Environment.NewLine}{ex.Message}");
                    }
                    Clear();
                    return;
                }
            }

            Cayley = new int[p, p];
            foreach (Tuple<int, int, int> tuple in ToCayley)
            {
                Cayley[tuple.Item1, tuple.Item2] = tuple.Item3;
            }
            UpdateSize();
            /* суммарно:
             * время:  O(n^2 * m + p^2)
             * память: O(p^2)
             */

            /* 2 способ:
             * время:  O(n^2 * log(n) * m + ...) (SortedSet)
             */

            /* 3 способ:
             * время:  O(n^2 * m + ...) (амортизованно (HashSet))
             */
        }

        /// <summary>
        /// Проверка на ассоциативность
        /// </summary>
        public void AssociativityCheck()
        {
            for (int i = 0; i < groupoid.Count; ++i)
            {
                for (int j = 0; j < groupoid.Count; ++j)
                {
                    for (int k = 0; k < groupoid.Count; ++k)
                    {
                        if (Cayley[Cayley[groupoid[i], groupoid[j]], groupoid[k]] != Cayley[groupoid[i], Cayley[groupoid[j], groupoid[k]]])
                        {
                            MakeText($"Groupoid is not associative:{Environment.NewLine}(({groupoid[i]}, {groupoid[j]}), {groupoid[k]}) ≠ ({groupoid[i]}, ({groupoid[j]}, {groupoid[k]}))");

                            Clear();
                            return;
                        }
                    }
                }
            }
            MakeText("Groupoid is associative");
            associativityTest = true;
        }

        /// <summary>
        /// Проверка на наличие нейтрального элемента
        /// </summary>
        public void NeutralCheck()
        {
            for (int i = 0; i < groupoid.Count; ++i)
            {
                bool isNeutral = true;
                for (int j = 0; j < groupoid.Count; ++j)
                {
                    isNeutral &= Cayley[groupoid[i], groupoid[j]] == groupoid[j];

                    isNeutral &= Cayley[groupoid[j], groupoid[i]] == groupoid[j];
                }
                if (isNeutral)
                {
                    neutral = groupoid[i];
                    MakeText($"Neutral element:{Environment.NewLine}{neutral}");
                    neutralTest = true;

                    return;
                }
            }
            MakeText("There is no neutral element in groupoid!");

            Clear();
        }
    }
}
