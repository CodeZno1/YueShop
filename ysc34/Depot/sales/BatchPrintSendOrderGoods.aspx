<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchPrintSendOrderGoods.aspx.cs"
    Inherits="Hidistro.UI.Web.Depot.sales.BatchPrintSendOrderGoods" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../css/css.css" />
    <link rel="stylesheet" href="/admin/css/bootstrap.min.css" type="text/css" media="screen" />
    <script language="javascript" type="text/javascript" src="/admin/js/bootstrap.min.js"></script>
    <style type="text/css">
        /**
 *    打印相关
*/ @media print {
            .notprint { display: none; }
            .PageNext { page-break-after: always; }
        }

        @media screen {
            .notprint { display: inline; cursor: hand; }
        }

        html, legend { color: #404040; background: #fff; }
        body, div, dl, dt, dd, ul, ol, li, h1, h2, h3, h4, h5, h6, pre, code, form, fieldset, legend, input, button, textarea, p, blockquote, th, td { margin: 0; padding: 0; }
        li { list-style: none; }
        body { font: 12px/1.5 "microsoft yahei",Tahoma,Helvetica,Arial,sans-serif; text-align: center; color: #000; }
        table { font-size: inherit; font: 100%; }
        .clear-both { clear: both; }
        #main { text-align: left; width: 63em; margin: 0 auto; }
        table { width: 100%; border-collapse: collapse; border-spacing: 0; border: 1px solid #858585; }
        h3 { margin: 0; font-size: 12px; }
        .print .info { position: relative; }
            .print .info h3 { font-size: 1.2em; }
            .print .info .prime-info { float: left; }
        .print ul.sub-info { overflow: hidden; }
        .clear { clear: both; }
        .print ul.sub-info li { float: left; padding-right: 3em; }
        button { width: 10em; height: 2em; text-align: center; }
        th { color: #000; text-align: left; padding: .4em .4em .4em 1.1em; border-bottom: 1px solid #858585; font-weight: 1; }
        td { border-bottom: 1px solid #858585; padding: .5em .8em; word-break: break-all; text-align: left; }
        .odd td { background-color: #F2F2F2; }
        .price { }
            .price li { float: left; margin: .5em 0; min-height: 1%; padding-right: 2em; }
                .price li span { float: left; }
        .order .col-0 { width: 6em; }
        .order .col-1 { width: 13em; }
        .order .col-2 { width: 12em; }
        .order .col-3 { width: 4em; }
        .order .col-4 { width: 4em; }
        .order .col-5 { width: 8em; }
        .order .col-6 { width: 8em; }
    </style>

    <script type="text/javascript">
        function clicks() {
            window.print();
        }
    </script>

</head>
<body>
    <div style="text-align: center; width: 700px; margin: 0 auto;" id="divContent" runat="server">
        <input type="button" value="确认打印" class="btn btn-primary notprint" id="printBtn" onclick="clicks()" />
    </div>
</body>
</html>
