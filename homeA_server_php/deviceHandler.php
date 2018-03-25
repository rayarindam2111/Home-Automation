<?php
	$UploadDirectory = '../uploadData/';
	
	$DeviceQueueFile = 'deviceQueue.txt';
	$DeviceStatusFile = 'deviceTasksPending.txt';
	
	$AppQueueFile = 'appQueue.txt';
	$AppStatusFile = 'appTasksPending.txt';
	
	
	if(isset($_GET['deviceToAppSerial']))
	{
		$serial = $_GET['deviceToAppSerial'];
		
		echo "Adding serial '" . $serial . "' to app queue.";
		
		if(!file_exists($UploadDirectory)){
			mkdir($UploadDirectory,0777,true);
		}
		
		$fp = 0;
		
		if(file_exists($UploadDirectory.$AppQueueFile))
		{
			$fp = fopen($UploadDirectory.$AppQueueFile , "a");
			fwrite($fp,"\r\n".$serial);
		}
		else
		{
			$fp = fopen($UploadDirectory.$AppQueueFile , "w");
			fwrite($fp,$serial);
		}
		
		
		fclose($fp);
		
		$fp = fopen($UploadDirectory.$AppStatusFile , "w");
		
		fwrite($fp,"1");
		
		fclose($fp);
			
		echo "<br>Serial successfully added to app queue.";
		
	}
	else if(isset($_GET['checkDeviceTasksPending']))
	{
		$return = 0;
		$fp = 0;
		
		if(file_exists($UploadDirectory.$DeviceStatusFile))
		{
			$fp = fopen($UploadDirectory.$DeviceStatusFile , "r");
			fscanf($fp, "%d" , $return);
			fclose($fp);
		}
		
		echo $return;
	}
	else if(isset($_GET['getDeviceQueue']))
	{
		$fp = 0;
		$data = "";
		if(file_exists($UploadDirectory.$DeviceQueueFile))
		{
			clearstatcache();
			$fp = fopen($UploadDirectory.$DeviceQueueFile , "r");
			$data = str_replace("\r\n" , "<br>" , fread($fp, filesize($UploadDirectory.$DeviceQueueFile)+1));
			fclose($fp);
			$fp = fopen($UploadDirectory.$DeviceStatusFile , "w");
			fwrite($fp,"0");
			fclose($fp);
			unlink($UploadDirectory.$DeviceQueueFile);
		}
		echo $data;

	}
	else
		echo "Please specify task to perform!<br><br>'?deviceToAppSerial=serialdata' to add serial data to app queue<br>'?checkDeviceTasksPending' to check device queue for pending tasks<br>'?getDeviceQueue' to get device queue";
?>