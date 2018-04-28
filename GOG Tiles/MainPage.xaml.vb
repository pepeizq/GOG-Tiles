﻿Imports FontAwesome.UWP
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Tiles"), FontAwesomeIcon.Home, 0))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Config"), FontAwesomeIcon.Cog, 1))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If Not item Is Nothing Then
            If item.Text = recursos.GetString("Tiles") Then
                GridVisibilidad(gridTiles, item.Text)
            ElseIf item.Text = recursos.GetString("Config") Then
                GridVisibilidad(gridConfig, item.Text)
            End If
        End If

    End Sub

    Private Sub Nv_ItemFlyout(sender As NavigationViewItem, args As TappedRoutedEventArgs)

        FlyoutBase.ShowAttachedFlyout(sender)

    End Sub

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()

        Dim recursos As New Resources.ResourceLoader()

        GridVisibilidad(gridTiles, recursos.GetString("Tiles"))
        nvPrincipal.IsPaneOpen = False

        GOGGalaxy.Generar(False)
        Configuracion.Iniciar()

        '--------------------------------------------------------

        Dim transpariencia As New UISettings
        TransparienciaEfectosFinal(transpariencia.AdvancedEffectsEnabled)
        AddHandler transpariencia.AdvancedEffectsEnabledChanged, AddressOf TransparienciaEfectosCambia

    End Sub

    Private Sub TransparienciaEfectosCambia(sender As UISettings, e As Object)

        TransparienciaEfectosFinal(sender.AdvancedEffectsEnabled)

    End Sub

    Private Async Sub TransparienciaEfectosFinal(estado As Boolean)

        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If estado = True Then
                                                                       gridAñadirTile.Background = App.Current.Resources("GridAcrilico")
                                                                       gridConfig.Background = App.Current.Resources("GridAcrilico")
                                                                       gridConfigTiles.Background = App.Current.Resources("GridTituloBackground")
                                                                   Else
                                                                       gridAñadirTile.Background = New SolidColorBrush(Colors.LightGray)
                                                                       gridConfig.Background = New SolidColorBrush(Colors.LightGray)
                                                                       gridConfigTiles.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
                                                                   End If
                                                               End Sub)

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridAñadirTile.Visibility = Visibility.Collapsed
        gridConfig.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    'TILES-----------------------------------------------------------------------------

    Private Async Sub BotonAñadirTile_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirTile.Click

        Dim tile As Tile = botonAñadirTile.Tag

        Dim carpeta As StorageFolder = ApplicationData.Current.LocalFolder
        Dim fichero As StorageFile = Await carpeta.CreateFileAsync("Ejecutable.txt", CreationCollisionOption.ReplaceExisting)
        Await FileIO.WriteTextAsync(fichero, tile.Enlace.ToString)

        Dim instalacion As StorageFolder = Package.Current.InstalledLocation
        Dim carpetaLanzador As StorageFolder = Await instalacion.GetFolderAsync("Lanzador")

        Await fichero.MoveAsync(carpetaLanzador, "Ejecutable.txt", NameCollisionOption.ReplaceExisting)

        Tiles.Generar(tile)

    End Sub

    Private Async Sub UsuarioClickeaImagen(sender As Object, e As RoutedEventArgs)

        Dim ficheroPicker As New FileOpenPicker
        ficheroPicker.FileTypeFilter.Add(".png")
        ficheroPicker.ViewMode = PickerViewMode.List

        Dim ficheroImagen As StorageFile = Await ficheroPicker.PickSingleFileAsync

        Dim boton As Button = sender
        Dim grid As Grid = boton.Content

        Dim vb As Viewbox = Nothing
        Dim imagen As ImageEx = Nothing

        If TypeOf grid.Children(0) Is Viewbox Then
            vb = grid.Children(0)
            imagen = vb.Child
        End If

        If TypeOf grid.Children(0) Is ImageEx Then
            imagen = grid.Children(0)
        End If

        Dim tb As TextBlock = grid.Children(1)

        Dim bitmap As New BitmapImage

        Try
            imagen.Visibility = Visibility.Visible
            tb.Visibility = Visibility.Collapsed

            Dim stream As FileRandomAccessStream = Await ficheroImagen.OpenAsync(FileAccessMode.Read)
            bitmap.SetSource(stream)

            imagen.Source = bitmap
            imagen.Tag = ficheroImagen
        Catch ex As Exception
            imagen.Visibility = Visibility.Collapsed
            imagen.Source = Nothing
            tb.Visibility = Visibility.Visible
        End Try

    End Sub

    'CONFIGTILES-----------------------------------------------------------------------------

    Private Sub CbTilesTitulo_Checked(sender As Object, e As RoutedEventArgs) Handles cbTilesTitulo.Checked

        Configuracion.MostrarTitulo(True)

    End Sub

    Private Sub CbTilesTitulo_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbTilesTitulo.Unchecked

        Configuracion.MostrarTitulo(False)

    End Sub

    Private Sub CbTilesIconos_Checked(sender As Object, e As RoutedEventArgs) Handles cbTilesIconos.Checked

        Configuracion.MostrarDRM(True)

    End Sub

    Private Sub CbTilesIconos_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbTilesIconos.Unchecked

        Configuracion.MostrarDRM(False)

    End Sub

    'CONFIG-----------------------------------------------------------------------------

    Private Sub BotonAñadirCarpetaGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirCarpetaGOGGalaxy.Click

        GOGGalaxy.Generar(True)

    End Sub

    Private Sub BotonBorrarCarpetasGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles botonBorrarCarpetasGOGGalaxy.Click

        GOGGalaxy.Borrar()

    End Sub

End Class
