Option Strict On

Imports MySql.Data.MySqlClient
Imports System.Timers

Public Class Login

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs) Handles _login_submit.Click

        If Authenticate(_login, _password, _password_retype, _password_retype_label, _feedback_label, _login_submit) Then

            'Threading.Thread.Sleep(3000) 'ms'
            'Make this form invisible'
            _login_grid.IsEnabled = False
            _login_grid.Visibility = Visibility.Collapsed

            'make the next area available'
            _InfoAccess_grid.IsEnabled = True
            _InfoAccess_grid.Visibility = Visibility.Visible

            'Build the personalised welcome message'
            _feedback_label1.Content = "Welcome inside our portal application " & GetName() & "!"
            'populate the combobox with applications'
            PopulateApps(_apps_comboBox)

        End If

    End Sub

    Private Sub _back_login_Click(sender As Object, e As RoutedEventArgs) Handles _back_login.Click

        'blank the inputs'
        _login.Text = ""
        _password.Password = ""
        _password_retype.Password = ""

        'Make this form invisible'
        _InfoAccess_grid.IsEnabled = False
        _InfoAccess_grid.Visibility = Visibility.Collapsed

        'make the next area available'
        _login_grid.IsEnabled = True
        _login_grid.Visibility = Visibility.Visible

    End Sub

    Private Sub Login_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Make infoaccess invisible'
        _InfoAccess_grid.IsEnabled = False
        _InfoAccess_grid.Visibility = Visibility.Collapsed

        'make login available'
        _login_grid.IsEnabled = True
        _login_grid.Visibility = Visibility.Visible
        Dim sharedtimer As New Timer
    End Sub
End Class
