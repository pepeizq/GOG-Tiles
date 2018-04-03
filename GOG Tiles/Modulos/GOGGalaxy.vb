Imports System.Text.RegularExpressions
Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.UI
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Media.Animation

Module GOGGalaxy

    Dim clave As String = "carpetagog01"

    Public Async Sub Generar(boolBuscarCarpeta As Boolean)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAñadir As Button = pagina.FindName("botonAñadirCarpetaGOGGalaxy")
        botonAñadir.IsEnabled = False

        Dim botonBorrar As Button = pagina.FindName("botonBorrarCarpetasGOGGalaxy")
        botonBorrar.IsEnabled = False

        Dim pr As ProgressRing = pagina.FindName("prTilesGOG")
        pr.Visibility = Visibility.Visible

        Dim gv As GridView = pagina.FindName("gridViewTilesGOG")

        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasGOGGalaxy")

        If Not tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = ""
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

        Dim listaIDs As New List(Of String)

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
                        ".dll"
                    }

                    Dim opciones As New QueryOptions(CommonFileQuery.DefaultQuery, filtro)

                    Dim query As StorageFileQueryResult = carpetaJuego.CreateFileQueryWithOptions(opciones)
                    Dim ficheros As IReadOnlyList(Of StorageFile) = Await query.GetFilesAsync()

                    If Not ficheros.Count = 0 Then
                        For Each fichero As StorageFile In ficheros
                            If fichero.DisplayName.Contains("goggame-") And fichero.FileType.Contains(".dll") Then
                                Dim id As String = fichero.DisplayName.Replace("goggame-", Nothing)
                                listaIDs.Add(id)

                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
            i += 1
        End While

        Dim listaFinal As New List(Of Tile)

        If listaIDs.Count > 0 Then
            Dim query As String = "http://api.gog.com/products?ids="

            i = 0
            For Each id In listaIDs
                If i = 0 Then
                    query = query + id
                Else
                    query = query + "%2C" + id
                End If

                i += 1
            Next

            Dim html As String = Await Decompiladores.HttpClient(New Uri(query))

            If Not html = Nothing Then
                For Each id In listaIDs
                    Dim temp0 As String
                    Dim int0 As Integer

                    int0 = html.IndexOf(ChrW(34) + "id" + ChrW(34) + ":" + id)
                    temp0 = html.Remove(0, int0 + 6)

                    Dim temp, temp2 As String
                    Dim int, int2 As Integer

                    int = temp0.IndexOf(ChrW(34) + "title" + ChrW(34))
                    temp = temp0.Remove(0, int + 9)

                    int2 = temp.IndexOf(ChrW(34))
                    temp2 = temp.Remove(int2, temp.Length - int2)

                    temp2 = temp2.Trim
                    temp2 = Regex.Unescape(temp2)

                    Dim titulo As String = temp2.Trim

                    Dim temp3, temp4 As String
                    Dim int3, int4 As Integer

                    int3 = temp0.IndexOf(ChrW(34) + "logo2x" + ChrW(34))
                    temp3 = temp0.Remove(0, int3 + 10)

                    int4 = temp3.IndexOf(ChrW(34))
                    temp4 = temp3.Remove(int4, temp3.Length - int4)

                    temp4 = temp4.Replace("\/", "/")
                    temp4 = "https:" + temp4

                    Dim imagenAncha As String = temp4.Trim

                    Dim temp5, temp6 As String
                    Dim int5, int6 As Integer

                    int5 = temp0.IndexOf(ChrW(34) + "background" + ChrW(34))
                    temp5 = temp0.Remove(0, int5 + 14)

                    int6 = temp5.IndexOf(ChrW(34))
                    temp6 = temp5.Remove(int6, temp5.Length - int6)

                    temp6 = temp6.Replace("\/", "/")
                    temp6 = "https:" + temp6

                    Dim imagenGrande As String = temp6.Trim

                    Dim temp7, temp8 As String
                    Dim int7, int8 As Integer

                    int7 = temp0.IndexOf(ChrW(34) + "icon" + ChrW(34))
                    temp7 = temp0.Remove(0, int7 + 8)

                    int8 = temp7.IndexOf(ChrW(34))
                    temp8 = temp7.Remove(int8, temp7.Length - int8)

                    temp8 = temp8.Replace("\/", "/")
                    temp8 = "https:" + temp8

                    Dim imagenPequeña As String = temp8.Trim

                    Dim ejecutable As String = "goggalaxy://openGameView/" + id

                    Dim tituloBool As Boolean = False
                    Dim g As Integer = 0
                    While g < listaFinal.Count
                        If listaFinal(g).Titulo = titulo Then
                            tituloBool = True
                        End If
                        g += 1
                    End While

                    If tituloBool = False Then
                        Dim juego As New Tile(titulo, id, ejecutable, New Uri(imagenPequeña), New Uri(imagenPequeña), New Uri(imagenAncha), New Uri(imagenGrande))

                        listaFinal.Add(juego)
                    End If
                Next
            End If
        End If

        Dim panelAvisoNoJuegos As DropShadowPanel = pagina.FindName("panelAvisoNoJuegos")
        Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")

        If listaFinal.Count > 0 Then
            panelAvisoNoJuegos.Visibility = Visibility.Collapsed
            gridSeleccionar.Visibility = Visibility.Visible

            listaFinal.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

            gv.Items.Clear()

            For Each juego In listaFinal
                Dim boton As New Button

                Dim imagen As New ImageEx
                Dim boolImagen As Boolean = True

                Try
                    imagen.Source = New BitmapImage(juego.ImagenAncha)
                Catch ex As Exception
                    boolImagen = False
                End Try

                If boolImagen = True Then
                    imagen.IsCacheEnabled = True
                    imagen.Stretch = Stretch.UniformToFill
                    imagen.Padding = New Thickness(0, 0, 0, 0)

                    boton.Tag = juego
                    boton.Content = imagen
                    boton.Padding = New Thickness(0, 0, 0, 0)
                    boton.BorderThickness = New Thickness(1, 1, 1, 1)
                    boton.BorderBrush = New SolidColorBrush(Colors.Black)
                    boton.Background = New SolidColorBrush(Colors.Transparent)

                    Dim tbToolTip As TextBlock = New TextBlock With {
                        .Text = juego.Titulo,
                        .FontSize = 16
                    }

                    ToolTipService.SetToolTip(boton, tbToolTip)
                    ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

                    AddHandler boton.Click, AddressOf BotonTile_Click
                    AddHandler boton.PointerEntered, AddressOf UsuarioEntraBoton
                    AddHandler boton.PointerExited, AddressOf UsuarioSaleBoton

                    gv.Items.Add(boton)
                End If
            Next

            If boolBuscarCarpeta = True Then
                Toast(listaFinal.Count.ToString + " " + recursos.GetString("GamesDetected"), Nothing)
            End If
        Else
            panelAvisoNoJuegos.Visibility = Visibility.Visible
            gridSeleccionar.Visibility = Visibility.Collapsed
        End If

        botonAñadir.IsEnabled = True
        botonBorrar.IsEnabled = True
        pr.Visibility = Visibility.Collapsed

    End Sub

    Private Sub BotonTile_Click(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonJuego As Button = e.OriginalSource
        Dim juego As Tile = botonJuego.Tag

        Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
        botonAñadirTile.Tag = juego

        Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
        imagenJuegoSeleccionado.Source = New BitmapImage(juego.ImagenAncha)

        Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
        tbJuegoSeleccionado.Text = juego.Titulo

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
        imagenPequeña.Source = juego.ImagenPequeña
        imagenPequeña.Visibility = Visibility.Visible

        Dim tbPequeña As TextBlock = pagina.FindName("tbTilePequeña")
        tbPequeña.Visibility = Visibility.Collapsed

        '---------------------------------------------

        Dim imagenMediana As ImageEx = pagina.FindName("imagenTileMediana")
        imagenMediana.Source = juego.ImagenMediana
        imagenMediana.Visibility = Visibility.Visible

        Dim tbMediana As TextBlock = pagina.FindName("tbTileMediana")
        tbMediana.Visibility = Visibility.Collapsed

        '---------------------------------------------

        Dim imagenAncha As ImageEx = pagina.FindName("imagenTileAncha")
        imagenAncha.Source = juego.ImagenAncha
        imagenAncha.Visibility = Visibility.Visible

        Dim tbAncha As TextBlock = pagina.FindName("tbTileAncha")
        tbAncha.Visibility = Visibility.Collapsed

        '---------------------------------------------

        Dim imagenGrande As ImageEx = pagina.FindName("imagenTileGrande")
        imagenGrande.Source = juego.ImagenGrande
        imagenGrande.Visibility = Visibility.Visible

        Dim tbGrande As TextBlock = pagina.FindName("tbTileGrande")
        tbGrande.Visibility = Visibility.Collapsed

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim imagen As ImageEx = boton.Content

        imagen.Saturation(0).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim imagen As ImageEx = boton.Content

        imagen.Saturation(1).Start()

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
