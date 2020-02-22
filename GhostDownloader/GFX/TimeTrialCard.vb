Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Iswenzz.GhostDownloader.Iswenzz.GhostDownloader.GFX

Namespace Iswenzz.GhostDownloader
    ''' <summary>
    ''' Represent a time trial card gfx.
    ''' </summary>
    Public Class TimeTrialCard
        Implements IDisposable

        Public Property CircuitName As String
        Public Property TimeText As String
        Public Property PlayerName As String

        Public Property ControllerImage As String
        Public Property ComboImage As String
        Public Property CountryCode As Integer

        Public Property Buffer As Bitmap
        Public Property Graphics As Graphics
        Public Property FontFamily As FontFamily
        Public Property FontSize As Integer
        Public Property Font As Font

        Private _isDisposed = False
        Public Property IsDisposed As Boolean
            Get
                Return _isDisposed
            End Get
            Private Set(value As Boolean)
                _isDisposed = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize a new <see cref="TimeTrialCard"/> object with default values.
        ''' </summary>
        Public Sub New()
            CircuitName = "Luigi Circuit"
            TimeText = "1:08.774"
            PlayerName = "Cole"

            ControllerImage = "Gamecube"
            ComboImage = "Spear_FunkyKong"
            FontFamily = New FontFamily("Monospac821 BT")
            FontSize = 32
            CountryCode = 18
            Font = New Font(FontFamily, FontSize, FontStyle.Regular)
        End Sub

        ''' <summary>
        ''' Initialize a new <see cref="TimeTrialCard"/> object.
        ''' </summary>
        ''' <param name="_circuitName">The circuit name</param>
        ''' <param name="_timeText">The player time</param>
        ''' <param name="_playerName">Player name</param>
        ''' <param name="_comboImage">Combo image path</param>
        ''' <param name="_controllerImage">Game controller image path</param>
        Public Sub New(_circuitName As String, _timeText As String, _playerName As String,
                       _comboImage As String, _controllerImage As String, Optional _countryCode As Integer = 18)
            CircuitName = _circuitName
            TimeText = _timeText
            PlayerName = _playerName
            ComboImage = _comboImage
            ControllerImage = _controllerImage
            CountryCode = _countryCode

            FontFamily = New FontFamily("Monospac821 BT")
            FontSize = 32
            Font = New Font(FontFamily, FontSize, FontStyle.Regular)
        End Sub

        ''' <summary>
        ''' Save the time trial card to the specified path.
        ''' </summary>
        ''' <param name="path">Saving path</param>
        Public Sub Save(Optional path As String = "card.png")
            'Get resource images
            Dim _comboImage As Bitmap = Nothing, _controllerImage As Bitmap = Nothing
            Try
                _comboImage = My.Resources.Characters.ResourceManager.GetObject(ComboImage)
                _controllerImage = My.Resources.Controllers.ResourceManager.GetObject(ControllerImage)
            Catch
            End Try

            'Text Alignment
            Using sfCenter As StringFormat = New StringFormat With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
                'Back buffer
                Buffer = New Bitmap(My.Resources.Misc.bg)
                Graphics = Graphics.FromImage(Buffer)
                Graphics.InterpolationMode = InterpolationMode.NearestNeighbor

                'Card
                Dim cardRect As RectangleF = New RectangleF(Buffer.Size.Width / 4, Buffer.Size.Height / 4,
                                                            Buffer.Width / 2, Buffer.Height / 2)
                Graphics.DrawImage(My.Resources.Misc.ttcard, cardRect)

                'Map name
                Dim pMap As PointF = New PointF(cardRect.X + (cardRect.Width / 2.0F),
                                                cardRect.Y + (cardRect.Height / 5.5F))
                Graphics.DrawString(CircuitName, Font, Brushes.Gainsboro, pMap, sfCenter)

                'Time
                Dim pTime As PointF = New PointF(cardRect.X + (cardRect.Width / 2.0F),
                                                 cardRect.Y + (cardRect.Height / 2.8F))
                Graphics.DrawString(TimeText, Font, Brushes.Gainsboro, pTime, sfCenter)

                'Name
                Dim pName As PointF = New PointF(cardRect.X + (cardRect.Width / 1.475F),
                                                 cardRect.Y + (cardRect.Height / 1.35F))
                Graphics.DrawString(PlayerName, Font, Brushes.Gainsboro, pName, sfCenter)

                'Controller
                Dim pController As RectangleF = New RectangleF(cardRect.X + (cardRect.Width / 1.4F),
                                                               cardRect.Y + (cardRect.Height / 3.9F),
                                                               cardRect.Height / 5, cardRect.Height / 5)
                If _controllerImage IsNot Nothing Then
                    Graphics.DrawImage(_controllerImage, pController)
                End If

                'Combo
                Dim comboRect As RectangleF = New RectangleF(cardRect.X + (cardRect.Width / 7.8F),
                                                             cardRect.Y + (cardRect.Height / 2.05F),
                                                             cardRect.Width / 5, cardRect.Height / 3)
                If _comboImage IsNot Nothing Then
                    Graphics.DrawImage(_comboImage, comboRect)
                End If

                'Flag
                Using atlas As BitmapAtlas = New BitmapAtlas(16, 16) With {
                    .Bitmap = My.Resources.Misc.flags_32,
                    .Size = New Size(512, 512)
                }
                    Using flag As Bitmap = atlas.GetBitmapFromIndex(CountryCode)
                        Dim flagRect As RectangleF =
                            New RectangleF(cardRect.X + (cardRect.Width / 1.6F),
                                           cardRect.Y + (cardRect.Height / 2.0F), cardRect.Height / 5,
                                           cardRect.Height / 5)
                        Graphics.DrawImage(flag, flagRect)
                    End Using
                End Using

                Buffer.Save(path)
            End Using
        End Sub

        ''' <summary>
        ''' Dispose all resources.
        ''' </summary>
        ''' <param name="disposing">Allows diposing managed resources</param>
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not IsDisposed Then
                If disposing Then
                    Buffer?.Dispose()
                    Graphics?.Dispose()
                    FontFamily?.Dispose()
                    Font?.Dispose()
                End If
            End If
            IsDisposed = True
        End Sub

        ''' <summary>
        ''' Dipose all resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
