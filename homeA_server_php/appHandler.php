<?php
	$UploadDirectory = '../uploadData/';
	
	$DeviceQueueFile = 'deviceQueue.txt';
	$DeviceStatusFile = 'deviceTasksPending.txt';
	
	$AppQueueFile = 'appQueue.txt';
	$AppStatusFile = 'appTasksPending.txt';
	
	
	if(isset($_GET['appToDeviceSerial']))
	{
		$serial = $_GET['appToDeviceSerial'];
		
		echo "Adding serial '" . $serial . "' to device queue.";
		
		if(!file_exists($UploadDirectory)){
			mkdir($UploadDirectory,0777,true);
		}
		
		$fp = 0;
		
		if(file_exists($UploadDirectory.$DeviceQueueFile))
		{
			$fp = fopen($UploadDirectory.$DeviceQueueFile , "a");
			fwrite($fp,"\r\n".$serial);
		}
		else
		{
			$fp = fopen($UploadDirectory.$DeviceQueueFile , "w");
			fwrite($fp,$serial);
		}
		
		
		fclose($fp);
		
		$fp = fopen($UploadDirectory.$DeviceStatusFile , "w");
		
		fwrite($fp,"1");
		
		fclose($fp);
			
		echo "<br>Serial successfully added to device queue.";
		
	}
	else if(isset($_GET['checkAppTasksPending']))
	{
		$return = 0;
		$fp = 0;
		
		if(file_exists($UploadDirectory.$AppStatusFile))
		{
			$fp = fopen($UploadDirectory.$AppStatusFile , "r");
			fscanf($fp, "%d" , $return);
			fclose($fp);
		}
		
		echo $return;
	}
	else if(isset($_GET['getAppQueue']))
	{
		$fp = 0;
		$data = "";
		if(file_exists($UploadDirectory.$AppQueueFile))
		{
			clearstatcache();
			$fp = fopen($UploadDirectory.$AppQueueFile , "r");
			$data = str_replace("\r\n" , "<br>" , fread($fp, filesize($UploadDirectory.$AppQueueFile)+1));
			fclose($fp);
			$fp = fopen($UploadDirectory.$AppStatusFile , "w");
			fwrite($fp,"0");
			fclose($fp);
			unlink($UploadDirectory.$AppQueueFile);
		}
		echo $data;

	}
	else
		echo "Please specify task to perform!<br><br>'?appToDeviceSerial=serialdata' to add serial data to device queue<br>'?checkAppTasksPending' to check app queue for pending tasks<br>'?getAppQueue' to get app queue";
?>