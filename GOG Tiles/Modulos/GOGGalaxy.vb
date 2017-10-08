Imports System.Text
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.Storage.Streams
Imports Windows.UI
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

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        Dim carpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim i As Integer = 0
        If boolBuscarCarpeta = True Then
            Try
                Dim picker As FolderPicker = New FolderPicker()

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

        Dim listaFinal As List(Of Tile) = New List(Of Tile)

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
                    Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpetaJuego.GetFilesAsync(CommonFileQuery.OrderByName)

                    For Each fichero As StorageFile In ficheros
                        If fichero.DisplayName.Contains("goggame-") And fichero.FileType.Contains(".dll") Then
                            Dim id As String = fichero.DisplayName.Replace("goggame-", Nothing)
                            Dim buffer As IBuffer = Await FileIO.ReadBufferAsync(fichero)
                            Dim lector As DataReader = DataReader.FromBuffer(buffer)
                            Dim contenido(lector.UnconsumedBufferLength - 1) As Byte
                            lector.ReadBytes(contenido)
                            Dim texto As String = Encoding.UTF8.GetString(contenido, 0, contenido.Length)

                            Dim temp, temp2 As String
                            Dim int, int2 As Integer

                            int = texto.IndexOf("<Name>")
                            temp = texto.Remove(0, int + 6)

                            int2 = temp.IndexOf("</Name>")
                            temp2 = temp.Remove(int2, temp.Length - int2)

                            Dim titulo As String = temp2.Trim

                            Dim html As String = Await Decompiladores.HttpClient(New Uri("http://api.gog.com/products/" + id + "?expand=description"))

                            If Not html = Nothing Then
                                Dim temp3, temp4 As String
                                Dim int3, int4 As Integer

                                int3 = html.IndexOf(ChrW(34) + "logo2x" + ChrW(34))
                                temp3 = html.Remove(0, int3 + 10)

                                int4 = temp3.IndexOf(ChrW(34))
                                temp4 = temp3.Remove(int4, temp3.Length - int4)

                                temp4 = temp4.Replace("\/", "/")
                                temp4 = "https:" + temp4
                                temp4 = temp4.Replace("_glx_logo_2x", "_196")

                                Dim imagenWide As String = temp4.Trim

                                Dim temp5, temp6 As String
                                Dim int5, int6 As Integer

                                int5 = html.IndexOf(ChrW(34) + "background" + ChrW(34))
                                temp5 = html.Remove(0, int5 + 14)

                                int6 = temp5.IndexOf(ChrW(34))
                                temp6 = temp5.Remove(int6, temp5.Length - int6)

                                temp6 = temp6.Replace("\/", "/")
                                temp6 = "https:" + temp6

                                Dim imagenLarge As String = temp6.Trim

                                Dim temp7, temp8 As String
                                Dim int7, int8 As Integer

                                int7 = html.IndexOf(ChrW(34) + "icon" + ChrW(34))
                                temp7 = html.Remove(0, int7 + 8)

                                int8 = temp7.IndexOf(ChrW(34))
                                temp8 = temp7.Remove(int8, temp7.Length - int8)

                                temp8 = temp8.Replace("\/", "/")
                                temp8 = "https:" + temp8

                                Dim imagenSmall As String = temp8.Trim

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
                                    Dim juego As New Tile(titulo, id, New Uri(ejecutable), New Uri(imagenWide), New Uri(imagenLarge), New Uri(imagenSmall), "GOG Galaxy", Nothing)
                                    juego.Tile = juego

                                    listaFinal.Add(juego)
                                End If
                            End If
                        End If
                    Next
                Next
            End If
            i += 1
        End While

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
                    imagen.Source = New BitmapImage(juego.ImagenWide)
                Catch ex As Exception
                    boolImagen = False
                End Try

                If boolImagen = True Then
                    imagen.IsCacheEnabled = True
                    imagen.Stretch = Stretch.Uniform
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

        Dim tbTitulo As TextBlock = pagina.FindName("tbTitulo")

        Dim gv As GridView = pagina.FindName("gridViewTilesGOG")

        Dim botonJuego As Button = e.OriginalSource

        Dim borde As Thickness = New Thickness(6, 6, 6, 6)
        If botonJuego.BorderThickness = borde Then
            botonJuego.BorderThickness = New Thickness(1, 1, 1, 1)
            botonJuego.BorderBrush = New SolidColorBrush(Colors.Black)

            Dim gridAñadir As Grid = pagina.FindName("gridAñadirTiles")
            gridAñadir.Visibility = Visibility.Collapsed

            Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")
            gridSeleccionar.Visibility = Visibility.Visible

            Dim recursos As New Resources.ResourceLoader()
            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + recursos.GetString("Tiles")
        Else
            For Each item In gv.Items
                Dim itemBoton As Button = item
                itemBoton.BorderThickness = New Thickness(1, 1, 1, 1)
                itemBoton.BorderBrush = New SolidColorBrush(Colors.Black)
            Next

            botonJuego.BorderThickness = New Thickness(6, 6, 6, 6)
            botonJuego.BorderBrush = New SolidColorBrush(App.Current.Resources("ColorSecundario"))

            Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
            Dim juego As Tile = botonJuego.Tag
            botonAñadirTile.Tag = juego

            Dim imageJuegoSeleccionado As ImageEx = pagina.FindName("imageJuegoSeleccionado")
            Dim imagenCapsula As String = juego.ImagenWide.ToString

            imageJuegoSeleccionado.Source = New BitmapImage(New Uri(imagenCapsula))

            Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
            tbJuegoSeleccionado.Text = juego.Titulo

            Dim gridAñadir As Grid = pagina.FindName("gridAñadirTiles")
            gridAñadir.Visibility = Visibility.Visible

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("tile", botonJuego)

            Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("tile")

            If Not animacion Is Nothing Then
                animacion.TryStart(gridAñadir)
            End If

            Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")
            gridSeleccionar.Visibility = Visibility.Collapsed

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + juego.Titulo
        End If

    End Sub

    Public Sub Borrar()

        StorageApplicationPermissions.FutureAccessList.Clear()

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        Dim numCarpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings
        numCarpetas.Values("numCarpetasGOG") = 0

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content
        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasGOGGalaxy")

        tbCarpetas.Text = recursos.GetString("NoFoldersDetected")

        Generar(False)

    End Sub

End Module
