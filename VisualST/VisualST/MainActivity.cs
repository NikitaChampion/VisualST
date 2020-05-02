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

        // Левая, правая граница подсчёта функции на отрезке
        private int? left, right;

        // Позиция элемента для обновления
        private int? position;

        // На какое значение обновляется элемент
        private int? x;

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


            FindViewById<SeekBar>(Resource.Id.SbDelay).ProgressChanged += HandleDelayAction;


            FindViewById<EditText>(Resource.Id.arrayElements).EditorAction += HandleArrElemAction;


            FindViewById<Button>(Resource.Id.generate).Click += Generate;

            FindViewById<Button>(Resource.Id.build).Click += Build;


            FindViewById<TextView>(Resource.Id.previous).Click += Previous;

            FindViewById<TextView>(Resource.Id.stop).Click += Stop;

            FindViewById<TextView>(Resource.Id.continue_).Click += Continue;

            FindViewById<TextView>(Resource.Id.next).Click += Next;


            FindViewById<EditText>(Resource.Id.left).EditorAction += HandleLeftAction;

            FindViewById<EditText>(Resource.Id.right).EditorAction += HandleRightAction;
            left = right = null;

            FindViewById<Button>(Resource.Id.compute).Click += Compute;


            FindViewById<EditText>(Resource.Id.position).EditorAction += HandlePosAction;
            position = null;

            FindViewById<EditText>(Resource.Id.x).EditorAction += HandleXAction;
            x = null;

            FindViewById<Button>(Resource.Id.update).Click += Update;


            FindViewById<Button>(Resource.Id.generate_groupoid).Click += GenerateGroupoid;

            FindViewById<EditText>(Resource.Id.module).EditorAction += HandleModuleAction;
            p = 10;

            FindViewById<EditText>(Resource.Id.operation).EditorAction += HandleOperationAction;
            operation = "X+Y";

            FindViewById<EditText>(Resource.Id.generating_set).EditorAction += HandleSetAction;
            generating_set = new int[] { 1 };

            FindViewById<Button>(Resource.Id.neutral).Click += NeutralCheck;

            FindViewById<Button>(Resource.Id.associative).Click += AssociativityCheck;

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

        private void ShowMessageCenter(string text)
        {
            Toast toast = Toast.MakeText(this, text, ToastLength.Long);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }

        public void PositionsClear()
        {
            position = left = right = null;
            FindViewById<EditText>(Resource.Id.left).Text = "";
            FindViewById<EditText>(Resource.Id.right).Text = "";
            FindViewById<EditText>(Resource.Id.position).Text = "";
        }

        public void UpdateInfo()
        {
            TextView counter = FindViewById<TextView>(Resource.Id.number);
            counter.Text = monoid.Count.ToString();

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

        private void NewFocus(EditText edit)
        {
            #region Прошлая версия
            //FindViewById<LinearLayout>(Resource.Id.focusedLayout).RequestFocus();
            #endregion

            edit.ClearFocus();

            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);

            inputManager.HideSoftInputFromWindow(edit.WindowToken, HideSoftInputFlags.NotAlways);
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

                NewFocus(arrayElements);

                if (!int.TryParse(arrayElements.Text, out int length) || length <= 0 || length > 8)
                {
                    ShowMessage("Error in number of elements in array!");
                    arrayElements.Text = arrayT.Length.ToString();
                    return;
                }

                if (length == arrayT.Length)
                    return;

                arrayT.UpdateN(length);

                MyST.UpdateN(length);

                PositionsClear();

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

        #region Подсчёт функции на отрезке
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
                    ShowMessageCenter("Error in left border!");
                    if (left != null)
                        left_.Text = left.ToString();
                    else
                        left_.Text = "";
                    return;
                }

                left = x;
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

                NewFocus(right_);

                if (!int.TryParse(right_.Text, out int x) || x < 0 || x >= arrayT.Length)
                {
                    ShowMessage("Error in number of elements in array!");
                    if (left != null)
                        right_.Text = right.ToString();
                    else
                        right_.Text = "";
                    return;
                }

                right = x;
            }
        }
        #endregion

        #region Обновление элемента
        /// <summary>
        /// Проверка и сохранение позиции элемента, который следует обновить (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePosAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next)
            {
                EditText pos = sender as EditText;

                if (!int.TryParse(pos.Text, out int x) || x < 0 || x >= arrayT.Length)
                {
                    ShowMessageCenter("Error in position!");
                    if (pos != null)
                        pos.Text = left.ToString();
                    else
                        pos.Text = "";
                    return;
                }

                position = x;
            }
        }

        /// <summary>
        /// Проверка и сохранение константы, на которую следует обновить элемент (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleXAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Done)
            {
                EditText x_ = sender as EditText;
                e.Handled = true;

                NewFocus(x_);

                if (!int.TryParse(x_.Text, out int x) || x < 0 || x >= p)
                {
                    ShowMessageCenter("Error in position!");
                    if (x_ != null)
                        x_.Text = left.ToString();
                    else
                        x_.Text = "";
                    return;
                }

                this.x = x;
            }
        }
        #endregion

        #region Параметры моноида
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
                    ShowMessageCenter("Error in module!");
                    module.Text = p.ToString();
                    return;
                }

                p = x;

                GroupoidClear();
            }
        }

        /// <summary>
        /// Проверка и сохранение содержимого в ячейке функции моноида (после закрытия клавиатуры)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleOperationAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next)
            {
                EditText operation_ = sender as EditText;

                if (operation_.Text.Length == 0)
                {
                    ShowMessageCenter("Error in function!");
                    operation_.Text = operation;
                    return;
                }

                operation = operation_.Text;

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

                NewFocus(set);

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
        #endregion

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

        /// <summary>
        /// Подсчёт функции на отрезке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Compute(object sender, EventArgs e) // update
        {
            if (left == null)
            {
                ShowMessage("Write left border");
                return;
            }
            if (right == null)
            {
                ShowMessage("Write right border");
                return;
            }
            MyST.GetAns((int)left, (int)right);
        }

        /// <summary>
        /// Обновление элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update(object sender, EventArgs e) // update
        {
            if (position == null)
            {
                ShowMessage("Write position");
                return;
            }
            if (x == null)
            {
                ShowMessage("Write element");
                return;
            }
            MyST.Update((int)position, (int)x);
        }
    }
}
