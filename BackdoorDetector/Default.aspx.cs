using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ListBox1.Style.Add("visibility", "hidden");
        //originalcontentLabel.Style.Add("visibility", "hidden");
        titleLabel.Text = ("Upload a .NET source file for backdoor analysis.");
        resultLabel.Style.Add("visibility", "hidden");
        reportLabel.Style.Add("visibility", "hidden");
        reportTextArea.Style.Add("visibility", "hidden");
        Button2.Style.Add("visibility", "hidden");
        filenameTextBox.Style.Add("visibility", "hidden");
        txtLabel.Style.Add("visibility", "hidden");
    }

    public string a = string.Empty;






    public void Button1_Click(object sender, EventArgs e)
    {
        
        if (FileUpload1.HasFile)
        {
            titleLabel.Style.Add("color", "black");
            string ext = System.IO.Path.GetExtension(FileUpload1.FileName);
            
            
            if(ext==".cs" || ext == ".aspx")
            {

                DateTime trh;
                trh = DateTime.Now.Date;



                int counter = 0;
                String[] trimmedReport = new string[100];
                //set the base directory paths
                string blacklistPath = Server.MapPath("documents/blacklist/malicious.txt");
                string uploadDirectory = Server.MapPath("documents/uploads//");

                a = FileUpload1.FileName;
                

                FileUpload1.SaveAs(uploadDirectory + FileUpload1.FileName);
                filenameLabel.Text = ("File Name: " + FileUpload1.FileName);
                filesizeLabel.Text = ("File Size: " + FileUpload1.PostedFile.ContentLength.ToString() + " bytes");
                filetypeLabel.Text = ("File Type: " + FileUpload1.PostedFile.ContentType.ToString());

                string filePath = uploadDirectory + FileUpload1.FileName;


                // This text is always added, making the file longer over time
                // if it is not deleted.
                //string appendText = "This is extra text" + Environment.NewLine;
                //File.AppendAllText(filePath, appendText);
                reportLabel.Style.Add("visibility", "visible");
                ListBox1.Style.Add("visibility", "visible");
                resultLabel.Style.Add("visibility", "visible");
                reportTextArea.Style.Add("visibility", "visible");
                Button2.Style.Add("visibility", "visible");
                filenameTextBox.Style.Add("visibility", "visible");
                txtLabel.Style.Add("visibility", "visible");

                // Open the file to read from.
                string[] readText = File.ReadAllLines(filePath);
                int i = 1;
                ListBox1.Items.Clear();
                //load source code into memory
                foreach (string s in readText)   
                {
                    ListBox1.Items.Add(new ListItem(Server.HtmlDecode(i + ". &nbsp; &nbsp; &nbsp;") + s));
                    i++; //count the lines
                }


                //load blacklist(malicious codes) into memory
                string[] blackList = File.ReadAllLines(blacklistPath);

                //start the timer
                Stopwatch sw = Stopwatch.StartNew();

                for (int j=0; j<(i-2); j++)
                { 
                    for (int k = 0; k < blackList.Length; k++)
                    {

                        if (ListBox1.Items[j].Value.Contains(blackList[k]) == true)
                        {
                            ListBox1.Items[j].Attributes.CssStyle.Add("background", "#FF8383");
                            ListBox1.Items[j].Attributes.CssStyle.Add("font-weight", "bold");
                            ListBox1.Items[j].Attributes.CssStyle.Add("padding", "3px;");
                            ListBox1.Items[j].Attributes.CssStyle.Add("border-radius", "8px;");
                            trimmedReport[counter] = ListBox1.Items[j].Value.Replace(" ", String.Empty);
                           
                            counter++; 
                        }
                    }

                }
                //stop the timer
                sw.Stop();

                //Session["numBackdoor"] = counter;

                

                //Analysis Report
                reportTextArea.InnerText = "Local .NET file," + FileUpload1.FileName + ", is scanned through static code analysis.\n";

                if (counter != 0) //some backdoor activity may be possible
                {

                    reportTextArea.InnerText += counter + " malicious backdoor activity is found in the source code.\n";



                    if (counter < 5)        //low risk
                    {
                        reportTextArea.InnerText += "Since the # of findings is below 5, the given application is considered as low risk. These are listed below:\n";

                    }
                    else if (counter > 5 && counter < 10)       //medium risk
                    {
                        reportTextArea.InnerText += "Since the # of findings is between 5-10, the given application is considered as medium risk. These are listed below:\n";
                    }
                    else                    //high risk
                    {
                        reportTextArea.InnerText += "Since the # of findings is more than 10, the given application is considered as high risk. Immediate action should be taken! These are listed below:\n";
                    }

                    

                    for (int t = 0; t < counter; t++)
                    {
                        reportTextArea.InnerText += "   Line #" + trimmedReport[t] + "\n";
                    }
                }
                else //backdoor activity couldn't be found
                {
                    reportTextArea.InnerText += "No backdoor activity is found!";
                }

                reportTextArea.InnerText += "\n\n\n=========================================\n";
                reportTextArea.InnerText += "Execution Date: " + trh + "\n";
                reportTextArea.InnerText += "Time taken: {" + sw.Elapsed.TotalMilliseconds + "} ms\n";
                reportTextArea.InnerText += "Total number of lines: " + (i - 1);

            
            }
            else
            {

                titleLabel.Text = ("Only .NET projects are accepted.");
                titleLabel.Style.Add("color", "red");
            }

        }
        else
        {
            titleLabel.Text = ("Please select an item first.");
            titleLabel.Style.Add("color", "red");
        }
        
    }

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {

    }


    protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void reportTextbox_TextChanged(object sender, EventArgs e)
    {

    }
    


    public void Button2_Click(object sender, EventArgs e)
    {
        DateTime trh;
        trh = DateTime.Now.Date;
        

        String path = Server.MapPath(@"documents/files/");
        String pathNew = path + filenameTextBox.Text + "_St.Cde.Anl.Report" + ".txt";

        if (filenameTextBox.Text.Length < 3)
        {
            Response.Write("<script>alert('Provide at least 3 characters for the file name.');</script>");
        }
        else
        {
            if (!File.Exists(pathNew))
            {
                using (var tw = new StreamWriter(pathNew, true))
                {
                    tw.WriteLine(reportTextArea.InnerText);
                    tw.Close();
                }
            }
             
            try
            {
                if (Convert.ToInt32(Session["downloadCounter"].ToString()) < 1)
                    Session["downloadCounter"] = "0";
            }
            catch (Exception ex)
            {
                Session["downloadCounter"] = "0";
            }
             
            if (Convert.ToInt32(Session["downloadCounter"].ToString()) < 3)
            {
                //Download the file

                Session["downloadCounter"] = (Convert.ToInt32(Session["downloadCounter"]) + 1).ToString();
                byte[] Content = File.ReadAllBytes(pathNew);
                //Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment; filename=" + filenameTextBox.Text + "_St.Cde.Anl.Report" + ".txt");
                Response.BufferOutput = true;
                Response.OutputStream.Write(Content, 0, Content.Length);
                Response.End();
            }
            else
            {
                Response.Write("<script>alert('You can download the file 3 times at a session.');</script>");
            }

        }
        

        txtLabel.Style.Add("visibility", "visible");
        filenameTextBox.Style.Add("visibility", "visible");
        reportLabel.Style.Add("visibility", "visible");
        ListBox1.Style.Add("visibility", "visible");
        resultLabel.Style.Add("visibility", "visible");
        reportTextArea.Style.Add("visibility", "visible");
        Button2.Style.Add("visibility", "visible");
        
    }

    protected void TextBox1_TextChanged1(object sender, EventArgs e)
    {

    }
}