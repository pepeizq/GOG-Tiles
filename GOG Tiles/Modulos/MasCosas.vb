Imports Windows.Services.Store
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI

Module MasCosas

    Dim codigoFuente As String = "https://github.com/pepeizq/GOG-Tiles"
    Dim traduccion As String = "https://poeditor.com/join/project/93E4lCQLWI"
    Dim calificar As Boolean = True
    Dim youtube As String = "https://www.youtube.com/watch?v=Pz80ufP90dQ"
    Dim pepeizqapps As Boolean = True
    Dim pepeizqdeals As Boolean = True
    Dim twitter As String = "https://twitter.com/pepeizqu"

    Public Sub Cargar()

        Dim recursos As New Resources.ResourceLoader

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim grid As Grid = pagina.FindName("gridMasCosas")

        Dim sv As New ScrollViewer With {
            .VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        }

        Dim sp As New StackPanel With {
            .Orientation = Orientation.Vertical,
            .Padding = New Thickness(10, 10, 30, 10)
        }

        If Not codigoFuente = Nothing Then
            sp.Children.Add(GenerarCaja(recursos.GetString("MoreThingsSourceCode"), recursos.GetString("MoreThingsSourceCodeDescription"),
                                        codigoFuente, Nothing, FontAwesome5.EFontAwesomeIcon.Brands_Github, False))
        End If

        If Not traduccion = Nothing Then
            sp.Children.Add(GenerarCaja(recursos.GetString("MoreThingsHelpTranslate"), recursos.GetString("MoreThingsHelpTranslateDescription"),
                                        traduccion, Nothing, FontAwesome5.EFontAwesomeIcon.Solid_GlobeEurope, False))
        End If

        If calificar = True Then
            sp.Children.Add(GenerarCaja(recursos.GetString("MoreThingsRateApp"), recursos.GetString("MoreThingsRateAppDescription"),
                                        Nothing, Nothing, FontAwesome5.EFontAwesomeIcon.Solid_ThumbsUp, True))
        End If

        If Not youtube = Nothing Then
            sp.Children.Add(GenerarCaja(recursos.GetString("MoreThingsVideo"), recursos.GetString("MoreThingsVideoDescription"),
                                        youtube, Nothing, FontAwesome5.EFontAwesomeIcon.Brands_Youtube, False))
        End If

        If pepeizqapps = True Then
            sp.Children.Add(GenerarCaja("pepeizqapps.com", recursos.GetString("MoreThingspepeizqappsDescription"),
                                        "https://pepeizqapps.com/", "https://pepeizqapps.com/contact/", FontAwesome5.EFontAwesomeIcon.Solid_Cube, False))
        End If

        If pepeizqdeals = True Then
            sp.Children.Add(GenerarCaja("pepeizqdeals.com", recursos.GetString("MoreThingspepeizqdealsDescription"),
                                        "https://pepeizqdeals.com/", Nothing, FontAwesome5.EFontAwesomeIcon.Solid_Cube, False))
        End If

        If Not twitter = Nothing Then
            sp.Children.Add(GenerarCaja("@pepeizqu", recursos.GetString("MoreThingsMyTwitterDescription"),
                                        twitter, Nothing, FontAwesome5.EFontAwesomeIcon.Brands_Twitter, False))
        End If

        If sp.Children.Count > 1 Then
            Dim spUltimo As StackPanel = sp.Children(sp.Children.Count - 1)
            spUltimo.Margin = New Thickness(0, 0, 0, 0)
        End If

        sv.Content = sp
        grid.Children.Add(sv)

    End Sub

    Private Function GenerarCaja(titulo As String, descripcion As String, enlace1 As String, enlace2 As String, icono2 As FontAwesome5.EFontAwesomeIcon, calificar As Boolean)

        Dim recursos As New Resources.ResourceLoader

        Dim colorFondo As New SolidColorBrush With {
            .Color = App.Current.Resources("ColorCuarto"),
            .Opacity = 0.5
        }

        Dim sp As New StackPanel With {
            .Orientation = Orientation.Vertical,
            .Padding = New Thickness(25, 25, 25, 25),
            .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
            .BorderThickness = New Thickness(1, 1, 1, 1),
            .Background = colorFondo,
            .Margin = New Thickness(0, 0, 0, 30)
        }

        Dim spTitulo As New StackPanel With {
            .Orientation = Orientation.Horizontal
        }

        Dim icono As New FontAwesome5.FontAwesome With {
            .Icon = icono2,
            .Foreground = New SolidColorBrush(Colors.White),
            .VerticalAlignment = VerticalAlignment.Center
        }

        spTitulo.Children.Add(icono)

        Dim tbTitulo As New TextBlock With {
            .Text = titulo,
            .Margin = New Thickness(15, 0, 0, 0),
            .Foreground = New SolidColorBrush(Colors.White),
            .FontSize = 17,
            .VerticalAlignment = VerticalAlignment.Center
        }

        spTitulo.Children.Add(tbTitulo)

        sp.Children.Add(spTitulo)

        Dim tbDescripcion As New TextBlock With {
            .Text = descripcion,
            .Margin = New Thickness(0, 15, 0, 0),
            .Foreground = New SolidColorBrush(Colors.White),
            .FontSize = 15,
            .VerticalAlignment = VerticalAlignment.Center,
            .TextWrapping = TextWrapping.Wrap
        }

        sp.Children.Add(tbDescripcion)

        Dim spEnlaces As New StackPanel With {
            .Orientation = Orientation.Horizontal,
            .Margin = New Thickness(0, 20, 0, 0)
        }

        If calificar = False Then
            Dim tbAbrir As New TextBlock With {
                .Foreground = New SolidColorBrush(Colors.White),
                .Text = recursos.GetString("MoreThingsOpen")
            }

            Dim botonAbrir As New Button With {
                .Background = New SolidColorBrush(App.Current.Resources("ColorCuarto")),
                .BorderThickness = New Thickness(0, 0, 0, 0),
                .Style = App.Current.Resources("ButtonRevealStyle"),
                .Padding = New Thickness(15, 12, 15, 12),
                .Tag = enlace1,
                .Content = tbAbrir
            }

            AddHandler botonAbrir.Click, AddressOf AbrirClick
            AddHandler botonAbrir.PointerEntered, AddressOf Interfaz.Entra_Boton_Texto
            AddHandler botonAbrir.PointerExited, AddressOf Interfaz.Sale_Boton_Texto

            spEnlaces.Children.Add(botonAbrir)
        Else
            Dim tbCalificar As New TextBlock With {
                .Foreground = New SolidColorBrush(Colors.White),
                .Text = recursos.GetString("MoreThingsRate")
            }

            Dim botonCalificar As New Button With {
                .Background = New SolidColorBrush(App.Current.Resources("ColorCuarto")),
                .BorderThickness = New Thickness(0, 0, 0, 0),
                .Style = App.Current.Resources("ButtonRevealStyle"),
                .Padding = New Thickness(15, 12, 15, 12),
                .Content = tbCalificar
            }

            AddHandler botonCalificar.Click, AddressOf CalificarClick
            AddHandler botonCalificar.PointerEntered, AddressOf Interfaz.Entra_Boton_Texto
            AddHandler botonCalificar.PointerExited, AddressOf Interfaz.Sale_Boton_Texto

            spEnlaces.Children.Add(botonCalificar)
        End If

        If Not enlace2 = Nothing Then
            Dim tbContactar As New TextBlock With {
                .Foreground = New SolidColorBrush(Colors.White),
                .Text = recursos.GetString("MoreThingsContact")
            }

            Dim botonContactar As New Button With {
                .Background = New SolidColorBrush(App.Current.Resources("ColorCuarto")),
                .BorderThickness = New Thickness(0, 0, 0, 0),
                .Style = App.Current.Resources("ButtonRevealStyle"),
                .Padding = New Thickness(15, 12, 15, 12),
                .Tag = enlace2,
                .Content = tbContactar,
                .Margin = New Thickness(25, 0, 0, 0)
            }

            AddHandler botonContactar.Click, AddressOf AbrirClick
            AddHandler botonContactar.PointerEntered, AddressOf Interfaz.Entra_Boton_Texto
            AddHandler botonContactar.PointerExited, AddressOf Interfaz.Sale_Boton_Texto

            spEnlaces.Children.Add(botonContactar)
        End If

        sp.Children.Add(spEnlaces)

        Return sp

    End Function

    Private Async Sub AbrirClick(sender As Object, e As RoutedEventArgs)

        Dim boton As Button = sender
        Dim enlace As String = boton.Tag

        Try
            Await Launcher.LaunchUriAsync(New Uri(enlace))
        Catch ex As Exception

        End Try

    End Sub

    Private Sub CalificarClick(sender As Object, e As RoutedEventArgs)

        CalificarApp(False)

    End Sub

    Public Async Sub CalificarApp(primeraVez As Boolean)

        Dim recursos As New Resources.ResourceLoader()

        Dim usuarios As IReadOnlyList(Of User) = Await User.FindAllAsync

        If Not usuarios Is Nothing Then
            If usuarios.Count > 0 Then
                Dim usuario As User = usuarios(0)

                Dim contexto As StoreContext = StoreContext.GetForUser(usuario)
                Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings

                Dim sacarVentana As Boolean = True

                If primeraVez = True Then
                    If config.Values("Calificar_App") = 1 Then
                        sacarVentana = False
                    End If
                End If

                If sacarVentana = True Then
                    Dim review As StoreRateAndReviewResult = Await contexto.RequestRateAndReviewAppAsync

                    If review.Status = StoreRateAndReviewStatus.Succeeded Then
                        Notificaciones.Toast(recursos.GetString("MoreThingsRateAppThanks"), Nothing)
                        config.Values("Calificar_App") = 1
                    ElseIf review.Status = StoreRateAndReviewStatus.Error Then
                        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))
                        config.Values("Calificar_App") = 1
                    Else
                        config.Values("Calificar_App") = 0
                    End If
                End If
            End If
        End If

    End Sub

End Module