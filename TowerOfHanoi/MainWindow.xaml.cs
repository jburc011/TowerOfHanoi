using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

namespace TowerOfHanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numberOfBlocks = 9;
        Thread solverThread = null;
        Rectangle[] blocks = new Rectangle[9];

        public MainWindow()
        {
            InitializeComponent();
            for(int i = 0; i < numberOfBlocks; i++)
            {
                blocks[i] = (Rectangle)tower1DockPanel.Children[i];
            }
        }

        private void InitializeTowers()
        {
            foreach (DockPanel panel in mainGrid.Children.OfType<DockPanel>())
            {
                panel.Children.Clear();
            }
            for (int i = 0; i < numberOfBlocks; i++)
            {
                tower1DockPanel.Children.Add(blocks[i]);
            }
        }

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            solverThread = new Thread(() => SolveTower(numberOfBlocks, tower1DockPanel, tower2DockPanel, tower3DockPanel));
            solverThread.Start();
        }

        // Move a stack of blocks from the source tower to the destination tower, making use of the spare tower as an intermediary
        private void SolveTower(int towerSize, DockPanel source, DockPanel destination, DockPanel spare)
        {
            if (towerSize > 0)
            {
                // Each call swaps the spare and destination towers so that blocks coming off the source tower will alternate between going to the spare tower vs the destination tower
                SolveTower(towerSize - 1, source, spare, destination);
                // Call back up to the main thread to move the blocks between towers and update the gui
                Dispatcher.Invoke(() =>
                {
                    var movingDisc = source.Children[source.Children.Count-1];
                    source.Children.Remove(movingDisc);
                    destination.Children.Add(movingDisc);
                });
                // Pause the thread for a moment so the user can see the process of the blocks moving
                System.Threading.Thread.Sleep(500);
                // Each call swaps the spare and source towers so that the next block will be moved from the spare tower back to the source or destination towers
                SolveTower(towerSize - 1, spare, destination, source);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            StopSolving();
            InitializeTowers();
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopSolving();
        }
        
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if(numberOfBlocks < 9)
            {
                StopSolving();
                numberOfBlocks++;
                InitializeTowers();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if(numberOfBlocks > 1)
            {
                StopSolving();
                numberOfBlocks--;
                InitializeTowers();
            }
        }

        private void StopSolving()
        {
            if (solverThread != null)
            {
                solverThread.Abort();
            }
        }
    }
}
