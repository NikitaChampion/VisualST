using Android.Widget;

namespace VisualST
{
    public class ArrayT
    {
        public int Length { get; private set; }

        public int[] array;

        public readonly TextView[] arr;

        private readonly Monoid monoid;

        public ArrayT(int length, TextView[] arr, Monoid monoid)
        {
            Length = length;
            this.arr = arr;
            this.monoid = monoid;

            array = null;
        }

        public void Generate()
        {
            UpdateColor();
            array = new int[Length];
            for (int i = 0; i < Length; ++i)
            {
                array[i] = monoid.GetRandom();
                arr[i].Text = array[i].ToString();
            }
        }

        public void UpdateN(int length)
        {
            Clear();
            Length = length;

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

        public void UpdateSize(int N, int pxx)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                if (i == N)
                    pxx = 0; // для всех TextView после N-ой ширина = 0
                LinearLayout.LayoutParams paramsss = new LinearLayout.LayoutParams(pxx, arr[i].Height);
                arr[i].LayoutParameters = paramsss;
            }
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
