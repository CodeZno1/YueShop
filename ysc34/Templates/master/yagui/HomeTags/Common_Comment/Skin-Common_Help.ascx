<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.SaleSystem.Tags" Assembly="Hidistro.UI.SaleSystem.Tags" %>
 
<li>
<div class="icon" style="display:none;"><Hi:HiImage ID="HiImage1" runat="server" DataField="IconUrl" alt='<%#Eval("Name") %>'  /></div>
<div class="bd">
<b class="title"><%# Eval("Name") %> </b>
    <asp:Repeater ID="rptHelp" runat="server" DataSource='<%# DataBinder.Eval(Container, "DataItem.Helps") %>'>
        <ItemTemplate>
           <div  class="list">
            <a href='/help/show/<%# Eval("HelpId")%>' target="_blank" title='<%#Eval("Title") %>'>
                    <Hi:SubStringLabel ID="SubStringLabel" Field="Title" runat="server" />
                </a>
           </div>
      </ItemTemplate>
    </asp:Repeater>
</div>
</li>
 