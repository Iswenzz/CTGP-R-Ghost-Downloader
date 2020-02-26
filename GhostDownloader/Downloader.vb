Imports System.Dynamic
Imports System.IO
Imports System.Net
Imports System.Threading
Imports Iswenzz.GhostDownloader.Iswenzz.GhostDownloader.Data
Imports Iswenzz.GhostDownloader.Iswenzz.GhostDownloader.Progress
Imports Newtonsoft.Json

Namespace Iswenzz.GhostDownloader
    Module Downloader
        Public Property ShellProgress As ShellProgress

        ''' <summary>
        ''' Download all leaderboards with Chadsoft API
        ''' </summary>
        Sub DownloadAllLeaderboards()
            If Not Directory.Exists("ghosts") Then
                Directory.CreateDirectory("ghosts")
            End If

            Dim Entries As List(Of LeaderboardEntry) = New List(Of LeaderboardEntry) From {
                New LeaderboardEntry("150cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards.json", "150cc"),
                New LeaderboardEntry("200cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards-200cc.json", "200cc"),
                New LeaderboardEntry("150cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards.json", "150cc"),
                New LeaderboardEntry("200cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards-200cc.json", "200cc")
            }

            For Each entry As LeaderboardEntry In Entries
                ShellProgress?.Dispose()
                If AskUser(Environment.NewLine & "Download " & entry.Name & "?: [y/n]") Then
                    Using wc As New WebClient
                        ShellProgress = New ShellProgress() With {
                            .Title = entry.Name
                        }
                        Dim json As String = wc.DownloadString(entry.URL)
                        DownloadAllGhosts(JsonConvert.DeserializeObject(Of ExpandoObject)(json), entry.Speed)
                    End Using
                End If
            Next
        End Sub

        ''' <summary>
        ''' Download all ghosts from a specified leaderboard.
        ''' </summary>
        ''' <param name="trackLeaderboards">Leaderboard JSON Object.</param>
        ''' <param name="speed"></param>
        Sub DownloadAllGhosts(ByVal trackLeaderboards As Object, ByVal speed As String)
            Dim i As Integer = 0
            ShellProgress.ProgressBar.MaxTicks = trackLeaderboards.leaderboards.Count
            For Each trackLink As Object In trackLeaderboards.leaderboards
                ShellProgress.ProgressBar.Message = $"- {ShellProgress.Title} Ghosts downloaded: {i}/{trackLeaderboards.leaderboards.Count}"
                Do
                    Using dl As New ProgressDownloadChild(ShellProgress,
                        $"http://tt.chadsoft.co.uk{trackLink._links.item.href}?start=0&limit=1&times=pb")
                        Try
                            'Go to track leaderboard
                            Dim json As String = dl.Download().Result
                            Debug.WriteLine(json)
                            Dim track As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(json)
                            dl.ProgressBar.MaxTicks = 1
                            dl.ProgressBar.Tick()
                            dl.FileName = track.name
                            dl.ProgressBar.Message = "- " & track.name
                            Console.Title = "CTGP-R Ghost Downloader"

                            'Download ghost
                            dl.WebClient.DownloadFile($"http://tt.chadsoft.co.uk{track.ghosts(0).href}",
                                "./ghosts/" & Path.GetFileName(DirectCast(track.ghosts(0).href, String)))
                            GhostDownloaded += 1

                            'Render a time trial card
                            Using card As TimeTrialCard = New TimeTrialCard() With {
                                .CircuitName = track.name,
                                .TimeText = track.ghosts(0).finishTimeSimple,
                                .ControllerImage = DirectCast(Integer.Parse(track.ghosts(0).controller), EControllers).ToString(),
                                .CountryCode = track.ghosts(0).country,
                                .PlayerName = track.ghosts(0).player,
                                .ComboImage = DirectCast(Integer.Parse(track.ghosts(0).vehicleId), EVehicles).ToString() +
                                    "_" + DirectCast(Integer.Parse(track.ghosts(0).driverId), EDrivers).ToString()
                            }
                                Dim savePath As String = $"{card.CircuitName}_{card.TimeText}_{speed}.png"
                                If Not Directory.Exists("cards") Then
                                    Directory.CreateDirectory("cards")
                                End If
                                card.Save(Path.Join("cards", String.Concat(savePath.Split(Path.GetInvalidFileNameChars()))))
                            End Using
                            Exit Do

                        Catch ex As Exception
                            If TypeOf ex Is WebException Then
                                Select Case DirectCast(DirectCast(ex, WebException).Response, HttpWebResponse).StatusCode
                                    Case HttpStatusCode.ServiceUnavailable
                                        Console.Title = "503 Service Unavailable... Retrying in 30sec"
                                        Thread.Sleep(30 * 1000)
                                        Exit Select
                                    Case Else
                                        Exit Do
                                End Select
                            Else
                                Exit Do
                            End If
                        End Try
                    End Using
                Loop

                i += 1
                ShellProgress.ProgressBar.Tick(message:=$"- {ShellProgress.Title} Ghosts downloaded: {i}/{trackLeaderboards.leaderboards.Count}")
            Next
        End Sub

        ''' <summary>
        ''' Prompt user for input.
        ''' </summary>
        ''' <param name="msg">The question string.</param>
        ''' <returns></returns>
        Function AskUser(ByVal msg As String) As Boolean
            Console.WriteLine(msg)
            Dim res As String = Console.ReadLine()

            If Not String.IsNullOrEmpty(res) Then
                If res.Trim().ToLower().Equals("y") Or res.Trim().ToLower().Equals("yes") Then
                    Return True
                ElseIf res.Trim().ToLower().Equals("n") Or res.Trim().ToLower().Equals("no") Then
                    Return False
                End If
            End If

            Return True
        End Function

        ''' <summary>
        ''' Release app resources.
        ''' </summary>
        Public Sub Shutdown()
            ShellProgress?.Dispose()
        End Sub
    End Module
End Namespace
