
Imports Newtonsoft.Json
Imports System.Net.Http


Module Module1


    Dim sqCon As New SqlClient.SqlConnection("Server=192.168.0.150\SQLEXPRESS;;Database=EnquiryLog; User Id=sa;Password=Dodid1")
    Dim sqCmd As New SqlClient.SqlCommand
    Dim sdrRow As SqlClient.SqlDataReader

    Dim apiKey As String

    Public client As HttpClient = New HttpClient()
    Sub Main()


        Dim custDomain = Console.ReadLine()
        Dim useKey = Console.ReadLine()

        Select Case useKey
            Case 1
                apiKey = "https://api.powerbi.com/beta/63e8ecb4-d2d0-4253-908d-5865a27137be/datasets/d0437870-9504-46f7-825b-c166e6f317e8/rows?key=WFtolgoejakCPr51C3KOcGlTxJNTLVY7X9yMrh5MZSJNmeRqNJuHIf1gjHfN1NpXGe3RX%2FOlIrSTFtbFdwaCgw%3D%3D"
            Case 2
                apiKey = "https://api.powerbi.com/beta/63e8ecb4-d2d0-4253-908d-5865a27137be/datasets/e2086132-5630-4a59-8444-30d3a461d19d/rows?key=MNqG5I6KRuvWcy4cR5wCw5AxwRvMnj6A8iSMOflPfPWX0aEbpuX27eGA7XRE825Wy2zZS5fkfLklqeLaXyoVjg%3D%3D"
            Case 3
                apiKey = "https://api.powerbi.com/beta/63e8ecb4-d2d0-4253-908d-5865a27137be/datasets/57164353-2cfb-482e-8c89-da0cb73ddb08/rows?key=6yMl4xA3U4CF1FGNV%2Fo8LB8c340YnibjgI6XD0R9T89vCXqS5gtTrdUvqLlG3IWuNs2%2Bn9yieGZ6ENEvHztkHw%3D%3D"
        End Select


        Dim JsonDt As String
        Dim timeFormat As String = "yyyy-MM-ddTHH:mm:ss.fffZ"
        Do While 1 = 1

            'GET AVERAGE TIMES FROM SQL
            Dim doubleTime As Double
            Dim column0 As String
            Dim column1 As String
            sqCmd.Connection = sqCon
            sqCmd.Parameters.Clear()
            sqCmd.CommandText = "select domain,turnaround from dbo.view_top_customer_quotation_turnaround where domain like @custParam"
            sqCmd.Parameters.AddWithValue("@custParam", "%" + custDomain + "%")
            sqCon.Open()
            sqCmd.ExecuteNonQuery()
            sdrRow = sqCmd.ExecuteReader()
            For Each itm In sdrRow
                column0 = sdrRow.GetValue(0)
                column1 = sdrRow.GetValue(1)

                Dim domain As String = column0
                Dim turnaround As Double = column1
                Dim target As Int16 = 8
                Dim maxHours As Int16 = 16
                Dim ts = New TimeSpan(0, 0, column1 * 60, 0)

                Dim stringTime = ts.ToString
                stringTime = stringTime.Substring(0, 5)
                stringTime = stringTime.Replace(":", ".")
                doubleTime = Convert.ToDouble(stringTime)



                JsonDt = "[{""domain"":""" & domain & """"
                JsonDt = JsonDt + ",""turnaround"":" & doubleTime
                JsonDt = JsonDt + ",""target"":" & target
                JsonDt = JsonDt + ",""datenow"":""" & Date.UtcNow.ToString(timeFormat)
                JsonDt = JsonDt + """,""maxHours"":" & maxHours
                JsonDt = JsonDt & "}]"

                Dim content As HttpContent = New StringContent(JsonDt)
                Dim url As New Uri(apiKey)

                Dim response As HttpResponseMessage
                response = client.PostAsync(url, content).Result
                System.Threading.Thread.Sleep(1000)
                Console.WriteLine(JsonDt.ToString)

            Next
            'make sure you close your connections!
            sdrRow.Close()
            sqCon.Close()







        Loop



    End Sub

End Module
