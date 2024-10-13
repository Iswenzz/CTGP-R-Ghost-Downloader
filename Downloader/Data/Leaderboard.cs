namespace CTGPR.Downloader.Data
{
    /// <summary>
    /// Initialize a new <see cref="Leaderboard"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="url">The URL.</param>
    public class Leaderboard(string name, string url)
    {
        public string Name { get; set; } = name;
        public string URL { get; set; } = url;
    }
}
