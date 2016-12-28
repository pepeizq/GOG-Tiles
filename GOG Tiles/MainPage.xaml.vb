Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI
Imports Windows.UI.StartScreen

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Page_Loading(sender As FrameworkElement, args As Object)

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        tbConfig.Text = recursos.GetString("Boton Config")
        tbGOGConfigInstrucciones.Text = recursos.GetString("Directorio")
        buttonGOGConfigPathTexto.Text = recursos.GetString("Boton Añadir")
        tbGOGConfigPath.Text = recursos.GetString("Carpeta No")
        tbCarpetaAvisoGOG.Text = recursos.GetString("Carpeta Aviso")

        checkboxTilesGOGTitulo.Content = recursos.GetString("Titulo Tile")

        menuItemConfig.Label = recursos.GetString("Boton Config")
        menuItemVote.Label = recursos.GetString("Boton Votar")
        menuItemShare.Label = recursos.GetString("Boton Compartir")
        menuItemContact.Label = recursos.GetString("Boton Contactar")
        menuItemWeb.Label = recursos.GetString("Boton Web")

        Listado.Generar(gridViewTilesGOG, buttonGOGConfigPath, prTilesGOG, scrollViewerGridGOG, False)

    End Sub

    'AÑADIRCARPETA-----------------------------------------------------------------------------

    Private Sub buttonGOGConfigPath_Click(sender As Object, e As RoutedEventArgs) Handles buttonGOGConfigPath.Click

        Listado.Generar(gridViewTilesGOG, buttonGOGConfigPath, prTilesGOG, scrollViewerGridGOG, True)

    End Sub

    '-----------------------------------------------------------------------------

    Private Sub checkboxTilesGOGTitulo_Checked(sender As Object, e As RoutedEventArgs) Handles checkboxTilesGOGTitulo.Checked

        ApplicationData.Current.LocalSettings.Values("titulotilegog") = "on"

    End Sub

    Private Sub checkboxTilesGOGTitulo_Unchecked(sender As Object, e As RoutedEventArgs) Handles checkboxTilesGOGTitulo.Unchecked

        ApplicationData.Current.LocalSettings.Values("titulotilegog") = "off"

    End Sub

    '-----------------------------------------------------------------------------

    Private Async Sub gridviewTiles_ItemClick(sender As Object, e As ItemClickEventArgs) Handles gridViewTilesGOG.ItemClick

        Dim tile As Tiles = e.ClickedItem

        Dim ficheroImagen As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync("headergog.png", CreationCollisionOption.GenerateUniqueName)
        Dim downloader As BackgroundDownloader = New BackgroundDownloader()
        Dim descarga As DownloadOperation = downloader.CreateDownload(tile.imagenUri, ficheroImagen)
        Await descarga.StartAsync

        Dim nuevaTile As SecondaryTile = New SecondaryTile(tile.id, tile.titulo, tile.enlace.AbsolutePath, New Uri("ms-appdata:///local/" + ficheroImagen.Name, UriKind.RelativeOrAbsolute), TileSize.Wide310x150)

        Dim frame As FrameworkElement = TryCast(sender, FrameworkElement)
        Dim button As GeneralTransform = frame.TransformToVisual(Nothing)
        Dim point As Point = button.TransformPoint(New Point())
        Dim rect As Rect = New Rect(point, New Size(frame.ActualWidth, frame.ActualHeight))

        nuevaTile.RoamingEnabled = False
        nuevaTile.VisualElements.Wide310x150Logo = New Uri("ms-appdata:///local/" + ficheroImagen.Name, UriKind.RelativeOrAbsolute)

        If ApplicationData.Current.LocalSettings.Values("titulotilegog") = "on" Then
            nuevaTile.VisualElements.ShowNameOnWide310x150Logo = True
        End If

        Await nuevaTile.RequestCreateForSelectionAsync(rect)

    End Sub

    '-----------------------------------------------------------------------------

    Private Sub GridVisibilidad(grid As Grid)

        gridTilesGOG.Visibility = Visibility.Collapsed
        gridConfig.Visibility = Visibility.Collapsed
        gridWebContacto.Visibility = Visibility.Collapsed
        gridWeb.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub hamburgerMaestro_ItemClick(sender As Object, e As ItemClickEventArgs) Handles hamburgerMaestro.ItemClick

        Dim menuItem As HamburgerMenuGlyphItem = TryCast(e.ClickedItem, HamburgerMenuGlyphItem)

        If menuItem.Tag = 1 Then
            GridVisibilidad(gridTilesGOG)
        End If

    End Sub

    Private Async Sub hamburgerMaestro_OptionsItemClick(sender As Object, e As ItemClickEventArgs) Handles hamburgerMaestro.OptionsItemClick

        Dim menuItem As HamburgerMenuGlyphItem = TryCast(e.ClickedItem, HamburgerMenuGlyphItem)

        If menuItem.Tag = 99 Then
            GridVisibilidad(gridConfig)
        ElseIf menuItem.Tag = 100 Then
            Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))
        ElseIf menuItem.Tag = 101 Then
            Dim datos As DataTransferManager = DataTransferManager.GetForCurrentView()
            AddHandler datos.DataRequested, AddressOf MainPage_DataRequested
            DataTransferManager.ShowShareUI()
        ElseIf menuItem.Tag = 102 Then
            GridVisibilidad(gridWebContacto)
        ElseIf menuItem.Tag = 103 Then
            GridVisibilidad(gridWeb)
        End If

    End Sub

    Private Sub MainPage_DataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)

        Dim request As DataRequest = e.Request
        request.Data.SetText("GOG Tiles")
        request.Data.Properties.Title = "GOG Tiles"
        request.Data.Properties.Description = "Add Tiles to your GOG Galaxy games in the Start Menu of Windows 10"

    End Sub

End Class
