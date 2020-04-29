﻿using System;
using System.Timers;
using Android.Widget;

namespace VisualST
{
    public class ST
    {
        private int n;

        public int N { get; private set; }

        private int[] numbers;

        private readonly TextView[] txt_num;

        private readonly Monoid monoid;

        public event Action<string> MakeText;

        private Timer timer;

        private int timer_counter;

        private int curSpeed;
        private int CurSpeed
        {
            get => curSpeed;
            set
            {
                curSpeed = value;
                if (timer != null)
                    timer.Interval = curSpeed;
            }
        }

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

        public ST(int n, TextView[] txt_num, Monoid monoid, int curSpeed)
        {
            this.n = n;
            this.txt_num = txt_num;
            this.monoid = monoid;
            CurSpeed = curSpeed;

            N = Next(n);
            numbers = new int[2 * N];

            timer_counter = 0;
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

        private void UpdateColor()
        {
            for (int i = 1; i < txt_num.Length; ++i)
                txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_gray);
        }

        public void Clear()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            timer_counter = 0;
            for (int i = 1; i < txt_num.Length; ++i)
            {
                txt_num[i].Text = "";
                txt_num[i].SetBackgroundResource(Resource.Drawable.rectangle_gray);
            }
        }

        private void SetTriple(int id, int resid)
        {
            txt_num[id].SetBackgroundResource(resid);
            txt_num[2 * id].SetBackgroundResource(resid);
            txt_num[2 * id + 1].SetBackgroundResource(resid);
        }

        private void Builder(ArrayT arrayT)
        {
            int pos = -1;
            if (pos++ == timer_counter)
            {
                ++timer_counter;
                UpdateColor();
                arrayT.UpdateColor();
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
                    UpdateColor();
                    arrayT.UpdateColor();

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
                    UpdateColor();
                    arrayT.UpdateColor();

                    SetTriple(i, Resource.Drawable.rectangle_purple);
                    return;
                }
            }
            if (pos == timer_counter)
            {
                UpdateColor();
                arrayT.UpdateColor();
            }
        }

        public void Build(ArrayT arrayT)
        {
            Clear();
            arrayT.UpdateColor();
            if (arrayT.array == null)
            {
                MakeText("Generate an array");
                return;
            }
            timer = new Timer(CurSpeed);
            timer.Elapsed += (s, e) => Builder(arrayT);
            timer.Start();
        }

        public void Previous()
        {
            if (timer == null)
                return;

            timer.Stop();
            if (timer_counter == 0)
                return;

            timer.AutoReset = false;
            timer_counter -= 2;
            timer.Start();
        }

        public void Stop()
        {
            if (timer != null)
                timer.Stop();
        }

        public void Continue()
        {
            if (timer != null) // сделать счетчик, обозначающий билд... (опционально)
            {
                timer.Stop();
                timer.AutoReset = true;
                timer.Start();
            }
        }

        public void Next()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.AutoReset = false;
                timer.Start();
            }
        }

        public int? GetAns(int l, int r)
        {
            if (l > r)
            {
                MakeText("left > right!");
                return null;
            }
            int ans = monoid.neutral;
            l += N;
            r += N;
            while (l <= r)
            {
                if (l % 2 == 1)
                    ans = monoid.GetCayley(ans, numbers[l]);
                if (r % 2 == 0)
                    ans = monoid.GetCayley(ans, numbers[r]);
                l = (l + 1) / 2;
                r = (r - 1) / 2;
            }
            return ans;
        }

        public void Update(int i, int x, Func<int, int, int> func)
        {
            i += N;
            numbers[i] = func(numbers[i], x);
            while ((i /= 2) != 0)
            {
                numbers[i] = monoid.GetCayley(numbers[2 * i], numbers[2 * i + 1]);
            }
        }
    }
}
