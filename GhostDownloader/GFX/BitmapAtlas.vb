Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace Iswenzz.GhostDownloader.GFX
    Public NotInheritable Class BitmapAtlas
        Implements IDisposable

        Public Property InterpolationMode As InterpolationMode = InterpolationMode.NearestNeighbor
        Public Property Bitmap As Bitmap
        Public Property Size As Size
        Public Property MaxX As Integer
        Public Property MaxY As Integer

        ''' <summary>
        ''' Initialize a new <see cref="BitmapAtlas"/> object.
        ''' </summary>
        ''' <param name="_mx">Max rows</param>
        ''' <param name="_my">Max columns</param>
        Public Sub New(_mx As Integer, _my As Integer)
            MaxX = _mx
            MaxY = _my
        End Sub

        ''' <summary>
        ''' Crop the right image from atlas.
        ''' </summary>
        ''' <param name="index">Image index</param>
        ''' <returns></returns>
        Public Function GetBitmapFromIndex(index As Integer) As Bitmap
            Return GetBitmapFromRowCol(index / MaxX, index Mod MaxX)
        End Function

        ''' <summary>
        ''' Crop the right image from atlas.
        ''' </summary>
        ''' <param name="x">Row index</param>
        ''' <param name="y">Column Index</param>
        ''' <returns></returns>
        Public Function GetBitmapFromRowCol(x As Integer, y As Integer) As Bitmap
            If Bitmap Is Nothing Then
                Return Nothing
            End If

            Dim r As Rectangle = New Rectangle(x * (Size.Width / MaxX), y * (Size.Height / MaxY),
                Size.Width / MaxX, Size.Height / MaxY)
            Dim nb As Bitmap = New Bitmap(r.Width, r.Height)

            Using g As Graphics = Graphics.FromImage(nb)
                g.InterpolationMode = InterpolationMode.NearestNeighbor
                g.DrawImage(Bitmap, New RectangleF(PointF.Empty, nb.Size), r, GraphicsUnit.Pixel)
            End Using

            Return nb
        End Function

        ''' <summary>
        ''' Release all resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Bitmap?.Dispose()
        End Sub

    End Class
End Namespace
