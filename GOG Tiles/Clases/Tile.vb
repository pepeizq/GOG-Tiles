Public Class Tile

    Public Titulo As String
    Public ID As String
    Public Enlace As String

    Public ImagenPequeña As String
    Public ImagenMediana As String
    Public ImagenAncha As String
    Public ImagenGrande As String

    Public Sub New(titulo As String, id As String, enlace As String,
                   imagenPequeña As String, imagenMediana As String, imagenAncha As String, imagenGrande As String)
        Me.Titulo = titulo
        Me.ID = id
        Me.Enlace = enlace

        Me.ImagenPequeña = imagenPequeña
        Me.ImagenMediana = imagenMediana
        Me.ImagenAncha = imagenAncha
        Me.ImagenGrande = imagenGrande
    End Sub
End Class
