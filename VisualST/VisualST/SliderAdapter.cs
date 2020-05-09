using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace VisualST
{
    public class SliderAdapter : PagerAdapter
    {
        private readonly Context context;
        private LayoutInflater inflater;

        public SliderAdapter(Context context)
        {
            this.context = context;
        }

        public override int Count => 8;

        public override bool IsViewFromObject(View view, Java.Lang.Object o)
        {
            return view == (RelativeLayout)o;
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            inflater = LayoutInflater.From(container.Context);
            View view = inflater.Inflate(Resource.Layout.activity_monoid, container, false);

            Button generate_groupoid = view.FindViewById<Button>(Resource.Id.generate_groupoid);
            generate_groupoid.Enabled = false;

            EditText module = view.FindViewById<EditText>(Resource.Id.module);
            module.Enabled = false;

            EditText operation = view.FindViewById<EditText>(Resource.Id.operation);
            operation.Enabled = false;

            EditText generating_set = view.FindViewById<EditText>(Resource.Id.generating_set);
            generating_set.Enabled = false;

            Button associative = view.FindViewById<Button>(Resource.Id.associative);
            associative.Enabled = false;

            Button neutral = view.FindViewById<Button>(Resource.Id.neutral);
            neutral.Enabled = false;

            Button next_ = view.FindViewById<Button>(Resource.Id.next_);
            next_.Enabled = false;

            switch (position)
            {
                case 1:
                    generate_groupoid.SetBackgroundResource(Resource.Drawable.button_settings_2);
                    break;
                case 2:
                    module.Text = "10";
                    module.SetTextColor(Android.Graphics.Color.Red);
                    break;
                case 3:
                    operation.Text = "X+Y";
                    operation.SetTextColor(Android.Graphics.Color.Red);
                    break;
                case 4:
                    generating_set.Text = "1";
                    generating_set.SetTextColor(Android.Graphics.Color.Red);
                    break;
                case 5:
                    associative.SetBackgroundResource(Resource.Drawable.button_settings_2);
                    break;
                case 6:
                    neutral.SetBackgroundResource(Resource.Drawable.button_settings_2);
                    break;
                case 7:
                    next_.SetBackgroundResource(Resource.Drawable.button_settings_2);
                    break;
            }
            container.AddView(view);

            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
        {
            container.RemoveView((RelativeLayout)obj);
        }
    }
}
