Option Strict Off
Imports System.IO

Namespace Iswenzz.GhostDownloader
    Module Program

        Property GhostDownloaded As Integer = 0
        Property LogFile As StreamWriter = New StreamWriter(
            New FileStream("ghosts.log", FileMode.Append, FileAccess.Write))

        ''' <summary>
        ''' Entry point of the program.
        ''' </summary>
        ''' <param name="args">Command line arguments.</param>
        Sub Main(args As String())
            LogFile.AutoFlush = True
            Log("CTGP-R Ghost Downloader (c) Iswenzz 2020")
            DownloadAllLeaderboards()

            Log($"Successfuly downloaded {GhostDownloaded} ghosts.")
            Log(Environment.NewLine & "Press ENTER to continue . . .")
            LogFile.Close()
            LogFile.Dispose()

            Console.ReadLine()
        End Sub

    End Module
End Namespace
