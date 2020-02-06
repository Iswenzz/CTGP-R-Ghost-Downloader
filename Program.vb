Option Strict Off
Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.Dynamic
Imports Newtonsoft.Json

Module Program

    Property GhostDownloaded As Integer = 0
    Property LogFile As StreamWriter = New StreamWriter(
        New FileStream("ghosts.log", FileMode.Append, FileAccess.Write))

    Sub Main(args As String())
        LogFile.AutoFlush = True
        Log("CTGP-R Ghost Downloader (c) Iswenzz 2020" & Environment.NewLine)
        DownloadAllLeaderboards()

        Log($"Successfuly downloaded {GhostDownloaded} ghosts.")
        Log(Environment.NewLine & "Press ENTER to continue . . .")
        LogFile.Close()
        LogFile.Dispose()

        Console.ReadLine()
    End Sub

    Sub DownloadAllLeaderboards()
        If Not Directory.Exists("ghosts") Then
            Directory.CreateDirectory("ghosts")
        End If

        Dim Entries As List(Of Entry) = New List(Of Entry) From {
            New Entry("150cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards.json"),
            New Entry("200cc Original Tracks", "http://tt.chadsoft.co.uk/original-track-leaderboards-200cc.json"),
            New Entry("150cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards.json"),
            New Entry("200cc CTGP Tracks", "http://tt.chadsoft.co.uk/ctgp-leaderboards-200cc.json")
        }

        For Each entry As Entry In Entries
            If AskUser("Download " & entry.Name & "?: [y/n]") Then
                Using webClient As New WebClient()
                    Log(Environment.NewLine & "Downloading" & entry.Name & "Leaderboards . . ." & Environment.NewLine)
                    Dim json As String = webClient.DownloadString(entry.URL)
                    DownloadAllGhosts(JsonConvert.DeserializeObject(Of ExpandoObject)(json))
                End Using
            End If
        Next
    End Sub

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

    Function AskUser(ByVal msg As String) As Boolean
        Log(msg)
        Dim res As String = Console.ReadLine()
        If Not String.IsNullOrEmpty(res) Then
            If res.Equals("y") Or res.Equals("yes") Then
                Return True
            ElseIf res.Equals("n") Or res.Equals("no") Then
                Return False
            End If
        End If
        Return True
    End Function

    Sub Log(Of T As {IComparable, IConvertible})(ByVal msg As T)
        LogFile.WriteLine(msg)
        Console.WriteLine(msg)
    End Sub
End Module
