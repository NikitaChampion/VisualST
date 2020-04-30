using System;
using Android.Widget;

namespace VisualST
{
    public class ArrayT
    {
        public int Length { get; private set; }

        public int[] array;

        public readonly TextView[] arr;

        private readonly Monoid monoid;

        public event Action<string> MakeText;

        public ArrayT(int length, TextView[] arr, Monoid monoid)
        {
            Length = length;
            this.arr = arr;
            this.monoid = monoid;

            Clear();
        }

        public void Generate()
        {
            UpdateColor();
            if (monoid.Count == 0)
            {
                MakeText("Monoid is empty!");
                return;
            }
            if (!monoid.neutralTest)
            {
                MakeText("Check for neutral element!");
                return;
            }
            if (!monoid.associativityTest)
            {
                MakeText("Check for associativity!");
                return;
            }
            array = new int[Length];
            for (int i = 0; i < Length; ++i)
            {
                array[i] = monoid.GetRandom();
                arr[i].Text = array[i].ToString();
            }
        }

        public void UpdateN(int length)
        {
            Length = length;
            Clear();

            for (int i = 0; i < arr.Length; ++i)
            {
                if (i < length)
                    arr[i].Visibility = Android.Views.ViewStates.Visible;
                else
                    arr[i].Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        public void UpdateColor()
        {
            for (int i = 0; i < arr.Length; ++i)
                arr[i].SetBackgroundResource(Resource.Drawable.rectangle_white);
        }

        public void Clear()
        {
            array = null;
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i].Text = "";
                arr[i].SetBackgroundResource(Resource.Drawable.rectangle_white);
            }
        }
    }
}
