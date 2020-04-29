using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Z.Expressions;

namespace VisualST
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // Моноид
        Monoid monoid;

        // Массив
        ArrayT arrayT;

        // Дерево отрезков
        ST MyST;

        // Операция
        private string operation;

        // Модуль операции
        private int p;

        // Порождающее множество моноида
        private int[] generating_set;

        private int? left, right;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            TextView[] txt_num = new TextView[16];
            txt_num[1] = FindViewById<TextView>(Resource.Id.t1);
            txt_num[2] = FindViewById<TextView>(Resource.Id.t2);
            txt_num[3] = FindViewById<TextView>(Resource.Id.t3);
            txt_num[4] = FindViewById<TextView>(Resource.Id.t4);
            txt_num[5] = FindViewById<TextView>(Resource.Id.t5);
            txt_num[6] = FindViewById<TextView>(Resource.Id.t6);
            txt_num[7] = FindViewById<TextView>(Resource.Id.t7);
            txt_num[8] = FindViewById<TextView>(Resource.Id.t8);
            txt_num[9] = FindViewById<TextView>(Resource.Id.t9);
            txt_num[10] = FindViewById<TextView>(Resource.Id.t10);
            txt_num[11] = FindViewById<TextView>(Resource.Id.t11);
            txt_num[12] = FindViewById<TextView>(Resource.Id.t12);
            txt_num[13] = FindViewById<TextView>(Resource.Id.t13);
            txt_num[14] = FindViewById<TextView>(Resource.Id.t14);
            txt_num[15] = FindViewById<TextView>(Resource.Id.t15);

            TextView[] arr = new TextView[8];
            arr[0] = FindViewById<TextView>(Resource.Id.arr0);
            arr[1] = FindViewById<TextView>(Resource.Id.arr1);
            arr[2] = FindViewById<TextView>(Resource.Id.arr2);
            arr[3] = FindViewById<TextView>(Resource.Id.arr3);
            arr[4] = FindViewById<TextView>(Resource.Id.arr4);
            arr[5] = FindViewById<TextView>(Resource.Id.arr5);
            arr[6] = FindViewById<TextView>(Resource.Id.arr6);
            arr[7] = FindViewById<TextView>(Resource.Id.arr7);

            Button generate = FindViewById<Button>(Resource.Id.generate);
            generate.Click += Generate;

            Button build = FindViewById<Button>(Resource.Id.build);
            build.Click += Build;

            TextView previous = FindViewById<TextView>(Resource.Id.previous);
            previous.Click += Previous;

            TextView stop = FindViewById<TextView>(Resource.Id.stop);
            stop.Click += Stop;

            TextView continue_ = FindViewById<TextView>(Resource.Id.continue_);
            continue_.Click += Continue;

            TextView next = FindViewById<TextView>(Resource.Id.next);
            next.Click += Next;


            EditText left_ = FindViewById<EditText>(Resource.Id.left);
            left_.EditorAction += HandleLeftAction;

            EditText right_ = FindViewById<EditText>(Resource.Id.right);
            right_.EditorAction += HandleRightAction;
            left = right = null;

            Button compute = FindViewById<Button>(Resource.Id.compute);
            compute.Click += Compute;


            Button newGroupoid = FindViewById<Button>(Resource.Id.generate_groupoid);
            newGroupoid.Click += GenerateGroupoid;

            EditText module = FindViewById<EditText>(Resource.Id.module);
            p = 10;
            module.EditorAction += HandleModuleAction;

            EditText function = FindViewById<EditText>(Resource.Id.function);
            operation = "X+Y";
            function.EditorAction += HandleFunctionAction;

            EditText set = FindViewById<EditText>(Resource.Id.generating_set);
            generating_set = new int[] { 1 };
            set.EditorAction += HandleSetAction;

            Button neutralCheck = FindViewById<Button>(Resource.Id.neutral);
            neutralCheck.Click += NeutralCheck;

            Button associativityCheck = FindViewById<Button>(Resource.Id.associative);
            associativityCheck.Click += AssociativityCheck;


            EditText arrayElements = FindViewById<EditText>(Resource.Id.arrayElements);
            arrayElements.EditorAction += HandleArrElemAction;

            SeekBar SbDelay = FindViewById<SeekBar>(Resource.Id.SbDelay);
            SbDelay.ProgressChanged += HandleDelayAction;

            monoid = new Monoid();
            monoid.MakeText += ShowMessage;
            monoid.Fun += Fun;

            arrayT = new ArrayT(8, arr, monoid);
            arrayT.MakeText += ShowMessage;

            MyST = new ST(8, txt_num, monoid, 1250);
            MyST.MakeText += ShowMessage;
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
                case Resource.Id.action_settings:
                    //Intent settings = new Intent(this, );
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void ShowMessage(string text) =>
            Toast.MakeText(this, text, ToastLength.Long).Show();

        public void UpdateInfo()
        {
            TextView counter = FindViewById<TextView>(Resource.Id.number);
            counter.Text = monoid.Count.ToString();

            left = right = null;
            FindViewById<EditText>(Resource.Id.left).Text = "";
            FindViewById<EditText>(Resource.Id.right).Text = "";
            arrayT.Clear();
            MyST.Clear();
        }

        /// <summary>
        /// Очистка группоида (моноида)
        /// </summary>
        private void GroupoidClear()
        {
            monoid.Clear();

            UpdateInfo();
        }

        private void NewFocus()
        {
            FindViewById<LinearLayout>(Resource.Id.mainLayout).RequestFocus();

            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);

            inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        /// <summary>
        /// Изменение слайдера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDelayAction(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            MyST.UpdateInterval(e.Progress);
            FindViewById<TextView>(Resource.Id.TvDelay).Text = (e.Progress / 1000.0) + " sec";
        }

        /// <summary>
        /// Проверка и сохранение количества элементов в массиве (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleArrElemAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Done)
            {
                EditText arrayElements = sender as EditText;
                e.Handled = true;

                NewFocus();

                if (!int.TryParse(arrayElements.Text, out int x) || x <= 0 || x > 8)
                {
                    ShowMessage("Error in number of elements in array!");
                    arrayElements.Text = arrayT.Length.ToString();
                    return;
                }

                int length = int.Parse(arrayElements.Text);

                arrayT.UpdateN(length);

                MyST.UpdateN(length);
                int pxx = MyST.N switch
                {
                    1 => Resources.GetDimensionPixelSize(Resource.Dimension.text_width320),
                    2 => Resources.GetDimensionPixelSize(Resource.Dimension.text_width160),
                    4 => Resources.GetDimensionPixelSize(Resource.Dimension.text_width80),
                    8 => Resources.GetDimensionPixelSize(Resource.Dimension.text_width40),
                    _ => 0,
                };
                for (int i = 0; i < arrayT.arr.Length; ++i)
                {
                    if (i < MyST.N)
                    {
                        LinearLayout.LayoutParams paramsss = new LinearLayout.LayoutParams(pxx, arrayT.arr[i].Height);
                        arrayT.arr[i].LayoutParameters = paramsss;
                    }
                    else
                    {
                        LinearLayout.LayoutParams paramsss = new LinearLayout.LayoutParams(0, arrayT.arr[i].Height);
                        arrayT.arr[i].LayoutParameters = paramsss;
                    }
                }
            }
        }

        /// <summary>
        /// Проверка и сохранение левой границы (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLeftAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next)
            {
                EditText left_ = sender as EditText;

                if (!int.TryParse(left_.Text, out int x) || x < 0 || x >= arrayT.Length)
                {
                    Toast toast = Toast.MakeText(this, "Error in left border!", ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                    if (left != null)
                        left_.Text = left.ToString();
                    else
                        left_.Text = "";
                    return;
                }

                left = int.Parse(left_.Text);
            }
        }


        /// <summary>
        /// Проверка и сохранение правой границы (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRightAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Done)
            {
                EditText right_ = sender as EditText;
                e.Handled = true;

                NewFocus();

                if (!int.TryParse(right_.Text, out int x) || x < 0 || x >= arrayT.Length)
                {
                    Toast toast = Toast.MakeText(this, "Error in right border!", ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                    if (left != null)
                        right_.Text = right.ToString();
                    else
                        right_.Text = "";
                    return;
                }

                right = int.Parse(right_.Text);
            }
        }

        /// <summary>
        /// Проверка и сохранение содержимого в ячейке модуля моноида (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleModuleAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next)
            {
                EditText module = sender as EditText;

                if (!int.TryParse(module.Text, out int x) || x <= 0 || x > 250)
                {
                    Toast toast = Toast.MakeText(this, "Error in module!", ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                    module.Text = p.ToString();
                    return;
                }

                p = int.Parse(module.Text);

                GroupoidClear();
            }
        }

        /// <summary>
        /// Проверка и сохранение содержимого в ячейке функции моноида (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFunctionAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next)
            {
                EditText function = sender as EditText;

                if (function.Text.Length == 0)
                {
                    Toast toast = Toast.MakeText(this, "Error in function!", ToastLength.Long);
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.Show();
                    function.Text = operation;
                    return;
                }

                operation = function.Text;

                GroupoidClear();
            }
        }

        /// <summary>
        /// Проверка и сохранение содержимого в ячейке порождающего множества моноида (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSetAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Done)
            {
                EditText set = sender as EditText;
                e.Handled = true;

                NewFocus();

                string[] splitted = set.Text.Split(", ");

                // если один из элементов порождающего множества не удовлетворяет условию
                if (!Array.TrueForAll(splitted, s => int.TryParse(s, out int x) && x >= 0))
                {
                    ShowMessage("Error in generating set!");
                    set.Text = string.Join(", ", generating_set);
                    return;
                }

                generating_set = new int[splitted.Length];
                for (int i = 0; i < splitted.Length; ++i)
                {
                    generating_set[i] = int.Parse(splitted[i]);
                }

                GroupoidClear();
            }
        }

        /// <summary>
        /// Функция
        /// </summary>
        /// <param name="X"> Первый аргумент </param>
        /// <param name="Y"> Второй аргумент </param>
        /// <returns></returns>
        private int Fun(int X, int Y)
        {
            int answer = Eval.Execute<int>(operation, new { X, Y }) % p;
            return answer + ((answer < 0) ? p : 0);
        }

        /// <summary>
        /// Обёртка для GenerateGroupoid, AssociativityCheck и NeutralCheck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="action"></param>
        public void Generation(object sender, Action action)
        {
            ((Button)sender).Clickable = false;

            action();

            UpdateInfo();

            ((Button)sender).Clickable = true;
        }

        /// <summary>
        /// Генерация группоида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateGroupoid(object sender, EventArgs e) =>
            Generation(sender, () => monoid.Generate(generating_set, p));

        /// <summary>
        /// Проверка на ассоциативность
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociativityCheck(object sender, EventArgs e) =>
            Generation(sender, () => monoid.AssociativityCheck());

        /// <summary>
        /// Проверка на наличие нейтрального элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeutralCheck(object sender, EventArgs e) =>
            Generation(sender, () => monoid.NeutralCheck());

        /// <summary>
        /// Генерация массива
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate(object sender, EventArgs e)
        {
            MyST.Clear();
            arrayT.Generate();
        }

        /// <summary>
        /// Построение дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build(object sender, EventArgs e) =>
            MyST.Build(arrayT);

        #region Methods
        /// <summary>
        /// Остановка алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop(object sender, EventArgs e) =>
            MyST.Stop();

        /// <summary>
        /// Продолжение алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Continue(object sender, EventArgs e) =>
            MyST.Continue();

        /// <summary>
        /// Следующее действие
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next(object sender, EventArgs e) =>
            MyST.Next();

        /// <summary>
        /// Предыдущее действие
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Previous(object sender, EventArgs e) =>
            MyST.Previous();
        #endregion

        /// <summary>
        /// Подсчёт функции на отрезке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Compute(object sender, EventArgs e) // update
        {
            if (left == null)
            {
                ShowMessage("Left!!");
                return;
            }
            if (right == null)
            {
                ShowMessage("Right!!");
                return;
            }
            int? ans = MyST.GetAns((int)left, (int)right);
            if (ans != null)
                ShowMessage(ans.ToString());
        }
    }
}
