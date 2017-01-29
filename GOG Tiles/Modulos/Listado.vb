Imports System.Text
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.Storage.Streams
Imports Windows.UI

Module Listado

    Dim clave As String = "carpetagog05"

    Public Async Sub Generar(boolBuscarCarpeta As Boolean)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim buttonAñadir As Button = pagina.FindName("buttonAñadirCarpeta")
        buttonAñadir.IsEnabled = False

        Dim buttonBorrar As Button = pagina.FindName("buttonBorrarCarpetas")
        buttonBorrar.IsEnabled = False

        Dim pr As ProgressRing = pagina.FindName("prTiles")
        pr.Visibility = Visibility.Visible

        Dim sv As ScrollViewer = pagina.FindName("scrollViewerGrid")
        Dim gridview As GridView = pagina.FindName("gridViewTiles")

        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadas")

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
                        While i < (carpetas.Values("numCarpetas") + 1)
                            Try
                                Dim carpetaTemp As StorageFolder = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                            Catch ex As Exception
                                StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave + i.ToString, carpeta)
                                carpetas.Values("numCarpetas") = i + 1
                                Exit While
                            End Try
                            i += 1
                        End While
                    End If
                End If

            Catch ex As Exception

            End Try
        End If

        While i < carpetas.Values("numCarpetas")
            Try
                Dim carpetaTemp As StorageFolder = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                tbCarpetas.Text = tbCarpetas.Text + carpetaTemp.Path + Environment.NewLine
            Catch ex As Exception

            End Try
            i += 1
        End While

        If tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = recursos.GetString("Ninguna")

            Dim gridTiles As Grid = pagina.FindName("gridTiles")
            gridTiles.Visibility = Visibility.Collapsed

            Dim gridConfig As Grid = pagina.FindName("gridConfig")
            gridConfig.Visibility = Visibility.Visible

            Dim gridConfigApp As Grid = pagina.FindName("gridConfigApp")
            gridConfigApp.Visibility = Visibility.Visible

            Dim buttonConfigApp As Button = pagina.FindName("buttonConfigApp")
            buttonConfigApp.Background = New SolidColorBrush(Microsoft.Toolkit.Uwp.ColorHelper.ToColor("#bfbfbf"))
            buttonConfigApp.BorderBrush = New SolidColorBrush(Colors.Black)
        Else
            tbCarpetas.Text = tbCarpetas.Text.Trim
        End If

        '-------------------------------------------------------------

        Dim listaFinal As List(Of Tiles) = New List(Of Tiles)

        i = 0
        If gridview.Items.Count > 0 Then
            While i < gridview.Items.Count
                Dim grid As Grid = gridview.Items(i)
                Dim tile As Tiles = grid.Tag

                Dim tituloBool As Boolean = False
                Dim g As Integer = 0
                While g < listaFinal.Count
                    If listaFinal(g).Titulo = tile.Titulo Then
                        tituloBool = True
                    End If
                    g += 1
                End While

                If tituloBool = False Then
                    listaFinal.Add(New Tiles(tile.Titulo, tile.ID, tile.Enlace, tile.Imagen, tile.ImagenUri))
                End If

                i += 1
            End While
        End If

        '-------------------------------------------------------------

        i = 0
        While i < carpetas.Values("numCarpetas") + 1
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

                            If Not texto = Nothing Then
                                Dim temp, temp2 As String
                                Dim int, int2 As Integer

                                int = texto.IndexOf("<Name>")
                                temp = texto.Remove(0, int + 6)

                                int2 = temp.IndexOf("</Name>")
                                temp2 = temp.Remove(int2, temp.Length - int2)

                                Dim titulo As String = temp2.Trim

                                Dim acceso As String = "goggalaxy://openGameView/" + id

                                Dim temp3, temp4, temp5, temp6 As String
                                Dim int3, int4, int5, int6 As Integer

                                int3 = texto.IndexOf("<Support>")
                                temp3 = texto.Remove(0, int3 + 6)

                                int4 = temp3.IndexOf("</Support>")
                                temp4 = temp3.Remove(int4, temp3.Length - int4)

                                int5 = temp4.IndexOf("<URLTask Link=")
                                temp5 = temp4.Remove(0, int5 + 15)

                                int6 = temp5.IndexOf(ChrW(34))
                                temp6 = temp5.Remove(int6, temp5.Length - int6)

                                temp6 = temp6.Replace("/en/support/", "/game/")

                                Dim html As String = Await Decompiladores.HttpClient(New Uri(temp6))

                                Dim temp7, temp8, temp9 As String
                                Dim int7, int8, int9 As Integer

                                int7 = html.IndexOf(ChrW(34) + "og:image" + ChrW(34))
                                temp7 = html.Remove(0, int7 + 5)

                                int8 = temp7.IndexOf("content=")
                                temp8 = temp7.Remove(0, int8 + 9)

                                int9 = temp8.IndexOf(ChrW(34))
                                temp9 = temp8.Remove(int9, temp8.Length - int9)

                                If Not temp9 = Nothing Then
                                    If Not temp9.Contains("http:") Then
                                        temp9 = "http:" + temp9
                                        temp9 = temp9.Replace(".jpg", "_340.jpg")
                                    End If

                                    Dim imagen As Uri = New Uri(temp9, UriKind.RelativeOrAbsolute)

                                    Dim bitmap As New BitmapImage
                                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache
                                    bitmap.UriSource = imagen

                                    Dim tituloBool As Boolean = False
                                    Dim g As Integer = 0
                                    While g < listaFinal.Count
                                        If listaFinal(g).Titulo = titulo Then
                                            tituloBool = True
                                        End If
                                        g += 1
                                    End While

                                    If tituloBool = False Then
                                        listaFinal.Add(New Tiles(titulo, id, New Uri(acceso), bitmap, imagen))
                                    End If
                                End If
                            End If
                        End If

                    Next
                Next
            End If
            i += 1
        End While

        If listaFinal.Count > 0 Then
            listaFinal.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))
            sv.Visibility = Visibility.Visible

            gridview.Items.Clear()

            For Each tile In listaFinal
                Dim grid As New Grid
                grid.Margin = New Thickness(1, 1, 1, 1)
                grid.Tag = tile
                grid.BorderBrush = New SolidColorBrush(Colors.Black)
                grid.BorderThickness = New Thickness(1, 1, 1, 1)
                grid.Width = 224
                grid.Height = 103

                Dim imagen As New ImageEx
                imagen.Source = New BitmapImage(tile.ImagenUri)
                imagen.Stretch = Stretch.UniformToFill

                grid.Children.Add(imagen)

                gridview.Items.Add(grid)
            Next

            If boolBuscarCarpeta = True Then
                Toast("GOG Tiles", listaFinal.Count.ToString + " " + recursos.GetString("Juegos Detectados"))
            End If
        Else
            If boolBuscarCarpeta = True Then
                Toast("GOG Tiles", recursos.GetString("Fallo1"))
            End If
        End If

        buttonAñadir.IsEnabled = True
        buttonBorrar.IsEnabled = True
        pr.Visibility = Visibility.Collapsed

    End Sub

    Public Sub Borrar()

        StorageApplicationPermissions.FutureAccessList.Clear()

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        Dim numCarpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings
        numCarpetas.Values("numCarpetas") = 0

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content
        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadas")

        tbCarpetas.Text = recursos.GetString("Ninguna")

        Generar(False)

    End Sub
End Module
