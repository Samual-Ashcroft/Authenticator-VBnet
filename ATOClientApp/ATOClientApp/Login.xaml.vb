Option Strict On

Imports MySql.Data.MySqlClient




Public Class Login

    Private Function Authenticate(_login As TextBox, _password As PasswordBox, _password_retype As PasswordBox, _password_retype_label As Label, _feedback_label As Label) As Boolean

        If (_password_retype.IsEnabled And _password_retype_label.IsEnabled) Then
            _feedback_label.Content = "You're password has been recorded"
            Console.WriteLine("You're password has been recorded")
            System.Threading.Thread.Sleep(1000)

            'rehiding the retype'
            _password_retype_label.Visibility = Visibility.Collapsed
            _password_retype_label.IsEnabled = False
            _password_retype.Visibility = Visibility.Collapsed
            _password_retype.IsEnabled = False
            Return True
        End If

        Dim connStr As String = "Database=ATO_authenticator;Data Source=localhost;" _
        & "User Id=user_client;Password=45h2g45hg23kjh53j456276DFSJG"

        Dim conn As New MySqlConnection(connStr)
        Dim cmd1 As New MySqlCommand()
        Dim cmd2 As New MySqlCommand()

        Try
            conn.Open()
            cmd1.Connection = conn

            cmd1.CommandText = "SELECT login, PASSid FROM persons, passwords WHERE persons.login = @login AND persons.LOGid = passwords.LOGid AND passwords.current = 1"
            cmd1.Prepare()

            cmd1.Parameters.AddWithValue("@login", _login.Text)
            Dim reader1 As MySqlDataReader = cmd1.ExecuteReader()

            If reader1.HasRows Then
                _feedback_label.Content = "You have a password"
                Console.WriteLine("You have a password")
                Console.WriteLine(reader1.GetName(0).PadRight(18) _
                                  & reader1.GetName(1).PadRight(18))
                While reader1.Read()
                    Console.WriteLine(reader1.GetString(0).PadRight(18) _
                                      & reader1.GetString(1).PadRight(18))
                End While
            Else
                Console.WriteLine("You dont have a password yet, \nplease enter and re-enter your new password in the field that has appeared")
                _password_retype_label.Visibility = Visibility.Visible
                _password_retype_label.IsEnabled = True
                _password_retype.Visibility = Visibility.Visible
                _password_retype.IsEnabled = True

            End If

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Console.WriteLine("Debug 3")

        Return True

    End Function

    Private Function LoginCheck(__login As String, __password As Security.SecureString) As Integer

        Dim connStr As String = "Database=ATO_authenticator;Data Source=localhost;" _
        & "User Id=user_client;Password=45h2g45hg23kjh53j456276DFSJG"

        Dim conn As New MySqlConnection(connStr)
        Dim cmd1 As New MySqlCommand()
        Dim cmd2 As New MySqlCommand()
        Dim state As Integer = -1

        Try
            conn.Open()
            cmd1.Connection = conn

            cmd1.CommandText = "SELECT login FROM persons WHERE login = @login"
            cmd1.Prepare()

            cmd1.Parameters.AddWithValue("@login", __login)
            Dim reader1 As MySqlDataReader = cmd1.ExecuteReader()

            If reader1.HasRows Then
                reader1.Close()
                MessageBox.Show("That login exists")
                state = 0
                cmd2.Connection = conn

                cmd2.CommandText = "SELECT login FROM persons WHERE login = @login AND password = @password"
                cmd2.Prepare()

                cmd2.Parameters.AddWithValue("@login", __login)
                cmd2.Parameters.AddWithValue("@password", __password)
                Dim reader2 As MySqlDataReader = cmd2.ExecuteReader()

                If reader2.HasRows Then
                    state = 1
                    reader2.Close()
                    MessageBox.Show("That is the correct password!")
                    Return 1
                Else
                    MessageBox.Show("That is the incorrect password!")
                    cmd2.CommandText = "SELECT login FROM persons WHERE login = @login AND password = @password"
                    cmd2.Prepare()

                    cmd2.Parameters.AddWithValue("@login", __login)
                    cmd2.Parameters.AddWithValue("@password", "")
                    If reader2.HasRows Then
                        state = 2
                        MessageBox.Show("You have no password yet, lets set this one as your password")
                    End If
                End If
                reader2.Close()
            Else
                state = -1
            End If

            reader1.Close()

        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return state

    End Function
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        Authenticate(_login, _password, _password_retype, _password_retype_label, _feedback_label)
        'If LoginCheck(_login.Text, _password.SecurePassword) = 1 Then'
        'MessageBox.Show("Welcome person!")'
        'Else'
        'MessageBox.Show("That login does not exist")'
        'End If'

    End Sub
End Class
