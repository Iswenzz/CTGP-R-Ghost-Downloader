Imports System.Net
Imports ShellProgressBar

Namespace Iswenzz.GhostDownloader.Progress
    Public Class ProgressDownloadChild
        Inherits ShellProgressChild
        Implements IDisposable

        Public Property WebClient As WebClient
        Public Property FileName As String
        Public Property DownloadURL As String
        Public Property Stopwatch As Stopwatch

        ''' <summary>
        ''' Initialize a new <see cref="ProgressDownloadChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        ''' <param name="_downloadUrl">Download URL</param>
        ''' <param name="_filename">File Name</param>
        Public Sub New(_parent As ShellProgress, _downloadUrl As String, Optional _filename As String = Nothing)
            Me.New(_parent, _downloadUrl, 100, _filename:=_filename)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ProgressDownloadChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        ''' <param name="_downloadUrl">Download URL</param>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_initialMsg">Progress title</param>
        ''' <param name="_filename">File Name</param>
        Public Sub New(_parent As ShellProgress, _downloadUrl As String, _maxTick As Long, Optional _initialMsg As String = Nothing,
                       Optional _filename As String = Nothing)
            Me.New(_parent, _downloadUrl, _maxTick, New ProgressBarOptions() With {
                .ProgressBarOnBottom = True,
                .ForegroundColor = ConsoleColor.DarkCyan,
                .ForegroundColorDone = ConsoleColor.DarkCyan,
                .BackgroundColor = ConsoleColor.DarkGray,
                .BackgroundCharacter = "\u2593",
                .ShowEstimatedDuration = True,
                .DisplayTimeInRealTime = True
            }, _initialMsg, _filename)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ProgressDownloadChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        ''' <param name="_downloadUrl">Download URL</param>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_options">Progress bar options</param>
        ''' <param name="_initialMsg">Progress title</param>
        ''' <param name="_filename">File Name</param>
        Public Sub New(_parent As ShellProgress, _downloadUrl As String, _maxTick As Long, _options As ProgressBarOptions,
                       Optional _initialMsg As String = Nothing, Optional _filename As String = Nothing)
            MyBase.New(_parent, _maxTick, _options, _initialMsg)
            DownloadURL = _downloadUrl
            FileName = If(_filename, "Unknown File")
            Stopwatch = New Stopwatch()
            StartClient()
        End Sub

        ''' <summary>
        ''' Start a web client and add progress handlers.
        ''' </summary>
        Private Sub StartClient()
            WebClient?.Dispose()
            WebClient = New WebClient()
            AddHandler WebClient.DownloadProgressChanged, AddressOf WebProgressChanged
            AddHandler WebClient.DownloadDataCompleted, AddressOf WebProgressCompleted

            Try
                WebClient.OpenRead(DownloadURL)
                ProgressBar.MaxTicks = Convert.ToInt64(WebClient.ResponseHeaders("Content-Length"))
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Download a file with the URL specified in <see cref="ProgressDownloadChild"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Async Function Download() As Task(Of String)
            Return Await WebClient.DownloadStringTaskAsync(New Uri(DownloadURL))
        End Function

        ''' <summary>
        ''' Callback on web client download finish.
        ''' </summary>
        Private Sub WebProgressCompleted(sender As Object, e As DownloadDataCompletedEventArgs)
            Stopwatch.Stop()
        End Sub

        ''' <summary>
        ''' Callback on web client download progress.
        ''' </summary>
        Private Sub WebProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
            Progress(e.BytesReceived)
        End Sub

        ''' <summary>
        ''' Report to the <see cref="ShellProgress"/>.
        ''' </summary>
        ''' <param name="currentTick"></param>
        Public Overrides Sub Progress(currentTick As Long, Optional _message As String = Nothing)
            If PTick = 0 Then
                Stopwatch.Restart()
                PTick = currentTick
                Return
            End If

            PTick = currentTick
            Dim estimatedTime As TimeSpan = TimeSpan.FromSeconds((ProgressBar.MaxTicks - PTick) / (PTick / Stopwatch.Elapsed.TotalSeconds))
            Dim ptick_formated As String = FormatSize(PTick)
            Dim maxtick_formated As String = FormatSize(ProgressBar.MaxTicks)
            Dim bytesPerSeconds_formated As String = FormatSize(PTick / Stopwatch.Elapsed.TotalSeconds) + "/s"
            ProgressBar.Tick(PTick, estimatedTime, $"{FileName}       {ptick_formated}/{maxtick_formated}        {bytesPerSeconds_formated}")
        End Sub

        ''' <summary>
        ''' Release all resources.
        ''' </summary>
        Public Overrides Sub Dispose() Implements IDisposable.Dispose
            MyBase.Dispose()
            WebClient?.Dispose()
        End Sub
    End Class
End Namespace
