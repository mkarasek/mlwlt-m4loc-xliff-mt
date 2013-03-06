<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="mlwlt_web_test._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="target-densitydpi=device-dpi, width=device-width, initial-scale=1.0, maximum-scale=1">
    <meta name="description" content="MLW-LT Testing Site (XLIFF/MT service)">
    <meta name="author" content="Milan Karásek, Moravia">
    <meta name="keywords" content="MLW-LT, Machine Translation, MT, XLIFF, Web service">
    <link href="css/modern.css" rel="stylesheet">
    <link href="css/modern-responsive.css" rel="stylesheet">
    <link href="css/site.css" rel="stylesheet" type="text/css">
    <link href="js/google-code-prettify/prettify.css" rel="stylesheet" type="text/css">

    <script type="text/javascript" src="js/assets/jquery-1.9.0.min.js"></script>
    <script type="text/javascript" src="js/assets/jquery.mousewheel.min.js"></script>

    <script type="text/javascript" src="js/modern/dropdown.js"></script>
    <script type="text/javascript" src="js/modern/accordion.js"></script>
    <script type="text/javascript" src="js/modern/buttonset.js"></script>
    <script type="text/javascript" src="js/modern/carousel.js"></script>
    <script type="text/javascript" src="js/modern/input-control.js"></script>
    <script type="text/javascript" src="js/modern/pagecontrol.js"></script>
    <script type="text/javascript" src="js/modern/rating.js"></script>
    <script type="text/javascript" src="js/modern/slider.js"></script>
    <script type="text/javascript" src="js/modern/tile-slider.js"></script>
    <script type="text/javascript" src="js/modern/tile-drag.js"></script>

    <title>MLW-LT Testing Site (XLIFF/MT service)</title>
    <style>
        html {overflow-y: scroll;}
        h3, h4 {margin: 30px 0 10px 0;}
		div.eleContainer {color:#000000; font-family:Lucida Console, Courier New; font-size: 10pt;}
		div.eleBegin {color:#000000;display:inline;}
		span.eleName {color:#0000FF; display:inline;}
		span.eleAttrib {color:#FF0000; display:inline;}
		div.eleBody { margin-left: 20px;}
		div.eleBodyInLine { display: inline;}
		div.eleEnd {color:#000000;display:inline;}
    </style>
</head>
<body class="modern-ui" onload="prettyPrint()">


<div class="page">

<!--#include file="Header.htm"-->

    <div class="page-region">
        <div class="page-region-content">


            <div class="grid">
                <div class="row">

                    <div class="span8" style="height:520px;">
                        <h1>MLW-LT Testing Site</h1>
                        <h2>XLIFF/MT service</h2>

                        <p style="margin-top: 20px;"> 
                            Choose the XLIFF file to upload to the web-service and process the Machine translation on this file. 
                        </p>
                        <p style="margin-top: 20px;"> 
                            To leverage all functionality of the service, XlLIFF file should be decorated by one of following ITS 2.0 metadata:
                            <ul style="margin: 20px 0 0 50px;">
                                <li>Translate</li>
                                <li>Domain</li>
                                <li>Text Analysis Anotations</li>
                            </ul>
                        </p>
                        &nbsp;<br />
                        &nbsp;<br />

                        <form id="Form1" enctype="multipart/form-data" runat="server">
                            <div style="border:solid 1px #C0C0C0; padding:10px;"><input id="file1" type="file" name="file1" runat="server" style="width:100%;"/></input></div>
                            <div style="padding:10px;text-align:left;"><asp:Button id="btnUpload" onclick="btnUpload_Click" runat="server" style="Width:90px;" Text="Upload"></asp:Button></div>
                        </form>

                    </div>
                    <div class="span4" style="height:520px;">

                        <div class="span4 padding30 text-center place-left bg-color-blueLight" id="sponsorBlock">
                            <img src="img/mlw-logo-lt-124.png" alt="MLW-LT" />
                            <br>
                            <h2 class="fg-color-red">MLW-LT Home</h2>
                            <p class="">More information on this site.</p>
                
                            <a href="http://www.w3.org/International/multilingualweb/lt/"><h1><i class="icon-arrow-right-3 fg-color-red"></i></h1></a>
                        </div>

                        <div class="carousel span4" style="height: 230px;" data-role="carousel" data-param-effect="fade" data-param-direction="left" data-param-period="3000" data-param-markers="off">
                            <div class="slides">
                                <div class="slide image" id="slide1">
                                    <img src="https://fbcdn-sphotos-c-a.akamaihd.net/hphotos-ak-ash3/561739_4082424971467_1979090193_n.jpg" />
                                </div>
                                <div class="slide image" id="slide2">
                                    <img src="http://farm8.staticflickr.com/7026/6731469965_ffc56ab60d_z.jpg" />
                                </div>
                                <div class="slide image" id="slide3">
                                    <img src="https://lh6.googleusercontent.com/--RZJ_qB0Fv8/Tx0uXJ3AojI/AAAAAAAAFAE/-3lLu_4Dij8/s800/MLW-LT-3.JPG" />
                                </div>
                                <div class="slide image" id="slide4">
                                    <img src="https://lh4.googleusercontent.com/-k-2-mkrAox8/Tx0uW9K_hII/AAAAAAAAE8w/6gUfuJu59PI/s800/MLW-LT-2.JPG" />
                                </div>
                                <div class="slide image" id="slide5">
                                    <img src="https://lh5.googleusercontent.com/-D6oTyjslj8k/Tx0uW4XsNaI/AAAAAAAAE6o/wqCSQVOXMTk/s800/MLW-LT-1.JPG" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="span12">
                        <asp:PlaceHolder id="phOut" Visible="false" runat="server" />
                    </div>
                </div>
            </div> <!-- /grid -->

        </div>
    </div>



<!--#include file="Footer.htm"-->

</div>


</body>
</html>
