Option Strict Off
Imports Iswenzz.GhostDownloader.Iswenzz.GhostDownloader.Progress

Namespace Iswenzz.GhostDownloader
    Module Program

        Property GhostDownloaded As Integer = 0

        ''' <summary>
        ''' Entry point of the program.
        ''' </summary>
        ''' <param name="args">Command line arguments.</param>
        Sub Main(args As String())
            Console.ForegroundColor = ConsoleColor.Gray
            Console.Title = "CTGP-R Ghost Downloader"
            Console.WriteLine("CTGP-R Ghost Downloader (c) Iswenzz 2020")
            DownloadAllLeaderboards()

            Console.WriteLine($"Successfuly downloaded {GhostDownloaded} ghosts.")
            Console.WriteLine(Environment.NewLine & "Press ENTER to continue . . .")
            Shutdown()

            Console.ReadLine()
        End Sub

    End Module
End Namespace
