<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core.Urls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<asp:Repeater ID="rp_MainCategorys" runat="server">
    <ItemTemplate>
        <li class="abc"><a  href='<%# RouteConfig.SubCategory(Convert.ToInt32(Eval("CategoryId")), Eval("RewriteName")) %>'><span><%# Eval("Name")%></span></a>
        <ul>
            <asp:Repeater ID="rp_towCategorys" runat="server">
                <ItemTemplate>
                    <li> 
                        <h3><a   href='<%# RouteConfig.SubCategory(Convert.ToInt32(Eval("CategoryId")), Eval("RewriteName")) %>'><%# Eval("Name") %></a></h3>
                          <div>
                            <asp:Repeater ID="rp_threeCategroys" runat="server">
                            <ItemTemplate>
                                 <a href='<%# RouteConfig.SubCategory(Convert.ToInt32(Eval("CategoryId")), Eval("RewriteName")) %>'><%# Eval("Name") %></a> 
                            </ItemTemplate>
                            </asp:Repeater>
			            </div>		
                    </li>
                </ItemTemplate>
            </asp:Repeater>  
        </ul>
        </li>
    </ItemTemplate>
</asp:Repeater>


