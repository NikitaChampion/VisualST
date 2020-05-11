using System;
using System.Timers;
using Android.Widget;

namespace VisualST
{
    /// <summary>
    /// Дерево отрезков
    /// </summary>
    public class ST
    {
        // Отображение текста
        public event Action<string> MakeText;

        // Длина массива
        private int n;

        // Длина массива ДО/2
        public int N { get; private set; }

        // Массив ДО
        private int[] numbers;

        // Сохранённый массив
        private int[] saved;

        private readonly TextView[] txt_num;

        // Хранит значение нейтрального элемента (либо функции ST)
        private readonly TextView ans_;

        // Моноид (функция ДО)
        private readonly Monoid monoid;

        // Таймер для отображения действий
        private Timer timer;

        // Метод timer-а (для кнопок Previous / Next)
        private Action TimerAction;

        // Текущее действие таймера
        private int timer_counter;

        // Очищен ли TextView[]
        private bool cleared;

        // Ответ на отрезке
        private int answer;

        // Нужно ли присваивать текущему массиву сохранённый
        private bool save;

        private int curSpeed;
        private int CurSpeed // скорость анимации
        {
            get => curSpeed;
            set
            {
                curSpeed = value;
                if (timer != null)
                    timer.Interval = curSpeed;
            }
        }

        /// <summary>
        /// Следующая степень двойки, O(log(log(x)))
        /// </summary>
        /// <param name="x"> Число </param>
        /// <returns> Следующая степень двойки x </returns>
        private int Next(int x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        public ST(int n, TextView[] txt_num, Monoid monoid, int curSpeed, TextView answer)
        {
            this.n = n;
            this.txt_num = txt_num;
            this.monoid = monoid;
            CurSpeed = curSpeed;
            ans_ = answer;

            N = Next(n);
            numbers = new int[2 * N];

            timer_counter = 0;
            cleared = true;
            save = false;
            saved = null;
        }

        public void UpdateN(int n)
        {
            Clear();

            this.n = n;
            N = Next(n);
            numbers = new int[2 * N];

            save = false;
            saved = null;

            for (int i = 1; i < txt_num.Length; ++i)
            {
                if (i < 2 * N)
                    txt_num[i].Visibility = Android.Views.ViewStates.Visible;
                else
                    txt_num[i].Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        public void UpdateInterval(int curSpeed) =>
            CurSpeed = curSpeed;

        private void UpdateTextView()
        {
            for (int i = 1; i < txt_num.Length; ++i)
            {
                txt_num[i].Text = "";
                txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_gray);
            }
        }

        private void UpdateColor()
        {
            for (int i = 1; i < txt_num.Length; ++i)
                txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_gray);
        }

        private void ClearTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            timer_counter = 0;
        }

        public void ClearAnswer()
        {
            ans_.Text = monoid.neutral.ToString();
            ans_.SetBackgroundResource(Resource.Drawable.rectangle_white);
        }

        public void Clear()
        {
            cleared = true;
            ClearTimer();
            UpdateTextView();
        }

        private void SetTriple(int id, int resid)
        {
            txt_num[id].SetBackgroundResource(resid);
            txt_num[2 * id].SetBackgroundResource(resid);
            txt_num[2 * id + 1].SetBackgroundResource(resid);
        }

        public void ShowSavedState()
        {
            for (int i = 1; i < numbers.Length; ++i)
            {
                numbers[i] = saved[i]; // numbers = saved
                txt_num[i].Text = numbers[i].ToString();
            }
        }

        private void Builder(ArrayT arrayT)
        {
            cleared = true;
            UpdateTextView();
            arrayT.UpdateColor();
            ClearAnswer();

            if (timer_counter < -1)
                timer_counter = -1;

            int pos = -1;
            if (pos++ == timer_counter)
            {
                ++timer_counter;
                return;
            }
            for (int i = 0; i < N; ++i)
            {
                if (i < n)
                    numbers[N + i] = arrayT.array[i];
                else
                    numbers[N + i] = monoid.neutral;

                txt_num[N + i].Text = numbers[N + i].ToString();

                if (pos++ == timer_counter)
                {
                    ++timer_counter;

                    if (i < n)
                    {
                        txt_num[N + i].SetBackgroundResource(Resource.Drawable.rectangle_purple);
                        arrayT.arr[i].SetBackgroundResource(Resource.Drawable.rectangle_purple);
                    }
                    else
                    {
                        txt_num[N + i].SetBackgroundResource(Resource.Drawable.rectangle_search_1);
                        ans_.SetBackgroundResource(Resource.Drawable.rectangle_search_1);
                    }
                    return;
                }
            }
            for (int i = N - 1; i > 0; --i)
            {
                numbers[i] = monoid.GetCayley(numbers[2 * i], numbers[2 * i + 1]);
                txt_num[i].Text = numbers[i].ToString();

                if (pos++ == timer_counter)
                {
                    ++timer_counter;

                    SetTriple(i, Resource.Drawable.rectangle_purple);
                    return;
                }
            }
            ++timer_counter;
            cleared = false;
            timer.Stop();
        }

        private void GetAnswer(int l, int r)
        {
            UpdateColor();
            ClearAnswer();
            answer = monoid.neutral;

            if (timer_counter < -1)
                timer_counter = -1;

            int pos = -1;
            if (pos++ == timer_counter)
            {
                ++timer_counter;
                return;
            }

            l += N;
            r += N;
            while (l <= r)
            {
                if (l % 2 == 1)
                {
                    answer = monoid.GetCayley(answer, numbers[l]);
                    ans_.Text = answer.ToString();

                    ans_.SetBackgroundResource(Resource.Drawable.rectangle_purple);
                    txt_num[l].SetBackgroundResource(Resource.Drawable.rectangle_red);
                }
                else
                    txt_num[l].SetBackgroundResource(Resource.Drawable.rectangle_purple);

                if (r % 2 == 0)
                {
                    answer = monoid.GetCayley(answer, numbers[r]);
                    ans_.Text = answer.ToString();

                    ans_.SetBackgroundResource(Resource.Drawable.rectangle_purple);
                    txt_num[r].SetBackgroundResource(Resource.Drawable.rectangle_red);
                }
                else
                    txt_num[r].SetBackgroundResource(Resource.Drawable.rectangle_purple);

                if (pos++ == timer_counter)
                {
                    ++timer_counter;
                    return;
                }
                l = (l + 1) / 2;
                r = (r - 1) / 2;
            }
            ++timer_counter;
            timer.Stop();
        }

        public void Updater(int i, int x)
        {
            save = true;

            ShowSavedState();
            UpdateColor();

            if (timer_counter < -1)
                timer_counter = -1;

            int pos = -1;
            if (pos++ == timer_counter)
            {
                ++timer_counter;
                return;
            }

            i += N;
            numbers[i] = x;
            txt_num[i].Text = numbers[i].ToString();
            txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_purple);
            if (pos++ == timer_counter)
            {
                ++timer_counter;
                return;
            }
            while ((i /= 2) != 0)
            {
                numbers[i] = monoid.GetCayley(numbers[2 * i], numbers[2 * i + 1]);
                txt_num[i].Text = numbers[i].ToString();
                txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_purple);
                if (pos++ == timer_counter)
                {
                    ++timer_counter;
                    return;
                }
            }
            ++timer_counter;
            save = false;
            timer.Stop();
        }

        public void Previous()
        {
            if (timer != null)
            {
                timer.Stop();
                timer_counter -= 2;

                TimerAction();
            }
        }

        public void Stop()
        {
            if (timer != null)
                timer.Stop();
        }

        public void Continue()
        {
            if (timer != null)
                timer.Start();
        }

        public void Next()
        {
            if (timer != null)
            {
                timer.Stop();

                TimerAction();
            }
        }

        public void Build(ArrayT arrayT)
        {
            if (arrayT.array == null)
            {
                MakeText("Generate an array");
                return;
            }

            Clear();
            arrayT.UpdateColor();
            ClearAnswer();

            save = false;

            timer = new Timer(CurSpeed);
            TimerAction = () => Builder(arrayT);
            timer.Elapsed += (s, e) => Builder(arrayT);
            timer.Start();
        }

        public void GetAns(int l, int r)
        {
            if (l > r)
            {
                MakeText("left > right");
                return;
            }
            if (cleared)
            {
                MakeText("Firstly build Segment Tree");
                return;
            }

            ClearTimer();
            UpdateColor();
            ClearAnswer();

            if (save)
            {
                ShowSavedState();
                save = false;
            }

            timer = new Timer(CurSpeed);
            TimerAction = () => GetAnswer(l, r);
            timer.Elapsed += (s, e) => GetAnswer(l, r);
            timer.Start();
        }

        public void Update(int i, int x) /* Func<int, int, int> func можно добавить ещё (на будущее) */
        {
            if (cleared)
            {
                MakeText("Firstly build Segment Tree");
                return;
            }
            if (!monoid.Contains(x))
            {
                MakeText($"{x} is not in monoid!");
                return;
            }

            ClearTimer();
            UpdateColor();
            ClearAnswer();

            if (!save)
            {
                saved = new int[numbers.Length];

                for (int j = 1; j < numbers.Length; ++j)
                    saved[j] = numbers[j]; // saved = numbers

                save = true;
            }

            timer = new Timer(CurSpeed);
            TimerAction = () => Updater(i, x);
            timer.Elapsed += (s, e) => Updater(i, x);
            timer.Start();
        }
    }
}
