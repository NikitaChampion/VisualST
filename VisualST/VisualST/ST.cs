using System;
using System.Timers;
using Android.Widget;

namespace VisualST
{
    public class ST
    {
        private int n; // длина массива

        public int N { get; private set; } // следующая степень двойки n

        private int[] numbers; // массив ДО

        private readonly TextView[] txt_num; // TextView[]

        private readonly Monoid monoid; // моноид, функция ДО

        public event Action<string> MakeText; // отображение текста

        private Timer timer; // таймер для отображения действий

        private Action TimerAction; // метод timer-а (для кнопок Previous / Next)

        private int timer_counter; // текущее действие таймера

        private bool cleared; // очищен ли TextView[]

        private int answer; // ответ на отрезке

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

        private int Next(int x) // следующая степень двойки, O(log(log(x)))
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        public ST(int n, TextView[] txt_num, Monoid monoid, int curSpeed)
        {
            this.n = n;
            this.txt_num = txt_num;
            this.monoid = monoid;
            CurSpeed = curSpeed;

            N = Next(n);
            numbers = new int[2 * N];

            timer_counter = 0;
            cleared = true;
        }

        public void UpdateN(int n)
        {
            this.n = n;
            N = Next(n);
            numbers = new int[2 * N];

            for (int i = 1; i < txt_num.Length; ++i)
            {
                if (i < 2 * N)
                    txt_num[i].Visibility = Android.Views.ViewStates.Visible;
                else
                    txt_num[i].Visibility = Android.Views.ViewStates.Invisible;
            }

            Clear();
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

        private void Builder(ArrayT arrayT)
        {
            cleared = true;
            UpdateTextView();
            arrayT.UpdateColor();

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
                    txt_num[l].SetBackgroundResource(Resource.Drawable.rectangle_red);
                }
                else
                    txt_num[l].SetBackgroundResource(Resource.Drawable.rectangle_purple);

                if (r % 2 == 0)
                {
                    answer = monoid.GetCayley(answer, numbers[r]);
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
            i += N;
            numbers[i] = x;
            txt_num[i].Text = numbers[i].ToString();
            while ((i /= 2) != 0)
            {
                numbers[i] = monoid.GetCayley(numbers[2 * i], numbers[2 * i + 1]);
                txt_num[i].Text = numbers[i].ToString();
            }
        } // пока без понятия

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

            answer = monoid.neutral;
            timer = new Timer(CurSpeed);
            TimerAction = () => GetAnswer(l, r);
            timer.Elapsed += (s, e) => GetAnswer(l, r);
            timer.Start();
        }

        public void Update(int i, int x) // проверка на полное обновление элемента нужна еще, /*, Func<int, int, int> func можно добавить еще*/
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

            timer = new Timer(CurSpeed);
            TimerAction = () => Updater(i, x);
            timer.Elapsed += (s, e) => Updater(i, x);
            timer.Start();
        }
    }
}
