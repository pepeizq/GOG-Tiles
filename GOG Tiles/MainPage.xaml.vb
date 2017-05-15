﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Windows.ApplicationModel.Core
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Acrilico.Generar(gridTopAcrilico)
        Acrilico.Generar(gridMenuAcrilico)

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonPressedBackgroundColor = Colors.DarkOrchid
        barra.ButtonInactiveBackgroundColor = Colors.Transparent
        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        botonTilesTexto.Text = recursos.GetString("Tiles")
        botonConfigTexto.Text = recursos.GetString("Boton Config")
        botonVotarTexto.Text = recursos.GetString("Boton Votar")
        botonMasCosasTexto.Text = recursos.GetString("Boton Cosas")

        botonContactarTexto.Text = recursos.GetString("Boton Contactar")
        botonMasAppsTexto.Text = recursos.GetString("Boton Web")
        botonCodigoFuenteTexto.Text = recursos.GetString("Boton Codigo Fuente")

        tbNoJuegosGOG.Text = recursos.GetString("No Config")
        tbAvisoSeleccionar.Text = recursos.GetString("Seleccionar")

        botonAñadirTileTexto.Text = recursos.GetString("Añadir Tile")

        cbTilesTitulo.Content = recursos.GetString("Tile Titulo")
        cbTilesIconos.Content = recursos.GetString("Tile Logo")

        tbGOGGalaxyCarpetaInstrucciones.Text = recursos.GetString("GOGGalaxy Carpetas Añadir")
        buttonAñadirCarpetaGOGGalaxyTexto.Text = recursos.GetString("Boton Añadir")
        tbCarpetasAñadidasGOGGalaxy.Text = recursos.GetString("Carpetas Añadidas")
        tbCarpetaAvisoGOGGalaxy.Text = recursos.GetString("GOGGalaxy Carpetas Aviso")
        buttonBorrarCarpetasGOGGalaxyTexto.Text = recursos.GetString("Boton Borrar")

        '--------------------------------------------------------

        GridVisibilidad(gridTilesGOG, botonTiles, recursos.GetString("Tiles"))
        GOGGalaxy.Generar(False)
        Config.Generar()

    End Sub

    Private Sub GridVisibilidad(grid As Grid, boton As Button, seccion As String)

        tbTitulo.Text = "GOG Tiles (" + SystemInformation.ApplicationVersion.Major.ToString + "." + SystemInformation.ApplicationVersion.Minor.ToString + "." + SystemInformation.ApplicationVersion.Build.ToString + "." + SystemInformation.ApplicationVersion.Revision.ToString + ") - " + seccion

        gridTilesGOG.Visibility = Visibility.Collapsed
        gridConfig.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

        If gridTilesGOG.Visibility = Visibility.Visible Then
            If gridAñadirTiles.Visibility = Visibility.Collapsed Then
                If panelAvisoNoJuegosGOG.Visibility = Visibility.Collapsed Then
                    popupAvisoSeleccionar.IsOpen = True
                End If
            End If
        End If

        botonTiles.Background = New SolidColorBrush(Colors.Transparent)
        botonConfig.Background = New SolidColorBrush(Colors.Transparent)

        If Not boton Is Nothing Then
            boton.Background = New SolidColorBrush(Colors.MediumOrchid)
        End If

    End Sub

    Private Sub BotonTiles_Click(sender As Object, e As RoutedEventArgs) Handles botonTiles.Click

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        GridVisibilidad(gridTilesGOG, botonTiles, recursos.GetString("Tiles"))

    End Sub

    Private Sub BotonConfig_Click(sender As Object, e As RoutedEventArgs) Handles botonConfig.Click

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        GridVisibilidad(gridConfig, botonConfig, recursos.GetString("Boton Config"))
        GridConfigVisibilidad(gridConfigGOG, buttonConfigGOG)

    End Sub

    Private Async Sub BotonVotar_Click(sender As Object, e As RoutedEventArgs) Handles botonVotar.Click

        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

    End Sub

    Private Sub BotonMasCosas_Click(sender As Object, e As RoutedEventArgs) Handles botonMasCosas.Click

        If popupMasCosas.IsOpen = True Then
            botonMasCosas.Background = New SolidColorBrush(Colors.Transparent)
            popupMasCosas.IsOpen = False
        Else
            botonMasCosas.Background = New SolidColorBrush(Colors.MediumOrchid)
            popupMasCosas.IsOpen = True
        End If

    End Sub

    Private Async Sub BotonMasApps_Click(sender As Object, e As RoutedEventArgs) Handles botonMasApps.Click

        Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/"))

    End Sub

    Private Async Sub BotonContactar_Click(sender As Object, e As RoutedEventArgs) Handles botonContactar.Click

        Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/contact/"))

    End Sub

    Private Async Sub BotonCodigoFuente_Click(sender As Object, e As RoutedEventArgs) Handles botonCodigoFuente.Click

        Await Launcher.LaunchUriAsync(New Uri("https://github.com/pepeizq/GOG-Tiles"))

    End Sub

    'TILES-----------------------------------------------------------------------------

    Private Sub BotonAñadirTile_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirTile.Click

        Dim tile As Tile = botonAñadirTile.Tag
        Tiles.Generar(tile)

    End Sub

    Private Sub PopupAvisoSeleccionar_LayoutUpdated(sender As Object, e As Object) Handles popupAvisoSeleccionar.LayoutUpdated

        popupAvisoSeleccionar.Width = panelAvisoSeleccionar.ActualWidth
        popupAvisoSeleccionar.Height = panelAvisoSeleccionar.ActualHeight

    End Sub

    'CONFIG-----------------------------------------------------------------------------

    Private Sub GridConfigVisibilidad(grid As Grid, boton As Button)

        If popupAvisoSeleccionar.IsOpen = True Then
            popupAvisoSeleccionar.IsOpen = False
        End If

        buttonConfigGOG.Background = New SolidColorBrush(Colors.MediumOrchid)

        boton.Background = New SolidColorBrush(Colors.DarkOrchid)

        gridConfigGOG.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub ButtonConfigGOG_Click(sender As Object, e As RoutedEventArgs) Handles buttonConfigGOG.Click

        GridConfigVisibilidad(gridConfigGOG, buttonConfigGOG)

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

    'CONFIGGOG-----------------------------------------------------------------------------

    Private Sub ButtonAñadirCarpetaGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles buttonAñadirCarpetaGOGGalaxy.Click

        GOGGalaxy.Generar(True)

    End Sub

    Private Sub ButtonBorrarCarpetasGOGGalaxy_Click(sender As Object, e As RoutedEventArgs) Handles buttonBorrarCarpetasGOGGalaxy.Click

        GOGGalaxy.Borrar()

    End Sub

End Class
