using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ShellProgressBar;

namespace CTGPR.Downloader.Progress
{
    /// <summary>
    /// Shell progress bar.
    /// </summary>
    public class ProgressBase : IDisposable
    {
        public string Title { get; set; }
        public string Message { get => Bar.Message; set => Bar.Message = value; }

        public int CurrentTick => Bar.CurrentTick;
        public int MaxTick { get => Bar.MaxTicks; set => Bar.MaxTicks = value; }

        protected ProgressBarBase Bar { get; set; }
        protected Stopwatch Stopwatch { get; set; } = new();

        /// <summary>
        /// Initialize a new <see cref="ProgressBase"/>.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parent">The parent progress.</param>
        public ProgressBase(string title, ProgressBase parent = null)
        {
            ProgressBarOptions options = new()
            {
                ProgressBarOnBottom = true,
                ForegroundColor = ConsoleColor.Cyan,
                ForegroundColorDone = ConsoleColor.Cyan,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593',
                ShowEstimatedDuration = true,
                DisplayTimeInRealTime = true
            };
            ProgressBarOptions childOptions = new()
            {
                ProgressBarOnBottom = true,
                ForegroundColor = ConsoleColor.DarkCyan,
                ForegroundColorDone = ConsoleColor.DarkCyan,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593',
                ShowEstimatedDuration = false,
                DisplayTimeInRealTime = true,
            };
            Title = title;
            Bar = parent == null ? new ProgressBar(100, title, options) : parent.Bar.Spawn(100, title, childOptions);
            Task.Run(Frame);
        }

        /// <summary>
        /// Progress bar tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        public virtual void Tick(int tick)
        {
            Bar.Tick(tick, Clock(tick), Message);
        }

        /// <summary>
        /// Clock control.
        /// </summary>
        /// <param name="tick">The tick.</param>
        protected virtual TimeSpan Clock(int tick)
        {
            tick = tick > 0 ? tick : 1;
            if (!Stopwatch.IsRunning)
                Stopwatch.Start();

            if (tick >= Bar.MaxTicks)
            {
                Stopwatch.Stop();
                return TimeSpan.Zero;
            }
            double secondsPerTick = Stopwatch.Elapsed.TotalSeconds / tick;
            double remainingSeconds = secondsPerTick * (Bar.MaxTicks - tick);
            return TimeSpan.FromSeconds(remainingSeconds);
        }

        /// <summary>
        /// Progress bar frame.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task Frame()
        {
            while (Bar != null && Bar.CurrentTick != Bar.MaxTicks)
            {
                Tick(CurrentTick);
                await Task.Delay(200);
            }
        }

        /// <summary>
        /// Release all resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (Bar is ProgressBar bar)
                bar.Dispose();

            if (Bar is ChildProgressBar child)
                child.Dispose();
        }
    }
}
