﻿Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.ApplicationModel.Core
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Tiles"), New SymbolIcon(Symbol.Home), 0))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Config"), New SymbolIcon(Symbol.Setting), 1))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("MoreThings"), New SymbolIcon(Symbol.More), 2))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If item.Text = recursos.GetString("Tiles") Then
            GridVisibilidad(gridTiles, item.Text)
        ElseIf item.Text = recursos.GetString("Config") Then
            GridVisibilidad(gridConfig, item.Text)
        ElseIf item.Text = recursos.GetString("MoreThings") Then
            GridVisibilidad(gridMasCosas, item.Text)

            Dim sv As ScrollViewer = gridMasCosas.Children(0)
            Dim gridRelleno As Grid = sv.Content
            Dim sp As StackPanel = gridRelleno.Children(0)
            Dim lv As ListView = sp.Children(0)

            MasCosas.Navegar(lv, "2", "https://pepeizqapps.com/")
        End If

    End Sub

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonInactiveBackgroundColor = Colors.Transparent

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        GridVisibilidad(gridTiles, recursos.GetString("Tiles"))
        nvPrincipal.IsPaneOpen = False

        GOGGalaxy.Generar(False)
        Config.Generar()
        MasCosas.Generar()

        '--------------------------------------------------------

        AddHandler botonAñadirTile.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonAñadirTile.PointerExited, AddressOf UsuarioSaleBoton
        AddHandler cbTilesTitulo.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler cbTilesTitulo.PointerExited, AddressOf UsuarioSaleBoton
        AddHandler cbTilesIconos.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler cbTilesIconos.PointerExited, AddressOf UsuarioSaleBoton

        AddHandler botonImagenTilePequeña.Click, AddressOf UsuarioClickeaImagen
        AddHandler botonImagenTilePequeña.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonImagenTilePequeña.PointerExited, AddressOf UsuarioSaleBoton

        AddHandler botonImagenTileMediana.Click, AddressOf UsuarioClickeaImagen
        AddHandler botonImagenTileMediana.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonImagenTileMediana.PointerExited, AddressOf UsuarioSaleBoton

        AddHandler botonImagenTileAncha.Click, AddressOf UsuarioClickeaImagen
        AddHandler botonImagenTileAncha.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonImagenTileAncha.PointerExited, AddressOf UsuarioSaleBoton

        AddHandler botonImagenTileGrande.Click, AddressOf UsuarioClickeaImagen
        AddHandler botonImagenTileGrande.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonImagenTileGrande.PointerExited, AddressOf UsuarioSaleBoton

        AddHandler botonAñadirCarpetaGOGGalaxy.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonAñadirCarpetaGOGGalaxy.PointerExited, AddressOf UsuarioSaleBoton
        AddHandler botonBorrarCarpetasGOGGalaxy.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler botonBorrarCarpetasGOGGalaxy.PointerExited, AddressOf UsuarioSaleBoton

        '--------------------------------------------------------

        Dim transpariencia As New UISettings
        TransparienciaEfectosFinal(transpariencia.AdvancedEffectsEnabled)
        AddHandler transpariencia.AdvancedEffectsEnabledChanged, AddressOf TransparienciaEfectosCambia

    End Sub

    Private Sub TransparienciaEfectosCambia(sender As UISettings, e As Object)

        TransparienciaEfectosFinal(sender.AdvancedEffectsEnabled)

    End Sub

    Private Async Sub TransparienciaEfectosFinal(estado As Boolean)

        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     If estado = True Then
                                                                         gridAñadirTile.Background = App.Current.Resources("GridAcrilico")
                                                                         gridConfig.Background = App.Current.Resources("GridAcrilico")
                                                                         gridConfigTiles.Background = App.Current.Resources("GridTituloBackground")
                                                                         gridMasCosas.Background = App.Current.Resources("GridAcrilico")
                                                                     Else
                                                                         gridAñadirTile.Background = New SolidColorBrush(Colors.LightGray)
                                                                         gridConfig.Background = New SolidColorBrush(Colors.LightGray)
                                                                         gridConfigTiles.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
                                                                         gridMasCosas.Background = New SolidColorBrush(Colors.LightGray)
                                                                     End If
                                                                 End Sub)

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridAñadirTile.Visibility = Visibility.Collapsed
        gridConfig.Visibility = Visibility.Collapsed
        gridMasCosas.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    'TILES-----------------------------------------------------------------------------

    Private Sub BotonAñadirTile_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirTile.Click

        Dim tile As Tile = botonAñadirTile.Tag
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

        ApplicationData.Current.LocalSettings.Values("titulotile") = "on"
        Config.Generar()

    End Sub

    Private Sub CbTilesTitulo_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbTilesTitulo.Unchecked

        ApplicationData.Current.LocalSettings.Values("titulotile") = "off"
        Config.Generar()

    End Sub

    Private Sub CbTilesIconos_Checked(sender As Object, e As RoutedEventArgs) Handles cbTilesIconos.Checked

        ApplicationData.Current.LocalSettings.Values("logotile") = "on"
        Config.Generar()

    End Sub

    Private Sub CbTilesIconos_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbTilesIconos.Unchecked

        ApplicationData.Current.LocalSettings.Values("logotile") = "off"
        Config.Generar()

    End Sub

    'CONFIG-----------------------------------------------------------------------------

    Private Sub BotonAñadirCarpetaGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirCarpetaGOGGalaxy.Click

        GOGGalaxy.Generar(True)

    End Sub

    Private Sub BotonBorrarCarpetasGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles botonBorrarCarpetasGOGGalaxy.Click

        GOGGalaxy.Borrar()

    End Sub

End Class
