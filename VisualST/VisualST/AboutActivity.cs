using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace VisualST
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class AboutActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FindViewById<Button>(Resource.Id.license).Click += (s, e) => StartActivity(typeof(LicenseActivity));
        }
    }
}