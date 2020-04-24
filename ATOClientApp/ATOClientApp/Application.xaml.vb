Imports MySql.Data.MySqlClient

Module DBinteractions

    Dim connStr As String = "Database=ATO_authenticator;Data Source=localhost;" _
        & "User Id=user_client;Password=45h2g45hg23kjh53j456276DFSJG"

    Private Function RequestAccess(connSTR As String, LOGid As Integer, minLength As Integer, maxLength As Integer) As String

        Dim charRef As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim rando As New Random
        Dim stringBuilder As String = ""

        Dim count As Integer = rando.Next(minLength, maxLength)

        For i As Integer = 1 To count
            Dim idx As Integer = rando.Next(0, charRef.Length)
            stringBuilder = stringBuilder & charRef.Substring(idx, 1)
        Next

        Dim conn As New MySqlConnection(connSTR)
        Dim cmd As New MySqlCommand()

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "INSERT INTO hashes(LOGid, hash) VALUES(@lOGid, @hash)"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@LOGid", LOGid)
            cmd.Parameters.AddWithValue("@hash", stringBuilder)
            cmd.ExecuteNonQuery()

        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return stringBuilder

    End Function

    Private Function PopulateAppsPriv(connstr As String, _apps_comboBox As ComboBox) As Boolean

        If TestHashPriv(connstr) < 0 Then
            Return False
        End If


        Dim name As String = ""
        Dim conn As New MySqlConnection(connstr)
        Dim cmd As New MySqlCommand()

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "SELECT appName FROM applications"
            cmd.Prepare()

            Dim reader1 As MySqlDataReader = cmd.ExecuteReader()

            'iterate the list of applications'
            _apps_comboBox.Items.Clear()
            While reader1.Read()
                _apps_comboBox.Items.Add(reader1.GetString(0))
            End While

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return False

    End Function

    Public Function PopulateApps(_apps_comboBox As ComboBox) As Boolean

        Return PopulateAppsPriv(connStr, _apps_comboBox)

    End Function
    Private Function GetNamePriv(connstr As String) As String

        Dim LOGid As Integer = TestHashPriv(connstr)
        If LOGid < 0 Then
            Return "Imposter!"
        End If

        Dim name As String = ""
        Dim conn As New MySqlConnection(connstr)
        Dim cmd As New MySqlCommand()

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "SELECT firstName, lastName FROM persons WHERE LOGid = @LOGid"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@LOGid", LOGid)
            Dim reader1 As MySqlDataReader = cmd.ExecuteReader()

            'Test if the user exists'
            If reader1.HasRows Then
                reader1.Read()
                name = reader1.GetString(0) & " " & reader1.GetString(1)

            End If

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return name

    End Function

    Public Function GetName() As String

        Return GetNamePriv(connStr)

    End Function

    Public Function TestHash() As Integer

        Return TestHashPriv(connStr)

    End Function

    Private Function TestHashPriv(connStr As String) As Integer

        Dim hash As String = ""
        Dim LOGid As Integer = -1
        Dim LOGidEV As Integer = -1

        Try
            hash = System.Environment.GetEnvironmentVariable("User_ATO_Privs", EnvironmentVariableTarget.User)
            LOGidEV = CType(hash.Split(CType(":", Char()))(0), Integer)
            hash = hash.Split(CType(":", Char()))(1)

            'Debugging'
            'Console.WriteLine("Hash: " & hash)'
            'Console.WriteLine("LOGidEV: " & CType(LOGidEV, String))'
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.ToString())
            Return LOGid
        End Try

        Dim conn As New MySqlConnection(connStr)
        Dim cmd As New MySqlCommand()

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "SELECT LOGid FROM hashes WHERE hash = @hash"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@hash", hash)
            Dim reader1 As MySqlDataReader = cmd.ExecuteReader()

            'Test if the user exists'
            If reader1.HasRows Then
                reader1.Read()
                LOGid = reader1.GetInt32(0)

            End If

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return LOGid

    End Function

    Private Function PushEnvironmentVariable(hash As String, LOGid As Integer) As Boolean

        System.Environment.SetEnvironmentVariable("User_ATO_Privs", CType(LOGid, String) & ":" & hash, EnvironmentVariableTarget.User)

        Return True

    End Function

    Private Function GenerateHash(connSTR As String, LOGid As Integer, minLength As Integer, maxLength As Integer) As String

        Dim charRef As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim rando As New Random
        Dim stringBuilder As String = ""

        Dim count As Integer = rando.Next(minLength, maxLength)

        For i As Integer = 1 To count
            Dim idx As Integer = rando.Next(0, charRef.Length)
            stringBuilder = stringBuilder & charRef.Substring(idx, 1)
        Next

        Dim conn As New MySqlConnection(connSTR)
        Dim cmd As New MySqlCommand()

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "INSERT INTO hashes(LOGid, hash) VALUES(@lOGid, @hash)"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@LOGid", LOGid)
            cmd.Parameters.AddWithValue("@hash", stringBuilder)
            cmd.ExecuteNonQuery()

        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return stringBuilder

    End Function

    Private Function CheckLogin(connStr As String, _login As TextBox) As Integer

        Dim conn As New MySqlConnection(connStr)
        Dim cmd As New MySqlCommand()
        Dim LOGid As Integer = -1

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "SELECT LOGid, login FROM persons WHERE login = @login"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@login", _login.Text)
            Dim reader1 As MySqlDataReader = cmd.ExecuteReader()

            'Test if the user exists'
            If reader1.HasRows Then
                reader1.Read()
                LOGid = reader1.GetInt32(0)

            End If

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return LOGid

    End Function

    Private Function CheckForPassword(connStr As String, LOGid As Integer) As Integer

        Dim conn As New MySqlConnection(connStr)
        Dim cmd As New MySqlCommand()
        Dim count As Integer = 0

        Try
            conn.Open()
            cmd.Connection = conn

            cmd.CommandText = "SELECT PASSid FROM passwords WHERE LOGid = @LOGid AND current = 1"
            cmd.Prepare()

            cmd.Parameters.AddWithValue("@LOGid", LOGid)
            Dim reader1 As MySqlDataReader = cmd.ExecuteReader()


            'Test if the user exists'
            While reader1.Read()
                count += 1
            End While

            Console.WriteLine("number of passwords" & CType(count, String))

            reader1.Close()
        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try

        Return count

    End Function

    Private Function AuthenticatePriv(connStr As String, _login As TextBox, _password As PasswordBox, _password_retype As PasswordBox, _password_retype_label As Label, _feedback_label As Label, _login_submit As Button) As Boolean

        Dim LOGid As Integer = CheckLogin(connStr, _login)

        If LOGid < 0 Then
            _feedback_label.Content = "That is not a valid Login ID"
            System.Threading.Thread.Sleep(1000)
            Return False

        End If

        'checking if a password exists'
        Console.WriteLine("yes" & CType(CheckForPassword(connStr, LOGid), String) & " Yote " & CType(_login.IsEnabled, String))
        If (CheckForPassword(connStr, LOGid) = 0 And _login.IsEnabled) Then
            _feedback_label.Content = "You dont have a password yet, please enter and re-enter your new password in the field that has appeared then hit submit"
            _password_retype_label.Visibility = Visibility.Visible
            _password_retype_label.IsEnabled = True
            _password_retype.Visibility = Visibility.Visible
            _password_retype.IsEnabled = True
            _login.IsEnabled = False
            _login_submit.Content = "Submit"
            Return False

        End If

        Dim conn As New MySqlConnection(connStr)
        Dim cmd1 As New MySqlCommand()
        Dim verified As Boolean = False
        Dim oldPass As Boolean = False

        Try
            conn.Open()
            cmd1.Connection = conn

            If (Not _login.IsEnabled And _password.Password = _password_retype.Password) Then
                _feedback_label.Content = "Your passwords match, registering new password"

                cmd1.CommandText = "INSERT INTO passwords(LOGid, password, current) VALUES(@LOGid, @password, '1')"
                cmd1.Prepare()

                cmd1.Parameters.AddWithValue("@LOGid", LOGid)
                cmd1.Parameters.AddWithValue("@password", _password.Password)
                cmd1.ExecuteNonQuery()
                _password_retype_label.Visibility = Visibility.Collapsed
                _password_retype_label.IsEnabled = False
                _password_retype.Visibility = Visibility.Collapsed
                _password_retype.IsEnabled = False
                _login.IsEnabled = True
                _login_submit.Content = "Login"

            ElseIf (Not _login.IsEnabled And (Not _password.Password = _password_retype.Password)) Then
                _feedback_label.Content = "Your passwords dont match, try again."
            Else

                cmd1.CommandText = "SELECT PASSid, current FROM passwords WHERE LOGid = @LOGid AND password = @password"
                cmd1.Prepare()

                cmd1.Parameters.AddWithValue("@LOGid", LOGid)
                cmd1.Parameters.AddWithValue("@password", _password.Password)
                Dim reader1 As MySqlDataReader = cmd1.ExecuteReader()

                oldPass = reader1.HasRows
                While reader1.Read()
                    verified = reader1.GetBoolean(1) Or verified

                End While

                If verified Then
                    Dim hash As String = GenerateHash(connStr, LOGid, 30, 44) '44 is just incase'

                    PushEnvironmentVariable(hash, LOGid)

                    _feedback_label.Content = "Welcome to our systems: " & hash
                ElseIf oldPass Then
                    _feedback_label.Content = "Wrong password, expired"
                Else
                    _feedback_label.Content = "Wrong password"
                End If

                reader1.Close()

            End If

        Catch ex As MySqlException
            Console.WriteLine("Error: " & ex.ToString())
        Finally
            conn.Close()
        End Try



        Return verified

    End Function

    Public Function Authenticate(_login As TextBox, _password As PasswordBox, _password_retype As PasswordBox, _password_retype_label As Label, _feedback_label As Label, _login_submit As Button) As Boolean

        Return AuthenticatePriv(DBinteractions.connStr, _login, _password, _password_retype, _password_retype_label, _feedback_label, _login_submit)

    End Function

End Module

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

End Class
