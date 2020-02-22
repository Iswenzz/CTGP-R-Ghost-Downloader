Imports System.Dynamic
Imports System.IO
Imports System.Net
Imports System.Threading
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
                New LeaderboardEntry("150cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards.json"),
                New LeaderboardEntry("200cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards-200cc.json"),
                New LeaderboardEntry("150cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards.json"),
                New LeaderboardEntry("200cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards-200cc.json")
            }

            For Each entry As LeaderboardEntry In Entries
                If AskUser(Environment.NewLine & "Download " & entry.Name & "?: [y/n]") Then
                    Using webClient As New WebClient()
                        Log("Downloading " & entry.Name & " Leaderboards . . ." & Environment.NewLine)
                        Dim json As String = webClient.DownloadString(entry.URL)
                        DownloadAllGhosts(JsonConvert.DeserializeObject(Of ExpandoObject)(json))
                    End Using
                End If
            Next
        End Sub

        ''' <summary>
        ''' Download all ghosts from a specified leaderboard.
        ''' </summary>
        ''' <param name="trackLeaderboards">Leaderboard JSON Object.</param>
        Sub DownloadAllGhosts(ByVal trackLeaderboards As Object)
            Using webClient As New WebClient()
                For Each trackLink As Object In trackLeaderboards.leaderboards
                    Do
                        Try
                            Dim json As String = webClient.DownloadString(
                                $"http://tt.chadsoft.co.uk{trackLink._links.item.href}")
                            Dim track As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(json)

                            Log($"{track.name} {track.ghosts(0).href}")
                            webClient.DownloadFile($"http://tt.chadsoft.co.uk{track.ghosts(0).href}",
                                "./ghosts/" & Path.GetFileName(DirectCast(track.ghosts(0).href, String)))
                            GhostDownloaded += 1
                            Exit Do

                        Catch ex As WebException
                            Select Case DirectCast(ex.Response, HttpWebResponse).StatusCode
                                Case HttpStatusCode.ServiceUnavailable
                                    Log("503 Service Unavailable... retrying in 30sec")
                                    Thread.Sleep(30 * 1000)
                                    Exit Select
                            End Select
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
