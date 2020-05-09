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
using Newtonsoft.Json;

namespace VisualST
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MonoidActivity : AppCompatActivity
    {
        // Моноид
        Monoid monoid;

        // Операция
        private string operation;

        // Модуль операции
        private int p;

        // Порождающее множество моноида
        private int[] generating_set;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_monoid);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


            FindViewById<Button>(Resource.Id.generate_groupoid).Click += GenerateGroupoid;

            FindViewById<EditText>(Resource.Id.module).EditorAction += HandleModuleAction;
            p = 10;

            FindViewById<EditText>(Resource.Id.operation).EditorAction += HandleOperationAction;
            operation = "X+Y";

            FindViewById<EditText>(Resource.Id.generating_set).EditorAction += HandleSetAction;
            generating_set = new int[] { 1 };

            FindViewById<Button>(Resource.Id.neutral).Click += NeutralCheck;

            FindViewById<Button>(Resource.Id.associative).Click += AssociativityCheck;


            TextView number = FindViewById<TextView>(Resource.Id.number);

            monoid = new Monoid(number);
            monoid.MakeText += ShowMessage;
            monoid.Fun += Fun;
            FindViewById<Button>(Resource.Id.next_).Click += StartActivity;
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

        /// <summary>
        /// Отображение сообщения
        /// </summary>
        /// <param name="text"> Текст для отображения </param>
        private void ShowMessage(string text) =>
            Toast.MakeText(this, text, ToastLength.Long).Show();

        /// <summary>
        /// Отображение сообщения в центре
        /// </summary>
        /// <param name="text"> Текст для отображения </param>
        private void ShowMessageCenter(string text)
        {
            Toast toast = Toast.MakeText(this, text, ToastLength.Long);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }

        private void NewFocus(EditText edit)
        {
            //#region Прошлая версия
            edit.ClearFocus();
            FindViewById<RelativeLayout>(Resource.Id.focusedLayout).RequestFocus();
            //#endregion

            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);

            inputManager.HideSoftInputFromWindow(edit.WindowToken, HideSoftInputFlags.NotAlways);
        }

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

                monoid.Clear();
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

                monoid.Clear();
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

                monoid.Clear();
            }
        }
        #endregion

        /// <summary>
        /// Функция
        /// </summary>
        /// <param name="X"> Первый аргумент </param>
        /// <param name="Y"> Второй аргумент </param>
        /// <returns> Результат применения функции к X и Y </returns>
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
        public void Disable(object sender, Action action)
        {
            ((Button)sender).Enabled = false;

            action();

            ((Button)sender).Enabled = true;
        }

        /// <summary>
        /// Генерация группоида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateGroupoid(object sender, EventArgs e) =>
            Disable(sender, () => monoid.Generate(generating_set, p));

        /// <summary>
        /// Проверка на ассоциативность
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociativityCheck(object sender, EventArgs e) =>
            Disable(sender, () => monoid.AssociativityCheck());

        /// <summary>
        /// Проверка на наличие нейтрального элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeutralCheck(object sender, EventArgs e) =>
            Disable(sender, () => monoid.NeutralCheck());

        private void StartActivity(object sender, EventArgs e)
        {
            if (monoid.Count == 0)
            {
                ShowMessage("Monoid is empty!");
                return;
            }
            if (!monoid.neutralTest)
            {
                ShowMessage("Check for neutral element!");
                return;
            }
            if (!monoid.associativityTest)
            {
                ShowMessage("Check for associativity!");
                return;
            }
            Intent intent = new Intent(this, typeof(VisualizationActivity));
            intent.PutExtra("operation", operation);
            intent.PutExtra("p", p);
            intent.PutExtra("generating_set", JsonConvert.SerializeObject(generating_set));
            intent.PutExtra("monoid", JsonConvert.SerializeObject(monoid));
            StartActivity(intent);
        }
    }
}
