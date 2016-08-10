<?php
	require_once(dirname(__FILE__).'/ConnectionInfo.php');
	
if (isset($_POST['_typeOfAction']) && isset($_POST['_typeOfAnimal']) && isset($_POST['_latitude']) && isset($_POST['_longtitude']) && isset($_POST['_infoAboutAction']) && isset($_POST['_imageFile']))
{
	//Get the POST variable
	$mtypeOfAction = $_POST[_typeOfAction];
	$mtypeOfAnimal = $_POST[_typeOfAnimal];
	$mlatitude = $_POST[_latitude];
	$mlongtitude = $_POST[_longtitude];
	$minfoAboutAction = $_POST[_infoAboutAction];
	$mImageBase64 = $_POST[_imageFile];
	
	//Set up connection
	$connectionInfo = new ConnectInfo();
	$connectionInfo->GetConnection();
	
	if(!$connectionInfo->conn)
	{
		//conection failed
		echo 'No connection';
	}
	
	else
	{
		//Insert new contact into database
		$query = 'INSERT INTO Animals (Animal_typeOfAction, Animal_typeOfAnimal, Animal_latitude, Animal_longtitude, Animal_InfoAboutAction) VALUES (?,?,?,?,?)';
		$parameters = array($mtypeOfAction,$mtypeOfAnimal, $mlatitude,  $mlongtitude,$minfoAboutAction );
		
		//Execute query
		$stmt = mysql_query($connectionInfo->conn, $query, $parameters);
		
		if(!stmt)
		{
			//The query failed
			echo 'Query Failed';
		}
		
		else
		{
			//The query succeded, now echo back new contact ID
			$query = "SELECT IDENT_CURRENT('Animals') AS ID";
			$stmt = mysql_query($connectionInfo->conn, $query);
			
			$row = mysql_fetch_array($stmt, MYSQL_FETCH_ASSOC);
			
			echo $row['ID'];
		}
		
	}
}

?>