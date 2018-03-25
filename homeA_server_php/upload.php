<?php
$uploadfile = "./pic.jpg";
if (is_array($_FILES['FileInput']['error']))
	die("0");
if(move_uploaded_file($_FILES['file']['tmp_name'],$uploadfile))
	{echo "1";}
else
	{echo "0";}
?>