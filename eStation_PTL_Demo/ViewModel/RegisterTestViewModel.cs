using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Helper;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace eStation_PTL_Demo.ViewModel
{
    public class RegisterTestViewModel : DialogViewModelBase
    {
        private int targetCount = 0;
        public string TargetCountText
        {
            get => targetCount.ToString();
            set
            {
                if (int.TryParse(value, out var v) && v >= 0)
                {
                    targetCount = v;
                }
                else
                {
                    targetCount = 0;
                }
                NotifyPropertyChanged(nameof(TargetCountText));
            }
        }

        private int receivedCount = 0;
        public int ReceivedCount
        {
            get => receivedCount;
            private set { receivedCount = value; NotifyPropertyChanged(nameof(ReceivedCount)); }
        }

        private string elapsedText = "00:00:00.000";
        public string ElapsedText
        {
            get => elapsedText;
            private set { elapsedText = value; NotifyPropertyChanged(nameof(ElapsedText)); }
        }

        private string runButtonText = "RUN";
        public string RunButtonText
        {
            get => runButtonText;
            private set { runButtonText = value; NotifyPropertyChanged(nameof(RunButtonText)); }
        }

        private bool testAutoRegister = false;
        /// <summary>
        /// Auto Register only during test
        /// </summary>
        public bool TestAutoRegister
        {
            get => testAutoRegister;
            set
            {
                if (testAutoRegister != value)
                {
                    testAutoRegister = value;
                    NotifyPropertyChanged(nameof(TestAutoRegister));
                    // If test is running apply immediately
                    if (cts != null)
                    {
                        SendService.Instance.OnAutoRegisterChanged(testAutoRegister);
                    }
                }
            }
        }

        public ICommand CmdRunStop { get; }

        private readonly HashSet<string> uniqueTagIds = new();
        private readonly Stopwatch stopwatch = new();
        private readonly object _locker = new();
        private CancellationTokenSource? cts;
        private System.Timers.Timer? timer;
        private readonly bool originalAutoRegister;

        public RegisterTestViewModel(bool initialConnect, bool initialAutoRegister)
        {
            IsConnect = initialConnect;
            originalAutoRegister = initialAutoRegister;
            testAutoRegister = initialAutoRegister;
            CmdRunStop = new MyCommand(DoRunStop, _ => true);
            SendService.Instance.Register(HandleTaskResult);
            // 监听IsConnect变化，断开时自动停止
            this.PropertyChanged += OnSelfPropertyChanged;
        }

        private void OnSelfPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsConnect) && !IsConnect && cts != null)
            {
                // 在UI线程停止测试
                Application.Current.Dispatcher.Invoke(Stop);
            }
        }

        private void DoRunStop(object _)
        {
            if (cts != null)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void Start()
        {
            if (!IsConnect) { 
                MsgHelper.Error("AP not connected"); 
                return; 
            }

            // 提示：此测试适用于勾选 Auto Register 后统计不同TagID反馈数量
            if (targetCount <= 0)
            {
                MsgHelper.Infor("Please enter test count");
                return;
            }

            uniqueTagIds.Clear();
            ReceivedCount = 0;
            stopwatch.Reset();
            stopwatch.Start();
            RunButtonText = "STOP";

            // Apply test auto register (only during test)
            SendService.Instance.OnAutoRegisterChanged(TestAutoRegister);

            cts = new CancellationTokenSource();
            timer = new System.Timers.Timer(100);
            timer.Elapsed += OnTimer;
            timer.AutoReset = true;
            timer.Start();
        }

        private void Stop()
        {
            cts?.Cancel();
            cts = null;
            if (timer != null)
            {
                timer.Elapsed -= OnTimer;
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            stopwatch.Stop();
            RunButtonText = "RUN";
            // 输出当前测试采集到的唯一标签ID到文件
            try
            {
                lock (_locker)
                {
                    var lines = uniqueTagIds.OrderBy(x => x).ToArray();
                    File.WriteAllLines("uniqueTagIds.txt", lines);
                }
            }
            catch { }
            SendService.Instance.OnAutoRegisterChanged(originalAutoRegister);
        }

        private void OnTimer(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ElapsedText = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            });
            if (targetCount > 0 && ReceivedCount >= targetCount)
            {
                Application.Current.Dispatcher.Invoke(Stop);
            }
        }

        private void HandleTaskResult(TaskResult result)
        {
            if (cts == null) return; // not running

            int count;
            lock (_locker)
            {
                foreach (var item in result.Items)
                {
                    if (item.DataType != 0) continue;
                    uniqueTagIds.Add(item.TagID);
                }
                count = uniqueTagIds.Count;
            }
            Application.Current.Dispatcher.BeginInvoke(() => { ReceivedCount = count; });
        }

        public void OnWindowClosed()
        {
            Stop();
            SendService.Instance.TaskResultHandler -= HandleTaskResult;
            this.PropertyChanged -= OnSelfPropertyChanged;
        }
    }
}
