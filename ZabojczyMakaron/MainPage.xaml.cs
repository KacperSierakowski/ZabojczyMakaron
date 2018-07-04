using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ZabojczyMakaron
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();

        bool MyCzlowiekCaptured = false;

        public MainPage()
        {
            this.InitializeComponent();
            

            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);
            
            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);

        }

        private void TargetTimer_Tick(object sender, object e)
        {
            pasekStanu.Value += 1;
            if(pasekStanu.Value>= pasekStanu.Maximum)
            {
                EndTheGame();
            }
        }

        private void EndTheGame()
        {
            if (!myPlayArea.Children.Contains(MyKoniecGryText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                MyCzlowiekCaptured = false;
                StartButton.Visibility = Visibility.Visible;
                myPlayArea.Children.Add(MyKoniecGryText);
            }
        }

        private void EnemyTimer_Tick(object sender, object e)
        {
            AddEnemy();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            MyCzlowiek.IsHitTestVisible = true;
            MyCzlowiekCaptured = false;
            pasekStanu.Value = 0;
            StartButton.Visibility = Visibility.Collapsed;
            myPlayArea.Children.Clear();
            myPlayArea.Children.Add(Portal);
            myPlayArea.Children.Add(MyCzlowiek);
            enemyTimer.Start();
            targetTimer.Start();

        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["MyEnemySzablon1"] as ControlTemplate;
            AnimateEnemy(enemy, 0, myPlayArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)myPlayArea.ActualHeight - 100), 
                random.Next((int)myPlayArea.ActualHeight - 100), "(Canvas.Top)");
            myPlayArea.Children.Add(enemy);

            enemy.PointerEntered += Enemy_PointerEntered;
            
        }

        private void Enemy_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (MyCzlowiekCaptured)
            {
                EndTheGame();
            }
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimete)
        {
            Storyboard myStoryBroad = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)))
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, propertyToAnimete);
            myStoryBroad.Children.Add(animation);
            myStoryBroad.Begin();
        }

        private void MyCzlowiek_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                MyCzlowiekCaptured = true;
                MyCzlowiek.IsHitTestVisible = false;
            }
        }

        private void Portal_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if(targetTimer.IsEnabled && MyCzlowiekCaptured)
            {
                pasekStanu.Value = 0;
                Canvas.SetLeft(Portal, random.Next(100, (int)myPlayArea.ActualWidth - 100));
                Canvas.SetTop(Portal, random.Next(100, (int)myPlayArea.ActualHeight - 100));
                Canvas.SetLeft(MyCzlowiek, random.Next(100, (int)myPlayArea.ActualWidth - 100));
                Canvas.SetTop(MyCzlowiek, random.Next(100, (int)myPlayArea.ActualHeight - 100));
                MyCzlowiekCaptured = false;
                MyCzlowiek.IsHitTestVisible = true;
            }
        }

        private void myPlayArea_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (MyCzlowiekCaptured)
            {
                Point pointerPosition = e.GetCurrentPoint(null).Position;
                Point relativePosition = grid.TransformToVisual(myPlayArea).TransformPoint(pointerPosition);
                if (Math.Abs(relativePosition.X-Canvas.GetLeft(MyCzlowiek))>MyCzlowiek.ActualWidth*3 ||
                    Math.Abs(relativePosition.Y - Canvas.GetTop(MyCzlowiek)) > MyCzlowiek.ActualHeight * 3)
                {
                    MyCzlowiekCaptured = false;
                    MyCzlowiek.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(MyCzlowiek, relativePosition.X - MyCzlowiek.ActualWidth / 2);
                    Canvas.SetTop(MyCzlowiek, relativePosition.Y - MyCzlowiek.ActualHeight / 2);
                }

            }
        }

        private void myPlayArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (MyCzlowiekCaptured)
            {
                EndTheGame();
            }
        }
    }
}
