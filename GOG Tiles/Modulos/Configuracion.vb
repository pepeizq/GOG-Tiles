Module Configuracion

    Public Sub Cargar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAbrirConfig As Button = pagina.FindName("botonAbrirConfig")

        AddHandler botonAbrirConfig.Click, AddressOf AbrirConfigClick
        AddHandler botonAbrirConfig.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonAbrirConfig.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim botonConfigImagen As Button = pagina.FindName("botonConfigImagen")

        AddHandler botonConfigImagen.Click, AddressOf AbrirImagenClick
        AddHandler botonConfigImagen.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonConfigImagen.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim botonBuscarCarpetaGOG As Button = pagina.FindName("botonBuscarCarpetaGOG")

        AddHandler botonBuscarCarpetaGOG.Click, AddressOf BuscarCarpetaClick
        AddHandler botonBuscarCarpetaGOG.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonBuscarCarpetaGOG.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

    End Sub

    Private Sub AbrirConfigClick(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridConfig As Grid = pagina.FindName("gridConfig")

        Dim recursos As New Resources.ResourceLoader()
        Interfaz.Pestañas.Visibilidad_Pestañas(gridConfig, recursos.GetString("Config"))

    End Sub

    Private Sub AbrirImagenClick(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim grid As Grid = pagina.FindName("gridConfigImagen")
        Dim icono As FontAwesome5.FontAwesome = pagina.FindName("iconoConfigImagen")

        If grid.Visibility = Visibility.Collapsed Then
            grid.Visibility = Visibility.Visible
            icono.Icon = FontAwesome5.EFontAwesomeIcon.Solid_AngleDoubleUp
        Else
            grid.Visibility = Visibility.Collapsed
            icono.Icon = FontAwesome5.EFontAwesomeIcon.Solid_AngleDoubleDown
        End If

    End Sub

    Private Sub BuscarCarpetaClick(sender As Object, e As RoutedEventArgs)

        GOGGalaxy.Generar(True)

    End Sub

    Public Sub Estado(estado As Boolean)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAbrirConfig As Button = pagina.FindName("botonAbrirConfig")
        botonAbrirConfig.IsEnabled = estado

        Dim botonConfigImagen As Button = pagina.FindName("botonConfigImagen")
        botonConfigImagen.IsEnabled = estado

        Dim botonBuscarCarpetaGOG As Button = pagina.FindName("botonBuscarCarpetaGOG")
        botonBuscarCarpetaGOG.IsEnabled = estado

    End Sub

End Module