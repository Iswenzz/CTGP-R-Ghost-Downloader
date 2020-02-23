Imports System.Dynamic
Imports System.IO
Imports System.Net
Imports System.Threading
Imports Iswenzz.GhostDownloader.Iswenzz.GhostDownloader.Data
Imports Newtonsoft.Json

Namespace Iswenzz.GhostDownloader
    Module Downloader
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
                If AskUser(Environment.NewLine & "Download " & entry.Name & "?: [y/n]") Then
                    Using webClient As New WebClient()
                        Log("Downloading " & entry.Name & " Leaderboards . . ." & Environment.NewLine)
                        Dim json As String = webClient.DownloadString(entry.URL)
                        DownloadAllGhosts(JsonConvert.DeserializeObject(Of ExpandoObject)(json), entry.Speed)
                    End Using
                End If
            Next
        End Sub

        ''' <summary>
        ''' Download all ghosts from a specified leaderboard.
        ''' </summary>
        ''' <param name="trackLeaderboards">Leaderboard JSON Object.</param>
        Sub DownloadAllGhosts(ByVal trackLeaderboards As Object, ByVal speed As String)
            Using webClient As New WebClient()
                For Each trackLink As Object In trackLeaderboards.leaderboards
                    Do
                        Try
                            'Go to track leaderboard
                            Dim json As String = webClient.DownloadString(
                                $"http://tt.chadsoft.co.uk{trackLink._links.item.href}")
                            Dim track As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(json)

                            'Download ghost
                            Log($"{track.name} {track.ghosts(0).href}")
                            webClient.DownloadFile($"http://tt.chadsoft.co.uk{track.ghosts(0).href}",
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
                                        Log("503 Service Unavailable... retrying in 30sec")
                                        Thread.Sleep(30 * 1000)
                                        Exit Select
                                    Case Else
                                        Exit Do
                                End Select
                            Else
                                Exit Do
                            End If
                        End Try
                    Loop
                Next
            End Using
        End Sub

        ''' <summary>
        ''' Prompt user for input.
        ''' </summary>
        ''' <param name="msg">The question string.</param>
        ''' <returns></returns>
        Function AskUser(ByVal msg As String) As Boolean
            Log(msg)
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
        ''' Write to console output and file.
        ''' </summary>
        ''' <typeparam name="T">Primitive types.</typeparam>
        ''' <param name="msg">Message to write.</param>
        Sub Log(Of T As {IComparable, IConvertible})(ByVal msg As T)
            LogFile.WriteLine(msg)
            Console.WriteLine(msg)
        End Sub
    End Module
End Namespace
