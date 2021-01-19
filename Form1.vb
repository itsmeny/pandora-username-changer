Imports System.IO
Public Class Form1
    Dim file_offsets As Long = &HBD000
    Private Sub pandora_patch(ByVal TargetFile As String, ByVal Offsets As Long, ByVal NewValue() As Byte)
        Try
            Dim br As BinaryReader = New BinaryReader(File.Open(TargetFile, FileMode.Open))
            br.BaseStream.Position = Offsets
            Dim byteB As Byte
            For Each byteB In NewValue
                If (byteB.ToString <> String.Empty) Then
                    br.BaseStream.WriteByte(byteB)
                Else
                    Exit For
                End If
            Next byteB
            br.Close()
        Catch
        End Try
    End Sub
    Private Function Read(ByVal TargetFile As String)
        Dim value(0 To 13 - 1) As Byte
        Using reader As New BinaryReader(File.Open(TargetFile, FileMode.Open))
            Dim fileLength As Long = reader.BaseStream.Length
            Dim byteCount As Integer = 0
            reader.BaseStream.Seek(file_offsets, SeekOrigin.Begin)
            While file_offsets < fileLength And byteCount < 13
                value(byteCount) = reader.ReadByte()
                file_offsets += 1
                byteCount += 1
            End While
            file_offsets = &HBD000
        End Using
        Return System.Text.Encoding.UTF8.GetString(value)
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If TextBox2.Text = "" Then
                MsgBox("invalid username!", MsgBoxStyle.Critical, "skrr skrr")
            Else
                pandora_patch(TextBox1.Text, file_offsets, New Byte() {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0})
                pandora_patch(TextBox1.Text, file_offsets, System.Text.Encoding.ASCII.GetBytes(TextBox2.Text))
                pandora_patch(TextBox1.Text, file_offsets + TextBox2.TextLength, New Byte() {&H0, &H0, &H5})
                MsgBox("username changed!", MsgBoxStyle.Information, "skrr skrr")
                Label2.Text = Read(TextBox1.Text)
                TextBox2.Clear()
            End If
        Catch ex As Exception
            MsgBox(ex)
        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        OpenFileDialog1.Title = "Select Pandora dll"
        OpenFileDialog1.InitialDirectory = "C:\"
        OpenFileDialog1.Filter = "Pandora dll | *.dll"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            TextBox1.Text = OpenFileDialog1.FileName
            Label2.Text = Read(TextBox1.Text)
        End If
    End Sub
End Class
