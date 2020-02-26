Imports System.Globalization
Imports ShellProgressBar

Namespace Iswenzz.GhostDownloader.Progress
    Public Class ShellProgress
        Implements IDisposable

        Public Property ProgressBar As ProgressBar
        Public Property ProgressBarOptions As ProgressBarOptions
        Public Property ProgressBarChildrens As List(Of ShellProgressChild)
        Public Property Title As String
        Public Property PTick As Long

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgress"/> object.
        ''' </summary>
        Public Sub New()
            Me.New(100)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgress"/> object.
        ''' </summary>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_initialMsg">Progress title</param>
        Public Sub New(_maxTick As Long, Optional _initialMsg As String = Nothing)
            Me.New(_maxTick, New ProgressBarOptions() With {
                .ProgressBarOnBottom = True,
                .ForegroundColor = ConsoleColor.Cyan,
                .ForegroundColorDone = ConsoleColor.Cyan,
                .BackgroundColor = ConsoleColor.DarkGray,
                .BackgroundCharacter = "\u2593",
                .ShowEstimatedDuration = True,
                .DisplayTimeInRealTime = True
            }, _initialMsg)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="ShellProgress"/> object.
        ''' </summary>
        ''' <param name="_maxTick">Progress max tick</param>
        ''' <param name="_options">Progress bar options</param>
        ''' <param name="_initialMsg">Progress title</param>
        Public Sub New(_maxTick As Long, _options As ProgressBarOptions, Optional _initialMsg As String = Nothing)
            ProgressBar = New ProgressBar(_maxTick, _initialMsg, _options)
            ProgressBarChildrens = New List(Of ShellProgressChild)
        End Sub

        ''' <summary>
        ''' Report to the <see cref="ShellProgress"/>.
        ''' </summary>
        ''' <param name="currentTick"></param>
        Public Overridable Sub Progress(currentTick As Long)
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
            ProgressBar.Dispose()
            For Each child As IProgressBar In ProgressBarChildrens
                child?.Dispose()
            Next
            ProgressBarChildrens?.Clear()
        End Sub
    End Class
End Namespace
