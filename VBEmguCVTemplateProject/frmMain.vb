'
'
'Emgu CV 3.0.0
'
'put this code in your main form, for example frmMain.vb
'
'add the following components to your form:
'
'tlpOuter (TableLayoutPanel)
'tlpInner (TableLayoutPanel)
'btnOpenFile (Button)
'lblChosenFile (Label)
'ibOriginal (Emgu ImageBox)
'ibCanny (Emgu ImageBox)
'ofdOpenFile (OpenFileDialog)
'
'NOTE: Do NOT copy/paste the entire text of this file into Visual Studio !! It will not work if you do !!
'Follow the video on my YouTube channel to create the project and have Visual Studio write part of the code for you,
'then copy/pase the remaining text as needed

Option Explicit On      'require explicit declaration of variables, this is NOT Python !!
Option Strict On        'restrict implicit data type conversions to only widening conversions

Imports Emgu.CV                 'usual Emgu Cv imports
Imports Emgu.CV.CvEnum          '
Imports Emgu.CV.Structure       '
Imports Emgu.CV.UI              '

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class frmMain

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim capWebcam As Capture                        'Capture object to use with webcam
    Dim blnWebcamCapturingInProcess As Boolean = False
    
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub btnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click
        Dim drChosenFile As DialogResult

        drChosenFile = ofdOpenFile.ShowDialog()                 'open file dialog
        
        If (drChosenFile <> DialogResult.OK Or ofdOpenFile.FileName = "") Then    'if user chose Cancel or filename is blank . . .
            lblChosenFile.Text = "file not chosen"              'show error message on label
            Return                                              'and exit function
        End If
        
        Dim imgOriginal As Mat

        Try
            imgOriginal = New Mat(ofdOpenFile.FileName, LoadImageType.Color)
        Catch ex As Exception                                                       'if error occurred
            lblChosenFile.Text = "unable to open image, error: " + ex.Message       'show error message on label
            Return                                                                  'and exit function
        End Try

        If (imgOriginal Is Nothing) Then                                  'if image could not be opened
            lblChosenFile.Text = "unable to open image"                 'show error message on label
            Return                                                      'and exit function
        End If

        Dim imgGrayscale As New Mat(imgOriginal.Size, DepthType.Cv8U, 1)
        Dim imgBlurred As New Mat(imgOriginal.Size, DepthType.Cv8U, 1)
        Dim imgCanny As New Mat(imgOriginal.Size, DepthType.Cv8U, 1)
        
        CvInvoke.CvtColor(imgOriginal, imgGrayscale, ColorConversion.Bgr2Gray)
        
        CvInvoke.GaussianBlur(imgGrayscale, imgBlurred, New Size(5, 5), 1.5)
        
        CvInvoke.Canny(imgBlurred, imgCanny, 100, 200)

        ibOriginal.Image = imgOriginal              'update image boxes
        CvInvoke.Imshow("imgCanny", imgCanny)       '
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub cbUseWebcam_CheckedChanged(sender As Object, e As EventArgs) Handles cbUseWebcam.CheckedChanged
        If (cbUseWebcam.Checked = True) Then
            Try
                capWebcam = New Capture(0)                  'associate the capture object to the default webcam
            Catch ex As Exception                           'catch error if unsuccessful
                                                            'show error via message box
                MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine + ex.Message)
                Return
            End Try

            AddHandler Application.Idle, New EventHandler(AddressOf Me.ProcessImageAndUpdateGUI)        'add process image function to the application's list of tasks
            blnWebcamCapturingInProcess = True

        ElseIf (cbUseWebcam.Checked = False) Then

            If(blnWebcamCapturingInProcess = True) Then
                RemoveHandler Application.Idle, New EventHandler(AddressOf Me.ProcessImageAndUpdateGUI)
                blnWebcamCapturingInProcess = False
                CvInvoke.DestroyAllWindows()
            End If

        Else
            'should never get here
        End If
        
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub ProcessImageAndUpdateGUI(sender As Object, arg As EventArgs)
        Dim imgOriginal As Mat
        
        imgOriginal = capWebcam.QueryFrame()
        
        If (imgOriginal Is Nothing) Then         'if we did not get a frame
            MessageBox.Show("unable to read frame from webcam")
            Return
        End If
        
        Dim imgGrayscale As New Mat(imgOriginal.Size(), DepthType.Cv8U, 1)
        Dim imgBlurred As New Mat(imgOriginal.Size(), DepthType.Cv8U, 1)
        Dim imgCanny As New Mat(imgOriginal.Size(), DepthType.Cv8U, 1)

        CvInvoke.CvtColor(imgOriginal, imgGrayscale, ColorConversion.Bgr2Gray)

        CvInvoke.GaussianBlur(imgGrayscale, imgBlurred, New Size(5, 5), 1.5)

        CvInvoke.Canny(imgBlurred, imgCanny, 100, 200)

        ibOriginal.Image = imgOriginal              'update image boxes
        CvInvoke.Imshow("imgCanny", imgCanny)       '
    End Sub

End Class

