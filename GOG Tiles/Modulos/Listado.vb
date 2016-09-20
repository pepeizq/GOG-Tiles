Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers

Module Listado

    Dim clave As String = "carpetagog03"

    Public Async Sub Generar(gridview As GridView, button As Button, pr As ProgressRing, sv As ScrollViewer, boolBuscarCarpeta As Boolean)

        button.IsEnabled = False
        pr.IsActive = True

        Dim carpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim i As Integer = 0
        If boolBuscarCarpeta = True Then
            Try
                Dim picker As FolderPicker = New FolderPicker()

                picker.FileTypeFilter.Add("*")
                picker.ViewMode = PickerViewMode.List

                Dim carpeta As StorageFolder = Await picker.PickSingleFolderAsync()
                Dim carpetaTemp As StorageFolder = Nothing

                i = 0
                While i < (carpetas.Values("numCarpetas") + 1)

                    Try
                        carpetaTemp = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                    Catch ex As Exception
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave + i.ToString, carpeta)
                        carpetas.Values("numCarpetas") = i + 1
                        Exit While
                    End Try

                    i += 1
                End While

            Catch ex As Exception

            End Try
        End If

        '-------------------------------------------------------------

        Dim listaFinal As List(Of Tiles) = New List(Of Tiles)

        i = 0
        If gridview.Items.Count > 0 Then
            While i < gridview.Items.Count
                Dim tile As Tiles = gridview.Items(i)

                Dim tituloBool As Boolean = False
                Dim g As Integer = 0
                While g < listaFinal.Count
                    If listaFinal(g).titulo = tile.titulo Then
                        tituloBool = True
                    End If
                    g += 1
                End While

                If tituloBool = False Then
                    listaFinal.Add(New Tiles(tile.titulo, tile.id, tile.enlace, tile.imagen, tile.imagenUri))
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
                    Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpetaJuego.GetFilesAsync()

                    For Each fichero As StorageFile In ficheros
                        If fichero.DisplayName.Contains("goggame-") And fichero.FileType.Contains(".info") Then
                            Dim texto As String = Await FileIO.ReadTextAsync(fichero)

                            If Not texto = Nothing Then
                                Dim temp, temp2, temp3 As String
                                Dim int, int2, int3 As Integer

                                int = texto.IndexOf(ChrW(34) + "gameId" + ChrW(34))
                                temp = texto.Remove(0, int + 8)

                                int2 = temp.IndexOf(ChrW(34))
                                temp2 = temp.Remove(0, int2 + 1)

                                int3 = temp2.IndexOf(ChrW(34))
                                temp3 = temp2.Remove(int3, temp2.Length - int3)

                                Dim id As String = temp3.Trim

                                '---------------------------

                                Dim temp4, temp5, temp6 As String
                                Dim int4, int5, int6 As Integer

                                int4 = texto.IndexOf(ChrW(34) + "name" + ChrW(34))
                                temp4 = texto.Remove(0, int4 + 6)

                                int5 = temp4.IndexOf(ChrW(34))
                                temp5 = temp4.Remove(0, int5 + 1)

                                int6 = temp5.IndexOf(ChrW(34))
                                temp6 = temp5.Remove(int6, temp5.Length - int6)

                                Dim titulo As String = temp6.Trim

                                '---------------------------

                                'Dim temp7, temp8, temp9 As String
                                'Dim int7, int8, int9 As Integer

                                'int7 = texto.IndexOf(ChrW(34) + "path" + ChrW(34))
                                'temp7 = texto.Remove(0, int7 + 6)

                                'int8 = temp7.IndexOf(ChrW(34))
                                'temp8 = temp7.Remove(0, int8 + 1)

                                'int9 = temp8.IndexOf(ChrW(34))
                                'temp9 = temp8.Remove(int9, temp8.Length - int9)

                                Dim acceso As String = Nothing

                                'If Not texto.Contains(ChrW(34) + "arguments" + ChrW(34)) Then
                                '    acceso = carpeta.Path + "\" + temp9.Trim
                                'Else
                                '    Dim temp10, temp11, temp12 As String
                                '    Dim int10, int11, int12 As Integer

                                '    int10 = texto.IndexOf(ChrW(34) + "arguments" + ChrW(34))
                                '    temp10 = texto.Remove(0, int10 + 9)

                                '    int11 = temp10.IndexOf(ChrW(34))
                                '    temp11 = temp10.Remove(0, int11 + 1)

                                '    int12 = temp11.IndexOf("}")
                                '    temp12 = temp11.Remove(int12, temp11.Length - int12)

                                '    temp12 = temp12.Trim
                                '    temp12 = temp12.Remove(temp12.Length - 1, 1)

                                '    acceso = carpeta.Path + "\" + temp9.Trim + temp12.Trim
                                'End If

                                'If Not acceso = Nothing Then
                                '    acceso = acceso.Replace("\\", "\")
                                'End If

                                acceso = "goggalaxy://openGameView/" + id

                                '---------------------------

                                Dim temp14, temp15, temp16 As String
                                Dim int14, int15, int16 As Integer

                                int14 = texto.IndexOf(ChrW(34) + "link" + ChrW(34))
                                temp14 = texto.Remove(0, int14 + 6)

                                int15 = temp14.IndexOf(ChrW(34))
                                temp15 = temp14.Remove(0, int15 + 1)

                                int16 = temp15.IndexOf(ChrW(34))
                                temp16 = temp15.Remove(int16, temp15.Length - int16)

                                temp16 = temp16.Replace("/en/support/", "/game/")

                                Dim html As String = Await Decompiladores.HttpClient(New Uri(temp16))

                                Dim temp17, temp18, temp19 As String
                                Dim int17, int18, int19 As Integer

                                int17 = html.IndexOf(ChrW(34) + "og:image" + ChrW(34))
                                temp17 = html.Remove(0, int17 + 5)

                                int18 = temp17.IndexOf("content=")
                                temp18 = temp17.Remove(0, int18 + 9)

                                int19 = temp18.IndexOf(ChrW(34))
                                temp19 = temp18.Remove(int19, temp18.Length - int19)

                                If Not temp19 = Nothing Then
                                    If Not temp19.Contains("http:") Then
                                        temp19 = "http:" + temp19
                                        temp19 = temp19.Replace(".jpg", "_340.jpg")
                                    End If

                                    Dim imagen As Uri = New Uri(temp19, UriKind.RelativeOrAbsolute)

                                    Dim bitmap As New BitmapImage
                                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache
                                    bitmap.UriSource = imagen

                                    Dim tituloBool As Boolean = False
                                    Dim g As Integer = 0
                                    While g < listaFinal.Count
                                        If listaFinal(g).titulo = titulo Then
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
            listaFinal.Sort(Function(x, y) x.titulo.CompareTo(y.titulo))
            sv.Visibility = Visibility.Visible
        Else
            If boolBuscarCarpeta = True Then
                Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
                MessageBox.EnseñarMensaje(recursos.GetString("Fallo1"))
            End If
        End If

        gridview.ItemsSource = listaFinal

        button.IsEnabled = True
        pr.IsActive = False

    End Sub
End Module
