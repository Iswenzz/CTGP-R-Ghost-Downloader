Imports System.Collections.Concurrent
Imports System.Globalization
Imports ShellProgressBar

Namespace Iswenzz.GhostDownloader.Progress
    Public Class ShellProgressChild
        Implements IDisposable

        Public Property Parent As ShellProgress
        Public Property ProgressBar As ChildProgressBar
        Public Property ProgressBarChildrens As ConcurrentBag(Of ShellProgressChild)
        Public Property ProgressBarOptions As ProgressBarOptions
        Public Property Title As String
        Public Property PTick As Long
        Public Property Stopwatch As Stopwatch

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgressChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        Public Sub New(_parent As ShellProgress)
            Me.New(_parent, 100)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgressChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_initialMsg">Progress title</param>
        Public Sub New(_parent As ShellProgress, _maxTick As Long, Optional _initialMsg As String = Nothing)
            Me.New(_parent, _maxTick, New ProgressBarOptions() With {
                .ProgressBarOnBottom = True,
                .ForegroundColor = ConsoleColor.DarkCyan,
                .ForegroundColorDone = ConsoleColor.DarkCyan,
                .BackgroundColor = ConsoleColor.DarkGray,
                .BackgroundCharacter = "\u2593",
                .ShowEstimatedDuration = True,
                .DisplayTimeInRealTime = True
            }, _initialMsg)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgressChild"/> object.
        ''' </summary>
        ''' <param name="_parent">Parent <see cref="ShellProgress"/> object</param>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_options">Progress bar options</param>
        ''' <param name="_initialMsg">Progress title</param>
        Public Sub New(_parent As ShellProgress, _maxTick As Long, _options As ProgressBarOptions, Optional _initialMsg As String = Nothing)
            Parent = _parent
            ProgressBar = Parent.ProgressBar.Spawn(_maxTick, _initialMsg, _options)
            Parent.ProgressBarChildrens.Add(Me)
            ProgressBarChildrens = New ConcurrentBag(Of ShellProgressChild)
            Stopwatch = New Stopwatch()
        End Sub

        ''' <summary>
        ''' Report to the <see cref="ShellProgress"/>.
        ''' </summary>
        ''' <param name="currentTick"></param>
        Public Overridable Sub Progress(currentTick As Long, Optional _message As String = Nothing)
            If currentTick = 0 Then
                Stopwatch.Restart()
                PTick = currentTick
                Return
            End If

            PTick = currentTick
            Dim estimatedTime As TimeSpan = TimeSpan.FromSeconds((ProgressBar.MaxTicks - PTick) / (PTick / Stopwatch.Elapsed.TotalSeconds))
            ProgressBar.Tick(PTick, estimatedTime, _message)
        End Sub

        ''' <summary>
        ''' Get a formated string from a file size.
        ''' </summary>
        ''' <param name="fileSize">File size in bytes</param>
        ''' <returns></returns>
        Public Overridable Function FormatSize(fileSize As Decimal) As String
            Dim sizes As String() = {"B", "KB", "MB", "GB", "TB"}
            Dim order As Integer = 0

            While fileSize >= 1024 And order < sizes.Length - 1
                order += 1
                fileSize /= 1024
            End While

            Return fileSize.ToString("0.##", CultureInfo.InvariantCulture) + " " + sizes(order)
        End Function

        ''' <summary>
        ''' Release all resources.
        ''' </summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            For Each child As IDisposable In ProgressBarChildrens
                child?.Dispose()
            Next
            ProgressBarChildrens?.Clear()
            ProgressBar.Dispose()
        End Sub
    End Class
End Namespace
