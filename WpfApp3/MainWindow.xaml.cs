using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WordWheel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker BWorker_Timer;
        BackgroundWorker BWorker_Game;
        BackgroundWorker BWorker_Data;
        SQLiteConnection DataBase;
        SQLiteCommand Command_Word;
        SQLiteCommand Command_Definition;
        SQLiteDataReader Reader_Word;
        SQLiteDataReader Reader_Definition;
        string TableName;
        string Query_Word;
        string Query_Definition;
        int IndexCount;


        string Definition_A;
        string Definition_B;
        string Definition_C;
        string Definition_D;
        string Definition_E;
        string Definition_F;
        string Definition_G;
        string Definition_H;
        string Definition_I;
        string Definition_J;
        string Definition_K;
        string Definition_L;
        string Definition_M;
        string Definition_N;
        string Definition_O;
        string Definition_P;
        string Definition_Q;
        string Definition_R;
        string Definition_S;
        string Definition_T;
        string Definition_U;
        string Definition_V;
        string Definition_W;
        string Definition_Y;
        string Definition_Z;

        string Word_A;
        string Word_B;
        string Word_C;
        string Word_D;
        string Word_E;
        string Word_F;
        string Word_G;
        string Word_H;
        string Word_I;
        string Word_J;
        string Word_K;
        string Word_L;
        string Word_M;
        string Word_N;
        string Word_O;
        string Word_P;
        string Word_Q;
        string Word_R;
        string Word_S;
        string Word_T;
        string Word_U;
        string Word_V;
        string Word_W;
        string Word_Y;
        string Word_Z;

        string TempDef;
        string TempTextBox;
        Brush TempCol;
        float TempOp;

        List<Border> Letters;
        List<string> Words;
        List<string> Definitions;
        List<Border> LetterRemoveList;
        List<string> WordRemoveList;
        List<string> DefinitionRemoveList;
        List<string> LetterIndexList;
        List<string> TempWordList;
        List<string> TempDefinitionList;


        Brush InUseColor;
        Brush PassedColor;
        Brush FailedColor;

        bool UpdateCheck;
        bool PassCheck;
        bool LoopPass;
        bool Time;

        public MainWindow()
        {
            InitializeComponent();
            Letters = new List<Border> { Letter_A, Letter_B, Letter_C, Letter_D, Letter_E, Letter_F, Letter_G, Letter_H, Letter_I, Letter_J, Letter_K, Letter_L, Letter_M, Letter_N, Letter_O, Letter_P, Letter_Q, Letter_R, Letter_S, Letter_T, Letter_U, Letter_V, Letter_W, Letter_Y, Letter_Z };
            Words = new List<string> { Word_A, Word_B, Word_C, Word_D, Word_E, Word_F, Word_G, Word_H, Word_I, Word_J, Word_K, Word_L, Word_M, Word_N, Word_O, Word_P, Word_Q, Word_R, Word_S, Word_T, Word_U, Word_V, Word_W, Word_Y, Word_Z };
            Definitions = new List<string> { Definition_A, Definition_B, Definition_C, Definition_D, Definition_E, Definition_F, Definition_G, Definition_H, Definition_I, Definition_J, Definition_K, Definition_L, Definition_M, Definition_N, Definition_O, Definition_P, Definition_Q, Definition_R, Definition_S, Definition_T, Definition_U, Definition_V, Definition_W, Definition_Y, Definition_Z };

            LetterIndexList = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "Y", "Z" };
            TempWordList = new List<string> { };
            TempDefinitionList = new List<string> { };

            LetterRemoveList = new List<Border> { };
            WordRemoveList = new List<string> { };
            DefinitionRemoveList = new List<string> { };

            BWorker_Data = new BackgroundWorker();
            BWorker_Data.DoWork += Game_Data;
            BWorker_Data.RunWorkerCompleted += Data_Completed;

            BWorker_Timer = new BackgroundWorker();
            BWorker_Timer.DoWork += Timer_Count;
            BWorker_Timer.ProgressChanged += Timer_Update;
            BWorker_Timer.WorkerReportsProgress = true;

            BWorker_Game = new BackgroundWorker();
            BWorker_Game.DoWork += Game_Loop;
            BWorker_Game.ProgressChanged += Game_Update;
            BWorker_Game.WorkerReportsProgress = true;

            LoopPass = false;
            Time = true;
            PassCheck = false;
            UpdateCheck = true;
            InUseColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#96aaff");
            PassedColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#64ff96");
            FailedColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#c83200");
        }
        private void Level_Chooser(object sender, RoutedEventArgs e)
        {
            TableName = (sender as Button).Name.Replace("_Button", "");
            BWorker_Data.RunWorkerAsync();

            Level_A1_Border.Visibility = Visibility.Collapsed;
            Level_A2_Border.Visibility = Visibility.Collapsed;
            Level_B1_Border.Visibility = Visibility.Collapsed;
            Level_B2_Border.Visibility = Visibility.Collapsed;
            Level_C1_Border.Visibility = Visibility.Collapsed;
            Level_Mix_Border.Visibility = Visibility.Collapsed;

            Level_A1_Button.Visibility = Visibility.Collapsed;
            Level_A2_Button.Visibility = Visibility.Collapsed;
            Level_B1_Button.Visibility = Visibility.Collapsed;
            Level_B2_Button.Visibility = Visibility.Collapsed;
            Level_C1_Button.Visibility = Visibility.Collapsed;
            Level_Mix_Button.Visibility = Visibility.Collapsed;

            Timer_Border.Visibility = Visibility.Visible;

        }

        private void Game_Data(object sender, DoWorkEventArgs e)
        {
            IndexCount = 0;

            DataBase = new SQLiteConnection("Data Source=DB_Game.sqlite");
            DataBase.Open();

            foreach (string Letter in LetterIndexList)
            {


                Query_Word = $"SELECT Word FROM {TableName} WHERE Letter='{Letter}'";
                Query_Definition = $"SELECT Definition FROM {TableName} WHERE Letter='{Letter}'";
                Command_Word = new SQLiteCommand(Query_Word, DataBase);
                Command_Definition = new SQLiteCommand(Query_Definition, DataBase);

                Reader_Word = Command_Word.ExecuteReader();
                Reader_Definition = Command_Definition.ExecuteReader();

                TempWordList = new List<string> { };
                TempDefinitionList = new List<string> { };
                while (Reader_Word.Read())
                {
                    TempWordList.Add(Reader_Word["Word"].ToString());
                }
                while (Reader_Definition.Read())
                {
                    TempDefinitionList.Add(Reader_Definition["Definition"].ToString());
                }

                Random Rand = new Random();
                int Num = Rand.Next(0, TempWordList.Count());
                Words[IndexCount] = TempWordList[Num];
                Definitions[IndexCount] = TempDefinitionList[Num];
                Console.WriteLine(Words[IndexCount]);
                Console.WriteLine(Definitions[IndexCount]);
                IndexCount++;
            }


        }

        private void Data_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Definition.Text = "Definition";
            Timer.Text = "Start";
            Timer_Button.Visibility = Visibility.Visible;
        }

        private void Game_Loop(object sender, DoWorkEventArgs e)
        {
            while (Letters.Count() != 0 && Time)
            {
                foreach (Border Letter in Letters)
                {
                    if (!Time)
                    {
                        break;
                    }
                    int Index = Letters.IndexOf(Letter);
                    TempOp = 1;
                    TempDef = Definitions[Index];
                    TempCol = InUseColor;
                    BWorker_Game.ReportProgress(Index);

                    while (!LoopPass)
                    {
                        if (!Time)
                        {
                            break;
                        }
                    }
                    BWorker_Game.ReportProgress(Index);
                    Thread.Sleep(100);
                    if (TempTextBox.ToUpper() == Words[Index].ToUpper())
                    {
                        TempCol = PassedColor;
                        LetterRemoveList.Add(Letter);
                        WordRemoveList.Add(Words[Index]);
                        DefinitionRemoveList.Add(Definitions[Index]);
                    }
                    else
                    {
                        if (!PassCheck)
                        {
                            TempCol = FailedColor;
                            BWorker_Game.ReportProgress(Index);
                            Thread.Sleep(300);
                        }
                        else
                        {
                            PassCheck = false;
                        }
                        TempCol = Brushes.LightSteelBlue;
                        TempOp = 0.82F;
                    }
                    BWorker_Game.ReportProgress(Index);
                    LoopPass = false;
                    Thread.Sleep(100);
                }
                foreach (Border RLetter in LetterRemoveList)
                {
                    Letters.Remove(RLetter);
                }
                foreach (string RWord in WordRemoveList)
                {
                    Words.Remove(RWord);
                }
                foreach (string RDefinition in DefinitionRemoveList)
                {
                    Definitions.Remove(RDefinition);
                }
            }
            UpdateCheck = false;
            BWorker_Game.ReportProgress(0);

        }

        private void Game_Update(object sender, ProgressChangedEventArgs e)
        {
            if (UpdateCheck)
            {
                int Index = e.ProgressPercentage;
                Border CLetter = Letters[Index];
                if (TempDef.Length < 75)
                {
                    Definition.FontSize = 20;
                }
                else if (TempDef.Length < 130)
                {
                    Definition.FontSize = 18;
                }
                else
                {
                    Definition.FontSize = 12;
                }
                Definition.Text = TempDef;
                CLetter.Opacity = TempOp;
                CLetter.BorderBrush = TempCol;
                TempTextBox = TextBox.Text;
                TextBox.Text = "";
            }
            if (Time && !UpdateCheck)
            {
                Definition.FontSize = 32;
                Definition.Text = "You Won";
            }
            else if (!Time && !UpdateCheck)
            {
                Definition.FontSize = 32;
                Definition.Text = "You Lost";
            }
        }

        private void Timer_Count(object sender, DoWorkEventArgs e)
        {
            for (int i = 300; i > 0; i--)
            {
                Thread.Sleep(1000);
                if (UpdateCheck)
                {
                    BWorker_Timer.ReportProgress(i);
                }
            }
            Time = false;
        }

        private void Timer_Update(object sender, ProgressChangedEventArgs e)
        {
            string Min = "0" + (e.ProgressPercentage / 60).ToString();
            string Sec = (e.ProgressPercentage % 60).ToString();


            if (Sec.Length == 1)
            {
                Sec = "0" + Sec;
            }
            Timer.Text = (Min + ":" + Sec);
    
        }
        

        private void Game_Start(object sender, RoutedEventArgs e)
        {

            BWorker_Timer.RunWorkerAsync();
            BWorker_Game.RunWorkerAsync();
            TextBox.Focus();
            Timer_Button.Visibility = Visibility.Collapsed;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Type Word Here")
            {
                TextBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "")
            {
                TextBox.Text = "Type Word Here";
            }

        }

        private void Pressed_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoopPass = true;
            }
        }

        private void Button_Send(object sender, RoutedEventArgs e)
        {
            LoopPass = true;
        }

        private void Button_Pass(object sender, RoutedEventArgs e)
        {
            PassCheck = true;
            LoopPass = true;
        }

    }
}