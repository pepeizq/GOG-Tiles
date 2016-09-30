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

        tbDirectorioGOG.Text = recursos.GetString("Directorio")
        buttonAñadirCarpetaGOGTexto.Text = recursos.GetString("Boton Añadir")
        buttonVolverTilesTexto.Text = recursos.GetString("Boton Volver")
        gridCargandoTexto.Text = recursos.GetString("Cargando")
        checkboxTilesGOGTitulo.Content = recursos.GetString("Titulo Tile")
        buttonVotacionesTexto.Text = recursos.GetString("Boton Votar")
        buttonCompartirTexto.Text = recursos.GetString("Boton Compartir")
        buttonContactarTexto.Text = recursos.GetString("Boton Contactar")
        buttonWebTexto.Text = recursos.GetString("Boton Web")

        Listado.Generar(gridViewTilesGOG, buttonAñadirCarpetaGOG, gridCargando, scrollViewerGridGOG, False)

    End Sub

    'AÑADIRCARPETA-----------------------------------------------------------------------------

    Private Sub buttonAñadirCarpetaSteam_Click(sender As Object, e As RoutedEventArgs) Handles buttonAñadirCarpetaGOG.Click

        Listado.Generar(gridViewTilesGOG, buttonAñadirCarpetaGOG, gridCargando, scrollViewerGridGOG, True)

    End Sub

    Private Sub buttonAñadirCarpetaGOG_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonAñadirCarpetaGOG.PointerEntered

        buttonAñadirCarpetaGOG.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonAñadirCarpetaGOG.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonAñadirCarpetaGOG_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonAñadirCarpetaGOG.PointerExited

        buttonAñadirCarpetaGOG.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonAñadirCarpetaGOG.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    'VOLVER-----------------------------------------------------------------------------

    Private Sub buttonVolverTiles_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonVolverTiles.PointerEntered

        buttonVolverTiles.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonVolverTiles.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonVolverTiles_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonVolverTiles.PointerExited

        buttonVolverTiles.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonVolverTiles.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    Private Sub buttonVolverTiles_Click(sender As Object, e As RoutedEventArgs) Handles buttonVolverTiles.Click

        gridWebContacto.Visibility = Visibility.Collapsed
        gridTilesGOG.Visibility = Visibility.Visible

        buttonVolverTiles.Visibility = Visibility.Collapsed
        buttonAñadirCarpetaGOG.Visibility = Visibility.Visible

    End Sub

    'VOTAR-----------------------------------------------------------------------------

    Private Sub buttonVotaciones_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonVotaciones.PointerEntered

        buttonVotaciones.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonVotaciones.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonVotaciones_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonVotaciones.PointerExited

        buttonVotaciones.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonVotaciones.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    Private Async Sub buttonVotaciones_Click(sender As Object, e As RoutedEventArgs) Handles buttonVotaciones.Click

        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

    End Sub

    'COMPARTIR-----------------------------------------------------------------------------

    Private Sub buttonCompartir_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonCompartir.PointerEntered

        buttonCompartir.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonCompartir.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonCompartir_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonCompartir.PointerExited

        buttonCompartir.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonCompartir.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    Private Sub buttonCompartir_Click(sender As Object, e As RoutedEventArgs) Handles buttonCompartir.Click

        Dim datos As DataTransferManager = DataTransferManager.GetForCurrentView()
        AddHandler datos.DataRequested, AddressOf MainPage_DataRequested
        DataTransferManager.ShowShareUI()

    End Sub

    Private Sub MainPage_DataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)

        Dim request As DataRequest = e.Request
        request.Data.SetText("GOG Tiles")
        request.Data.Properties.Title = "GOG Tiles"
        request.Data.Properties.Description = "Add Tiles to your GOG Galaxy games in the Start Menu of Windows 10"

    End Sub

    'CONTACTAR-----------------------------------------------------------------------------

    Private Sub buttonContactar_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonContactar.PointerEntered

        buttonContactar.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonContactar.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonContactar_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonContactar.PointerExited

        buttonContactar.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonContactar.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    Private Sub buttonContactar_Click(sender As Object, e As RoutedEventArgs) Handles buttonContactar.Click

        gridWebContacto.Visibility = Visibility.Visible
        gridTilesGOG.Visibility = Visibility.Collapsed

        buttonVolverTiles.Visibility = Visibility.Visible
        buttonAñadirCarpetaGOG.Visibility = Visibility.Collapsed

    End Sub

    'WEB-----------------------------------------------------------------------------

    Private Sub buttonWeb_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles buttonWeb.PointerEntered

        buttonWeb.BorderBrush = New SolidColorBrush(Colors.Black)
        buttonWeb.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub buttonWeb_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles buttonWeb.PointerExited

        buttonWeb.BorderBrush = New SolidColorBrush(Colors.Transparent)
        buttonWeb.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

    Private Async Sub buttonWeb_Click(sender As Object, e As RoutedEventArgs) Handles buttonWeb.Click

        Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/"))

    End Sub

    'CBTITULOS-----------------------------------------------------------------------------

    Private Sub checkboxTilesGOGTitulo_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles checkboxTilesGOGTitulo.PointerEntered

        checkboxTilesGOGTitulo.BorderBrush = New SolidColorBrush(Colors.Black)
        checkboxTilesGOGTitulo.Background = New SolidColorBrush(Colors.LightGray)

    End Sub

    Private Sub checkboxTilesGOGTitulo_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles checkboxTilesGOGTitulo.PointerExited

        checkboxTilesGOGTitulo.BorderBrush = New SolidColorBrush(Colors.Transparent)
        checkboxTilesGOGTitulo.Background = New SolidColorBrush(Colors.Transparent)

    End Sub

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

End Class
