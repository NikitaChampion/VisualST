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
            
            if (mCurrentPage == 0)
            {
                mNextBtn.Enabled = true;
                mBackBtn.Enabled = false;

                mNextBtn.Text = "Next";
            }
            else if (mCurrentPage == mDots.Length - 1)
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

            TextView edited = FindViewById<TextView>(Resource.Id.edited);

            switch (mCurrentPage)
            {
                case 0:
                    edited.Text = "Welcome! This is an instruction on how to use the application. Press the next button below or just flip the page to continue.";
                    break;
                case 1:
                    edited.Text = "This button generates groupoid using parameteres of groupoid below. Each parameter is described in more detail later.";
                    break;
                case 2:
                    edited.Text = "Module is an operation module. The default value is 10.";
                    break;
                case 3:
                    edited.Text = "Operation is a function. The default value is X+Y. First parameter - X, second - Y";
                    break;
                case 4:
                    edited.Text = "Generating set is set, which generates groupoid. The default value is 1. Values can be written through ', '.\nFor example, '1, 2'.";
                    break;
                case 5:
                    edited.Text = "For a groupoid to be a monoid, it needs to be associative and have a neutral element in it. Press the associativity button to check groupoid for associativity.";
                    break;
                case 6:
                    edited.Text = "For a groupoid to be a monoid, it needs to be associative and have a neutral element in it. Press the neutral button to check groupoid for neutral.";
                    break;
                case 7:
                    edited.Text = "Press the next button when you have generated the groupoid and checked it for all properties.";
                    break;
            }
        }
    }
}
