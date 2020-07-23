Imports System.Text.RegularExpressions
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.UI
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Media.Animation

Module GOGGalaxy

    Public anchoColumna As Integer = 375
    Dim clave As String = "carpetagog02"

    Public Async Sub Generar(boolBuscarCarpeta As Boolean)

        Dim helper As New LocalObjectStorageHelper

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAñadir As Button = pagina.FindName("botonAñadirCarpetaGOGGalaxy")
        botonAñadir.IsEnabled = False

        Dim botonBorrar As Button = pagina.FindName("botonBorrarCarpetasGOGGalaxy")
        botonBorrar.IsEnabled = False

        Dim spProgreso As StackPanel = pagina.FindName("spProgreso")
        spProgreso.Visibility = Visibility.Visible

        Dim pbProgreso As ProgressBar = pagina.FindName("pbProgreso")
        pbProgreso.Value = 0

        Dim tbProgreso As TextBlock = pagina.FindName("tbProgreso")
        tbProgreso.Text = String.Empty

        Dim botonCache As Button = pagina.FindName("botonConfigLimpiarCache")
        botonCache.IsEnabled = False

        Dim gridSeleccionarJuego As Grid = pagina.FindName("gridSeleccionarJuego")
        gridSeleccionarJuego.Visibility = Visibility.Collapsed

        Dim gv As GridView = pagina.FindName("gvTiles")
        gv.Items.Clear()

        Dim listaJuegos As New List(Of Tile)

        If Await helper.FileExistsAsync("juegos") = True Then
            listaJuegos = Await helper.ReadFileAsync(Of List(Of Tile))("juegos")
        End If

        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasGOGGalaxy")

        If Not tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = String.Empty
        End If

        Dim recursos As New Resources.ResourceLoader()
        Dim carpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim i As Integer = 0
        If boolBuscarCarpeta = True Then
            Try
                Dim picker As New FolderPicker()

                picker.FileTypeFilter.Add("*")
                picker.ViewMode = PickerViewMode.List

                Dim carpeta As StorageFolder = Await picker.PickSingleFolderAsync()

                If Not carpeta Is Nothing Then
                    Dim carpetasJuegos As IReadOnlyList(Of StorageFolder) = Await carpeta.GetFoldersAsync()
                    Dim detectadoBool As Boolean = False

                    For Each carpetaJuego As StorageFolder In carpetasJuegos
                        Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpetaJuego.GetFilesAsync()

                        For Each fichero As StorageFile In ficheros
                            If fichero.DisplayName.Contains("goggame-") Then
                                detectadoBool = True
                            End If
                        Next
                    Next

                    If detectadoBool = True Then
                        i = 0
                        While i < (carpetas.Values("numCarpetasGOG") + 1)
                            Try
                                Dim carpetaTemp As StorageFolder = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                            Catch ex As Exception
                                StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave + i.ToString, carpeta)
                                carpetas.Values("numCarpetasGOG") = i + 1
                                Exit While
                            End Try
                            i += 1
                        End While
                    End If
                End If

            Catch ex As Exception

            End Try
        End If

        While i < carpetas.Values("numCarpetasGOG")
            Try
                Dim carpetaTemp As StorageFolder = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                tbCarpetas.Text = tbCarpetas.Text + carpetaTemp.Path + Environment.NewLine
            Catch ex As Exception

            End Try
            i += 1
        End While

        If tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = recursos.GetString("NoFoldersDetected")
        Else
            tbCarpetas.Text = tbCarpetas.Text.Trim
        End If

        '-------------------------------------------------------------

        Dim listaTemporal As New List(Of Tile)

        i = 0
        While i < carpetas.Values("numCarpetasGOG") + 1
            Dim carpeta As StorageFolder = Nothing

            Try
                carpeta = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
            Catch ex As Exception

            End Try

            If Not carpeta Is Nothing Then
                Dim carpetasJuegos As IReadOnlyList(Of StorageFolder) = Await carpeta.GetFoldersAsync()

                For Each carpetaJuego As StorageFolder In carpetasJuegos
                    Dim filtro As New List(Of String) From {
                        ".dll", ".info"
                    }

                    Dim opciones As New QueryOptions(CommonFileQuery.DefaultQuery, filtro)

                    Dim query As StorageFileQueryResult = carpetaJuego.CreateFileQueryWithOptions(opciones)
                    Dim ficheros As IReadOnlyList(Of StorageFile) = Await query.GetFilesAsync()

                    If Not ficheros.Count = 0 Then
                        For Each fichero As StorageFile In ficheros
                            If fichero.DisplayName.Contains("goggame-") And fichero.FileType.Contains(".info") Then
                                Dim id As String = fichero.DisplayName.Replace("goggame-", Nothing)
                                Dim juego As New Tile(Nothing, id, carpetaJuego.Path, Nothing, Nothing, Nothing, Nothing)
                                listaTemporal.Add(juego)
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
            i += 1
        End While

        If listaTemporal.Count > 0 Then
            Dim query As String = "http://api.gog.com/products?ids="

            i = 0
            For Each temporal In listaTemporal
                Dim añadir As Boolean = True
                Dim g As Integer = 0
                While g < listaJuegos.Count
                    If listaJuegos(g).ID = temporal.ID Then
                        añadir = False
                    End If
                    g += 1
                End While

                If añadir = True Then
                    If query = "http://api.gog.com/products?ids=" Then
                        query = query + temporal.ID
                    Else
                        query = query + "%2C" + temporal.ID
                    End If
                End If

                i += 1
            Next

            If i > 0 Then
                Dim html As String = Await Decompiladores.HttpClient(New Uri(query))

                If Not html = Nothing Then
                    Dim listaJuegos2 As List(Of GOGAPIJuego) = JsonConvert.DeserializeObject(Of List(Of GOGAPIJuego))(html)

                    Dim k As Integer = 0
                    If Not listaJuegos2 Is Nothing Then
                        If listaJuegos2.Count > 0 Then
                            For Each juego2 In listaJuegos2
                                Dim titulo As String = juego2.Titulo

                                Dim id As String = juego2.ID

                                Dim fondo As String = Await Cache.DescargarImagen(juego2.Imagenes.Fondo, id, "fondo")

                                If Not fondo = String.Empty Then
                                    If Not fondo.Contains("https:") Then
                                        fondo = "https:" + fondo
                                    End If
                                End If

                                Dim icono As String = Await Cache.DescargarImagen(juego2.Imagenes.Icono, id, "icono")

                                If Not icono = String.Empty Then
                                    If Not icono.Contains("https:") Then
                                        icono = "https:" + icono
                                    End If
                                End If

                                Dim logo As String = Await Cache.DescargarImagen(juego2.Imagenes.Logo.Replace("_glx_logo", Nothing), id, "logo")

                                If Not logo = String.Empty Then
                                    If Not logo.Contains("https:") Then
                                        logo = "https:" + logo
                                    End If
                                End If

                                'Dim enlace As String = ChrW(34) + "C:\GOG Galaxy\GalaxyClient.exe" + ChrW(34) + " /gameId=" + temporal.ID + " /command=runGame /path=" + ChrW(34) + temporal.Enlace + ChrW(34)
                                Dim enlace As String = "goggalaxy://openGameView/" + id

                                Dim juego As New Tile(titulo, id, enlace, icono, icono, logo, fondo)
                                listaJuegos.Add(juego)

                                pbProgreso.Value = CInt((100 / listaJuegos2.Count) * k)
                                tbProgreso.Text = k.ToString + "/" + listaJuegos2.Count.ToString
                                k += 1
                            Next
                        End If
                    End If

                    'Dim k As Integer = 0
                    'For Each temporal In listaTemporal
                    '    Dim temp0 As String
                    '    Dim int0 As Integer

                    '    int0 = html.IndexOf(ChrW(34) + "id" + ChrW(34) + ":" + temporal.ID)

                    '    If Not int0 = -1 Then
                    '        temp0 = html.Remove(0, int0 + 6)

                    '        Dim temp, temp2 As String
                    '        Dim int, int2 As Integer

                    '        int = temp0.IndexOf(ChrW(34) + "title" + ChrW(34))
                    '        temp = temp0.Remove(0, int + 9)

                    '        int2 = temp.IndexOf(ChrW(34))
                    '        temp2 = temp.Remove(int2, temp.Length - int2)

                    '        temp2 = temp2.Trim
                    '        temp2 = Regex.Unescape(temp2)

                    '        Dim titulo As String = temp2.Trim

                    '        Dim temp3, temp4 As String
                    '        Dim int3, int4 As Integer

                    '        int3 = temp0.IndexOf(ChrW(34) + "logo2x" + ChrW(34))
                    '        temp3 = temp0.Remove(0, int3 + 10)

                    '        int4 = temp3.IndexOf(ChrW(34))
                    '        temp4 = temp3.Remove(int4, temp3.Length - int4)

                    '        temp4 = temp4.Replace("\/", "/")
                    '        temp4 = "https:" + temp4

                    '        Dim imagenAncha As String = temp4.Trim

                    '        imagenAncha = imagenAncha.Replace("_glx_logo_2x.jpg", "_product_tile_536.jpg")

                    '        Dim imagenGrande As String = imagenAncha

                    '        imagenGrande = imagenGrande.Replace("_glx_logo_2x.jpg", "_bg_crop_1366x655.jpg")

                    '        Dim temp7, temp8 As String
                    '        Dim int7, int8 As Integer

                    '        int7 = temp0.IndexOf(ChrW(34) + "icon" + ChrW(34))
                    '        temp7 = temp0.Remove(0, int7 + 8)

                    '        int8 = temp7.IndexOf(ChrW(34))
                    '        temp8 = temp7.Remove(int8, temp7.Length - int8)

                    '        temp8 = temp8.Replace("\/", "/")
                    '        temp8 = "https:" + temp8

                    '        Dim imagenPequeña As String = temp8.Trim

                    '        'Dim enlace As String = ChrW(34) + "C:\GOG Galaxy\GalaxyClient.exe" + ChrW(34) + " /gameId=" + temporal.ID + " /command=runGame /path=" + ChrW(34) + temporal.Enlace + ChrW(34)
                    '        Dim enlace As String = "goggalaxy://openGameView/" + temporal.ID

                    '        Dim juego As New Tile(titulo, temporal.ID, enlace, imagenPequeña, imagenPequeña, imagenAncha, imagenGrande)

                    '        listaJuegos.Add(juego)
                    '    End If

                    'pbProgreso.Value = CInt((100 / listaTemporal.Count) * k)
                    '    tbProgreso.Text = k.ToString + "/" + listaTemporal.Count.ToString
                    '    k += 1
                    'Next
                End If
            End If
        End If

        Await helper.SaveFileAsync(Of List(Of Tile))("juegos", listaJuegos)

        spProgreso.Visibility = Visibility.Collapsed

        Dim gridTiles As Grid = pagina.FindName("gridTiles")
        Dim gridAvisoNoJuegos As Grid = pagina.FindName("gridAvisoNoJuegos")
        Dim spBuscador As StackPanel = pagina.FindName("spBuscador")

        If Not listaJuegos Is Nothing Then
            If listaJuegos.Count > 0 Then
                gridTiles.Visibility = Visibility.Visible
                gridAvisoNoJuegos.Visibility = Visibility.Collapsed
                gridSeleccionarJuego.Visibility = Visibility.Visible
                spBuscador.Visibility = Visibility.Visible

                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                gv.Items.Clear()

                For Each juego In listaJuegos
                    BotonEstilo(juego, gv)
                Next

                'If boolBuscarCarpeta = True Then
                '    Toast(listaJuegos.Count.ToString + " " + recursos.GetString("GamesDetected"), Nothing)
                'End If
            Else
                gridTiles.Visibility = Visibility.Collapsed
                gridAvisoNoJuegos.Visibility = Visibility.Visible
                gridSeleccionarJuego.Visibility = Visibility.Collapsed
                spBuscador.Visibility = Visibility.Collapsed
            End If
        Else
            gridTiles.Visibility = Visibility.Collapsed
            gridAvisoNoJuegos.Visibility = Visibility.Visible
            gridSeleccionarJuego.Visibility = Visibility.Collapsed
            spBuscador.Visibility = Visibility.Collapsed
        End If

        botonAñadir.IsEnabled = True
        botonBorrar.IsEnabled = True
        botonCache.IsEnabled = True

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

        Dim grid As New Grid

        Dim imagenFondo As New ImageEx With {
            .Source = juego.ImagenAncha,
            .IsCacheEnabled = True,
            .Stretch = Stretch.UniformToFill,
            .Padding = New Thickness(0, 0, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center
        }

        grid.Children.Add(imagenFondo)

        boton.Tag = juego
        boton.Content = grid
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
        AddHandler boton.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler boton.PointerExited, AddressOf UsuarioSaleBoton

        gv.Items.Add(panel)

    End Sub

    Private Sub BotonTile_Click(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim spBuscador As StackPanel = pagina.FindName("spBuscador")
        spBuscador.Visibility = Visibility.Collapsed

        Dim botonJuego As Button = e.OriginalSource
        Dim juego As Tile = botonJuego.Tag

        Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
        botonAñadirTile.Tag = juego

        Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
        imagenJuegoSeleccionado.Source = New BitmapImage(New Uri(juego.ImagenAncha))

        Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
        tbJuegoSeleccionado.Text = juego.Titulo

        Dim gridSeleccionarJuego As Grid = pagina.FindName("gridSeleccionarJuego")
        gridSeleccionarJuego.Visibility = Visibility.Collapsed

        Dim gvTiles As GridView = pagina.FindName("gvTiles")

        If gvTiles.ActualWidth > anchoColumna Then
            ApplicationData.Current.LocalSettings.Values("ancho_grid_tiles") = gvTiles.ActualWidth
        End If

        gvTiles.Width = anchoColumna
        gvTiles.Padding = New Thickness(0, 0, 15, 0)

        Dim gridAñadir As Grid = pagina.FindName("gridAñadirTile")
        gridAñadir.Visibility = Visibility.Visible

        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("tile", botonJuego)

        Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("tile")

        If Not animacion Is Nothing Then
            animacion.TryStart(gridAñadir)
        End If

        Dim tbTitulo As TextBlock = pagina.FindName("tbTitulo")
        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + juego.Titulo

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

            imagenGrande.Source = juego.ImagenAncha
            imagenGrande.Tag = juego.ImagenAncha
        End If

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gvTiles As AdaptiveGridView = pagina.FindName("gvTiles")

        Dim boton As Button = sender

        boton.Saturation(0).Scale(1.05, 1.05, gvTiles.DesiredWidth / 2, gvTiles.ItemHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gvTiles As AdaptiveGridView = pagina.FindName("gvTiles")

        Dim boton As Button = sender

        boton.Saturation(1).Scale(1, 1, gvTiles.DesiredWidth / 2, gvTiles.ItemHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    Public Sub Borrar()

        StorageApplicationPermissions.FutureAccessList.Clear()

        Dim recursos As New Resources.ResourceLoader()
        Dim numCarpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings
        numCarpetas.Values("numCarpetasGOG") = 0

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content
        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasGOGGalaxy")

        tbCarpetas.Text = recursos.GetString("NoFoldersDetected")

        Generar(False)

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
