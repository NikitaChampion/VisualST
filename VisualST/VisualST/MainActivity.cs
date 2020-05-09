using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace VisualST
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FindViewById<Button>(Resource.Id.next).Click += (s, e) => StartActivity(typeof(IntroActivity));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_about:
                    StartActivity(typeof(AboutActivity));
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}
