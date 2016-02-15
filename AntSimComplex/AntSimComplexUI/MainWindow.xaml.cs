using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.Utilities;
using AntSimComplexTspLibItemManager;
using AntSimComplexUI.Dialogs;
using AntSimComplexUI.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace AntSimComplexUI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private bool _initialised;
    private bool _drawOptimal;
    private static bool _timerRunning;

    private double _optimalTourLength;

    private NodeSelectionStrategy _selectionStrategy = NodeSelectionStrategy.RouletteWheel;

    private TspLibItemManager _tspLibItemManager;
    private Visualiser _visualiser;
    private AntSystem _antSystem;
    private readonly ObservableCollection<ListViewTourItem> _tourItems;

    public MainWindow()
    {
      InitializeComponent();
      _tourItems = new ObservableCollection<ListViewTourItem>();

      // Automatically sort tour items by length when adding to the ListView
      TourListView.Items.SortDescriptions.Add(new SortDescription("Length", ListSortDirection.Ascending));
      TourListView.ItemsSource = _tourItems;
    }

    /// <summary>
    /// Called as soon as the main window is loaded.
    /// </summary>
    private void Initialise()
    {
      var tspLibPath = BrowseTsplibPath();

      // Load all symmetric TSP instances.
      _tspLibItemManager = new TspLibItemManager(tspLibPath);
      TspCombo.ItemsSource = _tspLibItemManager.AllProblemNames;
      SelectionStrategy.ItemsSource = Enum.GetNames(typeof(NodeSelectionStrategy));
      SelectionStrategy.SelectedIndex = (int)NodeSelectionStrategy.RouletteWheel;
    }

    /// <summary>
    /// Runs the algorithm against the selected problem and for the chosen number of iterations
    /// or seconds.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExecuteButtonClick(object sender, RoutedEventArgs e)
    {
      // Clear previous results.
      _antSystem.Reset();

      // Fire up a background worker so that we may execute the AS algorithm
      // while blocking the UI from receiving any user input.
      var worker = new BackgroundWorker();

      // This method gets called as soon as work has completed. UI interaction is re-enabled.
      worker.RunWorkerCompleted += ExecutionCompleted;

      // Are we executing by nr of iterations or time?
      if (RunCount.IsChecked == true)
      {
        IterateThroughRunCount(worker);
      }
      else if (RunTime.IsChecked == true)
      {
        IterateForRunTime(worker);
      }
      else
      {
        IterateForThreshold(worker);
      }

      // Set IsBusy before we start the thread.
      BusyIndicator.IsBusy = true;
      worker.RunWorkerAsync();
    }

    private void IterateForRunTime(BackgroundWorker worker)
    {
      // ReSharper disable once PossibleInvalidOperationException
      // Will never be NULL (*1000 for ms)
      var timeOut = (double)RunTimeInt.Value * 1000;
      _timerRunning = true;

      // No direct interaction with the UI is allowed from this method.
      worker.DoWork += (o, ea) =>
      {
        using (var timer = new Timer(timeOut))
        {
          timer.Elapsed += OnTimerElapsed;
          timer.Start();
          while (_timerRunning &&
                 _antSystem.IterationMinTourLength > _optimalTourLength
          )
          {
            _antSystem.Execute();
          }
        }
      };
    }

    private void IterateThroughRunCount(BackgroundWorker worker)
    {
      var runCount = RunCountInt.Value;

      // No direct interaction with the UI is allowed from this method.
      worker.DoWork += (o, ea) =>
      {
        var i = 0;
        while (i < runCount &&
               _antSystem.IterationMinTourLength > _optimalTourLength)
        {
          _antSystem.Execute();
          i++;
        }
      };
    }

    private void IterateForThreshold(BackgroundWorker worker)
    {
      var runThreshold = RunThresholdInt.Value;

      worker.DoWork += (o, ea) =>
      {
        while (_antSystem.IterationMinTourLength > runThreshold &&
               _antSystem.IterationMinTourLength > _optimalTourLength)
        {
          _antSystem.Execute();
        }
      };
    }

    private void LogStats()
    {
      var logMessages = new List<string>
      {
        $"Results for problem: {_tspLibItemManager.ProblemName}",
        IterationStatsItem.CsvHeader
      };

      logMessages.AddRange(_antSystem.IterationStats.Select(s => s.CsvString));
      StatsLogger.Logger.Log(logMessages);
    }

    /// <summary>
    /// Browse to the TSPLIB95 directory.
    /// </summary>
    /// <returns></returns>
    private string BrowseTsplibPath()
    {
      var packageRelPath = Properties.Settings.Default.PackageRelPath;
      var libPathRegistryKey = Properties.Settings.Default.LibPathRegistryKey;

      var tspLibPath = (string)Registry.GetValue(libPathRegistryKey, "", "");
      var startDirectory = Path.GetFullPath(packageRelPath);
      bool tspPathExists;

      do
      {
        var dialog = new DirectoryBrowserDialog(tspLibPath, startDirectory) { Owner = this };
        dialog.ShowDialog();

        tspLibPath = dialog.DirectoryPath;
        tspPathExists = Directory.Exists(tspLibPath);

        if (!tspPathExists)
        {
          MessageBox.Show(this, "Path to TSPLIB95 is invalid.", "Error!");
        }
      } while (string.IsNullOrWhiteSpace(tspLibPath) || !tspPathExists);

      Registry.SetValue(libPathRegistryKey, "", tspLibPath);
      return tspLibPath;
    }

    private void AddOptimalTourToListView()
    {
      if (_tspLibItemManager.HasOptimalTour)
      {
        _tourItems.Add(new ListViewTourItem(_tspLibItemManager.OptimalTour,
                                            _tspLibItemManager.OptimalTourLength,
                                            $"Current Problem (Optimal): {_tspLibItemManager.ProblemName}"));

        _optimalTourLength = _tspLibItemManager.OptimalTourLength;
      }
    }

    /// <summary>
    /// Draw everything we know about the selected TSP problem item.
    /// </summary>
    private void DrawTspLibItem(bool windowSizeChanged = false)
    {
      var item = TourListView.SelectedItem as ListViewTourItem;
      _visualiser.DrawTspLibItem(item, _drawOptimal, windowSizeChanged);
    }

    #region eventhandlers

    /// <summary>
    /// Loads the TspLib95 item for the selected problem and instantiates a corresponding
    /// AntSystem object for it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InitialiseSystemForSelectedTspItem(object sender, SelectionChangedEventArgs e)
    {
      var problemName = TspCombo.SelectedItem?.ToString();
      _tspLibItemManager.LoadItem(problemName);
      _visualiser = new Visualiser(Canvas, _tspLibItemManager);

      DrawTspLibItem();
      CreateAntSystem();
      _tourItems.Clear();
      AddOptimalTourToListView();
    }

    private void CreateAntSystem()
    {
      _antSystem = new AntSystem(_selectionStrategy,
                                 _tspLibItemManager.NodeCount,
                                 _tspLibItemManager.NearestNeighbourTourLength,
                                 _tspLibItemManager.Distances);
    }

    /// <summary>
    /// Called once the AS algorithm has finished executing.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="ea"></param>
    private void ExecutionCompleted(object o, RunWorkerCompletedEventArgs ea)
    {
      BusyIndicator.IsBusy = false;
      LoadBestToursIntoListView();
      LogStats();
    }

    private void LoadBestToursIntoListView()
    {
      _tourItems.Clear();
      AddOptimalTourToListView();

      var count = 1;
      foreach (var tour in _antSystem.BestTours)
      {
        var nodeTour = _tspLibItemManager.ConvertTourIndicesToNodes(tour.Tour);
        _tourItems.Add(new ListViewTourItem(nodeTour, tour.TourLength, $"Best Tour for Iteration {count}"));
        count++;
      }
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
      _timerRunning = false;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      Initialise();
      _initialised = true;
    }

    private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (_initialised)
      {
        DrawTspLibItem(true);
      }
    }

    private void ShowOptimalChecked(object sender, RoutedEventArgs e)
    {
      _drawOptimal = true;

      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void ShowOptimalUnchecked(object sender, RoutedEventArgs e)
    {
      _drawOptimal = false;

      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void TourListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_initialised)
      {
        DrawTspLibItem();
      }
    }

    private void AlphaIntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.Alpha = (int)e.NewValue;
    }

    private void BetaIntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.Beta = (int)e.NewValue;
    }

    private void EvapDoubleValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Parameters.EvaporationRate = (double)e.NewValue;
    }

    private void SelectionStrategyChanged(object sender, SelectionChangedEventArgs e)
    {
      _selectionStrategy = (NodeSelectionStrategy)Enum.Parse(typeof(NodeSelectionStrategy),
                                                              SelectionStrategy.SelectedValue.ToString());
      CreateAntSystem();
    }

    #endregion eventhandlers
  }
}