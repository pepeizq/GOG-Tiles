Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.UI
Imports Windows.UI.Xaml.Media.Animation

Module GOGGalaxy

    Public anchoColumna As Integer = 180
    Dim clave As String = "carpetagog03"

    Public Async Sub Generar(buscarCarpeta As Boolean)

        Dim helper As New LocalObjectStorageHelper

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim pbProgreso As ProgressBar = pagina.FindName("pbProgreso")
        pbProgreso.Value = 0

        Dim tbProgreso As TextBlock = pagina.FindName("tbProgreso")
        tbProgreso.Text = String.Empty

        Configuracion.Estado(False)
        Cache.Estado(False)

        Dim gv As AdaptiveGridView = pagina.FindName("gvTiles")
        gv.DesiredWidth = anchoColumna
        gv.Items.Clear()

        Dim listaJuegos As New List(Of Tile)

        If Await helper.FileExistsAsync("juegos") = True Then
            listaJuegos = Await helper.ReadFileAsync(Of List(Of Tile))("juegos")
        End If

        If listaJuegos Is Nothing Then
            listaJuegos = New List(Of Tile)
        End If

        Dim recursos As New Resources.ResourceLoader()
        Dim carpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim i As Integer = 0
        If buscarCarpeta = True Then
            Try
                Dim picker As New FolderPicker()

                picker.FileTypeFilter.Add("*")
                picker.ViewMode = PickerViewMode.List

                Dim carpeta As StorageFolder = Await picker.PickSingleFolderAsync()

                If Not carpeta Is Nothing Then
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave, carpeta)
                End If
            Catch ex As Exception

            End Try
        End If

        '-------------------------------------------------------------

        Dim listaTemporal As New List(Of Tile)
        Dim carpetaMaestra As StorageFolder = Nothing

        Try
            carpetaMaestra = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave)
        Catch ex As Exception

        End Try

        If Not carpetaMaestra Is Nothing Then
            Dim gridProgreso As Grid = pagina.FindName("gridProgreso")
            Interfaz.Pestañas.Visibilidad(gridProgreso, Nothing, Nothing)

            Dim carpetasJuegos As IReadOnlyList(Of StorageFolder) = Await carpetaMaestra.GetFoldersAsync()

            For Each carpetaJuego As StorageFolder In carpetasJuegos
                Dim filtro As New List(Of String) From {
                    ".webp"
                }

                Dim opciones As New QueryOptions(CommonFileQuery.DefaultQuery, filtro)

                Dim query As StorageFileQueryResult = carpetaJuego.CreateFileQueryWithOptions(opciones)
                Dim ficheros As IReadOnlyList(Of StorageFile) = Await query.GetFilesAsync()

                If Not ficheros.Count = 0 Then
                    For Each fichero As StorageFile In ficheros
                        If fichero.Name.Contains("glx_vertical_cover") Then
                            Dim juego As New Tile(Nothing, carpetaJuego.Name, Nothing, Nothing, Nothing, Nothing, fichero.Path)
                            listaTemporal.Add(juego)
                            Exit For
                        End If
                    Next
                End If
            Next
        End If

        Dim k As Integer = 0
        If listaTemporal.Count > 0 Then
            Dim query As String = "http://api.gog.com/products?ids="

            For Each temporal In listaTemporal
                Dim buscar As Boolean = True
                Dim g As Integer = 0

                While g < listaJuegos.Count
                    If listaJuegos(g).ID = temporal.ID Then
                        buscar = False
                    End If
                    g += 1
                End While

                If buscar = True Then
                    Dim html As String = Await Decompiladores.HttpClient(New Uri("http://api.gog.com/products?ids=" + temporal.ID))

                    If Not html = Nothing Then
                        Dim listaJuegos2 As List(Of GOGAPIJuego) = JsonConvert.DeserializeObject(Of List(Of GOGAPIJuego))(html)

                        If Not listaJuegos2 Is Nothing Then
                            If listaJuegos2.Count > 0 Then
                                For Each juego2 In listaJuegos2
                                    Dim titulo As String = juego2.Titulo
                                    Dim id As String = juego2.ID

                                    Dim imagenPequeña As String = String.Empty
                                    Dim imagenMediana As String = String.Empty
                                    Dim imagenAncha As String = String.Empty
                                    Dim imagenGrande As String = String.Empty

                                    imagenPequeña = juego2.Imagenes.Icono

                                    If Not imagenPequeña = String.Empty Then
                                        If Not imagenPequeña.Contains("https:") Then
                                            imagenPequeña = "https:" + imagenPequeña
                                        End If
                                    End If

                                    imagenPequeña = Await Cache.DescargarImagen(imagenPequeña, id, "pequeña")

                                    imagenMediana = imagenPequeña

                                    imagenAncha = juego2.Imagenes.Logo.Replace("_glx_logo", Nothing)

                                    If Not imagenAncha = String.Empty Then
                                        If Not imagenAncha.Contains("https:") Then
                                            imagenAncha = "https:" + imagenAncha
                                        End If
                                    End If

                                    imagenAncha = Await Cache.DescargarImagen(imagenAncha, id, "ancha")

                                    Dim temp2 As String = temporal.ImagenGrande
                                    Dim int As Integer = temp2.LastIndexOf("\")

                                    temp2 = temp2.Remove(0, int + 1)
                                    temp2 = temp2.Replace(".webp", ".png?namespace=gamesdb")
                                    temp2 = "https://images.gog.com/" + temp2

                                    imagenGrande = Await Cache.DescargarImagen(temp2.Trim, id, "grande")

                                    'Dim enlace As String = ChrW(34) + "C:\GOG Galaxy\GalaxyClient.exe" + ChrW(34) + " /gameId=" + temporal.ID + " /command=runGame /path=" + ChrW(34) + temporal.Enlace + ChrW(34)
                                    Dim enlace As String = "goggalaxy://openGameView/" + id

                                    Dim juego As New Tile(titulo, id, enlace, imagenPequeña, imagenMediana, imagenAncha, imagenGrande)
                                    listaJuegos.Add(juego)

                                    pbProgreso.Value = CInt((100 / listaTemporal.Count) * k)
                                    tbProgreso.Text = k.ToString + "/" + listaTemporal.Count.ToString
                                    k += 1
                                Next
                            End If
                        End If
                    End If
                End If
            Next
        End If

        Try
            Await helper.SaveFileAsync(Of List(Of Tile))("juegos", listaJuegos)
        Catch ex As Exception

        End Try

        If Not listaJuegos Is Nothing Then
            If listaJuegos.Count > 0 Then
                Dim gridJuegos As Grid = pagina.FindName("gridJuegos")
                Interfaz.Pestañas.Visibilidad(gridJuegos, recursos.GetString("Games"), Nothing)

                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                gv.Items.Clear()

                For Each juego In listaJuegos
                    BotonEstilo(juego, gv)
                Next
            Else
                Dim gridAvisoNoJuegos As Grid = pagina.FindName("gridAvisoNoJuegos")
                Interfaz.Pestañas.Visibilidad(gridAvisoNoJuegos, Nothing, Nothing)
            End If
        Else
            Dim gridAvisoNoJuegos As Grid = pagina.FindName("gridAvisoNoJuegos")
            Interfaz.Pestañas.Visibilidad(gridAvisoNoJuegos, Nothing, Nothing)
        End If

        Configuracion.Estado(True)
        Cache.Estado(True)

    End Sub

    Public Sub BotonEstilo(juego As Tile, gv As GridView)

        Dim panel As New DropShadowPanel With {
            .Margin = New Thickness(10, 10, 10, 10),
            .ShadowOpacity = 0.9,
            .BlurRadius = 10,
            .MaxWidth = anchoColumna + 20,
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center
        }

        Dim boton As New Button

        Dim imagen As New ImageEx With {
            .Source = juego.ImagenGrande,
            .IsCacheEnabled = True,
            .Stretch = Stretch.UniformToFill,
            .Padding = New Thickness(0, 0, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center,
            .EnableLazyLoading = True
        }

        boton.Tag = juego
        boton.Content = imagen
        boton.Padding = New Thickness(0, 0, 0, 0)
        boton.Background = New SolidColorBrush(Colors.Transparent)

        panel.Content = boton

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = juego.Titulo,
            .FontSize = 16,
            .TextWrapping = TextWrapping.Wrap
        }

        ToolTipService.SetToolTip(boton, tbToolTip)
        ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

        AddHandler boton.Click, AddressOf BotonTile_Click
        AddHandler boton.PointerEntered, AddressOf Interfaz.Entra_Boton_Imagen
        AddHandler boton.PointerExited, AddressOf Interfaz.Sale_Boton_Imagen

        gv.Items.Add(panel)

    End Sub

    Private Sub BotonTile_Click(sender As Object, e As RoutedEventArgs)

        Trial.Detectar()
        Interfaz.AñadirTile.ResetearValores()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonJuego As Button = e.OriginalSource
        Dim juego As Tile = botonJuego.Tag

        Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
        botonAñadirTile.Tag = juego

        Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
        imagenJuegoSeleccionado.Source = New BitmapImage(New Uri(juego.ImagenAncha))

        Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
        tbJuegoSeleccionado.Text = juego.Titulo

        Dim gridAñadirTile As Grid = pagina.FindName("gridAñadirTile")
        Interfaz.Pestañas.Visibilidad(gridAñadirTile, juego.Titulo, Nothing)

        '---------------------------------------------

        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("animacionJuego", botonJuego)
        Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("animacionJuego")

        If Not animacion Is Nothing Then
            animacion.Configuration = New BasicConnectedAnimationConfiguration
            animacion.TryStart(gridAñadirTile)
        End If

        '---------------------------------------------

        Dim tbImagenTituloTextoTileAncha As TextBox = pagina.FindName("tbImagenTituloTextoTileAncha")
        tbImagenTituloTextoTileAncha.Text = juego.Titulo
        tbImagenTituloTextoTileAncha.Tag = juego.Titulo

        Dim tbImagenTituloTextoTileGrande As TextBox = pagina.FindName("tbImagenTituloTextoTileGrande")
        tbImagenTituloTextoTileGrande.Text = juego.Titulo
        tbImagenTituloTextoTileGrande.Tag = juego.Titulo

        '---------------------------------------------

        Dim imagenPequeña As ImageEx = pagina.FindName("imagenTilePequeña")
        imagenPequeña.Source = Nothing

        Dim imagenMediana As ImageEx = pagina.FindName("imagenTileMediana")
        imagenMediana.Source = Nothing

        Dim imagenAncha As ImageEx = pagina.FindName("imagenTileAncha")
        imagenAncha.Source = Nothing

        Dim imagenGrande As ImageEx = pagina.FindName("imagenTileGrande")
        imagenGrande.Source = Nothing

        If Not juego.ImagenPequeña = Nothing Then
            imagenPequeña.Source = juego.ImagenPequeña
            imagenPequeña.Tag = juego.ImagenPequeña
        End If

        If Not juego.ImagenMediana = Nothing Then
            imagenMediana.Source = juego.ImagenMediana
            imagenMediana.Tag = juego.ImagenMediana
        End If

        If Not juego.ImagenAncha = Nothing Then
            imagenAncha.Source = juego.ImagenAncha
            imagenAncha.Tag = juego.ImagenAncha
        End If

        If Not juego.ImagenGrande = Nothing Then
            imagenGrande.Source = juego.ImagenGrande
            imagenGrande.Tag = juego.ImagenGrande
        End If

    End Sub

End Module

Public Class GOGAPIJuego

    <JsonProperty("id")>
    Public ID As String

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("images")>
    Public Imagenes As GOGAPIJuegoImagenes

End Class

Public Class GOGAPIJuegoImagenes

    <JsonProperty("background")>
    Public Fondo As String

    <JsonProperty("icon")>
    Public Icono As String

    <JsonProperty("logo")>
    Public Logo As String

End Class
