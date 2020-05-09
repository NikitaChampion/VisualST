using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Widget;

namespace VisualST
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class IntroActivity : AppCompatActivity
    {
        private ViewPager mSlideViewPager;
        private LinearLayout mDotLayout;

        private TextView[] mDots;

        private SliderAdapter sliderAdapter;

        private Button mNextBtn;
        private Button mBackBtn;

        private int mCurrentPage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_intro);

            mSlideViewPager = FindViewById<ViewPager>(Resource.Id.slideViewPager);
            mDotLayout = FindViewById<LinearLayout>(Resource.Id.dotsLayout);

            mDots = new TextView[8];
            mDots[0] = FindViewById<TextView>(Resource.Id.mDots0);
            mDots[1] = FindViewById<TextView>(Resource.Id.mDots1);
            mDots[2] = FindViewById<TextView>(Resource.Id.mDots2);
            mDots[3] = FindViewById<TextView>(Resource.Id.mDots3);
            mDots[4] = FindViewById<TextView>(Resource.Id.mDots4);
            mDots[5] = FindViewById<TextView>(Resource.Id.mDots5);
            mDots[6] = FindViewById<TextView>(Resource.Id.mDots6);
            mDots[7] = FindViewById<TextView>(Resource.Id.mDots7);

            sliderAdapter = new SliderAdapter(this);

            mSlideViewPager.Adapter = sliderAdapter;

            mSlideViewPager.PageScrolled += Changed;

            mNextBtn = FindViewById<Button>(Resource.Id.nextBtn);
            mBackBtn = FindViewById<Button>(Resource.Id.prevBtn);

            mNextBtn.Click += (s, e) =>
            {
                if (mCurrentPage == mDots.Length - 1)
                    StartActivity(typeof(MonoidActivity));
                else
                    mSlideViewPager.SetCurrentItem(mCurrentPage + 1, true);
            };

            mBackBtn.Click += (s, e) => mSlideViewPager.SetCurrentItem(mCurrentPage - 1, true);
        }


        private void ChangeDots(int id)
        {
            for (int i = 0; i < mDots.Length; ++i)
            {
                if (i == id)
                    mDots[i].SetBackgroundResource(Resource.Drawable.dark_circle);
                else
                    mDots[i].SetBackgroundResource(Resource.Drawable.circle);
            }
        }

        private void Changed(object sender, ViewPager.PageScrolledEventArgs e)
        {
            ChangeDots(e.Position);
            mCurrentPage = e.Position;
            if (e.Position == 0)
            {
                mNextBtn.Enabled = true;
                mBackBtn.Enabled = false;

                mNextBtn.Text = "Next";
            }
            else if (e.Position == mDots.Length - 1)
            {
                mNextBtn.Enabled = true;
                mBackBtn.Enabled = true;

                mNextBtn.Text = "Done";
            }
            else
            {
                mNextBtn.Enabled = true;
                mBackBtn.Enabled = true;

                mNextBtn.Text = "Next";
            }
        }
    }
}
