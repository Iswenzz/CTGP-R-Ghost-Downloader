﻿Namespace Iswenzz.GhostDownloader
    Public Class LeaderboardEntry
        Public Property Name As String
        Public Property URL As String
        Public Property Speed As String

        ''' <summary>
        ''' Initialize a new <see cref="LeaderboardEntry"/> object.
        ''' </summary>
        ''' <param name="_name">Leaderboard Name.</param>
        ''' <param name="_url">Leaderboard URL.</param>
        Public Sub New(ByVal _name As String, ByVal _url As String, ByVal _speed As String)
            Name = _name
            URL = _url
            Speed = _speed
        End Sub
    End Class
End Namespace
